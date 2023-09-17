// See https://aka.ms/new-console-template for more information

using kcp2k;
using Nico;

namespace Server
{
    static class ServerProgram
    {
        static async Task Main()
        {
            await ChatTest();
        }

        static async Task ChatTest()
        {
            KcpConfig config = KcpUtil.GetDefaultConfig();
            config.DualMode = false;
            KcpServerTransport transport = new KcpServerTransport(config, 24419);
            NetServer server = new NetServer(transport);
            server.onConnected += (id) => { Console.WriteLine($"conn:{id}"); };
            server.Start();
            Task endTask = Task.Run(() => { Console.ReadLine(); });

            server.RegisterHandler<StringMessage>((pack =>
            {
                Console.WriteLine($"{nameof(StringMessage)} from:{pack.connectId} content:{pack.msg.Msg}");
                pack.msg.Msg = $"Server SendToAll:{pack.msg.Msg} originId:{pack.connectId}";
                server.SendToAll(pack.msg);
            }));

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

        static async Task RpcTest()
        {
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
        }
    }
}