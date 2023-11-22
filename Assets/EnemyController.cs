using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is attached to all enemies and outlines how they are to behave.
public class EnemyController : MonoBehaviour
{
    [Header("GameObject Components")]
    [SerializeField]private BulletTypes bulletGenerator;
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
    
    // Start is called before the first frame update
    void Start()
    {
        BulletTypes newBullet = new BulletTypes();
        bulletGenerator = newBullet;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
        //if an enemy detects a collision with a bullet, inflict damage by subtracting class info.
        if((collision.CompareTag("Bullets") || collision.CompareTag ("Bullets_Reflect")) && canTakeDamage && collision.gameObject.GetComponent<bulletAttributes>().spawnedByPlayer == true) {
            // print("Bullet Damage [Before]: " + collision.gameObject.GetComponent<bulletAttributes>().bulletDamage);
            // print("Current Health [Before]: " + gameObject.GetComponent<EnemyAttributes>().enemyCurrentHealth);
            gameObject.GetComponent<EnemyAttributes>().enemyCurrentHealth -= collision.gameObject.GetComponent<bulletAttributes>().bulletDamage;
            // print("Bullet Damage [After]: " + collision.gameObject.GetComponent<bulletAttributes>().bulletDamage);
            print("[Damage Inflicted on Enemy] New Health: " + gameObject.GetComponent<EnemyAttributes>().enemyCurrentHealth);
            if(gameObject.GetComponent<EnemyAttributes>().enemyCurrentHealth <= 0) {
                Destroy(gameObject);
            }
            StartCoroutine(takeDamageCooldown());
        }
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
    IEnumerator takeDamageCooldown() {
        canTakeDamage = false;
        yield return new WaitForSeconds(.1f);
        canTakeDamage = true;
    }
}

