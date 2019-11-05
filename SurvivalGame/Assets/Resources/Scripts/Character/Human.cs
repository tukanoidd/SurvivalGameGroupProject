using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Human : Combustable
{
    [Range(0, 100)] public float health;
    public float hunger;
    public float hungerRate = 0.02f;

    public GameObject healthBar;
    public GameObject hungerBar;

    public GameObject tempBar;
    public GameObject tempFill;
    
    [SerializeField] private float minimapScale;

    // Start is called before the first frame update
    void Start()
    {
        var objectPlacer = gameObject.AddComponent<ObjectPlacer>();

        base.Start();
        name = "Human";
        flashpoint = 150; //At what temperature will the object burst into flames
        heatTransfer = 6; //On a scale of 1-10 how well does the object conduct heat
        temperature = 37;
        tempInitial = temperature;
        hunger = 0;
        health = 100;
        maxTemp = 200; //At what temperature will the object vaporize
        burnRate = 4;
        isPlayer = true;

        objectPlacer.PlaceMinimapSphere(transform, minimapScale);
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (hunger < 100)
        {
            hunger += hungerRate;
            temperature -= hungerRate / 2;
        }

        if (hunger > 50)
        {
            temperature -= hungerRate;
        }

        if (temperature > 70)
        {
            health -= 0.05f;
            tempFill.GetComponent<Image>().color = Color.red;
        }
        else if (temperature >= 20 && temperature <= 70)
        {
            tempFill.GetComponent<Image>().color = Color.green;
        }
        else if (temperature < 20)
        {
            health -= hungerRate;
            tempFill.GetComponent<Image>().color = Color.cyan;
        }

        healthBar.GetComponent<Slider>().value = Mathf.Clamp(health, 0, 100) / 100;
        hungerBar.GetComponent<Slider>().value = (100 - Mathf.Clamp(hunger, 0, 100)) / 100;
        tempBar.GetComponent<Slider>().value = Mathf.Clamp(temperature, 0, 200);
    }

    public void Eat(Food food)
    {
        health += food.energy;

        hunger = Mathf.Clamp(hunger -= food.hungerRefill, 0, 100);
        
        Destroy(food.gameObject);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Portal"))
        {
            SceneManager.LoadScene("Game");
        }
    }
}