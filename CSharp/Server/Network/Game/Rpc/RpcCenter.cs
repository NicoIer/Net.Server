using System.Runtime.CompilerServices;
using Google.Protobuf;

namespace Nico
{
    public enum RpcType
    {
        ServerRpc,
        ClientRpc,
        TargetRpc,
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ServerRpcAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ClientRpcAttribute : Attribute
    {
    }

    public static class RpcCenter
    {
        private static readonly Dictionary<int, RpcInvoker> _rpcInvokers = new Dictionary<int, RpcInvoker>();

        public static string FuncName<T>(string funcName) where T : INetBehavior
        {
            return $"{typeof(T).Name}.{funcName}";
        }


        public static int Register<T>(Action<T, ByteString, int> callDelegate, string funcName,
            bool needAuthority = false) where T : INetBehavior
        {
            int hash = funcName.GetStableHash(); // &0xFFFF 可以保证不会超过ushort
            if (_rpcInvokers.ContainsKey(hash))
                throw new Exception($"RpcInvoker {funcName} already exist");

            void Wrapper(INetBehavior behavior, ByteString payloads, int channel)
            {
                callDelegate((T)behavior, payloads, channel);
            }

            RpcInvoker invoker = new RpcInvoker(Wrapper, needAuthority);
            _rpcInvokers.Add(hash, invoker);
            return hash;
        }

        public static void Invoke(int methodHash, INetBehavior behavior, ByteString payloads, int channel)
        {
            if (_rpcInvokers.TryGetValue(methodHash, out var rpcInvoker))
            {
                rpcInvoker.func(behavior, payloads, channel);
                return;
            }

            // Error
            throw new ArgumentException($"Can't find RpcInvoker for{methodHash}");
        }
    }
}