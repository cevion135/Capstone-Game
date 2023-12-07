using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupRotation : MonoBehaviour {
    private float lowerBound = .75f;
    private float upperBound = 1.25f;
    private float speed = 1f;
    private float rotationSpeed = 30f;
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime * speed;

  
        float oscillation = Mathf.Sin(timer);


        float result = Mathf.Lerp(lowerBound, upperBound, (oscillation + 1f) / 2f);

        transform.position = new Vector3(transform.position.x, result, transform.position.z);
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        
        // Debug.Log(result);
    }
}