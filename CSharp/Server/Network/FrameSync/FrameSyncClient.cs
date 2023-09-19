namespace Nico
{
    public struct FrameRpc<T>
    {
        public T msg;
        public int channelId;
    }

    /// <summary>
    /// 帧同步客户端
    /// </summary>
    public class FrameSyncClient : NetClient
    {
        private readonly List<FrameSyncBehavior> _behaviors;

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
                FrameSyncRpcCenter.OnFrameRpc(behavior, msg.MethodHash, buffer, frameRpc.channelId);
            }
        }

        internal void CallFrameRpc(FrameSyncBehavior behavior, int methodHash, ProtoBuffer @params)
        {
            CallFrameRpc(behavior.idx, methodHash, @params);
        }

        internal void CallFrameRpc(int behaviorIdx, int methodHash, ProtoBuffer @params)
        {
            FrameRpc rpc = ProtobufHandler.Get<FrameRpc>();
            rpc.BehaviorIdx = behaviorIdx;
            rpc.MethodHash = methodHash;
            rpc.Params = @params;
            Send(rpc);
            rpc.Return();
        }
    }
}