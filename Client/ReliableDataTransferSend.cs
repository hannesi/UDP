using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

internal class ReliableDataTransferSend
{
    private UdpClient udpClient;
    private static byte sequenceNumber = 0;

    public ReliableDataTransferSend(int port)
    {
        udpClient = new UdpClient(port);
    }

    internal void Send(byte[] data, string v, int destPort)
    {
        byte[] packet = MakePacket(sequenceNumber, data);
        SendPacket(packet, v, destPort);
        ToggleSequence();
    }

    private void SendPacket(byte[] packet, string v, int destPort)
    {
        udpClient.Send(packet, packet.Length, v, destPort);
        var rep = new IPEndPoint(IPAddress.Any, 0);
        byte[] response = udpClient.Receive(ref rep);
        (string res, byte seq) = ParseResponse(response);
        if (res.Equals("ACK") && !seq.Equals(sequenceNumber))
        {
            SendPacket(packet, v, destPort);
        }
    }

    private (string, byte) ParseResponse(byte[] response)
    {
        string res = Encoding.UTF8.GetString(response.Take(response.Length - 1).ToArray()).Trim();
        byte seq = response.Last();
        return (res, seq);
    }



    /// <summary>
    /// kiikuttaa sekvenssinumeroa
    /// </summary>
    private void ToggleSequence()
    {
        sequenceNumber ^= 0x01;
    }

    /// <summary>
    /// Luo paketin, jossa sekvenssinumero, alkuperainen data ja SHA256-summa.
    /// </summary>
    /// <param name="seq">sekvenssinumero</param>
    /// <param name="data">Data, josta paketti halutaan tehda</param>
    /// <returns>Tavutaulukko, jonka alussa on alkuperainen data ja sen perassa SHA256-summa</returns>
    private static byte[] MakePacket(byte seq, byte[] data)
    {
        using (SHA256 sha = SHA256.Create())
        {
            byte[] hash = sha.ComputeHash(data);
            byte[] combined = new byte[] { seq }.Concat(data.Concat(hash)).ToArray();
            return combined;
        }
    }


}