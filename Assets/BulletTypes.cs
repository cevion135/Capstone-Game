using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class bulletInfo {
    public static string[] bullets = {"basic", "fast", "reflect", "spread"};
    public static float[] bulletSpeeds = {10f, 20f, 10f, 8f};
    public static float[] bulletDamages = {10f, 7f, 10f, 12f};
    public static float[] bulletCooldowns = {.5f, .1f, 1f, 1.5f};
    public static Vector3 last_velocity;
    public static Dictionary<GameObject, int> collisionCounts = new Dictionary<GameObject, int>();
    public static GameObject[] bulletPrefabs;
    public static bool canCollide = true;
}
public class BulletTypes : MonoBehaviour
{
    float bulletSpeed;
    float bulletDamage;
    bool canCollide = true;

    void Start(){
        bulletInfo.bulletPrefabs = Resources.LoadAll<GameObject>("Bullets");
    }

    public BulletTypes(){

    }
    public BulletTypes(float speed, float damage, GameObject prefab, Transform trans, Quaternion rot) {
            this.bulletSpeed = speed;
            this.bulletDamage = damage;
            // Debug.Log("New [" + prefab + "] Created... This Bullets SPEED: [" + bulletSpeed + "] This Bullets DAMAGE: [" + bulletDamage + "]");
            Vector3 bulletSpawn = trans.position + trans.forward * 1.1f;
            Quaternion bulletSpawnQuat = new Quaternion(bulletSpawn.x, bulletSpawn.y, bulletSpawn.z, 0);

            // Loop that creates spread type bullets.
            if(prefab == bulletInfo.bulletPrefabs[3]) {
                CreateSpreadbullet(bulletSpawn, bulletSpawnQuat, prefab, trans, rot, speed);
            }
            else{
                GameObject bullet = CreateBullet(bulletSpawn, bulletSpawnQuat, prefab, trans, rot, speed);
                    if(prefab == bulletInfo.bulletPrefabs[2]) {
                        bullet.tag = "Bullets_Reflect";
                    }
            }
    }
        void OnTriggerEnter(Collider collision) {
        //collision cooldown. This removes possibility of multiple registered collisions during 1 event.
        if (!canCollide) return;
        //handles reflect bullets.
        // Debug.Log(collision.tag + " At position: " + collision.transform.position);
        if(gameObject.tag == "Bullets_Reflect" && collision.gameObject.tag == "Wall"){
            Vector3 reflectionDirection = calcReflDir(gameObject.transform.position, collision.transform.forward);
            Reflect2(reflectionDirection);
            GameObject collidedObject = gameObject;
            if (!bulletInfo.collisionCounts.ContainsKey(collidedObject)){
                bulletInfo.collisionCounts[collidedObject] = 1;
            } else {
                bulletInfo.collisionCounts[collidedObject]++;
            }
             if (bulletInfo.collisionCounts[collidedObject] == 3) {
                Destroy(collidedObject);
                Debug.Log(collidedObject.name + " destroyed due to excessive collisions.");
                bulletInfo.collisionCounts.Remove(collidedObject); 
            }
        }   
        //handles every other bullet.
        else{
            switch(collision.gameObject.tag) {
                case "Wall":
                    // Debug.Log("BULLET DESTROYED");
                    Destroy(gameObject);
                    break;
                case "Enemy":
                    //inflict damage then...
                    Destroy(gameObject);
                    break;
                case "Player":
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
                BulletTypes newBasic = new BulletTypes((bulletInfo.bulletSpeeds[0]*sp), (bulletInfo.bulletDamages[0]*dm), bulletInfo.bulletPrefabs[0], trans, rotation);
                break;
            case "fast":
            //fast bullet
                BulletTypes newFast = new BulletTypes((bulletInfo.bulletSpeeds[1]*sp), (bulletInfo.bulletDamages[1]*dm), bulletInfo.bulletPrefabs[1], trans, rotation);
                break;
            case "reflect":
            //reflect bullet
            BulletTypes newReflect = new BulletTypes((bulletInfo.bulletSpeeds[2]*sp), (bulletInfo.bulletDamages[2]*dm), bulletInfo.bulletPrefabs[2], trans, rotation);
                break;
            case "spread":
            //spread bullet
            BulletTypes newSpread = new BulletTypes((bulletInfo.bulletSpeeds[3]*sp), (bulletInfo.bulletDamages[3]*dm), bulletInfo.bulletPrefabs[3], trans, rotation);
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
            SphereCollider sc = bullet.GetComponent<SphereCollider>();
            sc.isTrigger = true;
            bullet.AddComponent<BulletTypes>();
            Rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            Rb.velocity = bullet.transform.forward * speed;
            Debug.DrawLine(bullet.transform.position, bullet.transform.forward * 3f);
            bullet.tag = "Bullets";
            return bullet;
    }
    //Function that creates 3 bullets and shoots them at [blank] degree angle relative to forward vector. also adds appropriate components.
    private void CreateSpreadbullet(Vector3 bulletSpawn, Quaternion bulletSpawnQuat, GameObject prefab, Transform trans, Quaternion rot, float speed){
        
        float angle = 30f;
        Quaternion rotVectLeft = rot * Quaternion.Euler(0f, angle, 0f);
        Quaternion rotVectRight = rot * Quaternion.Euler(0f, -angle, 0f);

        Quaternion[] bulletAngle = {rotVectLeft, rot, rotVectRight};
        for(int i = 0; i <= 2; i++) {
            GameObject bullet = Instantiate(prefab, bulletSpawn, bulletAngle[i]);
            Debug.Log("Bullet rotation: " + bullet.transform.rotation);
            Rigidbody Rb = bullet.AddComponent<Rigidbody>();
            SphereCollider sc = bullet.GetComponent<SphereCollider>();
            sc.isTrigger = true;
            bullet.AddComponent<BulletTypes>();
            Rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            Rb.velocity = bullet.transform.forward * speed;
            bulletInfo.last_velocity = Rb.velocity;
            bullet.tag = "Bullets";
        }
    }
    //Math function for bullet reflections upon collision
    void Reflect(Collider coll){
        Debug.Log("HIT OBJECT");
        Rigidbody bulletRb = gameObject.GetComponent<Rigidbody>();
        // Debug.Log("RIGID BODY OF GAME OBJECT: [" + otherRigidbody + "]");
        Vector3 lastVelocity = bulletRb.velocity;
        // Debug.Log("VELOCITY OF GAME OBJECT: [" + lastVelocity + "]");
        var speed = lastVelocity.magnitude;
        // Debug.Log("MAGNITUDE OF GAME OBJECT: [" + speed + "]");
    }
    void Reflect2(Vector3 direction) {
        transform.forward = direction;
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = direction * bulletInfo.bulletSpeeds[2];
        // gameObject.GetComponent<Rigidbody>.velocity = direction * bulletInfo.bulletSpeeds[2];
    }
    //Calculates the reflection direction of a bullet.
    Vector3 calcReflDir(Vector3 bulletPosition, Vector3 surfacePosition){
        Vector3 incidentDirection = transform.forward;
        Vector3 reflectionDirection = Vector3.Reflect(incidentDirection, surfacePosition);
        return reflectionDirection.normalized;
    }
    private void ResetCollisionCooldown()
    {
        // Debug.Log("Collision Cooldown Set To True!");
        canCollide = true;
    }
}