namespace Nico
{
    /// <summary>
    /// 本地客户端 直接内存通信 不需要走socket
    /// </summary>
    public class HostClient : NetClient
    {
        public HostClient(ClientTransport transport, string address) : base(transport, address)
        {
        }
    }
}