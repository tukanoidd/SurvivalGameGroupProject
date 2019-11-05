using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatSource : MonoBehaviour
{
    public GameObject parent;
    public SphereCollider SOI_Collider;
    public float heat;
    public float size;
    public float maxRange = 5;
    public float maximumHeat;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        SOI_Collider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (parent != null)
        {
            burn(parent);
            size = heat / maximumHeat;
            SOI_Collider.radius = (size * maxRange);
        }
        else
        {
            size = heat / maximumHeat;
            SOI_Collider.radius = (size * maxRange);
        }
    }

    protected virtual void setMaxHeat(float maxHeat)
    {
        maximumHeat = maxHeat;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Combustable>() != null)
        {
            heatOther(other);
        }
    }

    void heatOther(Collider other)
    {
        GameObject go = other.gameObject;
        float parentTemp = 0f;
        GameObject heatSource = parent;
        
        float distance = Vector3.Distance(go.transform.position, transform.position);
        float intensity = 1 / (Mathf.Pow(distance, 2f));

        if (GetComponent<Combustable>() != null)
        {
            if (GetComponent<Combustable>().name == "Ember")
            {
                parentTemp = GetComponent<Combustable>().temperature * intensity;
                heatSource = gameObject;
            }
        }
        else
        {
            parentTemp = parent.GetComponent<Combustable>().temperature * intensity;
            heatSource = parent;
        }
        
                
        go.GetComponent<Combustable>().temperature += (parentTemp /
                                                       (go.GetComponent<Combustable>().heatTransfer)) / 100;
    
        go.GetComponent<Combustable>().heatedBy = heatSource;
    }

    void burn(GameObject parentObj)
    {
        float parentFuel = parentObj.GetComponent<Combustable>().fuel;
        float maximumTemp = parentObj.GetComponent<Combustable>().maxTemp;
        float burnRate = parentObj.GetComponent<Combustable>().burnRate;
        float objHeatTransfer = parentObj.GetComponent<Combustable>().heatTransfer;

        if (parentFuel > 0)
        {
            parentObj.GetComponent<Combustable>().fuel =
                (parentObj.GetComponent<Combustable>().fuel - (burnRate / 10f));
            if (heat < maximumHeat)
            {
                heat++;
            }

            if (parentObj.GetComponent<Combustable>().temperature < maximumTemp)
            {
                parentObj.GetComponent<Combustable>().temperature += (heat * (objHeatTransfer / 10)) / 1000;
            }
            else
            {
                parentObj.GetComponent<Combustable>().vaporize();
            }
        }
        else
        {
            if (heat > 0)
            {
                heat -= 0.5f;
            }
        }
    }
}