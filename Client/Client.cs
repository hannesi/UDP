using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    private const int DEFAULT_PORT = 6665;
    private const int DEFAULT_DEST_PORT = 6666;

    static void Main(String[] args)
    {
        int port = args.Length > 0 ? int.Parse(args[0]) : DEFAULT_PORT;
        int destPort = args.Length > 1 ? int.Parse(args[1]) : DEFAULT_DEST_PORT;
        var udpClient = new UdpClient(port);
        while (true)
        {
            string message = Console.ReadLine() ?? string.Empty;
            if (message == "/quit") return;
            byte[] data = Encoding.UTF8.GetBytes(message);
            try
            {
                udpClient.Send(data, data.Length, "localhost", destPort);
            } 
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}