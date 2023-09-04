using System;
using System.Net;
using kcp2k;

namespace Moba
{
    /// <summary>
    /// Kcp的服务器端传输
    /// </summary>
    public class KcpServerTransport : ServerTransport
    {
        public ushort port { get; private set; }

        private readonly KcpConfig _config;


        private KcpServer _server;

        public KcpServerTransport(KcpConfig config, ushort port)
        {
            this.port = port;
            this._config = config;
            _server = new KcpServer(
                (connectId) => OnConnected?.Invoke(connectId),
                (connectId, data, channel) => OnDataReceived?.Invoke(connectId, data, KcpUtil.FromKcpChannel(channel)),
                (connectionId) => OnDisconnected?.Invoke(connectionId),
                (connectionId, error, msg) => OnError?.Invoke(connectionId, KcpUtil.ToTransportError(error), msg),
                this._config
            );
        }


        public override Uri Uri()
        {
            UriBuilder builder = new UriBuilder();
            builder.Scheme = nameof(KcpServerTransport);
            builder.Host = System.Net.Dns.GetHostName();
            builder.Port = port;
            return builder.Uri;
        }

        public override bool Active() => _server.IsActive();


        public override void Start() => _server.Start(port);


        public override void Send(int connectionId, ArraySegment<byte> segment, int channelId = Channels.Reliable)
        {
            _server.Send(connectionId, segment, KcpUtil.ToKcpChannel(channelId));
            OnDataSent?.Invoke(connectionId, segment, channelId);
        }

        public override void Disconnect(int connectionId) => _server.Disconnect(connectionId);

        public override string GetClientAddress(int connectionId)
        {
            IPEndPoint endPoint = _server.GetClientEndPoint(connectionId);
            if (endPoint != null)
            {
                if (endPoint.Address.IsIPv4MappedToIPv6)
                {
                    return endPoint.Address.MapToIPv4().ToString();
                }

                return endPoint.Address.ToString();
            }

            return "";
        }

        public override void Stop() => _server.Stop();

        public override int GetMaxPacketSize(int channelId = Channels.Reliable)
        {
            switch (channelId)
            {
                case Channels.Unreliable:
                    return KcpPeer.UnreliableMaxMessageSize(_config.Mtu);
                default:
                    return KcpPeer.ReliableMaxMessageSize(_config.Mtu, _config.ReceiveWindowSize);
            }
        }

        public void TickOutgoing()
        {
            _server.TickOutgoing();
        }

        public void TickIncoming()
        {
            _server.TickIncoming();
        }

        public override void Shutdown()
        {
        }
    }
}