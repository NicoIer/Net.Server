using Google.Protobuf;
using Nico;

namespace Client
{
    public class TestBehavior : NetworkBehavior
    {
        private static Dictionary<string, int> _rpcHashes = new Dictionary<string, int>();

        static TestBehavior()
        {
            string funcName = RpcCenter.FuncName<TestBehavior>(nameof(OnXXXRpc));
            int hash = RpcCenter.Register<TestBehavior>(OnXXXRpc, funcName);
            _rpcHashes.Add(nameof(OnXXXRpc), hash);
            Console.WriteLine($"rpc hash:{hash} name:{typeof(TestBehavior).Name}.{nameof(OnXXXRpc)}");
        }
        

        public static void OnXXXRpc(TestBehavior behavior, ByteString payloads, int channel)
        {
            Console.WriteLine($"OnXXXRpc");
        }

        public void SendXXXRpc()
        {
            Console.WriteLine($"SendXXXRpc");
            int hash = _rpcHashes[nameof(OnXXXRpc)];
            world.SendServerRpc(idx, hash, ByteString.Empty);
        }
    }
}