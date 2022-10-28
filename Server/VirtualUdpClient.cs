using System.Net;
using System.Net.Sockets;

class VirtualUdpClient : UdpClient
{
    // paketin pudottamisen todennaikoisyys
    private static double packetDropChance = 0.25;
    // paketin viivastyttamisen todennakoisyys
    private static double packetDelayChance = 0.25;
    // viivastyneen paketin viive ms
    private static int packetDelayMs = 1000;
    // bittivirheen todennakoisyys
    private static double packetBitErrorChance = 0.25;
    private static readonly Random random = new();

    public VirtualUdpClient(int portti) : base(portti) { }
    public new byte[] Receive(ref IPEndPoint? remoteEP)
    {
        while (true)
        {
            byte[] bytes = base.Receive(ref remoteEP);
            // pudottaminen
            if (random.NextDouble() <= packetDropChance)
            {
                Console.WriteLine("Paketti pudotettu!");
                continue;
            }
            // bittivirhe
            if (random.NextDouble() <= packetBitErrorChance)
            {
                Console.WriteLine("Pakettiin tullut bittivirhe!");
                int errorByteIndex = random.Next(bytes.Length - 1);
                bytes[errorByteIndex] ^= 1;
            }
            // viivastyminen
            if (random.NextDouble() <= packetDelayChance)
            {
                Console.WriteLine("Paketti viivästyy!");
                Thread.Sleep(packetDelayMs);
            }
            return bytes;
        }
    }
}