using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    private float movementSpeed = 1000f;
//     public float dashBoost = 50f;
//     public float maxSpeed = 20f;
    public Transform player;
    public Rigidbody rb;
    public BoxCollider bc;
    public Camera cam;
    //private float distanceToCam = 10.0f;


    //public float turningDelay = 5f;
    //public float lastRotationTime;
    //public bool canChangeShape;
    //public Vector3 slideShape = new Vector3(0.3031172f, 0.181347f, 0.2000002f);

    
    //public Collision lastCollision = null;

    

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY |
        RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
    void FixedUpdate()
    {
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
    }
    void trackMouse() {

        // Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        // Debug.Log(mousePosition);
        // Vector3 direction = new Vector3(mousePosition.x - player.position.x, 0, mousePosition.z - player.position.y);
        // player.rotation = Quaternion.LookRotation(direction.normalized);

        // Vector3 mousePosition = Input.mousePosition;
        // mousePosition.z - distanceToCam;

        //SEMI WORKING
        Ray cameraRay = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength)) {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue);
            Debug.Log("Casting Ray");
            // transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
            player.rotation = Quaternion.LookRotation()
        }



        // Vector3 lookDirection = worldMousePosition - player.position;
        // lookDirection.y = 0;
        // transform.rotation = Quaternion.LookRotation(lookDirection.normalized);

        // Vector3 mapMousePosition = cam.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.z));
        // Vector3 lineToMouse =  mapMousePosition - player.position;
        // Quaternion rotation = Quaternion.LookRotation(lineToMouse, Vector3.up);
        // player.rotation = Quaternion.LookRotation(rotation.normalized);
        // Debug.DrawLine(mapMousePosition, player.position, Color.red);
        return;
    }
}