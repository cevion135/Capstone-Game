using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BulletTypes : MonoBehaviour
{
    private float basic_speed = 10f;
    private float basic_damage = 10f;
    private float basic_cooldown = .5f;
    private bool basic_reflect = false;

    private float fast_speed = 20f;
    private float fast_damage = 7f;
    private float fast_cooldown = .1f;
    private bool fast_reflect = false;

    private float reflect_speed = 10f;
    private float reflect_damage = 10f;
    private float reflect_cooldown = 1f;
    private bool reflect_reflect = true;
    private Vector3 last_velocity;
    private Dictionary<GameObject, int> collisionCounts = new Dictionary<GameObject, int>();

    private float spread_speed = 8f;
    private float spread_damage = 12f;
    private float spread_cooldown = 2f;
    private bool spread_reflect = false;
    private static GameObject[] bulletPrefabs;
    private bool canCollide = true;

    void Start(){
        bulletPrefabs = Resources.LoadAll<GameObject>("Bullets");
    }

    public BulletTypes(){

    }
    public BulletTypes(float speed, float damage, bool refl, GameObject prefab, Transform trans, Quaternion rot) {
            // Debug.Log("New [" + prefab + "] Created");
            Vector3 bulletSpawn = trans.position + trans.forward * 1.1f;
            Quaternion bulletSpawnQuat = new Quaternion(bulletSpawn.x, bulletSpawn.y, bulletSpawn.z, 0);

            // Loop that creates spread type bullets.
            if(prefab == bulletPrefabs[3]) {
                CreateSpreadbullet(bulletSpawn, bulletSpawnQuat, prefab, trans, rot, speed);
            }
            else{
                GameObject bullet = CreateBullet(bulletSpawn, bulletSpawnQuat, prefab, trans, rot, speed);
                    if(prefab == bulletPrefabs[2]) {
                        bullet.tag = "Bullets_Reflect";
                    }
            }
    }
    void OnCollisionEnter(Collision collision) {
        //collision cooldown. This removes possibility of multiple registered collisions during 1 event.
        if (!canCollide) return;
        //handles reflect bullets.
        if(gameObject.tag == "Bullets_Reflect"){
            Reflect(collision);
            GameObject collidedObject = gameObject;
            if (!collisionCounts.ContainsKey(collidedObject)){
                collisionCounts[collidedObject] = 1;
            } else {
                collisionCounts[collidedObject]++;
            }
             if (collisionCounts[collidedObject] == 3) {
                Destroy(collidedObject);
                Debug.Log(collidedObject.name + " destroyed due to excessive collisions.");
                collisionCounts.Remove(collidedObject); 
            }
        }   
        //handles every other bullet.
        else{
            switch(collision.gameObject.tag) {
                case "Wall":
                    Destroy(gameObject);
                    break;
                case "Enemy":
                    //inflict damage then...
                    Destroy(gameObject);
                    break;
            }
        }
        canCollide = false;
        Invoke("ResetCollisionCooldown", 0.1f);
    }
    public void instantiateBullet(string type, float dmgMult, float speedMult, Transform trans, Quaternion rot) {
        float dm = dmgMult;
        float sp = speedMult;
        // Debug.Log("String of type: " + type + " passed to instantiation method");
        // Vector3 position = pos;
        Quaternion rotation = rot;
        switch (type) {
            case "basic":
            //basic bullet
                // Debug.Log("Called Successfully " + bulletPrefabs[0]);
                BulletTypes newBasic = new BulletTypes((basic_speed*sp), (basic_damage*dm), basic_reflect, bulletPrefabs[0], trans, rotation);
                break;
            case "fast":
            //fast bullet
                BulletTypes newFast = new BulletTypes((fast_speed*sp), (fast_damage*dm), fast_reflect, bulletPrefabs[1], trans, rotation);
                break;
            case "reflect":
            //reflect bullet
            BulletTypes newReflect = new BulletTypes((reflect_speed*sp), (reflect_damage*dm), reflect_reflect, bulletPrefabs[2], trans, rotation);
                break;
            case "spread":
            //spread bullet
            BulletTypes newSpread = new BulletTypes((spread_speed*sp), (spread_damage*dm), spread_reflect, bulletPrefabs[3], trans, rotation);
                break;    
            default:
                Debug.Log("Missing Bullet Type");
                break;
        }
    }
    //Function that creates individual bullets and adds appropriate components.
    private GameObject CreateBullet(Vector3 bulletSpawn, Quaternion bulletSpawnQuat, GameObject prefab, Transform trans, Quaternion rot, float speed){
        GameObject bullet = Instantiate(prefab, bulletSpawn, rot);
            Rigidbody Rb = bullet.AddComponent<Rigidbody>();
            SphereCollider sc = bullet.AddComponent<SphereCollider>();
            bullet.AddComponent<BulletTypes>();
            Rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            Rb.velocity = trans.forward * speed;
            bullet.tag = "Bullets";
            return bullet;
    }
    //Function that creates 3 bullets and shoots them at [blank] degree angle relative to forward vector. also adds appropriate components.
    private void CreateSpreadbullet(Vector3 bulletSpawn, Quaternion bulletSpawnQuat, GameObject prefab, Transform trans, Quaternion rot, float speed){
        float angle = 60f;
        Quaternion rotation1 = Quaternion.Euler(0f, angle, 0f);
        Quaternion rotation2 = Quaternion.Euler(0f, -angle, 0f);
        Quaternion rotVectLeft = rotation1 * bulletSpawnQuat;
        Quaternion rotVectRight = rotation2 * bulletSpawnQuat;
        Quaternion[] bulletAngle = {rotVectLeft, rot, rotVectRight};
        for(int i = 0; i <= 2; i++) {
            GameObject bullet = Instantiate(prefab, bulletSpawn, bulletAngle[i]);
            Rigidbody Rb = bullet.AddComponent<Rigidbody>();
            SphereCollider sc = bullet.AddComponent<SphereCollider>();
            bullet.AddComponent<BulletTypes>();
            Rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            Rb.velocity = trans.forward * speed;
            last_velocity = Rb.velocity;
            bullet.tag = "Bullets";
    
            // Gizmos.color = Color.blue;
            //  Gizmos.DrawLine(transform.position, transform.position + transform.forward * lineLength);
            // Time.timeScale = .2f;
        }
    }
    //Math function for bullet reflections upon collision
    void Reflect(Collision coll){
        Debug.Log("HIT OBJECT");
        Rigidbody bulletRb = gameObject.GetComponent<Rigidbody>();
        // Debug.Log("RIGID BODY OF GAME OBJECT: [" + otherRigidbody + "]");
        Vector3 lastVelocity = bulletRb.velocity;
        // Debug.Log("VELOCITY OF GAME OBJECT: [" + lastVelocity + "]");
        var speed = lastVelocity.magnitude;
        // Debug.Log("MAGNITUDE OF GAME OBJECT: [" + speed + "]");
        var direction = Vector3.Reflect(bulletRb.transform.forward, coll.contacts[0].normal);
        bulletRb.velocity = direction * Mathf.Max(reflect_speed, 0f);
    }
    private void ResetCollisionCooldown()
    {
        canCollide = true;
    }
}