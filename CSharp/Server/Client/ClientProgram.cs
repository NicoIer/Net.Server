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
            var t1 = FrameTest(1);
            var t2 = FrameTest(2);

            await Task.WhenAll(t1, t2);
        }


        static async Task FrameTest(int id)
        {
            KcpConfig config = KcpUtil.GetDefaultConfig();
            config.DualMode = false;
            KcpClientTransport transport = new KcpClientTransport(config, 24419);
            FrameSyncClient client = new FrameSyncClient(transport, "localhost");
            client.OnConnected += () => { Console.WriteLine($"Client: OnConnected :{client.connected}"); };
            client.OnError += (error, msg) => { Console.WriteLine($"Client: OnError:{msg}"); };
            client.Start();


            TestFrameSyncBehavior behavior = client.Create<TestFrameSyncBehavior>();
            behavior.id = id;
            byte frame = byte.MinValue;
            while (true)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(330));
                if (client.connected)
                {
                    if (frame == byte.MaxValue)
                    {
                        frame = byte.MinValue;
                    }

                    frame += 1;
                    Console.WriteLine($"call byte Rpc:{frame}");
                    behavior.CallByteRpc(frame);
                }

                client.OnEarlyUpdate();
                client.OnLateUpdate();
            }
        }
    }
}