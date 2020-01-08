using System.Net.Sockets;

public class Player
{
    public Socket Socket; //网络套接字
    public int id;
    public Player(Socket socket, int id)
    {
        Socket = socket;
        this.id = id;
    }
}