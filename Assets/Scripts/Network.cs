using System;
using Multiplay;

public class Network
{
    /// <summary>
    /// 启动服务器
    /// </summary>
    /// <param name="ip">IPv4地址</param>
    public Network(string ip)
    {
        //注册
        Server.Register(MessageType.Ready, _Ready);
        Server.Register(MessageType.GameInfo, _GameInfo);
        //启动服务器
        Server.Start(ip);
    }

    private void _Ready(Player player, byte[] data)
    {
        Ready receive = NetworkUtils.Deserialize<Ready>(data);
        if(receive.ready == true)
        {
            
        }
    }

    private void _GameInfo(Player player, byte[] data)
    {
        GameInfo result = new GameInfo();
        GameInfo receive = NetworkUtils.Deserialize<GameInfo>(data);
    }
}