using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private bool canChangeScene = false;
    [SerializeField] public static bool stillAlive = true;

   void Awake(){
        DontDestroyOnLoad(gameObject);
   }
    void Start()
    {
        initializeLevel0();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] numOfEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        // Debug.Log(numOfEnemies.Length);
        if(numOfEnemies.Length == 0 && isPlayerAlive()){
            canChangeScene = true;
        }
        if(!isPlayerAlive()){
            killAllEnemies();
        }
        // if(numOfEnemies.Length == 0 && SceneManager.GetActiveScene().buildIndex == 5){
        //     youWin();
        // }
        // else{
        if(numOfEnemies.Length == 0 && canChangeScene == true) {      
            loadNextScene();
        }
            // if(Input.GetKey(KeyCode.Space)){
                // loadNextScene();
            // }
            // if(Input.GetKeyDown(KeyCode.Backspace)){
                // loadPreviousScene();
            // }
        // }
    }
    public void youWin(){
        Debug.Log("CONGRATULATIONS, YOU BEAT THE GAME!");
    }
    public bool isPlayerAlive(){
        return BasicMovement.playerStatus;
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
        canChangeScene = false;
    }
    public void loadPreviousScene(){
         int curSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if((curSceneIndex - 1) >= 0){
            SceneManager.LoadScene(curSceneIndex - 1);
        }
    }
   public void initializeLevel0(){


        EnemySpawner.createBasicEnemy(new Vector3(-15f,.5f,-19f));  
        EnemySpawner.createBasicEnemy(new Vector3(-15f,.5f,19f)); 
            EnemySpawner.createBasicEnemy(new Vector3(15f,.5f,-19f)); 
            EnemySpawner.createBasicEnemy(new Vector3(15f,.5f,19f));
            EnemySpawner.createRevolverEnemy(new Vector3(15f, .5f, 0f));
            EnemySpawner.createRevolverEnemy(new Vector3(-15f, .5f, 0f));
            EnemySpawner.createRotatorEnemy(new Vector3(0f, .5f, 15f)); 
            EnemySpawner.createRotatorEnemy(new Vector3(0f, .5f, -15f)); 
   }
    public void initializeLevel1(){
        //Heal & Place character at specific location.
        killAllEnemies();
        BasicMovement.curr_health = BasicMovement.max_health;
        playerTransform.position = new Vector3(0f, .5f, 0f);
        Debug.Log("LEVEL 1 INTIALIZATION CALLED");

             //Spawn Enemies
            EnemySpawner.createBasicEnemy(new Vector3(-0f,.5f,-10f));   
            EnemySpawner.createBasicEnemy(new Vector3(0f,.5f,10f));

            EnemySpawner.createRevolverEnemy(new Vector3(-10f, .5f, 20f));
            EnemySpawner.createRevolverEnemy(new Vector3(10f, .5f, 20f));

            EnemySpawner.createRevolverEnemy(new Vector3(.7f, .5f, 6f));
        
            canChangeScene = false;
        }
    public void initializeLevel2(){
        //Heal & Place character at specific location.
        killAllEnemies();
        BasicMovement.curr_health = BasicMovement.max_health;
        playerTransform.position = new Vector3(-44f, .5f, -44f);

        //Spawn Enemies
        EnemySpawner.createRotatorEnemy(new Vector3(-12f,.5f,-40f)); 
        EnemySpawner.createRotatorEnemy(new Vector3(-38f,.5f,-14f));
        EnemySpawner.createRotatorEnemy(new Vector3(-38f,.5f, 14f));
        EnemySpawner.createRotatorEnemy(new Vector3(-12f,.5f, 35f));
        EnemySpawner.createRotatorEnemy(new Vector3(0f,.5f, 0f));

        EnemySpawner.createBasicEnemy(new Vector3(10f, .5f, 35f));
        EnemySpawner.createBasicEnemy(new Vector3(10f, .5f, -40f));
        EnemySpawner.createBasicEnemy(new Vector3(38f, .5f, -14f));
        EnemySpawner.createBasicEnemy(new Vector3(42f, .5f, 40f));

        EnemySpawner.createRevolverEnemy(new Vector3(12f, .5f, 3f));
        EnemySpawner.createRevolverEnemy(new Vector3(12f, .5f, -19f));
        EnemySpawner.createRevolverEnemy(new Vector3(33f, .5f, 3f));
        EnemySpawner.createRevolverEnemy(new Vector3(33f, .5f, -19f));

        canChangeScene = false;
    }
    public void initializeLevel3(){
        killAllEnemies();
        //Heal & Place character at specific location.   
        BasicMovement.curr_health = BasicMovement.max_health;
        playerTransform.position = new Vector3(-44f, .5f, -44f);

        //Spawn Enemies
        EnemySpawner.createBasicEnemy(new Vector3(-10f,.5f, -10f));
        EnemySpawner.createBasicEnemy(new Vector3(-44f,.5f, -30f));
        EnemySpawner.createBasicEnemy(new Vector3(28f, .5f, -24f));
        EnemySpawner.createBasicEnemy(new Vector3(42f, .5f, -38f));
        EnemySpawner.createBasicEnemy(new Vector3(0f, .5f, 18f));
        EnemySpawner.createBasicEnemy(new Vector3(42f, .5f, 18f));

        EnemySpawner.createRotatorEnemy(new Vector3(34f, .5f, -4f));
        EnemySpawner.createRotatorEnemy(new Vector3(-16f, .5f, 30f));
        EnemySpawner.createRotatorEnemy(new Vector3(-16f, .5f, -4f));

        EnemySpawner.createRevolverEnemy(new Vector3(6f, .5f, -16f));
        EnemySpawner.createRevolverEnemy(new Vector3(16f, .5f, -16f));
        EnemySpawner.createRevolverEnemy(new Vector3(16f, .5f, -16f));
        EnemySpawner.createRevolverEnemy(new Vector3(-8f, .5f, 6f));
        EnemySpawner.createRevolverEnemy(new Vector3(-34f, .5f, 30f));
        EnemySpawner.createRevolverEnemy(new Vector3(-24f, .5f, -4f));
        EnemySpawner.createRevolverEnemy(new Vector3(-24f, .5f, -14f));
        
        canChangeScene = false;
    }
    public void initializeLevel4(){
        killAllEnemies();
        //Heal & Place character at specific location.
        BasicMovement.curr_health = BasicMovement.max_health;
        playerTransform.position = new Vector3(-.5f, .5f, -103f);


        //Spawn Enemies | Top and bottom path
        EnemySpawner.createBasicEnemy(new Vector3(-12f,.5f,-51f)); 
        EnemySpawner.createBasicEnemy(new Vector3(14f,.5f,-51f)); 
        EnemySpawner.createRotatorEnemy(new Vector3(0f,.5f,50f));



        //bottom left box
        EnemySpawner.createBasicEnemy(new Vector3(-34f,.5f,-36f)); 
        EnemySpawner.createBasicEnemy(new Vector3(-66f,.5f,-36f)); 
        EnemySpawner.createBasicEnemy(new Vector3(-34f,.5f,-65f)); 
        EnemySpawner.createBasicEnemy(new Vector3(-66f,.5f,-65f)); 

        //top right box
        EnemySpawner.createBasicEnemy(new Vector3(34f,.5f,34f)); 
        EnemySpawner.createBasicEnemy(new Vector3(34f,.5f,64f));
        EnemySpawner.createBasicEnemy(new Vector3(64f,.5f,64f));  
        EnemySpawner.createBasicEnemy(new Vector3(64f,.5f,34f)); 

        //bottom right box
        EnemySpawner.createRevolverEnemy(new Vector3(52f, .5f, -15f));
        EnemySpawner.createRevolverEnemy(new Vector3(52f, .5f, 20f));

        //top left box
        EnemySpawner.createRevolverEnemy(new Vector3(-68f, .5f, 34f));
        EnemySpawner.createRevolverEnemy(new Vector3(-68f, .5f, -68f));


        //right & left side path
        EnemySpawner.createRevolverEnemy(new Vector3(50f,.5f,18f)); 
        EnemySpawner.createRevolverEnemy(new Vector3(50f,.5f,-11f));
        EnemySpawner.createRevolverEnemy(new Vector3(-50f,.5f,18f)); 
        EnemySpawner.createRevolverEnemy(new Vector3(-50f,.5f,-11f));
        
        //middle
        EnemySpawner.createRotatorEnemy(new Vector3(0f, .5f, 0f));
        EnemySpawner.createBasicEnemy(new Vector3(-12f, .5f, 0f));
        EnemySpawner.createBasicEnemy(new Vector3(12f, .5f, 0f));


        canChangeScene = false;
   
    }
    public void initializeLevel5(){
        //Heal & Place character at specific location.
        BasicMovement.curr_health = BasicMovement.max_health;
        canChangeScene = false;
    }
    public static void killAllEnemies(){
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        var objectsToDestroy = new System.Collections.Generic.List<GameObject>(enemies);
        foreach (GameObject enemy in objectsToDestroy)
        {
            print(enemy + " HAS NOW BEEN DESTROYED");
            Destroy(enemy);
        }
    }
    IEnumerator TwoSecCooldown(){
        yield return new WaitForSeconds(2f);
    }
}
  
