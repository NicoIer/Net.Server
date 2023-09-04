using Moba;


KcpServerTransport serverTransport = new KcpServerTransport(KcpUtil.defaultConfig, 24419);

serverTransport.Start();

// serverTransport.OnConnected += (connectionId) => { Console.WriteLine($"OnConnected {connectionId}"); };
// 2byte的消息类型 余下就是消息本体
serverTransport.OnDataReceived += (connectionId, data, channelId) =>
{
    Console.WriteLine($"OnDataReceived {connectionId}  , {data.Count}");
};

Task endTask = Task.Run(Console.ReadLine);
double frameRate = 144; //帧率
while (true)
{
    await Task.Delay(TimeSpan.FromSeconds(1 / frameRate)); //要减去计算耗费的时间,加入计算的时间超过了1帧,就不要延迟了
    if (endTask.IsCompleted)
    {
        break;
    }

    serverTransport.TickIncoming();
    serverTransport.TickOutgoing();
}