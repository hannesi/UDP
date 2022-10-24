using System.Net;

internal class ReliableDataTransferReceive
{
    private VirtualUdpClient virtualUdp;

    public ReliableDataTransferReceive(int port)
    {
        virtualUdp = new VirtualUdpClient(port);
        Console.WriteLine("RDT Receive initiated!");
    }

    internal byte[] Receive(ref IPEndPoint rep)
    {
        byte[] packet = virtualUdp.Receive(ref rep);
        byte[] data = ExtractPacket(packet);
        return data;
    }

    private byte[] ExtractPacket(byte[] packet)
    {
        return packet;
    }
}