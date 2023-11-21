using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This is the enemy spawner script.
Functions here are intended to be called by
the game manager script*/

public class EnemyTypes {
    public static string[] enemyTypes = {"basic", "revolver", "rotator"};
    public static float[] movementSpeeds = {5f, 0f, 10f};
    public static float[] max_healths = {100f, 200f, 20f};
    public static GameObject[] enemyPrefabs;
    // public static List<EnemySpawner> currEnemyInstances = new List<EnemySpawner>();
}
public class EnemyAttributes : MonoBehaviour{
    [SerializeField]public string enemyType;
    [SerializeField]public float enemySpeed;
    [SerializeField]public float enemyMaxHealth;
    [SerializeField]public float enemyCurrentHealth;
    [SerializeField]public GameObject enemyPrefab;
    public EnemyAttributes(string type, float speed, float health, GameObject prefab) {
        enemyType = type;
        enemySpeed = speed;
        enemyMaxHealth = health;
        enemyCurrentHealth = health;
        enemyPrefab = prefab;
    }

}
public class EnemySpawner : MonoBehaviour
{
    public EnemySpawner(string type, float speed, float max_health, GameObject prefab, Vector3 position){
        // Enemy attributes = new Enemy(type, speed, max_health, prefab);
        // enemyType = type;
        // enemySpeed = speed;
        // enemyMaxHealth = max_health;
        // enemyPrefab = prefab;
        GameObject enemy = Instantiate(prefab, position, Quaternion.identity);
        enemy.AddComponent<EnemyAttributes>();
        enemy.GetComponent<EnemyAttributes>().enemyType = type;
        enemy.GetComponent<EnemyAttributes>().enemySpeed = speed;
        enemy.GetComponent<EnemyAttributes>().enemyMaxHealth = max_health;
        enemy.GetComponent<EnemyAttributes>().enemyCurrentHealth = max_health;
        enemy.GetComponent<EnemyAttributes>().enemyPrefab = prefab;
        enemy.AddComponent<BoxCollider>();
        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        enemy.AddComponent<EnemyController>();
        //IMPORTANT:Remove child object tagging when enemy models are added.
        Transform child = enemy.transform.Find("Enemy");
        if(child != null) {
            // Debug.Log("CHILD ENEMY FOUND");
            child.gameObject.tag = "Enemy";
        }
        else{
            Debug.Log("child enemy not found");
            enemy.tag = "Enemy";
        }

    }
    void Awake()
    {
        EnemyTypes.enemyPrefabs = Resources.LoadAll<GameObject>("Enemies");
        // Debug.Log("[File Start] validation for enemy types: " + EnemyTypes.enemyTypes[0]);
        // Debug.Log("[File Start] validation for enemy movement speeds: " + EnemyTypes.movementSpeeds[0]);
        // Debug.Log("[File Start] validation for enemy health: " + EnemyTypes.max_healths[0]);
        // Debug.Log("[File Start] validation for enemy prefabs: " + EnemyTypes.enemyPrefabs[0]);

    }
    void Update(){
        // Debug.Log(EnemyTypes.enemyPrefabs.Length);

    }
    public static void createBasicEnemy(Vector3 position){
        // Debug.Log("[Instantiation] validation for enemy types: " + EnemyTypes.enemyTypes[0]);
        // Debug.Log("[Instantiation] validation for enemy movement speeds: " + EnemyTypes.movementSpeeds[0]);
        // Debug.Log("[Instantiation] validation for enemy health: " + EnemyTypes.max_healths[0]);
        // Debug.Log("[Instantiation] validation for enemy prefabs: " + EnemyTypes.enemyPrefabs[0]);
        // Debug.Log("[Instantiation] validation for position: " + position);
        EnemySpawner enemy = new EnemySpawner(EnemyTypes.enemyTypes[0], EnemyTypes.movementSpeeds[0],
        EnemyTypes.max_healths[0], EnemyTypes.enemyPrefabs[0], position);
        // SpawnEnemy(EnemyTypes.enemyTypes[0], EnemyTypes.movementSpeeds[0], EnemyTypes.max_healths[0], EnemyTypes.enemyPrefabs[0], position);
        // Debug.Log("Basic Enemy Created");
    }
    public static void createRotatorEnemy(Vector3 position){
        EnemySpawner enemy = new EnemySpawner(EnemyTypes.enemyTypes[1], EnemyTypes.movementSpeeds[1],
        EnemyTypes.max_healths[1], EnemyTypes.enemyPrefabs[1], position);
        // SpawnEnemy(EnemyTypes.enemyTypes[1], EnemyTypes.movementSpeeds[1], EnemyTypes.max_healths[1], EnemyTypes.enemyPrefabs[1], position);
    }
    public static void createRevolverEnemy(Vector3 position){
        EnemySpawner enemy = new EnemySpawner(EnemyTypes.enemyTypes[2], EnemyTypes.movementSpeeds[2],
        EnemyTypes.max_healths[2], EnemyTypes.enemyPrefabs[2], position);
        // SpawnEnemy(EnemyTypes.enemyTypes[2], EnemyTypes.movementSpeeds[2], EnemyTypes.max_healths[2], EnemyTypes.enemyPrefabs[2], position);
    }


    // public static GameObject SpawnEnemy(string type, float speed, float max_health, GameObject prefab, Vector3 position) {
    //     GameObject enemy = Instantiate(prefab, position, Quaternion.identity) as GameObject;
    //     enemy.AddComponent<Enemy>();
    //     enemy.GetComponent<Enemy>().enemyType = type;
    //     enemy.GetComponent<Enemy>().enemySpeed = speed;
    //     enemy.GetComponent<Enemy>().enemyMaxHealth = max_health;
    //     enemy.GetComponent<Enemy>().enemyCurrentHealth = max_health;
    //     enemy.GetComponent<Enemy>().enemyPrefab = prefab;
    //     Rigidbody rb = enemy.GetComponent<Rigidbody>();
    //     rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    //     enemy.AddComponent<EnemyController>();
    //     enemy.tag = "Enemy";
    //     return enemy;
    // }
}



