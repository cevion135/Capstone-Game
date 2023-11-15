using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float CameraMovementSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        followPlayer();
    }
    void followPlayer() {
        if(player != null) {
            Vector3 playerPos = player.position;
            playerPos.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, playerPos, CameraMovementSpeed * Time.deltaTime);
        }
    }
    void trackPlayerAndTargets() {

    }
}
