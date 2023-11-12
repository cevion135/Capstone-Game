using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    private float movementSpeed = 1000f;
//     public float dashBoost = 50f;
//     public float maxSpeed = 20f;
    [SerializeField] private Transform centerOfPlayer;
    //public Transform 
    [SerializeField] private Rigidbody rb;
    [SerializeField] private BoxCollider bc;
    [SerializeField] private Camera cam;
    // private float shootingCooldown = 1f;
    private BulletTypes bulletGenerator;
    private float cooldown = .5f;
    private float cd_reduction = 1f;
    private bool canShoot = true;
    private bool canSwitchBullets = true;

    private string[] bullets = {"basic", "fast", "reflect", "spread"};
    private float[] cooldowns = {.1f, .5f, 1f, 1f};
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
        
        rb.constraints = RigidbodyConstraints.FreezePositionY |
        RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        BulletTypes newBullet = new BulletTypes();
        bulletGenerator = newBullet;
        
    }
    void FixedUpdate()
    {
        //player.rotation.x = 90;
        trackMouse();
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
        if((Input.GetKey("e") && canShoot)) {
            bulletGenerator.instantiateBullet(bullets[selectionIterator], 1f, 1f, transform, transform.rotation);
            StartCoroutine(shootBulletCooldown());
        }
        if(Input.GetKey("q") && canSwitchBullets) {
            changeBulletType();
            StartCoroutine(changeBulletCooldown());
        }
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
            Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue);
            //Debug.DrawLine(centerOfPlayer.position, towardsMouse, Color.red);
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
    IEnumerator shootBulletCooldown(){
        canShoot = false;
        yield return new WaitForSeconds(cooldowns[selectionIterator]*cd_reduction);
        canShoot = true;
    }
    IEnumerator changeBulletCooldown(){
        canSwitchBullets = false;
        yield return new WaitForSeconds(1f);
        canSwitchBullets = true;
    }
    void changeBulletType() {
        if (selectionIterator == bullets.Length-1){
            selectionIterator = 0;
            return;
        }
        selectionIterator++;
        Debug.Log("Iterator Value: " + selectionIterator + " Associating Bullet: " + bullets[selectionIterator]);
        Debug.Log("Bullet Length: " + bullets.Length);
    }
}