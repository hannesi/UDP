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

    internal void Send(byte[] data, string v, int destPort)
    {
        byte[] packet = MakePacket(data);
        udpClient.Send(packet, packet.Length, v, destPort);
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