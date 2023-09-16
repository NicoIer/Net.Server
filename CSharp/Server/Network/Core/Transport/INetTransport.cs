namespace Nico
{
    public interface INetTransport
    {
        //获取对应信道下每次最大可以发送的包大小
        public int GetMaxPacketSize(int channelId = Channels.Reliable);
        //停止传输
        public void Shutdown();
        //从外界接受数据
        public void TickOutgoing();
        //将数据发送出去
        public void TickIncoming();
    }
}