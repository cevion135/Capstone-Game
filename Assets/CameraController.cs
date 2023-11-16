using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float CameraMovementSpeed;
    [SerializeField] private float zoom;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private bool zoomed;
    [SerializeField] private bool isZoomed;
    [SerializeField] private bool canZoom = true;
    private GameObject[] enemies;

    // Start is called before the first frame update
    void Start()
    {
   
    }

    // Update is called once per frame
    void LateUpdate()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        // print(enemies.Length);
        if(enemies.Length == 0) {
            followPlayer();
        }
        if(enemies.Length != 0) {
            trackPlayerAndTargets();
        }
    }
    void followPlayer() {
        if(player != null) {
            Vector3 playerPos = player.position;
            playerPos.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, playerPos, CameraMovementSpeed * Time.deltaTime);
            manualZoom();
        
        }
    }
    void trackPlayerAndTargets() {
        Vector3 avgEnemyPos = Vector3.zero;
        //create and average position for all enemies present.
        foreach (GameObject enemy in enemies) {
            avgEnemyPos += enemy.transform.position;
        }
        avgEnemyPos /= enemies.Length;
        //split the difference between them.
        Vector3 midpoint = (player.position + avgEnemyPos) / 2f;
        Vector3 newCamPos = new Vector3(midpoint.x, 30f, midpoint.z);
        transform.position = Vector3.Lerp(transform.position, newCamPos, CameraMovementSpeed * Time.deltaTime);
    }
    void manualZoom(){
        // Debug.Log("ManualZoom Gets Called");
        //hit tab and not zoomed will allow player to zoom in.
        if(Input.GetKeyDown(KeyCode.Tab) && !zoomed && canZoom) {
            Debug.Log("ZOOMING IN!");
            transform.position = new Vector3(transform.position.x, transform.position.y - 15f, transform.position.z);
            // transform.position = Vector3.Lerp(transform.position, zoom, .2f);
            zoomed = !zoomed;
            StartCoroutine(manualZoomCooldown());
        }
        //hit tab and zoomed will allow player to zoom out.
        if(Input.GetKeyDown(KeyCode.Tab) && zoomed && canZoom) {
            Debug.Log("ZOOMING OUT!");
            transform.position = new Vector3(transform.position.x, transform.position.y + 15f, transform.position.z);
            // transform.position = Vector3.Lerp(transform.position, zoom, .2f);
            zoomed = !zoomed;
            StartCoroutine(manualZoomCooldown());
        }
        zoom = Mathf.Max(zoom, 0.1f);
    }
    IEnumerator manualZoomCooldown(){
        canZoom = false;
        yield return new WaitForSeconds(.1f);
        canZoom = true;
    }
}
