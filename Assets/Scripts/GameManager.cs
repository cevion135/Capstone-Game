using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private bool canChangeScene = false;
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
        // Debug.Log(numOfEnemies.Length);
        if(numOfEnemies.Length == 0){
            canChangeScene = true;
        }
        if(numOfEnemies.Length == 0 && canChangeScene == true) {      
            loadNextScene();
        }
        if(Input.GetKey(KeyCode.Space)){
            loadNextScene();
        }
        if(Input.GetKeyDown(KeyCode.Backspace)){
            loadPreviousScene();
        }
    }
    public void queueTransition() {

    }
    public void loadNextScene(){
        StartCoroutine(TwoSecCooldown());
        int curSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("New Scene Number: " + (curSceneIndex+1) + " Total Scene Count: " + SceneManager.sceneCountInBuildSettings);
        if((curSceneIndex + 1) < SceneManager.sceneCountInBuildSettings){
            SceneManager.LoadScene(curSceneIndex + 1);
            switch(curSceneIndex + 1){
                case 2:
                    initializeLevel1();
                    break;
                case 3:
                    initializeLevel2();
                    break;
                case 4: 
                    initializeLevel3();
                    break;
                case 5:
                    initializeLevel4();
                    break;
            }
        }

    }
    public void loadPreviousScene(){
         int curSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if((curSceneIndex - 1) >= 0){
            SceneManager.LoadScene(curSceneIndex - 1);
        }
    }
   public void initializeLevel0(){
        //Place character at specific location.
   }
    public void initializeLevel1(){
        //Place character at specific location.
        playerTransform.position = new Vector3(0f, .5f, 0f);
        Debug.Log("LEVEL INTIALIZATION CALLED");
        
            EnemySpawner.createBasicEnemy(new Vector3(-2f,.5f,-10f));   
            EnemySpawner.createBasicEnemy(new Vector3(31f,.5f,12f)); 
            EnemySpawner.createBasicEnemy(new Vector3(8f,.5f,44f)); 
            EnemySpawner.createBasicEnemy(new Vector3(8f,.5f,44f));
            EnemySpawner.createRevolverEnemy(new Vector3(-13f, .5f, 6f));
            EnemySpawner.createRevolverEnemy(new Vector3(-13f, .5f, 6f));
            EnemySpawner.createRotatorEnemy(new Vector3(0f, .5f, 0f));

            canChangeScene = false;
        }
    public void initializeLevel2(){
        //Place character at specific location.
        playerTransform.position = new Vector3(-44f, .5f, -44f);
        canChangeScene = false;
    }
    public void initializeLevel3(){
        //Place character at specific location.   
        playerTransform.position = new Vector3(-44f, .5f, -44f);
        canChangeScene = false;
    }
    public void initializeLevel4(){
        playerTransform.position = new Vector3(-.5f, .5f, -103f);
        canChangeScene = false;
        //Place character at specific location.    
    }
    public void initializeLevel5(){
        //Place character at specific location.
        canChangeScene = false;
    }
    IEnumerator TwoSecCooldown(){
        yield return new WaitForSeconds(2f);
    }
}
  
