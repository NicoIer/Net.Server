using Google.Protobuf;

namespace Nico
{
    /// <summary>
    /// 网络游戏世界
    /// 这里主要处理RRC
    /// </summary>
    public sealed class World
    {
        public  NetClient client { get; private set; }
        public readonly List<INetBehavior> behaviors;

        public World(NetClient client)
        {

            this.client = client;
            this.behaviors = new List<INetBehavior>();
            client.RegisterHandler<ClientRpc>(OnClientRpc);
            client.RegisterHandler<TargetRpc>(OnTargetRpc);
        }
        


        ~World()
        {
            foreach (var netObj in behaviors)
            {
                netObj.OnDeSpawn();
            }
            client.UnRegisterHandler<ClientRpc>(OnClientRpc);
            client.UnRegisterHandler<TargetRpc>(OnTargetRpc);
        }

        public T Add<T>() where T : INetBehavior, new()
        {
            T behavior = new T();
            behavior.OnInit(behaviors.Count, this);
            behaviors.Add(behavior);
            return behavior;
        }
        internal void OnClientRpc(ServerPack<ClientRpc> pack)
        {
            ClientRpc rpc = pack.msg;
            INetBehavior behavior = behaviors[rpc.BehaviorIdx];
            RpcCenter.Invoke(rpc.MethodId, behavior, rpc.Payloads, pack.channelId);
        }

        internal void OnTargetRpc(ServerPack<TargetRpc> pack)
        {
            TargetRpc rpc = pack.msg;
            INetBehavior behavior = behaviors[rpc.BehaviorIdx];
            RpcCenter.Invoke(rpc.MethodId, behavior, rpc.Payloads, pack.channelId);
        }


        public void SendServerRpc(int behaviorIdx, int methodId,
            ByteString payloads,
            int channel = Channels.Reliable)
        {
            ServerRpc rpc = ProtoHandler.Get<ServerRpc>();
            rpc.BehaviorIdx = behaviorIdx;
            rpc.MethodId = methodId;
            rpc.Payloads = payloads;
            client.Send(rpc, channel);
            rpc.Return();
        }
    }
}