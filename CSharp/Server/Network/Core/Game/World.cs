using Google.Protobuf;

namespace Nico
{
    // public delegate void RpcReader(INetBehavior behavior, ByteString data, int channel);
    // internal record RemoteCallIndex(int ObjId, int BehaviorIdx, int MethodId);

    /// <summary>
    /// 网络游戏世界
    /// 这里主要处理RRC
    /// </summary>
    public sealed class World
    {
        public NetClient client { get; private set; }
        public List<NetObj> netObjs { get; private set; }
        private int _nextObjId;

        public World(NetClient client)
        {
            this._nextObjId = 0;
            this.client = client;
            this.netObjs = new List<NetObj>();
            client.RegisterHandler<ClientRpc>(OnClientRpc);
        }

        ~World()
        {
            foreach (var netObj in netObjs)
            {
                netObj.Destroy();
            }

            client.UnRegisterHandler<ClientRpc>(OnClientRpc);
        }

        public NetObj AddObj()
        {
            if (_nextObjId == int.MaxValue)
            {
                throw new ArgumentException(); //TODO 
            }

            NetObj obj = new NetObj(_nextObjId, this);
            netObjs.Add(obj);
            return obj;
        }

        public void OnServerRpc(ServerPack<ServerRpc> pack)
        {
            ServerRpc rpc = pack.msg;
            INetBehavior behavior = netObjs[rpc.ObjId].components[rpc.BehaviorIdx];
            behavior.OnRpc(rpc.MethodId, rpc.Payloads, pack.channelId);
        }

        public void OnClientRpc(ServerPack<ClientRpc> pack)
        {
            ClientRpc rpc = pack.msg;
            INetBehavior behavior = netObjs[rpc.ObjId].components[rpc.BehaviorIdx];
            behavior.OnRpc(rpc.MethodId, rpc.Payloads, pack.channelId);
        }

        public void OnTargetRpc(ServerPack<TargetRpc> pack)
        {
            TargetRpc rpc = pack.msg;
            INetBehavior behavior = netObjs[rpc.ObjId].components[rpc.BehaviorIdx];
            behavior.OnRpc(rpc.MethodId, rpc.Payloads, pack.channelId);
        }


        public void SendServerRpc(int id, int behaviorIdx, int methodId,
            ArraySegment<byte> payloads,
            int channel = Channels.Reliable)
        {
            ServerRpc rpc = ProtoHandler.Get<ServerRpc>();
            rpc.ObjId = id;
            rpc.BehaviorIdx = behaviorIdx;
            rpc.MethodId = methodId;
            rpc.Payloads = ByteString.CopyFrom(payloads);
            client.Send(rpc, channel);
            rpc.Return();
        }

        public void SendClientRpc(NetServer server, int id, int behaviorIdx, int methodId,
            ArraySegment<byte> payloads,
            int channel = Channels.Reliable)
        {
            ClientRpc rpc = ProtoHandler.Get<ClientRpc>();
            rpc.ObjId = id;
            rpc.BehaviorIdx = behaviorIdx;
            rpc.Payloads = ByteString.CopyFrom(payloads);
            rpc.MethodId = methodId;
            server.SendToAll(rpc);
            rpc.Return();
        }

        public void SendTargetRpc(NetServer server, int connectId, int id, int behaviorIdx, int methodId,
            ArraySegment<byte> payloads,
            int channel = Channels.Reliable)
        {
            TargetRpc rpc = ProtoHandler.Get<TargetRpc>();
            rpc.ObjId = id;
            rpc.BehaviorIdx = behaviorIdx;
            rpc.Payloads = ByteString.CopyFrom(payloads);
            rpc.ClientId = connectId;
            rpc.MethodId = methodId;
            server.Send(connectId, rpc);
            rpc.Return();
        }
    }
}