namespace Nico
{
    public class NetworkBehavior : INetBehavior
    {
        public int idx { get; private set; }
        public World world { get; private set; }

        public virtual void OnInit(int idx, World world)
        {
            this.idx = idx;
            this.world = world;
        }

        public int GetIdx()
        {
            return idx;
        }

        public World GetWorld()
        {
            return world;
        }

        public virtual void OnSpawn()
        {
        }

        public virtual void OnDeSpawn()
        {
        }

        public virtual void OnConnect()
        {
        }

        public virtual void OnDisconnect()
        {
        }
    }
}