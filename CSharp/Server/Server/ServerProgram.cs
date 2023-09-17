using kcp2k;
using Nico;

namespace Server
{
    static class ServerProgram
    {
        static async Task Main()
        {
            await FrameTest();
        }

        static async Task FrameTest()
        {
            KcpConfig config = KcpUtil.GetDefaultConfig();
            config.DualMode = false;
            KcpServerTransport transport = new KcpServerTransport(config, 24419);
            FrameSyncServer server = new FrameSyncServer(transport);
            server.Start();
            while (true)
            {
                await Task.Delay(TimeSpan.FromMicroseconds(330));
                server.OnEarlyUpdate();
                server.OnLateUpdate();
            }
        }
    }
}