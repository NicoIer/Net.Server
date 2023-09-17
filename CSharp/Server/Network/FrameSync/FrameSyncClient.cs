using Google.Protobuf;

namespace Nico
{
    /// <summary>
    /// 帧同步客户端
    /// </summary>
    public class FrameSyncClient : NetClient
    {
        private List<FrameSyncBehavior> _behaviors;

        public FrameSyncClient(ClientTransport transport, string address) : base(transport, address)
        {
            _behaviors = new List<FrameSyncBehavior>();
            RegisterHandler<FrameRpc>(OnFrameRpc);
        }

        public T Create<T>() where T : FrameSyncBehavior, new()
        {
            T behavior = new T();
            behavior.OnCreate(_behaviors.Count, this);
            _behaviors.Add(behavior);
            return behavior;
        }

        private void OnFrameRpc(ServerPack<FrameRpc> frameRpc)
        {
            FrameRpc msg = frameRpc;
            FrameSyncBehavior behavior = _behaviors[msg.BehaviorIdx];
            using (ProtoBuffer buffer = ProtoBuffer.Get())
            {
                buffer.From(frameRpc.msg.Params);
                behavior.OnRpc(msg.MethodHash, buffer, frameRpc.channelId);
            }
        }

        internal void CallFrameRpc(int behaviorIdx, int methodHash, ProtoBuffer @params)
        {
            FrameRpc rpc = ProtoHandler.Get<FrameRpc>();
            rpc.BehaviorIdx = behaviorIdx;
            rpc.MethodHash = methodHash;
            rpc.Params = @params;
            Send(rpc);
            rpc.Return();
        }
    }
}