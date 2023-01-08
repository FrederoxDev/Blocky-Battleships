using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootcrate : MonoBehaviour
{
    public GameObject[] commonPrefabs;
    public int commonWeight = 3;

    public GameObject[] rarePrefabs;
    public int rareWeight = 1;

    public int minRolls = 1;
    public int maxRolls = 4;

    private BlockShared blockShared;

    private void Awake()
    {
        blockShared = FindObjectOfType<BlockShared>();
    }

    public void Open()
    {
        Destroy(gameObject);

        int rolls = Random.Range(minRolls, maxRolls);
        List<GameObject[]> table = new List<GameObject[]>();
        float angleBetween = 360f / rolls;
        
        for (int i = 0; i < commonWeight; i++) table.Add(commonPrefabs);
        for (int i = 0; i < rareWeight; i++) table.Add(rarePrefabs);

        for (int i = 0; i < rolls; i++)
        {
            int selectIdx = Random.Range(0, table.Count);
            float angle = i * angleBetween;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector3 position = Vector3.MoveTowards(transform.position, transform.position + rotation * Vector2.up * 1.5f, 5f);
            GameObject prefab = table[selectIdx][Random.Range(0, table[selectIdx].Length)];

            Instantiate(prefab, position, rotation);
            ParticleSystem particle = Instantiate(blockShared.blockDestroyParticle, position, Quaternion.identity);
            particle.Play();
        }
    }
}
