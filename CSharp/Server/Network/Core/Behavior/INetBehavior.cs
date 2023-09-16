namespace Nico
{
    public interface INetBehavior
    {
        internal void OnInit<TNetBehavior>(INetObj<TNetBehavior> netObj) where TNetBehavior : INetBehavior;
        public void SetIndex(ushort idx);
        public ushort GetIndex();
        public void OnNetEnable(); // 在网络中生成
        public void OnNetDisable(); // 在网络中销毁
    }

    public interface IClientBehavior : INetBehavior
    {
        public void OnConnect();
        public void OnDisconnect();
    }


    public interface IServerBehavior : INetBehavior
    {
        public void OnConnect(int connectionId);
        public void OnDisconnect(int connectionId);
    }
}