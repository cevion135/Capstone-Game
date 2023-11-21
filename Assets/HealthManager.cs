using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static List<EnemySpawner> AllEnemies = new List<EnemySpawner>();
    public float playerMaxHealth = BasicMovement.max_health;
    public float playerCurHealth = BasicMovement.curr_health;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // getEnemies();
        // foreach (enemy in AllEnemies)
    }
    void getEnemies(){
        // AllEnemies = EnemyTypes.currEnemyInstances;
    }
}
