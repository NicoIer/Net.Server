using Google.Protobuf;
using kcp2k;
using Nico;

ProtoReader.Reader<PacketHeader>.reader = PacketHeader.Parser.ParseFrom;
ProtoReader.Reader<ErrorMessage>.reader = ErrorMessage.Parser.ParseFrom;
ProtoReader.Reader<PingMessage>.reader = PingMessage.Parser.ParseFrom;;
ProtoReader.Reader<PongMessage>.reader = PongMessage.Parser.ParseFrom;


Console.WriteLine("Nico.Server");
KcpConfig config = KcpUtil.GetDefaultConfigCopy();
config.DualMode = false;

ServerTransport serverTransport = new KcpServerTransport(config, 24419);

serverTransport.Start();

ServerCenter center = new ServerCenter();

serverTransport.OnDataReceived += (connectId, bytes, channel) =>
{
    center.OnData(connectId, PacketHeader.Parser.ParseFrom(bytes), channel);
};

center.Register<PingMessage>((connectId, msg, channel) =>
{
    Console.WriteLine($"ping message from {connectId} channel:{channel} clientTime:{msg.ClientTime} ");
});

serverTransport.OnError += (connectId, error, msg) =>
{
    Console.WriteLine(msg);
};


int frame = 0;
double tickInterval = 1000 / 60f;


// 结束任务
Task endTask = Task.Run(Console.ReadLine);
while (true)
{
    if (endTask.IsCompleted)
    {
        break;
    }

    await Task.Delay(TimeSpan.FromMilliseconds(tickInterval)); // 60fps
    serverTransport.TickIncoming();
    serverTransport.TickOutgoing();
    ++frame;
}

serverTransport.Shutdown();