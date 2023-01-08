using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mortar : Block
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10.0f;
    public float timeBetweenShots = 1f;
    private float timeSinceLastShot = 0;

    new void Update()
    {
        base.Update();
        timeSinceLastShot += Time.deltaTime;
    }

    public void Shoot()
    {
        if (timeSinceLastShot < timeBetweenShots) return;

        GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.up, transform.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * projectileSpeed, ForceMode2D.Impulse);
        timeSinceLastShot = 0;
    }
}
