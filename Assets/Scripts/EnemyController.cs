using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEditor;


//This script is attached to all enemies and outlines how they are to behave.
public class EnemyController : MonoBehaviour
{
    [Header("GameObject Components")]
    private BulletTypes bulletGenerator; //DO NOT MAKE THIS SERIALIZEFIELD
    [SerializeField] private Transform playerTransform;
    [Header("Movement Information")]
    [SerializeField] private float radOfSatis = 15f;
    [SerializeField] private float playerRadOfSatis = 8f;
    [SerializeField] private float speedAndDir = 40f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float revolutionSpeed = 10f;
    [Header("Cooldown Information")]
    [SerializeField] private bool canShoot = true;
    [SerializeField] private bool canTakeDamage = true;
    [SerializeField] private bool canChangeDir = true;
    [SerializeField] private bool canTakeBeamDamage = true;
    [SerializeField] private bool BeamDPS = true;
    [SerializeField] private bool beamDuration = false;
    [Header("Visual Effects")]
    [SerializeField] private GameObject DamageIndicatorFX;
    [SerializeField] private GameObject deathFX;
    [SerializeField] private GameObject beamFX;
    
    // Start is called before the first frame update
    void Start()
    {
        BulletTypes newBullet = new BulletTypes();
        bulletGenerator = newBullet;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        // deathFX = GameObject.Find("ScifiTris_2");
        // deathFX = GameObject.FindGameObjectWithTag("FX_Death");
        // DamageIndicatorFX = GameObject.FindGameObjectWithTag("FX_DmgInd");
        deathFX = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VFX/ScifiTris_2.prefab");
        DamageIndicatorFX = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Damage_Indicator.prefab");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // deathFX.Play();
        if(playerTransform){
            switch(GetComponent<EnemyAttributes>().enemyType) {
                case "basic":
                    lookAndShoot(3);
                    break;
                case "revolver":
                    chaseAndShoot(1);
                    break;
                case "rotator":
                    rotate();
                    shoot(4);
                    break;
            }
            // if(GetComponent<EnemyAttributes>().enemyType == "basic") {
            //     chaseAndShoot(1);
            // }
        }
    }
     void OnTriggerEnter(Collider collision) {
        // Debug.Log(collision.gameObject);
        //if an enemy detects a collision with a bullet, inflict damage by subtracting class info.
        if((collision.CompareTag("Bullets") || collision.CompareTag ("Bullets_Reflect")) && canTakeDamage && collision.gameObject.GetComponent<bulletAttributes>().spawnedByPlayer == true) {
            
            //Subtract bullet damage from enemy.
            gameObject.GetComponent<EnemyAttributes>().enemyCurrentHealth -= collision.gameObject.GetComponent<bulletAttributes>().bulletDamage;
            //create and instantiate text damage indicator.
            createDmgIndicator(collision);
            Debug.Log("[Damage Inflicted on Enemy] New Health: " + gameObject.GetComponent<EnemyAttributes>().enemyCurrentHealth);
            //if under 1000, add damage value to the players beam gauge counter.
            if(BasicMovement.beamGauge <= 100f) {
                BasicMovement.beamGauge += collision.gameObject.GetComponent<bulletAttributes>().bulletDamage;
                Debug.Log("Beam Gauge Increased to: " + BasicMovement.beamGauge);
            }
        }
        if(collision.CompareTag("Beam") && canTakeBeamDamage) {
            Debug.Log("BEAM IS HITTING THE ENEMY: Trigger");
            StartCoroutine(inflictBeamDamage(collision,3f));
        }
    
        if(gameObject.GetComponent<EnemyAttributes>().enemyCurrentHealth <= 0) {
            killEnemy();
        }
            StartCoroutine(takeDamageCooldown());
        }
    //function that displays amount of damage on screen that an enemy has taken.
    private void createDmgIndicator(Collider collision){
        Quaternion rot = Quaternion.Euler(90f,0f,0f);
        // GameObject dmgTxt = Instantiate(Resources.Load("Damage_Indicator", typeof(GameObject))) as GameObject;
        GameObject dmgTxt = Instantiate(DamageIndicatorFX, transform.position, rot);
        if(collision.CompareTag("Beam")){
        dmgTxt.GetComponent<TextMesh>().text = (bulletInfo.bulletDamages[5]).ToString();   
        }
        else{
            dmgTxt.GetComponent<TextMesh>().text = (collision.gameObject.GetComponent<bulletAttributes>().bulletDamage).ToString();  
        }
        if(collision.CompareTag("Beam")){
            dmgTxt.GetComponent<TextMesh>().color = Color.cyan;
        }
        else{
            switch(collision.gameObject.GetComponent<bulletAttributes>().bullet) {
                case "basic":
                    dmgTxt.GetComponent<TextMesh>().color = Color.red;
                    break;
                case "fast":
                    dmgTxt.GetComponent<TextMesh>().color = Color.blue;
                    break;
                case "reflect":
                    dmgTxt.GetComponent<TextMesh>().color = Color.green;
                    break;
                case "spread":
                    dmgTxt.GetComponent<TextMesh>().color = Color.yellow;
                    break;
                case "radial":
                    dmgTxt.GetComponent<TextMesh>().color = Color.magenta;
                    break;
                case "beam":
                    dmgTxt.GetComponent<TextMesh>().color = Color.cyan;
                    break;
            }
        }
        Destroy(dmgTxt, 2);
    }
    private void lookAndShoot(int bulletType){
        //Vector from Enemy to Player
        Vector3 towardsTarget = playerTransform.position - transform.position;
        //Do nothing if player is outside of enemy range.
        if (!isInRange(0)) {
            return;
        } 
        towardsTarget = towardsTarget.normalized;
        //look towards player
        Quaternion targetRotation = Quaternion.LookRotation (towardsTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, .2f);
        //shoot bullets
        shoot(bulletType);
        
    }
    private void chaseAndShoot(int bulletType){

        //Vector from Enemy to Player
        Vector3 towardsTarget = playerTransform.position - transform.position;
        towardsTarget = towardsTarget.normalized;
        //look towards player
        Quaternion targetRotation = Quaternion.LookRotation (towardsTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, .2f);
        if(isInRange(1)){
            shoot(bulletType);
            // if enemy type revolver... revolve()
            revolvePlayer();
            return;
        }
        else {
        //move towards player
        Vector3 newPosition = transform.position;
        newPosition += transform.forward * moveSpeed * Time.fixedDeltaTime;
        transform.position = newPosition;
        //shoot bullets
        shoot(bulletType);
        }
    }
    private bool isInRange(int magDirection) {
        // 0 = check if player is close enough to enemy.
        if(magDirection == 0) {
            Vector3 towardsTarget = playerTransform.position - transform.position;
            // Debug.Log("mag from enemy to player: " + towardsTarget.magnitude + " Enemy rad of satis: " + radOfSatis + " Verdict: " + ((towardsTarget.magnitude >= radOfSatis) ? false : true));
            return (towardsTarget.magnitude >= radOfSatis) ? false : true;
        }
        //1 = check if enemy is close enough to player
        if (magDirection == 1) {
            Vector3 towardsEnemy = transform.position - playerTransform.position;
            // Debug.Log("mag from player to enemy: " + towardsEnemy.magnitude + " player of satis: " + playerRadOfSatis + " Verdict: " + ((towardsEnemy.magnitude >= playerRadOfSatis) ? false : true));
            return (towardsEnemy.magnitude >= playerRadOfSatis) ? false : true;
        }
        return false; 
    }
    private void revolvePlayer() {
        // float speedAndDir = 40f;
        float castDistance = 4f;
        Ray rayRight = new Ray(transform.position, transform.right);
        Ray rayLeft = new Ray(transform.position, -transform.right);
        RaycastHit hit1;
        RaycastHit hit2;
        bool raycastHit1 = Physics.Raycast(rayRight, out hit1, castDistance);
        bool raycastHit2 = Physics.Raycast(rayLeft, out hit2, castDistance);
        Debug.DrawRay(rayRight.origin, rayRight.direction * castDistance, raycastHit1 ? Color.red : Color.green);
        Debug.DrawRay(rayLeft.origin, rayLeft.direction * castDistance, raycastHit2 ? Color.red : Color.green);
        if((raycastHit1 || raycastHit2) && canChangeDir){
            canChangeDir = false;
            Debug.Log("Raycast HIT!");
            speedAndDir = -speedAndDir;
            StartCoroutine(revolveChangeDirCooldown(canChangeDir));
        }

        transform.RotateAround(playerTransform.position, Vector3.up, speedAndDir * Time.deltaTime);
    }
    // private void playDeathVFX(){
    //     if(death != null) {
    //         deathFX.transform.position = transform.position;
    //         deathFX.Play();
    //     }
    // }
    void OnDrawGizmos(){
        if(playerTransform) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y, transform.position.z), radOfSatis);
            Gizmos.DrawWireSphere(playerTransform.position, playerRadOfSatis);
        }
    }
    private void shoot(int bulletType){
        if(canShoot){
            bulletGenerator.instantiateBullet(bulletInfo.bullets[bulletType], 1f, 1f, transform, transform.rotation, false);
            StartCoroutine(shootBulletCooldown(bulletType));
        }
    }
    private void rotate(){  
         transform.Rotate(Vector3.up * GetComponent<EnemyAttributes>().enemySpeed * Time.deltaTime);
    }
    private void sprayNoCooldown(int bulletType){
        if(canShoot){
            bulletGenerator.instantiateBullet(bulletInfo.bullets[bulletType], 1f, 1f, transform, transform.rotation, false);
        }
    }
    IEnumerator shootBulletCooldown(int bulletType){
        canShoot = false;
        yield return new WaitForSeconds(bulletInfo.bulletCooldowns[bulletType]);
        canShoot = true;
    }
    IEnumerator revolveChangeDirCooldown(bool canChangeDir){
        Debug.Log("REVOLUTION COOLDOWN INITIATED");
        yield return new WaitForSeconds(3f);
        canChangeDir = true;
    }
    //Creates damage per second loop to harm enemy with beam
    IEnumerator inflictBeamDamage(Collider collision, float _time){
        float timeLimit = _time;
        float timeElapsed = 0f;
        while(timeElapsed <= timeLimit){
            createDmgIndicator(collision);
            gameObject.GetComponent<EnemyAttributes>().enemyCurrentHealth -= bulletInfo.bulletDamages[5];
            Debug.Log("[Damage Inflicted on Enemy] New Health: " + gameObject.GetComponent<EnemyAttributes>().enemyCurrentHealth);
            timeElapsed += Time.deltaTime;
            if(gameObject.GetComponent<EnemyAttributes>().enemyCurrentHealth <= 0){
                killEnemy();
            }
            yield return new WaitForSeconds(.2f);
        }
    }
    //sets loop to duration of bream.
    public void killEnemy(){
        Destroy(gameObject);
        GameObject death = Instantiate(deathFX, transform.position, transform.rotation);
        Destroy(death, 2);
    }
    IEnumerator beamTimeElapsed(){
        yield return new WaitForSeconds(3f);
        beamDuration = false;
    }
    IEnumerator takeDamageCooldown() {
        canTakeDamage = false;
        yield return new WaitForSeconds(.1f);
        canTakeDamage = true;
    }
    
    //gives time to play visual effects before enemy is destroyed.
    IEnumerator deathVFXCooldown() {
        yield return new WaitForSeconds(10f);
    }
}

