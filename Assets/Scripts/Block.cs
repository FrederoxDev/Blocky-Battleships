using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Block : MonoBehaviour
{
    public Material defaultMat;
    public SpriteRenderer sr;
    public BlockShared blockShared;

    public int baseHealth;
    [SerializeField] public int currentHealth;

    public bool isFlashing = false;
    public float timeSinceLastHit = 0;

    public AudioSource audioSource;

    // [0] Up, [1] Right, [2] Down, [3] Left
    [SerializeField] private Block[] connections;

    public int rotation = 0;

    public void Awake()
    {
        audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        currentHealth = baseHealth;
        sr = GetComponent<SpriteRenderer>();
        connections = new Block[4];
        defaultMat = sr.material;
        blockShared = FindObjectOfType<BlockShared>();
    }

    public void Update()
    {
        timeSinceLastHit += Time.deltaTime;

        if (isFlashing && timeSinceLastHit >= blockShared.damageFlashTime)
        {
            sr.material = defaultMat;
            isFlashing = false;
        }
    }

    public void CreateConnection(Block other, Dir dirToOtherBlock, bool updateOther = false)
    {
        connections[(int)dirToOtherBlock] = other;
        transform.GetChild((int)dirToOtherBlock).gameObject.SetActive(false);

        if (updateOther)
        {
            other.CreateConnection(this, this.InverseDir(dirToOtherBlock));
            other.SetParent();
        }
    }

    public void RemoveAllConnections()
    {
        this.RemoveConnection(Dir.Up, true);
        this.RemoveConnection(Dir.Down, true);
        this.RemoveConnection(Dir.Right, true);
        this.RemoveConnection(Dir.Left, true);
    }

    public void RemoveConnection(Dir dirToOther, bool updateOther = false)
    {
        transform.GetChild((int)dirToOther).gameObject.SetActive(true);
        if (connections[(int)dirToOther] == null) return;

        if (updateOther)
        {
            connections[(int)dirToOther].RemoveConnection(InverseDir(dirToOther));
        }

        connections[(int)dirToOther] = null;
    }

    public bool CheckForCab(GameObject obj)
    {
        if (obj.gameObject.name == "PlayerCab") return true;
        else if (obj.transform.parent != null) return CheckForCab(obj.transform.parent.gameObject);
        else return false;
    }

    public bool CheckForEnemyCab(GameObject obj)
    {
        if (obj.gameObject.name == "EnemyCab") return true;
        else if (obj.transform.parent != null) return CheckForEnemyCab(obj.transform.parent.gameObject);
        else return false;
    }

    public Dir InverseDir(Dir dir)
    {
        if (dir == Dir.Up) return Dir.Down;
        else if (dir == Dir.Right) return Dir.Left;
        else if (dir == Dir.Down) return Dir.Up;
        else return Dir.Right;
    }

    public void SetParent()
    {
        if (connections.Length > 0)
        {
            if (connections[0] != null) transform.parent = connections[0].transform;
            else if (connections[1] != null) transform.parent = connections[1].transform;
            else if (connections[2] != null) transform.parent = connections[2].transform;
            else if (connections[3] != null) transform.parent = connections[3].transform;
            else transform.parent = null;
        }

        else transform.parent = null;
    }

    public void ApplyDamage(int damage)
    {
        currentHealth -= damage;
        sr.material = blockShared.damageMaterial;
        isFlashing = true;
        timeSinceLastHit = 0;

        if (currentHealth <= 0)
        {
            this.RemoveAllConnections();

            Block[] blocks = gameObject.GetComponentsInChildren<Block>();

            foreach (Block block in blocks)
            {
                block.RemoveAllConnections();
                block.transform.parent = null;
            }

            OnBlockDestroyed();
            Destroy(gameObject);
        }

        audioSource.PlayOneShot(blockShared.blockHit[Random.Range(0, blockShared.blockHit.Length)], 0.3f);
    }

    public void Heal()
    {
        sr.material = blockShared.damageMaterial;
        isFlashing = true;
        timeSinceLastHit = 0;

        currentHealth = baseHealth;
    }

    public virtual void OnBlockDestroyed()
    {
        ParticleSystem particle = Instantiate(blockShared.blockDestroyParticle, transform.position, Quaternion.identity);
        particle.Play();
    }
}

public enum Dir
{
    Up,
    Right,
    Down,
    Left
}