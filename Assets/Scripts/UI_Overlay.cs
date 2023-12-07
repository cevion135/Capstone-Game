using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Overlay : MonoBehaviour
{
    public Slider healthSlider;
    public Slider EaseHealth;
    public Slider beamSlider;
    public float maxHealth = 100f;
    public float currentHealth;
    public float maxBeam = 200f;
    public float currentBeam;
    private float lerpVal = 0.05f; 
    // Start is called before the first frame update
    void Awake(){
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        getValues();  
        updateBars(); 
        updateEase();
    }
    public void getValues(){
        currentHealth = BasicMovement.curr_health;
        currentBeam = BasicMovement.beamGauge;

    }
    public void updateBars(){
        healthSlider.value = currentHealth;
        beamSlider.value = currentBeam;
    }
    public void updateEase(){
        if(healthSlider.value != EaseHealth.value){
            EaseHealth.value = Mathf.Lerp(EaseHealth.value, currentHealth, lerpVal);
        }
    }
}
