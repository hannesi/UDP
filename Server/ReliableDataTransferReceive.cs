using System.Net;
using System.Security.Cryptography;
using System.Text;

internal class ReliableDataTransferReceive
{
    private VirtualUdpClient virtualUdp;
    private const uint CHECKSUM_LENGTH = 32;

    public ReliableDataTransferReceive(int port)
    {
        virtualUdp = new VirtualUdpClient(port);
        Console.WriteLine("RDT Receive initiated!");
    }

    internal byte[] Receive(ref IPEndPoint rep)
    {
        byte[] packet = virtualUdp.Receive(ref rep);
        (bool valid, byte[] data) = ExtractPacket(packet);
        if (valid)
        {
            Console.WriteLine("Checksums match");
        } else
        {
            Console.WriteLine("Checksums don't match");
        }
        return data;
    }

    private (bool, byte[]) ExtractPacket(byte[] packet)
    {
        byte[] data = packet.Take((int)(packet.Length - CHECKSUM_LENGTH)).ToArray();
        byte[] checksum = packet.Skip((int)(packet.Length - CHECKSUM_LENGTH)).ToArray();
        byte[] dataChecksum;
        using (SHA256 sha = SHA256.Create())
        {
            dataChecksum = sha.ComputeHash(data);
        }
        bool valid = dataChecksum.SequenceEqual(checksum);
        return (valid, data);
    }
}