using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockShared : MonoBehaviour
{
    public Material damageMaterial;
    public float damageFlashTime;
    public ParticleSystem blockDestroyParticle;
    public GameObject lootcratePrefab;
    public GameObject radarEnemyUI;
    public GameObject radarUI;

    [Header("Audio")]
    public AudioClip[] blockHit;
    public AudioClip[] blockPlace;
}
