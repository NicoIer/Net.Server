namespace Nico
{
    public enum NetObjState
    {
        UnConnected,
        Connected,
    }

    public interface INetObj<TBehavior>
        where TBehavior : INetBehavior 
    {
        public uint id { get; }
        public NetObjState state { get; }

        public void AddBehavior<T>() where T : TBehavior, new();
        public void RemoveBehavior<T>(T behavior) where T : TBehavior, new();
    }


    public abstract class NetObj<TBehavior> : INetObj<TBehavior>
        where TBehavior : INetBehavior 
    {
        public uint id { get; set; }
        public NetObjState state { get; set; }
        
        protected HashSet<TBehavior> behaviors;
        public NetObj(uint id)
        {
            this.id = id;
            state = NetObjState.UnConnected;
            behaviors = new HashSet<TBehavior>();
        }

        public void AddBehavior<T>() where T : TBehavior, new()
        {
            T behavior = new T();
            behavior.OnInit(this);
            ushort idx;
            behavior.SetIndex(idx);
            behaviors.Add(behavior);
            if (state == NetObjState.Connected)
            {
                behavior.OnNetEnable();
            }
        }

        public void RemoveBehavior<T>(T behavior) where T : TBehavior, new()
        {
            behavior.OnNetDisable();
            behaviors.Remove(behavior);
            queries.Remove(behavior.GetIndex());
        }
    }
}