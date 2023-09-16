namespace Nico
{
    public class ClientObj : NetObj<IClientBehavior>
    {
        private readonly NetClient _client;

        public ClientObj(uint id, NetClient client) : base(id)
        {
            _client = client;
            client.OnConnected += OnConnected;
            client.OnDisconnected += OnDisconnected;
        }

        ~ClientObj()
        {
            _client.OnConnected -= OnConnected;
            _client.OnDisconnected -= OnDisconnected;
        }

        private void OnConnected()
        {
            state = NetObjState.Connected;
            foreach (var behavior in behaviors)
            {
                behavior.OnNetEnable();
            }
            queries.Clear();
        }

        private void OnDisconnected()
        {
            state = NetObjState.UnConnected;
            foreach (var behavior in behaviors)
            {
                behavior.OnNetDisable();
            }
            queries.Clear();
        }

        
        
        public void SendRpc(int methodHash)
        {
            // methodHash behaviorId channelId payloads
        }

        public void OnRpc(int methodHash,int index,byte[] payloads)
        {
            
        }
    }
}