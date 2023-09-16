using Google.Protobuf;
using Nico;

namespace Server;

public class TestBehavior : INetBehavior
{
    public NetObj netObj { get; set; }
    public int idx { get; set; }
    private int testFuncId;

    public TestBehavior()
    {
        testFuncId = nameof(TestRpc).GetStableHash();
    }

    public void OnNetSpawn()
    {
    }

    public void OnNetDeSpawn()
    {
    }

    public void OnConnect()
    {
        Console.WriteLine($"{netObj.id}:{idx} OnConnect");
    }

    public void OnDisconnect()
    {
    }

    public void OnRpc(int methodId, ByteString payload, int channelId)
    {
        Console.WriteLine($"OnRpc methodId:{methodId}");
    }

    public void TestRpc()
    {
        // Console.WriteLine("rpc");
        netObj.world.SendServerRpc(netObj.id, idx, testFuncId, ArraySegment<byte>.Empty, 0);
    }
}