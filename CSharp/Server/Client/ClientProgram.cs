// See https://aka.ms/new-console-template for more information

using Google.Protobuf;
using kcp2k;
using Nico;

namespace Client
{
    static class ClientProgram
    {
        static async Task Main()
        {
            await FrameTest();
        }


        static async Task FrameTest()
        {
            KcpConfig config = KcpUtil.GetDefaultConfig();
            config.DualMode = false;
            KcpClientTransport transport = new KcpClientTransport(config, 24419);
            FrameSyncClient client = new FrameSyncClient(transport, "localhost");
            client.OnConnected += () => { Console.WriteLine($"Client: OnConnected :{client.connected}"); };
            client.OnError += (error, msg) => { Console.WriteLine($"Client: OnError:{msg}"); };
            client.Start();
            
            
            FrameSyncBehavior behavior = client.Create<FrameSyncBehavior>();

            while (true)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(330));
                if (client.connected)
                {
                    using (ProtoBuffer buffer = ProtoBuffer.Get())
                    {
                        buffer.WriteBlittable<int>(24419);
                        behavior.CallFrameRpc(0, buffer);
                    }
                }

                client.OnEarlyUpdate();
                client.OnLateUpdate();
            }
        }
    }
}