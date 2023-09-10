using Google.Protobuf;
using kcp2k;
using Nico;

ProtoHandler.Reader<PacketHeader>.reader = PacketHeader.Parser.ParseFrom;
ProtoHandler.Reader<ErrorMessage>.reader = ErrorMessage.Parser.ParseFrom;
ProtoHandler.Reader<PingMessage>.reader = PingMessage.Parser.ParseFrom;
ProtoHandler.Reader<PongMessage>.reader = PongMessage.Parser.ParseFrom;


Console.WriteLine("Nico.Server");
KcpConfig config = KcpUtil.defaultConfig;
config.DualMode = false;

ServerTransport serverTransport = new KcpServerTransport(config, 24419);

serverTransport.Start();

ServerCenter center = new ServerCenter();

serverTransport.onDataReceived += (connectId, bytes, channel) =>
{
    center.OnData(connectId, PacketHeader.Parser.ParseFrom(bytes), channel);
};

center.Register<PingMessage>((connectId, msg, channel) =>
{
    Console.WriteLine($"ping message from {connectId} channel:{channel} clientTime:{msg.ClientTime} ");
});

serverTransport.onError += (connectId, error, msg) => { Console.WriteLine(msg); };


int frame = 0;
double tickInterval = 1000 / 60f;
long frameStart = DateTime.Now.Ticks;
long frameEnd = frameStart;

// 结束任务
Task endTask = Task.Run(Console.ReadLine);
while (true)
{
    if (endTask.IsCompleted)
    {
        break;
    }

    double waitTime = tickInterval - (frameEnd - frameStart) / 10000f;
    await Task.Delay(TimeSpan.FromMilliseconds(waitTime));
    frameStart = DateTime.Now.Ticks;
    serverTransport.TickIncoming();
    serverTransport.TickOutgoing();
    frameEnd = DateTime.Now.Ticks;
    ++frame;
}

serverTransport.Shutdown();