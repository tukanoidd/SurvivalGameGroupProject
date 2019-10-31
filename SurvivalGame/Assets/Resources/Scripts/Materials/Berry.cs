using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berry : Food
{
    private GameObject berry;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        poisonous = true;
        burnt = false;
        energy = -5;
        cookedPoint = 100;
        cookingTemp = 100;
        cookingCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if (transform.parent != null)
        {
            if (transform.parent.parent.GetComponent<Combustable>() != null)
            {
                if (transform.parent.parent.GetComponent<Combustable>().isPicked &&
                    GetComponent<Rigidbody>().isKinematic)
                {
                    Debug.Log(transform.parent);
                    GetComponent<Rigidbody>().isKinematic = false;
                    
                    transform.parent = null;
                }
            }
        }


        if (temperature < burnedTemp || !burnt)
        {
            if (temperature > cookingTemp)
            {
                Cooking();
            }
        }
        else
        {
            burnt = true;
            renderer.material.color = Color.black;
        }
    }
}