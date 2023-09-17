using Google.Protobuf;

namespace Nico
{
    public class FrameSyncBehavior : INetBehavior
    {
        public int idx { get; private set; }
        public FrameSyncClient client { get; private set; }

        public void OnCreate(int idx, FrameSyncClient client)
        {
            this.idx = idx;
            this.client = client;
        }

        public void OnRpc(int methodHash, ProtoBuffer @params, int channel)
        {
            
        }

        public void CallFrameRpc(int methodHash, ProtoBuffer @params)
        {
            client.CallFrameRpc(idx, methodHash, @params);
        }

        public virtual void OnSpawn()
        {
        }

        public virtual void OnDeSpawn()
        {
        }

        public virtual void OnConnect()
        {
        }

        public virtual void OnDisconnect()
        {
        }
    }
}