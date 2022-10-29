using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

internal class ReliableDataTransferSend
{
    private UdpClient udpClient;

    public ReliableDataTransferSend(int port)
    {
        udpClient = new UdpClient(port);
    }
    /// <summary>
    /// Lahettaa datasta ja tarkastussummasta koostetun paketin pyydettyyn kohteeseen ja odottaa ACK tai NACK. Mikali vastauksena NACK, lahetetaan paketti uudestaan kunnes vastauksena ACK.
    /// </summary>
    internal void Send(byte[] data, string v, int destPort)
    {
        byte[] packet = MakePacket(data);
        udpClient.Send(packet, packet.Length, v, destPort);
        Console.WriteLine("Viesti lahetetty, odotetaan ACK tai NACK.");
        var rep = new IPEndPoint(IPAddress.Any, 0);
        byte[] response = udpClient.Receive(ref rep);
        string responseString = Encoding.UTF8.GetString(response);
        if (responseString.Equals("ACK"))
        {
            Console.WriteLine("Vastaanotettu ACK.");
        }
        else if (responseString.Equals("NACK"))
        {
            Console.WriteLine("Vastaanotettu NACK. Lahetetaan paketti uudestaan.");
            Send(data, v, destPort);
        }
    }

    /// <summary>
    /// Luo paketin, jossa alkuperainen data ja SHA256-summa
    /// </summary>
    /// <param name="data">Data, josta paketti halutaan tehda</param>
    /// <returns>Tavutaulukko, jonka alussa on alkuperainen data ja sen perassa SHA256-summa</returns>
    private static byte[] MakePacket(byte[] data)
    {
        using (SHA256 sha = SHA256.Create())
        {
            byte[] hash = sha.ComputeHash(data);
            byte[] combined = data.Concat(hash).ToArray();
            return combined;
        }
    }
}