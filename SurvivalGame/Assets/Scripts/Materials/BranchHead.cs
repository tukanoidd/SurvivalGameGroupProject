using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchHead : Plant
{
    public float branchStrength = 100;
    public HingeJoint joint;
    public GameObject trunk;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        HingeJoint[] treeJoints = trunk.GetComponents<HingeJoint>();

        for(int i = 0; i < treeJoints.Length; i++)
        {
            var connected = treeJoints[i].connectedBody;
            if(connected == GetComponent<Rigidbody>())
            {
                joint = treeJoints[i];
            }
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if(temperature > tempInitial)
        {
            if(joint != null)
            {
                var strength = (1/temperature)*10000;
                var clamp = Mathf.Clamp(strength, 0, 100);
                branchStrength = clamp;

                if(branchStrength < 30)
                {
                    joint.useSpring = false;
                }

                if(joint.useSpring)
                {
                    JointSpring jointSpring = joint.spring;

                    jointSpring.spring = branchStrength;
                    joint.spring = jointSpring;
                }
            }
        }
    }
}
