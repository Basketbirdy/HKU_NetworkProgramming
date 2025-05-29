using System.IO;
using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

namespace Test
{
    public abstract class NetworkMessage
    {
        public abstract int GetID();
        public abstract void Encode(ref DataStreamWriter writer);
        public abstract void Decode(ref DataStreamReader reader);
    }

    public class SendPositionMessage : NetworkMessage
    {
        public override void Decode(ref DataStreamReader reader)
        {
        }

        public override void Encode(ref DataStreamWriter writer)
        {
        }

        public override int GetID()
        {
            return 0;
        }
    }
}
