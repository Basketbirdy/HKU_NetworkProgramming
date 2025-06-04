using System.Collections.Generic;
using UnityEngine;

public enum ObjectType { GAMEMANAGER, }
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }
    private static uint nextNetworkId = 0;
    public static uint NextNetworkId {  get { return nextNetworkId; }}

    [SerializeField] private SpawnInfo spawnInfo;
    Dictionary<uint, GameObject> networkedObjects = new Dictionary<uint, GameObject>();


    private void Awake()
    {
        if(Instance == null) { Instance = this; }
        else { Destroy(this); }
    }

    public bool Create(ObjectType type, uint id, out GameObject obj)
    {
        obj = null;
        if(networkedObjects.ContainsKey(id)) { return false; }
        else
        {
            obj = GameObject.Instantiate(spawnInfo.prefabs[(int)type]);
            NetworkedBehaviour networkedBehaviour = obj.GetComponent<NetworkedBehaviour>();
            if(networkedBehaviour == null) { networkedBehaviour = obj.AddComponent<NetworkedBehaviour>(); }
            networkedBehaviour.networkId = id;

            networkedObjects.Add(id, obj);

            return true;
        }
    }

    public bool Get(uint id, out GameObject obj)
    {
        obj = null;
        if(networkedObjects.ContainsKey(id))
        {
            obj = networkedObjects[id];
            return true;
        }
        return false;
    }

    public bool Destroy(uint id) 
    {
        if (networkedObjects.ContainsKey(id))
        {
            Destroy(networkedObjects[id]);
            networkedObjects.Remove(id);
            return true;
        }
        else
        {
            return false;
        }
    }
}

public struct SpawnInfo
{
    // list should be in the same order as ObjectType enum
    public List<GameObject> prefabs;
}
