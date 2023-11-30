using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // public static GameManager = instance;
   void Awake(){
        DontDestroyOnLoad(gameObject);
   }
    void Start()
    {
        EnemySpawner.createBasicEnemy(new Vector3(-2f,.5f,-10f));  
        EnemySpawner.createBasicEnemy(new Vector3(31f,.5f,12f)); 
            EnemySpawner.createBasicEnemy(new Vector3(8f,.5f,-14f)); 
            EnemySpawner.createBasicEnemy(new Vector3(8f,.5f,14f));
            EnemySpawner.createRevolverEnemy(new Vector3(-13f, .5f, 6f));
            EnemySpawner.createRevolverEnemy(new Vector3(-13f, .5f, 6f));
            EnemySpawner.createRotatorEnemy(new Vector3(5f, .5f, 5f)); 
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] numOfEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        // if(numOfEnemies.Length <= 3) {
        //     EnemySpawner.createBasicEnemy(new Vector3(Random.Range(-11f, 35f),.5f,Random.Range(7f, 53f)));
        // }
        if(Input.GetKeyDown(KeyCode.Space)){
            loadNextScene();
        }
        if(Input.GetKeyDown(KeyCode.Backspace)){
            loadNextScene();
        }
    }
    public void queueTransition() {

    }
    public void loadNextScene(){
        int curSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(curSceneIndex + 1);

    }
    public void loadPreviousScene(){
         int curSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if((curSceneIndex - 1) >= 0){
            SceneManager.LoadScene(curSceneIndex - 1);
        }
    }
    public void initializeLevel1(){
            EnemySpawner.createBasicEnemy(new Vector3(-2f,.5f,-10f));   
            EnemySpawner.createBasicEnemy(new Vector3(31f,.5f,12f)); 
            EnemySpawner.createBasicEnemy(new Vector3(8f,.5f,44f)); 
            EnemySpawner.createBasicEnemy(new Vector3(8f,.5f,44f));
            EnemySpawner.createRevolverEnemy(new Vector3(-13f, .5f, 6f));
            EnemySpawner.createRevolverEnemy(new Vector3(-13f, .5f, 6f));
            EnemySpawner.createRotatorEnemy(new Vector3(0f, .5f, 0f));
        }
    public void initializeLevel2(){

    }
    public void initializeLevel3(){

    }
    public void initializeLevel4(){

    }
    public void initializeLevel5(){

    }
}
  
