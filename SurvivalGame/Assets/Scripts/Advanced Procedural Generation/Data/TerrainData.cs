using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map Data/Terrain Data")]
public class TerrainData : UpdatableData
{
    public float uniformScale = 2.5f;
    
    public bool useFlatShading;
    
    public bool useFalloff;

    public float meshHeightMultiplier = 40;
    public AnimationCurve meshHeightCurve;
}
