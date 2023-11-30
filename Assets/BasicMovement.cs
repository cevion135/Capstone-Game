using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BasicMovement : MonoBehaviour
{
    [Header("Player Information")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float dashForce;
    [SerializeField] public static float max_health = 100f;
    [SerializeField] public static float curr_health;
    [SerializeField] private Rigidbody rb;
    // [SerializeField] private BoxCollider bc;
    [SerializeField] private Camera cam;
    [SerializeField] public MeshRenderer meshRenderer;

    [Header("Cooldown Information")]
    [SerializeField] private bool canShoot = true;
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool canSwitchBullets = true;
    [SerializeField] private bool canTakeDamage = true;
    [SerializeField] private bool canReflect = true;
    [SerializeField] private bool canUseBeam = true;
    [Header("VFX")]
    [SerializeField] private VisualEffect VFX_Reflect;
    [SerializeField] private VisualEffect VFX_Beam;
    [Header("Other Information")]
    [SerializeField] public static bool beamActive = false;
    private int selectionIterator = 0;
    private float cd_reduction = 1f;
    private BulletTypes bulletGenerator;
    private float[] playerBulletCooldowns = {.1f, .5f, .2f, .35f};

    private float min = 5f;
    private float max = 50f;
    private float sensitivity = 5f;
    public static float spreadValue;
    private float reflectionAngle = 45f;
    public static float beamGauge = 0f;


    //public float turningDelay = 5f;
    //public float lastRotationTime;
    //public bool canChangeShape;
    //public Vector3 slideShape = new Vector3(0.3031172f, 0.181347f, 0.2000002f);

    
    //public Collision lastCollision = null;

    // public enum bulletTypes{
    //     basic,
    //     fast,
    //     reflect,
    //     spread,
    //     beam
    // }
    private void Awake(){
    
    }
    void Start() {
        rb.drag = 1;
        rb.constraints = RigidbodyConstraints.FreezePositionY |
        RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        BulletTypes newBullet = new BulletTypes();
        bulletGenerator = newBullet;
        curr_health = max_health;
        Transform child = transform.Find("Player");
        meshRenderer = child.GetComponent<MeshRenderer>();
        spreadValue = (min + max) / 2f;
        DontDestroyOnLoad(gameObject);
        // VFX_Reflect.SetActive(false);
    }
    void FixedUpdate()
    {
        
        trackMouse();
        movementStyle();
        if((Input.GetKey("e") && canShoot)) {
            bulletGenerator.instantiateBullet(bulletInfo.bullets[selectionIterator], 1f, 1f, transform, transform.rotation, true);
            StartCoroutine(shootBulletCooldown());
        }
        if(Input.GetKey("q") && canSwitchBullets) {
            changeBulletType();
            StartCoroutine(changeBulletCooldown());
        }
        if(Input.GetKey(KeyCode.LeftShift) && canDash) {
            // Vector3 currentVelocity = rb.velocity;
            dash();
            StartCoroutine(dash());
        }
        if(Input.GetKey(KeyCode.Mouse1) && canReflect){
            checkForBullet();
            VFX_Reflect.Play();
            StartCoroutine(reflectionCooldown());
        }
        if(Input.GetKey(KeyCode.Mouse0) && (beamGauge >= 10f) && canUseBeam) {
            useBeam();
        }
    }
    private void checkForBullet(){
        // Debug.Log("Checking for Bullets...");

        //loop that creates a radial spread in front of the player.
        for(float angle = -reflectionAngle; angle <= reflectionAngle; angle += 5f){
            //creates a coneshaped raycast to check for bullets.
            Vector3 playerForward = transform.forward;
            Vector3 rotatedDir = Quaternion.Euler(0, angle, 0) * playerForward;
            Debug.DrawRay(transform.position, rotatedDir * 3f, Color.red, 0.1f);
            RaycastHit[] hits = Physics.RaycastAll(transform.position, rotatedDir, 3f);
            //if a bullet is detected in spread, send to reflect function.
            foreach (RaycastHit hit in hits)
            {
                //MAKE SURE IT IS ONLY DOING THIS WITH BULLET OBJECTS!!!
                GameObject bullet = hit.collider.gameObject;
                if (bullet != null)
                {
                    // Reflect the bullet based on the player's forward direction.
                    reflectBullet(bullet, playerForward);
                }
            }
        }
    }
    private void useBeam(){
        beamActive = true;
        VFX_Beam.Play();
        Debug.Log("Imagine this is a BIG GIANT BEAM!");

        //reset beam gauge
        beamGauge = 0f;
        StartCoroutine(waitBeamLength());
    }
    //function that reflects bullet in direction relative to players forward vector.
    private void reflectBullet(GameObject bullet, Vector3 reflDir){
        //calculate direction.
        Vector3 reflectedDirection = Vector3.Reflect(reflDir, Vector3.up);
        //check for null values in rigidbody and bullet speed.
        if((bullet.GetComponent<Rigidbody>() != null) && (bullet.GetComponent<bulletAttributes>().bulletSpeed != null)) {
            //assign direction and speed to the bullets rigidbody velocity component.
            bullet.GetComponent<Rigidbody>().velocity = reflectedDirection * bullet.GetComponent<bulletAttributes>().bulletSpeed;
            //transfers ownership of bullet from enemy to player.
            bullet.GetComponent<bulletAttributes>().spawnedByPlayer = !bullet.GetComponent<bulletAttributes>().spawnedByPlayer;
        }
    }
    // void movementStyle1() {
    //     //movement by individual key inputs
    //     if((Input.GetKey("s"))) {
    //             rb.AddForce(0, 0, -movementSpeed*Time.deltaTime);
    //     }
    //     if((Input.GetKey("d"))) {
    //             rb.AddForce(movementSpeed*Time.deltaTime, 0, 0);     
    //     }    
    //     if((Input.GetKey("w"))) {
    //             rb.AddForce(0, 0, movementSpeed*Time.deltaTime);
    //     }
    //     if((Input.GetKey("a"))) {
    //             rb.AddForce(-movementSpeed*Time.deltaTime, 0, 0);   
    //     }
    // }
    void movementStyle(){
        //movement by Input.GetAxis with Decelerating force.
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        Vector3 acceleration = movement * movementSpeed * 1f;

        rb.AddForce(acceleration * Time.deltaTime);
    }
    // void trackMouse() {

        //SEMI WORKING
        // Ray cameraRay = cam.ScreenPointToRay(Input.mousePosition);
        // Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        // float rayLength;

        // if (groundPlane.Raycast(cameraRay, out rayLength)) {
            // Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            // Vector3 towardsMouse = pointToLook - centerOfPlayer.position;
            // towardsMouse.y = 0;
            // towardsMouse = towardsMouse.normalized;
            // Quaternion targetRotation = Quaternion.LookRotation (towardsMouse);
            // Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue);
            // Debug.DrawLine(centerOfPlayer.position, towardsMouse, Color.red);
            // Debug.Log("Casting BLUE Ray: " + pointToLook);
            // Debug.Log("Casting RED Ray: " + towardsMouse);
            // Debug.Log("Printing Look Rotation: " + towardsMouse);
            // Debug.Log("Printing Target Rotation: " + targetRotation);
            // centerOfPlayer.rotation = Quaternion.Slerp(centerOfPlayer.rotation, targetRotation, Time.deltaTime * 5f);
        // }
        
        


        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // RaycastHit hit;
        // if (Physics.Raycast(ray, out hit)) {
        //     Vector3 mousePosition = hit.point;
        //     Vector3 direction = mousePosition - centerOfPlayer.position;
        //     // direction.y = 0;
        //     Quaternion targetRotation = Quaternion.LookRotation(direction);
        //     centerOfPlayer.rotation = Quaternion.Slerp(centerOfPlayer.rotation, targetRotation, 1f * Time.deltaTime);
        // }
        // return;
    // }
    void trackMouse(){
        //Section for character rotation
       Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
       Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;
        Vector3 pointDir = Vector3.zero;

        if(groundPlane.Raycast(camRay, out rayLength)) {
            pointDir = camRay.GetPoint(rayLength);
        }
        transform.LookAt(new Vector3(pointDir.x, transform.position.y, pointDir.z));
        //Section for tracking scroll wheel value
        spreadValue += Input.mouseScrollDelta.y * sensitivity;
        spreadValue = Mathf.Clamp(spreadValue, min, max);
    }
     void OnTriggerEnter(Collider collision) {
        //if an enemy detects a collision with a bullet, inflict damage by subtracting class info.
        if((collision.CompareTag("Bullets") || collision.CompareTag ("Bullets_Reflect")) && canTakeDamage && collision.gameObject.GetComponent<bulletAttributes>().spawnedByPlayer == false) {
            // print("Bullet Damage [Before]: " + collision.gameObject.GetComponent<bulletAttributes>().bulletDamage);
            // print("Current Health [Before]: " + gameObject.GetComponent<EnemyAttributes>().enemyCurrentHealth);
            curr_health -= collision.gameObject.GetComponent<bulletAttributes>().bulletDamage;
            // print("Bullet Damage [After]: " + collision.gameObject.GetComponent<bulletAttributes>().bulletDamage);
            print("[Damage Inflicted on Player] New Health: " + curr_health);
            if(curr_health <= 0) {
                Destroy(gameObject);
            }
            StartCoroutine(takeDamageCooldown());
        }
    }
    IEnumerator dash(){
        StartCoroutine(dashMeshRenderer());
        rb.drag = 2;
        Vector3 currentVelocity = rb.velocity;
        Debug.Log("Dashing Initiated");
        canDash = false;
        Vector3 dashDirection = Quaternion.Euler(0, transform.eulerAngles.y, 0) * Vector3.forward;
        rb.AddForce(dashDirection*dashForce);
        yield return new WaitForSeconds(1.5f);
        canDash = true;
        rb.drag = 1;
    }
    IEnumerator dashMeshRenderer(){
        yield return new WaitForSeconds(.03f);
        meshRenderer.enabled = false;
        yield return new WaitForSeconds(.1f);
        meshRenderer.enabled = true;

    }
    //places x amount of cooldown time for each individual bullet.
    IEnumerator shootBulletCooldown(){
        canShoot = false;
        yield return new WaitForSeconds(playerBulletCooldowns[selectionIterator]*cd_reduction);
        canShoot = true;
    }
     //cooldown to prevent from switching bullet types too fast.
    IEnumerator changeBulletCooldown(){
        canSwitchBullets = false;
        yield return new WaitForSeconds(.3f);
        canSwitchBullets = true;
    }
    //player grace period after taking damage.
    IEnumerator takeDamageCooldown(){
        canTakeDamage = false;
        meshRenderer.enabled = false;
        yield return new WaitForSeconds(.1f);
        meshRenderer.enabled = true;
        canTakeDamage = true;
    }
    IEnumerator reflectionCooldown(){
        canReflect = false;
        yield return new WaitForSeconds(1f);
        canReflect = true;
    }
    IEnumerator waitBeamLength(){
        yield return new WaitForSeconds(VFX_Beam.GetFloat("BeamDuration")); //MAKE THIS VARIABLE MATCH DURATION IN VFX GRAPH
        beamActive = false;
    }
    //cycles through array containing all available bullet types.
    void changeBulletType() {
        Debug.Log(bulletInfo.bulletPrefabs[4]);
        if (selectionIterator == 3){
            selectionIterator = 0;
            return;
        }
        selectionIterator++;
        Debug.Log("Bullet Type #" + selectionIterator + " Associating Bullet: " + bulletInfo.bullets[selectionIterator]);
    }
}