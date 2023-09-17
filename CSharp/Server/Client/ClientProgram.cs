// See https://aka.ms/new-console-template for more information

using kcp2k;
using Nico;
using Server;

async Task ClientFunc(int id)
{
    KcpConfig config = KcpUtil.GetDefaultConfig();
    config.DualMode = false;
    KcpClientTransport transport = new KcpClientTransport(config, 24419);
    NetClient client = new NetClient(transport, "localhost");
    client.OnConnected += () => { Console.WriteLine($"Client: OnConnected :{client.connected}"); };
    client.OnError += (error, msg) => { Console.WriteLine($"Client: OnError:{msg}"); };
    World world = new World(client);

    TestBehavior behavior = world.Add<TestBehavior>();

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
    client.OnConnected += () => { Console.WriteLine($"Client{id}: OnConnected :{client.connected}"); };
    client.OnError += (error, msg) => { Console.WriteLine($"Client{id}: OnError:{msg}"); };
    while (true)
    {
        if (endTask.IsCanceled || endTask.IsCompleted || !client.connected)
        {
            break;
        }

        await Task.Delay(TimeSpan.FromMilliseconds(330));
        client.OnEarlyUpdate();
        client.OnLateUpdate();
        behavior.SendXXXRpc();
    }

    client.Stop();
}

await Task.WhenAll(ClientFunc(0), ClientFunc(1));