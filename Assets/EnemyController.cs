using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float radiusOfSatisfaction;
    private BulletTypes bulletGenerator;
    private bool canShoot = true;
    private float max_health = 200f;
    // Start is called before the first frame update
    void Start()
    {
        BulletTypes newBullet = new BulletTypes();
        bulletGenerator = newBullet;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        kinematicArrive();    
    }
    private void kinematicArrive(){
        //Vector from Enemy to Player
        Vector3 towardsTarget = playerTransform.position - transform.position;
        // Debug.Log("Vector Towards Player: [" + towardsTarget + "]" );
        // Debug.Log("Magnitude: [" + towardsTarget.magnitude + "] RoS: [" + radiusOfSatisfaction + "]");
        if (towardsTarget.magnitude >= radiusOfSatisfaction) {
            // Debug.Log("Rad of Satisfaction met");
            return;
        }
        
        towardsTarget = towardsTarget.normalized;
        //look towards player
        Quaternion targetRotation = Quaternion.LookRotation (towardsTarget);
        transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, .02f);
        shoot();
        
    }
    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y, transform.position.z), radiusOfSatisfaction);
    }
    private void shoot(){
        if(canShoot){
            bulletGenerator.instantiateBullet(bulletInfo.bullets[0], 1f, 1f, transform, transform.rotation);
            StartCoroutine(shootBulletCooldown());
        }
    }
    IEnumerator shootBulletCooldown(){
        canShoot = false;
        yield return new WaitForSeconds(.4f);
        canShoot = true;
    }
}

