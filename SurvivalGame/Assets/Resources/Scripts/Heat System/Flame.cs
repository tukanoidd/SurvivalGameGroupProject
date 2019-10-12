using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : HeatSource
{
    public float flameMaxHeat = 500;
    public GameObject flameObj;
    public GameObject flameForceField;
    public Light flameLight;
    public ParticleSystem particleSystemMain;
    //public float maxFlameSizeRadius;
    public GameObject parentObj;
    //public Vector3 parentSizeVector;

    // Start is called before the first frame update
    protected override void Start()
    {
        parentObj = GetComponent<HeatSource>().parent;
        //parentSizeVector = parent.GetComponent<Renderer>().bounds.size;

        particleSystemMain = GetComponent<ParticleSystem>();

        base.setMaxHeat(flameMaxHeat);

        Vector3 flameObjSize = GetComponent<Renderer>().bounds.size;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + (flameObjSize.y/2), gameObject.transform.position.z);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
        var emitterMain = particleSystemMain.main;
        var emitterShape = particleSystemMain.shape;
        var ratio = heat/maximumHeat;


        emitterShape.radius = size;
        emitterMain.startSize = ratio;
        emitterMain.startSpeed = ratio * 2;
        flameLight.intensity = ratio;
    }

    float getLargestSide(Vector3 sizeVector)
    {
        var max = -1f;

        for(int i = 0; i < 3; i++)
        {
            if(sizeVector[i] > max)
            {
                max = sizeVector[i];
            }
        }

        return max;
    }
}
