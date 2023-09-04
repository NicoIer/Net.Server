// See https://aka.ms/new-console-template for more information

using Moba;

KcpClientTransport clientTransport = new KcpClientTransport(KcpUtil.defaultConfig, 24419);

clientTransport.Connect("localhost");


Task endTask = Task.Run(Console.ReadLine);

byte[] data = new byte[1024];
while (true)
{
    await Task.Delay(33); //33毫秒
    if (endTask.IsCompleted)
    {
        break;
    }
    
    clientTransport.TickIncoming();
    clientTransport.TickOutgoing();
}