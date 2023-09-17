// See https://aka.ms/new-console-template for more information

using kcp2k;
using Nico;


async Task ServerFunc()
{
    KcpConfig config = KcpUtil.GetDefaultConfig();
    config.DualMode = false;
    KcpServerTransport transport = new KcpServerTransport(config, 24419);
    NetServer server = new NetServer(transport);
    server.Start();
    server.onConnected += (id) => { Console.WriteLine($"conn:{id}"); };
    server.RegisterHandler<ServerRpc>(pack =>
    {
        Console.WriteLine($"ServerRpc From {pack.connectId} methodId:{pack.msg.MethodId}");
        ServerRpc rpc = pack.msg;
        ClientRpc clientRpc = ProtoHandler.Get<ClientRpc>();
        clientRpc.BehaviorIdx = rpc.BehaviorIdx;
        clientRpc.MethodId = rpc.MethodId;
        clientRpc.Payloads = rpc.Payloads;
        server.SendToAll(clientRpc, pack.channelId);
        rpc.Return();
    });
    Task endTask = Task.Run(() => { Console.ReadLine(); });
    while (true)
    {
        if (endTask.IsCanceled || endTask.IsCompleted)
        {
            break;
        }

        await Task.Delay(TimeSpan.FromMicroseconds(330));
        server.OnEarlyUpdate();
        server.OnLateUpdate();
    }

    server.Stop();
}

await Task.Run(ServerFunc);