using Nico;

namespace Moba.Server
{
    public class ServerHandler
    {
        private readonly NetServer _server;
        public ServerHandler(NetServer server)
        {
            this._server = server;
            
            _server.Listen<PingMessage>(OnPing);
        }
        void OnPing(ClientMsg<PingMessage> pack)
        {
            long delta = DateTime.Now.ToUniversalTime().Ticks - pack.msg.ClientTime;
            double ms = delta / 10000f;
            Console.WriteLine(
                $"ping message from {pack.channelId} channel:{pack.channelId} delta:{delta} = {ms:0000} ms");

            PongMessage pong = ProtoHandler.Get<PongMessage>();
            pong.ServerTime = DateTime.Now.ToUniversalTime().Ticks;
            _server.Send(pack.channelId, pong);
            pong.Return();
        }
    }
}