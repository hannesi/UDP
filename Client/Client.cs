using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    private const int DEFAULT_PORT = 6665;
    private const int DEFAULT_DEST_PORT = 6666;
    private const string QUIT_COMMAND = "/quit";

    static void Main(String[] args)
    {
        int port = args.Length > 0 ? int.Parse(args[0]) : DEFAULT_PORT;
        int destPort = args.Length > 1 ? int.Parse(args[1]) : DEFAULT_DEST_PORT;
        var rdtSend = new ReliableDataTransferSend(port);
        while (true)
        {
            string message = Console.ReadLine() ?? string.Empty;
            if (message == QUIT_COMMAND) return;
            // jos syotteena tyhja merkkijono, ei laheteta
            if (message == string.Empty) continue;
            byte[] data = Encoding.UTF8.GetBytes(message);
            try
            {
                rdtSend.Send(data, "localhost", destPort);
            } 
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}