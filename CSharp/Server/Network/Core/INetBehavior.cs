namespace Nico
{
    public interface INetBehavior
    {
        public void OnSpawn(); // 在网络中生成
        public void OnDeSpawn(); // 在网络中销毁
        public void OnConnect(); //连接上触发
        public void OnDisconnect(); //断开连接触发
    }
}