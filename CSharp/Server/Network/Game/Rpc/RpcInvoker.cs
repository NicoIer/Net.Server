using Google.Protobuf;

namespace Nico
{
    public delegate void RemoteCallDelegate(INetBehavior behavior, ByteString payloads, int channel);

    internal sealed class RpcInvoker : IEquatable<RpcInvoker>
    {
        public readonly RemoteCallDelegate func;
        public readonly bool needAuthority;

        public RpcInvoker(RemoteCallDelegate func, bool needAuthority = false)
        {
            this.func = func;
            this.needAuthority = this.needAuthority;
        }

        public bool Equals(RpcInvoker? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return func.Equals(other.func) &&
                   needAuthority == other.needAuthority;
        }
    }
}