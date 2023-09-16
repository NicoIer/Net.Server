using System.Reflection;
using Google.Protobuf;

namespace Nico
{
    public interface INetBehavior
    {
        public NetObj netObj { get; set; }
        public int idx { get; set; }

        internal void OnInit(NetObj netObj, int idx) //C#中的接口可以通过get set特性实现部分方法
        {
            this.netObj = netObj;
            this.idx = idx;
        }

        public void OnNetSpawn(); // 在网络中生成
        public void OnNetDeSpawn(); // 在网络中销毁

        public void OnConnect(); //连接上触发
        public void OnDisconnect(); //断开连接触发

        public void OnRpc(int methodId, ByteString payload, int channelId); //当收到rpc时触发
    }

    // public class NetBehavior : INetBehavior
    // {
    //     public NetObj netObj { get; set; }
    //     public int idx { get; set; }
    //     public virtual void OnNetSpawn()
    //     {
    //         
    //     }
    //
    //     public virtual void OnNetDeSpawn()
    //     {
    //         
    //     }
    //
    //     public virtual void OnConnect()
    //     {
    //         
    //     }
    //
    //     public virtual void OnDisconnect()
    //     {
    //
    //     }
    //
    //     public void OnRpc(int methodId, ByteString payload, int channelId)
    //     {
    //         
    //     }
    // }
}