using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float radOfSatis;
    [SerializeField] private float playerRadOfSatis;
    private BulletTypes bulletGenerator;
    private float moveSpeed = 5f;
    private bool canShoot = true;
    
    // Start is called before the first frame update
    void Start()
    {
        BulletTypes newBullet = new BulletTypes();
        bulletGenerator = newBullet;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // (likely unnecessary) **foreach gameobject with tag "Enemy"**

        //switch(gameObject.GetComponent<EnemySpawner>().enemyType);
        //case "basic":
            //do corresponding attack pattern.  
        //case "rotator":
            //do corresponding attack pattern.  
        chaseAndShoot(1);
    }
    private void lookAndShoot(int bulletType){
        //Vector from Enemy to Player
        Vector3 towardsTarget = playerTransform.position - transform.position;
        //Do nothing if player is outside of enemy range.
        if (towardsTarget.magnitude >= radOfSatis) {
            return;
        } 
        towardsTarget = towardsTarget.normalized;
        //look towards player
        Quaternion targetRotation = Quaternion.LookRotation (towardsTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, .02f);
        //shoot bullets
        shoot(bulletType);
        
    }
    private void chaseAndShoot(int bulletType){
        //Vector from Enemy to Player
        Vector3 towardsTarget = playerTransform.position - transform.position;
        towardsTarget = towardsTarget.normalized;
        //look towards player
        Quaternion targetRotation = Quaternion.LookRotation (towardsTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, .02f);
        //move towards player
        Vector3 newPosition = transform.position;
        newPosition += transform.forward * moveSpeed * Time.fixedDeltaTime;
        transform.position = newPosition;
        //shoot bullets
        shoot(bulletType);
    }
    private void revolvePlayer() {
        
    }
    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y, transform.position.z), radOfSatis);
    }
    private void shoot(int bulletType){
        if(canShoot){
            bulletGenerator.instantiateBullet(bulletInfo.bullets[bulletType], 1f, 1f, transform, transform.rotation);
            StartCoroutine(shootBulletCooldown());
        }
    }
    private void sprayNoCooldown(int bulletType){
        if(canShoot){
            bulletGenerator.instantiateBullet(bulletInfo.bullets[bulletType], 1f, 1f, transform, transform.rotation);
        }
    }
    IEnumerator shootBulletCooldown(){
        canShoot = false;
        yield return new WaitForSeconds(.4f);
        canShoot = true;
    }
}

