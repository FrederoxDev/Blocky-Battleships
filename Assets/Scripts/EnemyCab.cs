using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyCab : Block
{
    private WaveManager waveManager;
    private PlayerCab player;
    private Rigidbody2D rb;
    public float speed = 10f;
    public float rotSpeed = 10f;

    [Header("AI GOALS")]
    public EnemyState state = EnemyState.Idle;
    public float wanderRange;
    public Vector2 targetPos;
    public float fleeDist;

    public float chaseDistance = 30f;
    public float shootDistance = 10f;

    GameObject radarPointUI;

    private new void Awake()
    {
        base.Awake();
        waveManager = FindObjectOfType<WaveManager>();
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerCab>();

        radarPointUI = Instantiate(blockShared.radarEnemyUI, blockShared.radarUI.transform);
    }

    private new void Update()
    {
        base.Update();
        if (state == EnemyState.Idle) Idle();
        else if (state == EnemyState.Chase) Chase();
        else if (state == EnemyState.Flee) Flee();
        else if (state == EnemyState.ChaseAndShoot) ChaseAndShoot();

        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (!player.IsDestroyed())
        {
            Vector2 playerDir = (transform.position - player.transform.position).normalized;
            float playerAngle = Mathf.Atan2(playerDir.y, playerDir.x) * Mathf.Rad2Deg - 90f;
            radarPointUI.transform.rotation = Quaternion.Euler(0, 0, playerAngle);
        }
        
        rb.AddForce(transform.up * speed * Time.deltaTime * 10);
    }

    private void Idle()
    {
        // Change the target position if the AI reaches the previous pos
        if (Vector2.Distance(targetPos, transform.position) < 4f)
        {
            targetPos = (Vector2) transform.position + new Vector2(Random.Range(-wanderRange, wanderRange), Random.Range(-wanderRange, wanderRange));
        }

        if (!player.IsDestroyed())
        {
            if (Vector2.Distance(transform.position, player.transform.position) < chaseDistance)
                state = EnemyState.Chase;

            if (this.currentHealth < this.baseHealth / 4) state = EnemyState.Flee;
        }  
    }

    private void Chase()
    {
        if (player.IsDestroyed())
        {
            state = EnemyState.Idle;
            return;
        }

        targetPos = player.transform.position;

        if (Vector2.Distance(transform.position, player.transform.position) < shootDistance) 
            state = EnemyState.ChaseAndShoot;

        if (Vector2.Distance(transform.position, player.transform.position) > chaseDistance)
            state = EnemyState.Idle;

        if (this.currentHealth < this.baseHealth / 4) state = EnemyState.Flee;
    }

    private void ChaseAndShoot()
    {
        if (player.IsDestroyed())
        {
            state = EnemyState.Idle;
            return;
        }

        targetPos = player.transform.position;
        Shoot();

        if (Vector2.Distance(transform.position, player.transform.position) > shootDistance)
            state = EnemyState.Chase;

        if (this.currentHealth < this.baseHealth / 4) state = EnemyState.Flee;
    }

    private void Shoot()
    {
        MachineGun[] machineGuns = gameObject.GetComponentsInChildren<MachineGun>();
        Mortar[] mortars = gameObject.GetComponentsInChildren<Mortar>();

        foreach (Mortar m in mortars) m.Shoot();
        foreach (MachineGun m in machineGuns) m.Shoot();
    }

    private void Flee()
    {
        if (player.IsDestroyed())
        {
            state = EnemyState.Idle;
            return;
        }

        Vector2 direction = (player.transform.position - transform.position).normalized;
        Vector2 oppositeDirection = -direction;
        targetPos = (Vector2)transform.position + oppositeDirection * fleeDist;
    }

    public override void OnBlockDestroyed()
    {
        base.OnBlockDestroyed();
        waveManager.ReportDeath();

        Instantiate(blockShared.lootcratePrefab, transform.position, Quaternion.identity);
        Destroy(radarPointUI);
    }
}

public enum EnemyState
{
    Idle,
    Chase,
    Flee,
    ChaseAndShoot
}