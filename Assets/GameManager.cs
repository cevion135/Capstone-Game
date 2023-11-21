using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // GameObject Enemies;
    // Start is called before the first frame update
    void Start()
    {
        EnemySpawner.createBasicEnemy(new Vector3(12f,.5f,12f));   
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] numOfEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        if(numOfEnemies.Length <= 3) {
            EnemySpawner.createBasicEnemy(new Vector3(Random.Range(-11f, 35f),.5f,Random.Range(7f, 53f)));
        }
    }
}
  
