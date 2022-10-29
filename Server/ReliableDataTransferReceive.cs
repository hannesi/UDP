using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

internal class ReliableDataTransferReceive
{
    private VirtualUdpClient virtualUdp;
    private const int CHECKSUM_LENGTH = 32;
    private const int SEQUENCE_LENGTH = 1;
    // LAASTARI: alustetaan ykkonen, koska ensimmainen saapuva sekvenssi on 0
    private byte lastCorrectSeq = 1;

    public ReliableDataTransferReceive(int port)
    {
        virtualUdp = new VirtualUdpClient(port);
    }

    internal byte[] Receive(ref IPEndPoint rep)
    {
        byte[] packet = virtualUdp.Receive(ref rep);
        (byte seq, byte[] data, byte[] checksum) = SplitPacket(packet);
        if (seq.Equals(lastCorrectSeq))
        {
            Console.WriteLine("Vastaanotettu duplikaatti, jatetaan palauttamatta. Sisalto: " + Encoding.UTF8.GetString(data));
            return Array.Empty<byte>();
        }
        bool valid = CompareChecksum(data, checksum);
        if (valid)
        {
            lastCorrectSeq = seq;
        } 
        Console.WriteLine("Saapuneen paketin sekvenssi: " + seq + ", Lahetetaan ACK " + lastCorrectSeq);
        byte[] responseACK = MakeACK(lastCorrectSeq);
        virtualUdp.Send(responseACK, responseACK.Length, rep);
        return valid ? data : Array.Empty<byte>();
    }

    private static byte[] MakeACK(byte lastCorrectSeq)
    {
        return Encoding.UTF8.GetBytes("ACK ").Concat(new byte[] {lastCorrectSeq}).ToArray();
    }

    /// <summary>
    /// Jakaa paketin dataan ja tarkastussummaan
    /// </summary>
    /// <param name="packet">Datasta ja tarkastussummasta koostuva paketti</param>
    /// <returns>Tavutaulukot, joista ensimmainen sisaltaa datan ja jalkimmainen tarkastussumman</returns>
    private static (byte, byte[], byte[]) SplitPacket(byte[] packet)
    {
        byte sequenceNumber = packet.First();
        byte[] data = packet.Skip(SEQUENCE_LENGTH).Take(packet.Length - CHECKSUM_LENGTH - SEQUENCE_LENGTH).ToArray();
        byte[] checksum = packet.Skip(packet.Length - CHECKSUM_LENGTH).ToArray();
        return (sequenceNumber, data, checksum);
    }

    /// <summary>
    /// Laskee ensimmaisen parametrin sha256-summan ja vertaa sita toisena parametrina tuotuun tavutaulukkoon
    /// </summary>
    /// <param name="data">datan sisaltava tavutaulukko</param>
    /// <param name="checksum">tavutaulukko, johon datasta laskettua tarkastussummaa verrataan</param>
    /// <returns>boolean-arvo tarkastussummien yhtasuuruudesta</returns>
    private static bool CompareChecksum(byte[] data, byte[] checksum)
    {
        using (SHA256 sha = SHA256.Create())
        {
            byte[] dataChecksum = sha.ComputeHash(data);
            bool valid = dataChecksum.SequenceEqual(checksum);
            return valid;
        }
    }
}