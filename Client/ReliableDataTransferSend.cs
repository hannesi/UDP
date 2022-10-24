using System.Net.Sockets;

internal class ReliableDataTransferSend
{
    private UdpClient udpClient;

    public ReliableDataTransferSend(int port)
    {
        udpClient = new UdpClient(port);
        Console.WriteLine("RDT Send initiated!");
    }

    internal void Send(byte[] data, string v, int destPort)
    {
        byte[] packet = MakePacket(data);
        udpClient.Send(data, data.Length, v, destPort);
    }

    private static byte[] MakePacket(byte[] data)
    {
        return data;
    }
}