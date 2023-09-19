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

        /// <summary>
        /// 调用帧同步方法
        /// </summary>
        /// <param name="methodHash"></param>
        /// <param name="params"></param>
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