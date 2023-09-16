namespace Nico
{
    public enum NetObjState
    {
        UnConnected,
        Connected,
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ServerRpcAttribute : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ClientRpcAttribute : Attribute
    {
        
    }

    /// <summary>
    /// 我们认为 所谓服务器世界 不过也是一个客户端
    /// 使用NetIOType来区分 如果是server 则 不需要走socket通信
    /// </summary>
    public sealed class NetObj
    {
        public int id { get; set; }
        public NetObjState state { get; private set; }
        internal readonly List<INetBehavior> components;
        private int _nextBehaviorId;

        public World world { get; private set; }
        private NetClient client => world.client;
        

        internal NetObj(int id, World world)
        {
            this.id = id;
            this.world = world;
            _nextBehaviorId = int.MinValue;
            state = NetObjState.UnConnected;
            components = new List<INetBehavior>();
            _nextBehaviorId = 0;

            client.OnConnected += OnConnected;
            client.OnDisconnected += OnDisconnected;
        }


        ~NetObj()
        {
            Destroy();
        }

        internal void Destroy()
        {
            foreach (var netBehavior in components)
            {
                netBehavior.OnDisconnect();
                netBehavior.OnNetDeSpawn();
            }

            components.Clear();

            client.OnConnected -= OnConnected;
            client.OnDisconnected -= OnDisconnected;
        }

        #region Event Handler

        internal void OnConnected()
        {
            foreach (var netBehavior in components)
            {
                netBehavior.OnConnect();
            }
        }

        internal void OnDisconnected()
        {
            foreach (var netBehavior in components)
            {
                netBehavior.OnDisconnect();
            }
        }

        #endregion


        //只许Add不许Remove
        public T AddBehavior<T>() where T : INetBehavior, new()
        {
            T behavior = new T();
            behavior.OnInit(this, _nextBehaviorId); //这里的id是用来标识这个behavior的
            if (_nextBehaviorId == int.MaxValue)
                throw new Exception("behavior id overflow");
            components.Add(behavior);
            
            
            ++_nextBehaviorId;

            behavior.OnNetSpawn(); //在网络中生成
            if (state == NetObjState.Connected)
            {
                behavior.OnConnect(); //如果添加时已经连接上了，那么就触发连接事件
            }

            return behavior;
        }
    }
}