using System;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf;

namespace Nico
{
    public struct ClientPack<T>
    {
        public int connectId;
        public T msg;
        public int channelId;

        public static implicit operator T(ClientPack<T> pack)
        {
            return pack.msg;
        }
    }


    public class NetServer
    {
        private readonly ServerTransport _transport;
        public bool isRunning => _transport.Active();
        public event Action<int, TransportError, string> OnError;
        public event Action<int> OnDisconnected;
        public event Action<int> OnConnected;
        public event Action<int, ArraySegment<byte>, int> OnDataReceived;
        public event Action<int, ArraySegment<byte>, int> OnDataSent;
        private EventCenter _eventCenter;

        private readonly Dictionary<int, Action<int, ByteString, int>> _handlers;

        public NetServer(ServerTransport transport)
        {
            _transport = transport;
            transport.onConnected += _OnConnected;
            transport.onDisconnected += _OnDisconnected;
            transport.onError += _OnError;
            transport.onDataReceived += _OnDataReceived;
            transport.onDataSent += _OnDataSent;

            _handlers = new Dictionary<int, Action<int, ByteString, int>>();

            _eventCenter = new EventCenter();
        }

        ~NetServer()
        {
            _transport.onConnected -= _OnConnected;
            _transport.onDisconnected -= _OnDisconnected;
            _transport.onError -= _OnError;
            _transport.onDataReceived -= _OnDataReceived;
            _transport.onDataSent -= _OnDataSent;
            Stop();
        }

        #region Transport Event

        private void _OnError(int connectId, TransportError error, string msg)
        {
            OnError?.Invoke(connectId, error, msg);
        }

        private void _OnDisconnected(int connectId)
        {
            OnDisconnected?.Invoke(connectId);
        }

        private void _OnConnected(int connectId)
        {
            OnConnected?.Invoke(connectId);
        }

        private void _OnDataSent(int connectId, ArraySegment<byte> data, int channel)
        {
            OnDataSent?.Invoke(connectId, data, channel);
        }

        private void _OnDataReceived(int connectId, ArraySegment<byte> data, int channel)
        {
            PacketHeader header = ProtoHandler.Get<PacketHeader>();
            ProtoHandler.UnPack(ref header, data);
            if (!_handlers.ContainsKey(header.Id))
            {
                throw new InvalidDataException($"unregistered message id:{header.Id}");
            }

            _handlers[header.Id](connectId, header.Body, channel);
            OnDataReceived?.Invoke(connectId, data, channel);
            header.Return();
        }

        public void Send<T>(int connectId, T msg, int channelId = Channels.Reliable)
            where T : IMessage<T>, new()
        {
            using (ProtoBuffer buffer = ProtoBuffer.Get())
            {
                ProtoHandler.Pack(buffer, msg);
                _transport.Send(connectId, buffer, channelId);
            }
        }

        public void SendToAll<T>(T msg, int channelId = Channels.Reliable) where T : IMessage<T>, new()
        {
            using (ProtoBuffer buffer = ProtoBuffer.Get())
            {
                ProtoHandler.Pack(buffer, msg);
                _transport.SendToAll(buffer, channelId);
            }
        }

        #endregion


        #region Life Loop

        public void Start()
        {
            _transport.Start();
        }

        public void Stop()
        {
            _transport.Stop();
            _transport.Shutdown();
        }

        public void OnEarlyUpdate()
        {
            _transport.TickIncoming();
        }

        public void OnLateUpdate()
        {
            _transport.TickOutgoing();
        }

        #endregion

        #region Handler

        public void RegisterHandler<T>(Action<ClientPack<T>> handler) where T : IMessage<T>, new()
        {
            int id = TypeId<T>.ID;
            if (!_handlers.ContainsKey(id))
            {
                _handlers[id] = (connectId, data, channel) =>
                {
                    T msg = ProtoHandler.Get<T>();
                    ProtoHandler.UnPack(ref msg, data);
                    _eventCenter.Fire(new ClientPack<T>
                    {
                        connectId = connectId,
                        msg = msg,
                        channelId = channel
                    });
                    msg.Return();
                };
            }

            _eventCenter.Register<ClientPack<T>>(handler);
        }

        public void UnRegisterHandler<T>(Action<ClientPack<T>> handler) where T : IMessage<T>, new()
        {
            int id = TypeId<T>.ID;
            if (!_handlers.ContainsKey(id))
            {
                return;
            }

            _eventCenter.UnRegister<ClientPack<T>>(handler);
        }

        #endregion
    }
}