using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    private int MAX_TIME_IN_WORLD = 15;
    public int damage;

    private void Start()
    {
        Destroy(gameObject, MAX_TIME_IN_WORLD);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Block")) {
            Destroy(gameObject);

            collision.GetComponent<Block>().ApplyDamage(damage);
        }
    }
}
