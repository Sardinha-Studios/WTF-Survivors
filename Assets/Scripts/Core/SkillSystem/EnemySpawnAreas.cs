using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnAreas : MonoBehaviour
{
    [SerializeField] private List<BoxCollider2D> spawnAreas = new List<BoxCollider2D>();

    public List<BoxCollider2D> SpawnAreas 
    {
        get => spawnAreas;
    }
}
