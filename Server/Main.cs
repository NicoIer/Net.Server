using Google.Protobuf;
using Nico;

Console.WriteLine("Nico.Server");
kcp2k.KcpConfig config = KcpUtil.defaultConfig;
config.DualMode = false;

NetServer server = new NetServer(new KcpServerTransport(config, 24419));
server.Start();


server.Register<PingMessage>((connectId, msg, channel) =>
{
    long delta = DateTime.Now.ToUniversalTime().Ticks - msg.ClientTime;
    double ms = delta / 10000f;
    Console.WriteLine(
        $"ping message from {connectId} channel:{channel} delta:{delta} = {ms:0000} ms");
    
    PongMessage pong = ProtoHandler.Get<PongMessage>();
    pong.ServerTime = DateTime.Now.ToUniversalTime().Ticks;
    server.Send(connectId, pong);
    pong.Return();
});

server.onError += (connectId, error, msg) => { Console.WriteLine(msg); };

kcp2k.Log.Info = Console.WriteLine;

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
    waitTime = Math.Abs(waitTime);
    await Task.Delay(TimeSpan.FromMilliseconds(waitTime));
    frameStart = DateTime.Now.ToUniversalTime().Ticks;
    server.OnEarlyUpdate();
    server.OnLateUpdate();
    frameEnd = DateTime.Now.ToUniversalTime().Ticks;
    ++frame;
}

server.Stop();