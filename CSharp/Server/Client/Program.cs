// See https://aka.ms/new-console-template for more information

using kcp2k;
using Nico;
using Server;

async Task ClientFunc()
{
    KcpConfig config = KcpUtil.GetDefaultConfig();
    config.DualMode = false;
    KcpClientTransport transport = new KcpClientTransport(config, 24419);
    NetClient client = new NetClient(transport, "localhost");
    World world = new World(client);
    NetObj obj1 = world.AddObj();
    TestBehavior behavior = obj1.AddBehavior<TestBehavior>();

    Task endTask = Task.Run(() => { Console.ReadLine(); });
    client.Start();

    await Task.Run(async () =>
    {
        while (!client.connected)
        {
            client.OnEarlyUpdate();
            client.OnLateUpdate();
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    });
    client.OnConnected += () => { Console.WriteLine($"Client: OnConnected :{client.connected}"); };
    client.OnError+= (error,msg) => { Console.WriteLine($"Client: OnError:{msg}"); };
    while (true)
    {
        if (endTask.IsCanceled || endTask.IsCompleted || !client.connected)
        {
            break;
        }

        await Task.Delay(TimeSpan.FromMilliseconds(330));
        client.OnEarlyUpdate();
        client.OnLateUpdate();
        behavior.TestRpc();
    }

    client.Stop();
}
await Task.WhenAll( ClientFunc(),ClientFunc());


