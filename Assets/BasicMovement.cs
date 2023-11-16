using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float decel;
    [SerializeField] private float dashForce;
    
    [SerializeField] private float max_health = 100f;
    [SerializeField] private Transform centerOfPlayer;
    //public Transform 
    [SerializeField] private Rigidbody rb;
    [SerializeField] private BoxCollider bc;
    [SerializeField] private Camera cam;
    private LayerMask hitlayer;
    // private float shootingCooldown = 1f;
    private BulletTypes bulletGenerator;
    // private float cooldown = .5f;
    private float cd_reduction = 1f;
    private bool canShoot = true;
    private bool canDash = true;
    private bool canSwitchBullets = true;


    private float[] playerBulletCooldowns = {.1f, .5f, .3f, .8f};
    private int selectionIterator = 0;

    //private float distanceToCam = 10.0f;


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

    void Start() {
        rb.drag = 1;
        rb.constraints = RigidbodyConstraints.FreezePositionY |
        RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        BulletTypes newBullet = new BulletTypes();
        bulletGenerator = newBullet;
        
    }
    void FixedUpdate()
    {
        // float moveHorizontal = Input.GetAxis("Horizontal");
        // float moveVertical = Input.GetAxis("Vertical");
        // Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // Vector3 acceleration = movement * movementSpeed * 1f;

        // rb.AddForce(acceleration * Time.deltaTime);

        // Vector3 slowingForceDirection = -rb.velocity.normalized;
        // Vector3 slowingForceVector = decel * slowingForceDirection;
        // rb.AddForce(slowingForceVector * Time.deltaTime);
        trackMouse2();
        movementStyle2();
        if((Input.GetKey("e") && canShoot)) {
            bulletGenerator.instantiateBullet(bulletInfo.bullets[selectionIterator], 1f, 1f, transform, transform.rotation);
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
    }
    IEnumerator dash(){
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
    void movementStyle1() {
        //movement by individual key inputs
        if((Input.GetKey("s"))) {
                rb.AddForce(0, 0, -movementSpeed*Time.deltaTime);
        }
        if((Input.GetKey("d"))) {
                rb.AddForce(movementSpeed*Time.deltaTime, 0, 0);     
        }    
        if((Input.GetKey("w"))) {
                rb.AddForce(0, 0, movementSpeed*Time.deltaTime);
        }
        if((Input.GetKey("a"))) {
                rb.AddForce(-movementSpeed*Time.deltaTime, 0, 0);   
        }
    }
    void movementStyle2(){
        //movement by Input.GetAxis with Decelerating force.
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        Vector3 acceleration = movement * movementSpeed * 1f;

        rb.AddForce(acceleration * Time.deltaTime);

        // Vector3 slowingForceDirection = -rb.velocity.normalized;
        // Vector3 slowingForceVector = decel * slowingForceDirection;
        // rb.AddForce(slowingForceVector * Time.deltaTime);
    }
    void trackMouse() {

        //SEMI WORKING
        Ray cameraRay = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength)) {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            Vector3 towardsMouse = pointToLook - centerOfPlayer.position;
            towardsMouse.y = 0;
            towardsMouse = towardsMouse.normalized;
            Quaternion targetRotation = Quaternion.LookRotation (towardsMouse);
            // Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue);
            // Debug.DrawLine(centerOfPlayer.position, towardsMouse, Color.red);
            // Debug.Log("Casting BLUE Ray: " + pointToLook);
            // Debug.Log("Casting RED Ray: " + towardsMouse);
            // Debug.Log("Printing Look Rotation: " + towardsMouse);
            // Debug.Log("Printing Target Rotation: " + targetRotation);
            centerOfPlayer.rotation = Quaternion.Slerp(centerOfPlayer.rotation, targetRotation, Time.deltaTime * 5f);
        }
        
        


        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // RaycastHit hit;
        // if (Physics.Raycast(ray, out hit)) {
        //     Vector3 mousePosition = hit.point;
        //     Vector3 direction = mousePosition - centerOfPlayer.position;
        //     // direction.y = 0;
        //     Quaternion targetRotation = Quaternion.LookRotation(direction);
        //     centerOfPlayer.rotation = Quaternion.Slerp(centerOfPlayer.rotation, targetRotation, 1f * Time.deltaTime);
        // }
        return;
    }
    void trackMouse2(){
       Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
       Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;
        Vector3 pointDir = Vector3.zero;

        if(groundPlane.Raycast(camRay, out rayLength)) {
            pointDir = camRay.GetPoint(rayLength);
        }
        transform.LookAt(new Vector3(pointDir.x, transform.position.y, pointDir.z));
    }
    IEnumerator shootBulletCooldown(){
        canShoot = false;
        yield return new WaitForSeconds(playerBulletCooldowns[selectionIterator]*cd_reduction);
        canShoot = true;
    }
    IEnumerator changeBulletCooldown(){
        canSwitchBullets = false;
        yield return new WaitForSeconds(1f);
        canSwitchBullets = true;
    }
    void changeBulletType() {
        if (selectionIterator == bulletInfo.bullets.Length-1){
            selectionIterator = 0;
            return;
        }
        selectionIterator++;
        Debug.Log("Bullet Type #" + selectionIterator + " Associating Bullet: " + bulletInfo.bullets[selectionIterator]);
        // Debug.Log("Bullet Length: " + bullets.Length);
    }
}