using Nico;

namespace Client
{
    public static class TestRpcMethodCenter
    {
        public static int OnByteRpcHash = $"{nameof(TestFrameSyncBehavior)}.{nameof(OnByteRpc)}".GetStableHash();

        static TestRpcMethodCenter()
        {
            FrameSyncRpcCenter.RegisterFrameRpc<TestFrameSyncBehavior>(OnByteRpcHash, OnByteRpc);
        }
        public static void CallByteRpc(this TestFrameSyncBehavior behavior, byte value)
        {
            using (ProtoBuffer buffer = ProtoBuffer.Get())
            {
                buffer.WriteBlittable(value);
                behavior.CallFrameRpc(OnByteRpcHash, buffer);
            }
        }

        public static void OnByteRpc(this TestFrameSyncBehavior behavior, ProtoBuffer @params, int channel)
        {
            byte a = @params.ReadBlittable<byte>();
            Console.WriteLine($"id:{behavior.id}  OnByteRpc:{a}");
        }
    }
}

