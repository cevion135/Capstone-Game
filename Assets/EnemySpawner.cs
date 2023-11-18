using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This is the enemy spawner script.
Functions here are intended to be called by
the game manager script*/

public class EnemyTypes {
    public static string[] enemyTypes = {"basic", "rotator", "revolver"};
    public static float[] movementSpeeds = {5f, 0f, 10f};
    public static float[] max_healths = {100f, 200f, 70f};
    public static GameObject[] enemyPrefabs;
    
}
public class EnemySpawner : MonoBehaviour
{
    public string enemyType;
    public float enemySpeed;
    public float enemyMaxHealth;
    public float enemyCurrentHealth;
    public GameObject enemyPrefab;
    public EnemySpawner(string type, float speed, float max_health, GameObject prefab, Vector3 position){
        enemyType = type;
        enemySpeed = speed;
        enemyMaxHealth = max_health;
        enemyPrefab = prefab;
        GameObject enemy = Instantiate(prefab, position, Quaternion.identity);
        enemy.AddComponent<EnemyController>();
        enemy.tag = "enemy";

    }
    void Start()
    {
        
    }
    void createBasicEnemy(Vector3 position){
        EnemySpawner enemy = new EnemySpawner(EnemyTypes.enemyTypes[0], EnemyTypes.movementSpeeds[0],
        EnemyTypes.max_healths[0], EnemyTypes.enemyPrefabs[0], position);
    }
    void createRotatorEnemy(Vector3 position){
        EnemySpawner enemy = new EnemySpawner(EnemyTypes.enemyTypes[1], EnemyTypes.movementSpeeds[1],
        EnemyTypes.max_healths[1], EnemyTypes.enemyPrefabs[1], position);
    }
    void createRevolverEnemy(Vector3 position){
        EnemySpawner enemy = new EnemySpawner(EnemyTypes.enemyTypes[2], EnemyTypes.movementSpeeds[2],
        EnemyTypes.max_healths[2], EnemyTypes.enemyPrefabs[2], position);
    }
}
