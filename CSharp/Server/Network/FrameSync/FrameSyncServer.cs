namespace Nico
{
    /// <summary>
    /// 帧同步服务器
    /// </summary>
    public class FrameSyncServer : NetServer
    {
        public FrameSyncServer(ServerTransport transport) : base(transport)
        {
            RegisterHandler<FrameClientReady>(OnFrameClientReady);
            RegisterHandler<FrameRpc>(OnFrameRpc);
        }

        public void OnFrameRpc(ClientPack<FrameRpc> frameRpc)
        {
            SendToAll(frameRpc.msg);
        }
        
        public void OnFrameClientReady(ClientPack<FrameClientReady> frameClientReady)
        {
            
        }
    }
}