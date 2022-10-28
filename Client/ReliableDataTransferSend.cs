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