using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using TMPro;
using UnityEngine.SceneManagement;

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
    [SerializeField] public static bool playerStatus = true;

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
    [SerializeField] private GameObject VFX_BeamCollider;
    [SerializeField] private GameObject VFX_SparkCollision;
    [SerializeField] private GameObject Reflect_Object;
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI UI_SelectedBullet;
    [SerializeField] private TextMeshProUGUI UI_DamageMultiplier;
    [Header("Other Bullet Stuff")]
    [SerializeField] private float damageMutlipler = 1f;
    [SerializeField] public static bool beamActive = false;
    public static int mostRecentScene;
    private int selectionIterator = 0;
    private float cd_reduction = 1f;
    private BulletTypes bulletGenerator; //DO NOT MAKE THIS SERIALIZEFIELD
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
        VFX_BeamCollider.GetComponent<BoxCollider>().enabled = false;

        UI_SelectedBullet.text = "[Basic Bullet]";
        UI_SelectedBullet.color = Color.red;
        UI_DamageMultiplier.text = "Dmg: [" + damageMutlipler.ToString() + "x]";
        
        // Time.timeScale = 0.2f;

    }
    void FixedUpdate()
    {
        
        trackMouse();
        movementStyle();

        if((Input.GetKey("e") && canShoot)) {
            bulletGenerator.instantiateBullet(bulletInfo.bullets[selectionIterator], damageMutlipler, 1f, transform, transform.rotation, true);
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
            StartCoroutine(reflectionCooldown());
        }
        if(Input.GetKey(KeyCode.Mouse0) && (beamGauge >= 10f) && canUseBeam) {
            useBeam();
        }
    }
    private void checkForBullet(){
        VFX_Reflect.Play();
        VFX_Reflect.SetFloat("SpeedAndDir", -VFX_Reflect.GetFloat("SpeedAndDir"));
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
                if(hit.collider.gameObject.tag == "Bullets" || hit.collider.gameObject.tag == "Bullets_Reflect"){
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
    }
    //function that handles beam damage and collision.
    private void useBeam(){
        beamActive = true;
        
        VFX_Beam.Play();
        VFX_BeamCollider.GetComponent<BoxCollider>().enabled = true;
        VFX_BeamCollider.GetComponent<Animation>().Play("BeamCollider");
        // Debug.Log("Imagine this is a BIG GIANT BEAM!");
        
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
        Debug.Log(collision);
        //if an enemy detects a collision with a bullet, inflict damage by subtracting class info.
        if((collision.CompareTag("Bullets") || collision.CompareTag ("Bullets_Reflect")) && canTakeDamage && collision.gameObject.GetComponent<bulletAttributes>().spawnedByPlayer == false) {
            curr_health -= collision.gameObject.GetComponent<bulletAttributes>().bulletDamage;
            // Debug.Log("[Damage Inflicted on Player] New Health: " + curr_health);
            //Kill player if health is below 0.
            if(curr_health <= 0) {
                Destroy(gameObject);
                GameManager.killAllEnemies();
                playerStatus = false;

                mostRecentScene = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(7);
            }
            StartCoroutine(takeDamageCooldown());
        }

        //pick up healthpack and destroy game object.
        if(collision.CompareTag("Healthpack") && !collision.transform.IsChildOf(transform)) {
            if(curr_health + 30 > max_health){
                curr_health = 100f;
            }
            if(curr_health + 30 < max_health){
                curr_health += 30f;
            }
            Debug.Log("HEALTHPACK ACQUIRED!... +30 HEALTH");
            Destroy(collision.gameObject);
        }

        //pick up damage multipler and destroy game object.
        if(collision.CompareTag("DmgUp") && !collision.transform.IsChildOf(transform)){
            Destroy(collision.gameObject);
            StartCoroutine(ApplyDmgMultiplier());
        }
    }
    void OnCollisionEnter(Collision col){
        if(col.gameObject.CompareTag("Wall")){
            // Debug.Log("Collision with WALL");
            GameObject sparks = Instantiate(VFX_SparkCollision, transform.position, transform.rotation);
            sparks.transform.localScale = new Vector3(0.1f, 0f, 0.1f);
            Destroy(sparks, 2);
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
        VFX_BeamCollider.GetComponent<BoxCollider>().enabled = false;
    }
    //Applies damage bonus to player after buff is picked up.
    IEnumerator ApplyDmgMultiplier(){
        damageMutlipler += 0.5f;
        UI_DamageMultiplier.text = "Dmg: [" + damageMutlipler.ToString() + "x]";
         yield return new WaitForSeconds(15f);
        damageMutlipler -= 0.5f;
        UI_DamageMultiplier.text = "Dmg: [" + damageMutlipler.ToString() + "x]";

    }
    //cycles through array containing all available bullet types.
    void changeBulletType() {
        Debug.Log(bulletInfo.bulletPrefabs[4]);
        // if (selectionIterator == 3){
        //     selectionIterator = 0;
        // }
        selectionIterator++;
        if (selectionIterator >= 4){
            selectionIterator = 0;
        }
        //changes on screen text depending on selected bullet.
        switch(selectionIterator){
            case 0:
               UI_SelectedBullet.text = "[Basic Bullet]";
               UI_SelectedBullet.color = Color.red;
                break;
            case 1:
                UI_SelectedBullet.text = "[Fast Bullet]";
                UI_SelectedBullet.color = Color.blue;
                break;
            case 2:
                UI_SelectedBullet.text = "[Reflect Bullet]";
                UI_SelectedBullet.color = Color.green;
                break;
            case 3: 
                UI_SelectedBullet.text = "[Spread Bullet]";
                UI_SelectedBullet.color = Color.yellow;
                break;
        }
        Debug.Log("Bullet Type #" + selectionIterator + " Associating Bullet: " + bulletInfo.bullets[selectionIterator]);
    }

    public static void reset(){
        curr_health = max_health;
        // gameObject.transform.position = new Vector3(0f, .5f, 0f);
        playerStatus = true;
    }
}