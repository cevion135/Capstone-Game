using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class bulletInfo {
    public static string[] bullets = {"basic", "fast", "reflect", "spread", "radial", "beam"};
    public static float[] bulletSpeeds = {10f, 20f, 10f, 8f, 4f};
    public static float[] bulletDamages = {4f, 10f, 8f, 6f, 2f, 1f};
    public static float[] bulletCooldowns = {.5f, 1.2f, 1f, 1.5f, 1.5f};
    public static Vector3 last_velocity;
    public static Dictionary<GameObject, int> collisionCounts = new Dictionary<GameObject, int>();
    public static GameObject[] bulletPrefabs;
    public static bool canCollide = true;
}
public class bulletAttributes : MonoBehaviour {
    [SerializeField] public float bulletSpeed;
    [SerializeField] public float bulletDamage;
    [SerializeField] public float bulletLifespan = 5f;
    [SerializeField] public float spawnTimer;
    [SerializeField] public bool spawnedByPlayer;
    [SerializeField] public string bullet;
}
public class BulletTypes : MonoBehaviour
{
    float bulletSpeed;
    float bulletDamage;
    bool canCollide = true;

    void Awake(){
        bulletInfo.bulletPrefabs = Resources.LoadAll<GameObject>("Bullets");
    }
    // void FixedUpdate(){
    //     if(gameObject){  
    //         float spawn = gameObject.GetComponent<bulletAttributes>().spawnTimer;
    //         float lifespan = gameObject.GetComponent<bulletAttributes>().bulletLifespan;
    //         if(Time.time - spawn > lifespan){
    //             Destroy(gameObject);
    //         }
    //     }
    // }
    public BulletTypes(){

    }
    public BulletTypes(float speed, float damage, GameObject prefab, Transform trans, Quaternion rot, bool whoSpawned, string bulletName) {
            Vector3 bulletSpawn = trans.position + trans.forward * 1.1f;
            Quaternion bulletSpawnQuat = new Quaternion(bulletSpawn.x, bulletSpawn.y, bulletSpawn.z, 0);

            // Loop that creates spread type bullets.
            if(prefab == bulletInfo.bulletPrefabs[3]) {
                CreateSpreadbullet(bulletSpawn, bulletSpawnQuat, prefab, trans, rot, speed, damage, whoSpawned, bulletName);
            }
            if(prefab == bulletInfo.bulletPrefabs[4]) {
                CreateRadialBullet(bulletSpawn, bulletSpawnQuat, prefab, trans, rot, speed, damage, whoSpawned, bulletName);
            }
            else{
                GameObject bullet = CreateBullet(bulletSpawn, bulletSpawnQuat, prefab, trans, rot, speed, damage, whoSpawned, bulletName);
                    if(prefab == bulletInfo.bulletPrefabs[2]) {
                        bullet.tag = "Bullets_Reflect";
                    }
            }
    }
        void OnTriggerEnter(Collider collision) {
        //collision cooldown. This removes possibility of multiple registered collisions during 1 event.
        if (!canCollide) return;
        //handles reflect bullets. 3 collisions will cause the bullet to despawn.
        if(gameObject.tag == "Bullets_Reflect" && collision.gameObject.tag == "Wall"){
            Vector3 reflectionDirection = calcReflDir(gameObject.transform.position, collision.transform.forward);
            Reflect(reflectionDirection);
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
                    // Debug.Log("BULLET HIT WALL AND WAS DESTROYED");
                    Destroy(gameObject);
                    break;
                case "Enemy":
                    //Damage infliction handled in "EnemyController.cs"
                    Destroy(gameObject);
                    // Debug.Log("Bullet hit enemy and was destroyed");
                    break;
                case "Player":
                    Destroy(gameObject);
                    break;
            }
        }
        canCollide = false;
        Invoke("ResetCollisionCooldown", 0.1f);
    }
    public void instantiateBullet(string type, float dmgMult, float speedMult, Transform trans, Quaternion rot, bool whoSpawned) {
        float dm = dmgMult;
        float sp = speedMult;
        Quaternion rotation = rot;
        switch (type) {
            case "basic":
            //basic bullet
                // Debug.Log("Called Successfully " + bulletPrefabs[0]);
                BulletTypes newBasic = new BulletTypes((bulletInfo.bulletSpeeds[0]*sp), (bulletInfo.bulletDamages[0]*dm), bulletInfo.bulletPrefabs[0], trans, rotation, whoSpawned, bulletInfo.bullets[0]);
                break;
            case "fast":
            //fast bullet
                BulletTypes newFast = new BulletTypes((bulletInfo.bulletSpeeds[1]*sp), (bulletInfo.bulletDamages[1]*dm), bulletInfo.bulletPrefabs[1], trans, rotation, whoSpawned, bulletInfo.bullets[1]);
                break;
            case "reflect":
            //reflect bullet
            BulletTypes newReflect = new BulletTypes((bulletInfo.bulletSpeeds[2]*sp), (bulletInfo.bulletDamages[2]*dm), bulletInfo.bulletPrefabs[2], trans, rotation, whoSpawned, bulletInfo.bullets[2]);
                break;
            case "spread":
            //spread bullet
            BulletTypes newSpread = new BulletTypes((bulletInfo.bulletSpeeds[3]*sp), (bulletInfo.bulletDamages[3]*dm), bulletInfo.bulletPrefabs[3], trans, rotation, whoSpawned, bulletInfo.bullets[3]);
                break;  
            case "radial":
            //spread bullet
                BulletTypes newRadial = new BulletTypes((bulletInfo.bulletSpeeds[4]*sp), (bulletInfo.bulletDamages[4]*dm), bulletInfo.bulletPrefabs[4], trans, rotation, whoSpawned, bulletInfo.bullets[4]); 
                break;
            default:
                Debug.Log("Missing Bullet Type");
                break;
        }
    }
    //Function that creates individual bullets and adds appropriate components.
    private GameObject CreateBullet(Vector3 bulletSpawn, Quaternion bulletSpawnQuat, GameObject prefab, Transform trans, Quaternion rot, float speed, float damage, bool whoSpawned, string bulletName){
        GameObject bullet = Instantiate(prefab, bulletSpawn, rot);
            Rigidbody Rb = bullet.AddComponent<Rigidbody>();
            SphereCollider sc = bullet.GetComponent<SphereCollider>();
            sc.isTrigger = true;
            bullet.AddComponent<BulletTypes>();           
            bullet.AddComponent<bulletAttributes>();
            bullet.GetComponent<bulletAttributes>().bulletSpeed = speed;
            bullet.GetComponent<bulletAttributes>().bulletDamage = damage;
            bullet.GetComponent<bulletAttributes>().spawnedByPlayer = whoSpawned;
            bullet.GetComponent<bulletAttributes>().spawnTimer = Time.time;
            bullet.GetComponent<bulletAttributes>().bullet = bulletName;
            Rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            Rb.velocity = bullet.transform.forward * speed;
            Debug.DrawLine(bullet.transform.position, bullet.transform.forward * 3f);
            bullet.tag = "Bullets";
            return bullet;
    }
    //Function that creates 3 bullets and shoots them at [blank] degree angle relative to forward vector. also adds appropriate components.
    private void CreateSpreadbullet(Vector3 bulletSpawn, Quaternion bulletSpawnQuat, GameObject prefab, Transform trans, Quaternion rot, float speed, float damage, bool whoSpawned, string bulletName){
        Quaternion[] bulletAngle;
        if(whoSpawned) {
            Quaternion rotVectLeft = rot * Quaternion.Euler(0f, BasicMovement.spreadValue, 0f);
            Quaternion rotVectRight = rot * Quaternion.Euler(0f, -BasicMovement.spreadValue, 0f);
            bulletAngle = new[]  {rotVectLeft, rot, rotVectRight};
        }
        else {
            float angle = 30f;
            Quaternion rotVectLeft = rot * Quaternion.Euler(0f, angle, 0f);
            Quaternion rotVectRight = rot * Quaternion.Euler(0f, -angle, 0f);
            bulletAngle = new[] {rotVectLeft, rot, rotVectRight};
           
        }
        for(int i = 0; i <= 2; i++) {
            //setting bullet properties
            GameObject bullet = Instantiate(prefab, bulletSpawn, bulletAngle[i]);
            Rigidbody Rb = bullet.AddComponent<Rigidbody>();
            SphereCollider sc = bullet.GetComponent<SphereCollider>();
            sc.isTrigger = true;
            bullet.AddComponent<BulletTypes>();
            bullet.AddComponent<bulletAttributes>();
            bullet.GetComponent<bulletAttributes>().bulletSpeed = speed;
            bullet.GetComponent<bulletAttributes>().bulletDamage = damage;
            bullet.GetComponent<bulletAttributes>().spawnedByPlayer = whoSpawned;
            bullet.GetComponent<bulletAttributes>().spawnTimer = Time.time;
            bullet.GetComponent<bulletAttributes>().bullet = bulletName;
            
            Rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            Rb.velocity = bullet.transform.forward * speed;
            bulletInfo.last_velocity = Rb.velocity;
            bullet.tag = "Bullets";
        }
    }
    private void CreateRadialBullet(Vector3 bulletSpawn, Quaternion bulletSpawnQuat, GameObject prefab, Transform trans, Quaternion rot, float speed, float damage, bool whoSpawned, string bulletName){
        float numOfBullets = 8;
        float radialStep = 360f/numOfBullets;
        for(int i=0; i<numOfBullets; i++){
            float angle = i * radialStep;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            GameObject bullet = Instantiate(prefab, bulletSpawn, Quaternion.identity);

            Rigidbody Rb = bullet.AddComponent<Rigidbody>();
            SphereCollider sc = bullet.GetComponent<SphereCollider>();
            sc.isTrigger = true;
            bullet.AddComponent<BulletTypes>();
            bullet.AddComponent<bulletAttributes>();
            bullet.GetComponent<bulletAttributes>().bulletSpeed = speed;
            bullet.GetComponent<bulletAttributes>().bulletDamage = damage;
            bullet.GetComponent<bulletAttributes>().spawnedByPlayer = whoSpawned;
            bullet.GetComponent<bulletAttributes>().spawnTimer = Time.time;
             bullet.GetComponent<bulletAttributes>().bullet = bulletName;
            Rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            Rb.velocity = direction * speed;
            bulletInfo.last_velocity = Rb.velocity;
            bullet.tag = "Bullets";

        }
    }
    //Math function for bullet reflections upon collision
    void Reflect(Vector3 direction) {
        transform.forward = direction;
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = direction * bulletInfo.bulletSpeeds[2];
    }
    //Calculates the reflection direction of a bullet.
    Vector3 calcReflDir(Vector3 bulletPosition, Vector3 surfacePosition){
        Vector3 incidentDirection = transform.forward;
        Vector3 reflectionDirection = Vector3.Reflect(incidentDirection, surfacePosition);
        return reflectionDirection.normalized;
    }
    private void ResetCollisionCooldown(){
        canCollide = true;
    }
}