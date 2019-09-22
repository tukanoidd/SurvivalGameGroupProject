using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatableData : ScriptableObject
{
    public event Action OnValuesUpdated;
    public bool autoUpdate;

    protected virtual void OnValidate()
    {
        if (autoUpdate)
        {
            NotifyOfUpdateValues();
        }
    }

    public void NotifyOfUpdateValues()
    {
        if (OnValuesUpdated != null)
        {
            OnValuesUpdated();
        }
    }
}
