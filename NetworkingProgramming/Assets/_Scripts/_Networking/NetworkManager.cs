using System.Collections.Generic;
using UnityEngine;

public enum ObjectType { }
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    private Dictionary<uint, GameObject> objects = new Dictionary<uint, GameObject>();
    private SpawnInfo spawnInfo;

    private static uint nextId;
    public static uint NextId {  get { return nextId; }}

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
