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
    }

    internal byte[] Receive(ref IPEndPoint rep)
    {
        byte[] packet = virtualUdp.Receive(ref rep);
        (byte[] data, byte[] checksum) = SplitPacket(packet);
        bool valid = CompareChecksum(data, checksum);
        if (!valid)
        {
            Console.WriteLine("Checksums don't match!");
        } 
        return data;
    }

    /// <summary>
    /// Jakaa paketin dataan ja tarkastussummaan
    /// </summary>
    /// <param name="packet">Datasta ja tarkastussummasta koostuva paketti</param>
    /// <returns>Tavutaulukot, joista ensimmainen sisaltaa datan ja jalkimmainen tarkastussumman</returns>
    private (byte[], byte[]) SplitPacket(byte[] packet)
    {
        byte[] data = packet.Take((int)(packet.Length - CHECKSUM_LENGTH)).ToArray();
        byte[] checksum = packet.Skip((int)(packet.Length - CHECKSUM_LENGTH)).ToArray();
        return (checksum, data);
    }

    /// <summary>
    /// Laskee ensimmaisen parametrin sha256-summan ja vertaa sita toisena parametrina tuotuun tavutaulukkoon
    /// </summary>
    /// <param name="data">datan sisaltava tavutaulukko</param>
    /// <param name="checksum">tavutaulukko, johon datasta laskettua tarkastussummaa verrataan</param>
    /// <returns>boolean-arvo tarkastussummien yhtasuuruudesta</returns>
    private bool CompareChecksum(byte[] data, byte[] checksum)
    {
        using (SHA256 sha = SHA256.Create())
        {
            byte[] dataChecksum = sha.ComputeHash(data);
            bool valid = dataChecksum.SequenceEqual(checksum);
            return valid;
        }
    }
}