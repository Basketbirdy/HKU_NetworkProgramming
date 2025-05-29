using System.Reflection;
using Unity.Collections;
using UnityEngine;

public class RPCMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.REMOTE_PROCEDURE_CALL;

    public uint networkId;
    public NetworkedBehaviour target;
    public string methodName;
    public object[] data;

    public MethodInfo methodInfo;
    public ParameterInfo[] parameters;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);

        writer.WriteUInt(networkId);
        writer.WriteFixedString128(methodName);

        methodInfo = target.GetType().GetMethod(methodName);
        if(methodInfo == null) { throw new System.ArgumentException($"[RPC Message] Object of type {target.GetType()} does not contain method called {methodName}"); }

        parameters = methodInfo.GetParameters();
        for(int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i].ParameterType == typeof(string))
            {
                writer.WriteFixedString128((string)data[i]);
            }
            else if (parameters[i].ParameterType == typeof(float))
            {
                writer.WriteFloat((float)data[i]);
            }
            else if (parameters[i].ParameterType == typeof(int))
            {
                writer.WriteInt((int)data[i]);
            }
            else if (parameters[i].ParameterType == typeof(Vector3))
            {
                Vector3 v = (Vector3)data[i];
                writer.WriteFloat(v.x);
                writer.WriteFloat(v.y);
                writer.WriteFloat(v.z);
            }
            else if (parameters[i].ParameterType == typeof(Vector2))
            {
                Vector2 v = (Vector2)data[i];
                writer.WriteFloat(v.x);
                writer.WriteFloat(v.y);
            }
            else
            {
                throw new System.ArgumentException($"[RPC Message] Unhandled RPC type: {parameters[i].ParameterType.ToString()}");
            }
        }
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        networkId = reader.ReadUInt();
        methodName = reader.ReadFixedString128().ToString();

        GameObject obj;
        if(NetworkManager.instance.Get(networkId, out obj))
        {
            target = obj.GetComponent<NetworkedBehaviour>();
            methodInfo = target.GetType().GetMethod(methodName);
            if(methodName == null)
            {
                throw new System.ArgumentException($"[RPC Message] Object of type {target.GetType()} does not contain method called {methodName}");
            }
        }
        else
        {
            Debug.LogError($"Could not find object with id {networkId}");
        }

        parameters = methodInfo.GetParameters();
        for(int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i].ParameterType == typeof(string))
            {
                data[i] = reader.ReadFixedString128();
            }
            else if (parameters[i].ParameterType == typeof(float))
            {
                data[i] = reader.ReadFloat();
            }
            else if (parameters[i].ParameterType == typeof(int))
            {
                data[i] = reader.ReadInt();
            }
            else if (parameters[i].ParameterType == typeof(Vector3))
            {
                float x = reader.ReadFloat();
                float y = reader.ReadFloat();
                float z = reader.ReadFloat();
                data[i] = new Vector3(x, y, z);
            }
            else if (parameters[i].ParameterType == typeof(Vector2))
            {
                float x = reader.ReadFloat();
                float y = reader.ReadFloat();
                data[i] = new Vector2(x, y);
            }
            else
            {
                throw new System.ArgumentException($"[RPC Message] Unhandled RPC type: {parameters[i].ParameterType.ToString()}");
            }
        }
    }
}
