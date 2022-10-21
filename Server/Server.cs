using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    private const int DEFAULT_PORT = 6666;
    static void Main(String[] args)
    {
        int port = args.Length > 0 ? int.Parse(args[0]) : DEFAULT_PORT;
        var virtualUdpClient = new VirtualUdpClient(port);

        while (true) {
            var rep = new IPEndPoint(IPAddress.Any, 0);
            byte[] rec = virtualUdpClient.Receive(ref rep);
            string s = Encoding.UTF8.GetString(rec);
            Console.WriteLine(s);
        }
    }
}