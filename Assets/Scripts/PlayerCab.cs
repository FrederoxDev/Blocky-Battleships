using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class PlayerCab : Block
{
    [SerializeField] private Block selectedBlock;
    [SerializeField] private Vector2 lastMousePos;
    private Collider2D lastCollider;
    private Rigidbody2D rb;

    public float speed = 10.0f;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private new void Update()
    {
        base.Update();
        BlockMovement();

        if (Input.GetKey(KeyCode.Space)) Shoot();

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float size = Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * 5f;
            size = Mathf.Clamp(size, 3.5f, 18f);
            Camera.main.orthographicSize = size;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        rb.AddTorque(-horizontal * speed * Time.deltaTime * 10);
        rb.AddForce(transform.up * vertical * speed * Time.deltaTime * 10);

        Vector3 pos = transform.position;
        pos.z = -10;

        Camera.main.transform.position = pos;
    }

    private void Shoot()
    {
        MachineGun[] machineGuns = gameObject.GetComponentsInChildren<MachineGun>();
        Mortar[] mortars = gameObject.GetComponentsInChildren<Mortar>();

        foreach(Mortar m in mortars) m.Shoot();
        foreach(MachineGun m in machineGuns) m.Shoot();
    }

    private void BlockMovement()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 size = new Vector2(0.1f, 0.1f);
            Collider2D[] collider = Physics2D.OverlapBoxAll(mousePos, size * 2, 0);
            lastMousePos = mousePos;

            for (int i = 0; i < collider.Length; i++)
            {
                if (collider[i].CompareTag("Lootcrate"))
                {
                    collider[i].GetComponent<Lootcrate>().Open();
                    break;
                }
            }

            for (int i = 0; i < collider.Length; i++)
            {
                if (collider[i].tag != "Block") continue;
                if (collider[i].name == "PlayerCab") continue;
                if (CheckForEnemyCab(collider[i].gameObject)) continue;
                selectedBlock = collider[i].GetComponent<Block>();
                break;
            }
        }

        else if (Input.GetMouseButton(0))
        {
            if (selectedBlock != null)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    selectedBlock.rotation -= 90;
                    selectedBlock.transform.rotation = Quaternion.Euler(0, 0, selectedBlock.rotation);
                }

                SnapBlockToNearest(mousePos);
            }

            lastMousePos = mousePos;
        }

        else if (Input.GetMouseButtonUp(0))
        {
            selectedBlock = null;
        }
    }

    private void OnDestroy()
    {
        foreach(Transform child in transform)
        {
            if (!child.CompareTag("Block")) return;
            child.parent = null;
        }
    }

    public override void OnBlockDestroyed()
    {
        base.OnBlockDestroyed();
        FindObjectOfType<WaveManager>().PlayerDied();
        Debug.Log("PlayerDied playercab");
    }

    private void SnapBlockToNearest(Vector2 mousePos)
    {
        Vector2 size = new Vector2(0.1f, 0.1f);
        Collider2D[] colliders = Physics2D.OverlapBoxAll(mousePos, size * 2, 0);
        colliders = colliders.Where(
            c => c.CompareTag("TopCollider") || c.CompareTag("RightCollider") || 
            c.CompareTag("DownCollider") || c.CompareTag("LeftCollider")
        ).ToArray();

        colliders = colliders.Where(c => c.transform.parent != selectedBlock.transform && c != lastCollider).ToArray();

        if (colliders.Length > 0)
        {
            selectedBlock.RemoveAllConnections();
            Block other = colliders[0].transform.parent.GetComponent<Block>();

            if (CheckForCab(other.gameObject))
            {
                if (colliders[0].CompareTag("TopCollider")) other.CreateConnection(selectedBlock, Dir.Up, true);
                else if (colliders[0].CompareTag("RightCollider")) other.CreateConnection(selectedBlock, Dir.Right, true);
                else if (colliders[0].CompareTag("DownCollider")) other.CreateConnection(selectedBlock, Dir.Down, true);
                else other.CreateConnection(selectedBlock, Dir.Left, true);

                selectedBlock.transform.position = colliders[0].transform.position;
                selectedBlock.transform.rotation = Quaternion.Euler(0, 0,
                    colliders[0].transform.rotation.eulerAngles.z + selectedBlock.rotation
                );

                selectedBlock.audioSource.PlayOneShot(blockShared.blockPlace[Random.Range(0, blockShared.blockPlace.Length)], 0.6f);

                selectedBlock = null;
                lastCollider = colliders[0];
                return;
            }
        }

        selectedBlock.RemoveAllConnections();

        foreach (Block child in selectedBlock.GetComponentsInChildren<Block>())
        {
            child.RemoveAllConnections();
        }

        selectedBlock.SetParent();
        selectedBlock.transform.position = mousePos;
        selectedBlock.transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + selectedBlock.rotation);
       
    }
}
