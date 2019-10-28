using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : Combustable
{
    public GameObject berry;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        name = "Bush";
        berry = Resources.Load<GameObject>("Prefabs/Food/RedBerry");
        populateBush();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    void populateBush()
    {
        for (int i = 1; i < 8; i++)
        {
            var slotName = "BerrySlot" + i;
            var child = transform.Find("Leaves").transform.Find(slotName);
            var b = Instantiate(berry, child);
        }
    }
}