// See https://aka.ms/new-console-template for more information

using kcp2k;
using Nico;

namespace Client
{
    static class ClientProgram
    {
        static async Task Main()
        {
            int i = 30;
            List<Task> tasks = new List<Task>();
            for (int j = 0; j < i; j++)
            {
                Task task = ChatTest();
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }

        static async Task RpcTest()
        {
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
        }

        static async Task ChatTest()
        {
            KcpConfig config = KcpUtil.GetDefaultConfig();
            config.DualMode = false;
            KcpClientTransport transport = new KcpClientTransport(config, 24419);
            NetClient client = new NetClient(transport, "localhost");
            client.OnConnected += () => { Console.WriteLine($"Client: OnConnected :{client.connected}"); };
            bool onDisConnted = false;
            client.OnDisconnected += () => { onDisConnted = true; };
            client.OnError += (error, msg) => { Console.WriteLine($"Client: OnError:{msg}"); };
            Task endTask = Task.Run(() => { Console.ReadLine(); });
            client.Start();

            client.RegisterHandler<StringMessage>((pack) =>
            {
                Console.WriteLine($"{nameof(StringMessage)} from server content:{pack.msg.Msg}");
            });

            await Task.Run(async () =>
            {
                while (!client.connected)
                {
                    client.OnEarlyUpdate();
                    client.OnLateUpdate();
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });
            int frame = 0;
            while (!onDisConnted)
            {
                if (endTask.IsCanceled || endTask.IsCompleted || !client.connected)
                {
                    break;
                }

                await Task.Delay(TimeSpan.FromMilliseconds(330));

                if (frame % 60 == 0)
                {
                    StringMessage msg = ProtoHandler.Get<StringMessage>();
                    msg.Msg = $"Hello World! frame:{frame}";
                    client.Send(msg, Channels.Reliable);
                    msg.Return();
                }

                client.OnEarlyUpdate();
                client.OnLateUpdate();
                ++frame;
            }

            client.Stop();
        }
    }
}