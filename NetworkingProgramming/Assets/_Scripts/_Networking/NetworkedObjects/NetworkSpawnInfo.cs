using UnityEngine;
using System.Collections.Generic;

public enum NetworkObjectType 
{
    // !!! - Needs to be in same order as prefabs list in SpawnInfo
    PLAYER = 0,
}

[CreateAssetMenu(menuName = "Networking/SpawnInfo")]
public class SpawnInfo : ScriptableObject
{
    // !!! - list should be in the same order as NetworkObjectType enum
    public List<GameObject> prefabs;
}
