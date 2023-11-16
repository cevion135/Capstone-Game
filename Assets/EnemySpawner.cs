using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyTypes {
    private string[] enemyTypes = {"basic", "rotator", "revolver"};
    private float[] movementSpeeds = {5f, 0f, 10f};
    private float[] max_healths = {100f, 200f, 70f};
    public static GameObject[] enemyPrefabs;
    
}
public class EnemySpawner : MonoBehaviour
{
    string enemyType;
    float enemySpeed;
    float enemyMaxHealth;
    float enemyCurrentHealth;
    GameObject enemyPrefab;
    public EnemySpawner(string type, float speed, float max_health, GameObject prefab){
        enemyType = type;
        enemySpeed = speed;
        enemyMaxHealth = max_health;
        enemyPrefab = prefab;
        //GameObject [Blank].Instantiate(prefab, spawnpoint, rotation);
        //[Blank].AddComponent<EnemyController>();
        //[Blank].tag = "enemy;

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
