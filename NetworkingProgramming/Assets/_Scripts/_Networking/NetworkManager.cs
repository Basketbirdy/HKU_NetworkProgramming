using System.Collections.Generic;
using UnityEngine;

public enum ObjectType { }
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    private static uint nextNetworkId = 0;
    public static uint NextNetworkId {  get { return nextNetworkId; }}


    [SerializeField] private SpawnInfo spawnInfo;
    private Dictionary<uint, GameObject> networkedObjects = new Dictionary<uint, GameObject>();


    private void Awake()
    {
        if(instance == null) { instance = this; }
        else { Destroy(this); }
    }

    public bool Create(ObjectType type, uint id, out GameObject obj)
    {
        obj = null;
        return false;
    }

    public bool Get(uint id, out GameObject obj)
    {
        obj = null;
        return false;
    }

    public bool Destroy(uint id, out GameObject obj) 
    {
        obj = null;
        return false;
    }
}

public struct SpawnInfo
{
    public List<GameObject> prefabs;
}
