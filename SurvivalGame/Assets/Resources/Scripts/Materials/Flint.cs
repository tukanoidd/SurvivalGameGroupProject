using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Flint : Stone
{
    public GameObject sparkObj;
    public ParticleSystem sparks;
    public bool sparking = false;
    public GameObject sparkInstance;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        name = "Flint";
        hardness = 11;
        temperature = 15f;
        tempInitial = 15f;
        heatResistance = 8;
        glowTemp = 400;
        flashpoint = glowTemp;
        doesIgnite = false;
        sparkObj = Resources.Load<GameObject>("Prefabs/Effects/Sparks");
        sparks = sparkObj.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        impactForce = collision.relativeVelocity.magnitude;
        contactPoints = collision.contacts;
        impactedObject = collision.gameObject;

        if (impactForce >= 8)
        {
            if (impactedObject != null && impactedObject.GetComponent<Stone>() != null)
            {
                var contactVector = contactPoints[0].point;
                float random = Random.Range(0, 1);

                SparksAndAmber(contactVector);
            }
        }
    }

    public void SparksAndAmber(Vector3 position)
    {
        sparkInstance = Instantiate(sparkObj, position, Quaternion.identity);
        sparkInstance.transform.parent = gameObject.transform;

        var obj = Resources.Load<GameObject>("Prefabs/Effects/Ember");
        var ember = Instantiate(obj, position, Quaternion.identity);


        Destroy(sparkInstance, sparks.duration);
    }
}