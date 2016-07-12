using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Oasis.Network.Cryptography;

namespace Oasis.Network
{

    public class Proxy : ISessionObject
    {

        public event EventHandler<Packet> OnServerPacket;
        public event EventHandler<Packet> OnClientPacket;

        public string Name { get { return this.name; } }

        private string name;

        private Socket serverSocket;
        private Socket clientSocket;

        private Socket networkSocket;

        private IPEndPoint serverEndPoint;
        private IPEndPoint clientEndPoint;

        private Transformer transformer;

        public Proxy(string name, ushort serverPort, string clientIpAddress, ushort clientPort)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (String.IsNullOrEmpty(clientIpAddress))
                throw new ArgumentNullException(nameof(clientIpAddress));

            this.name = name;

            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            this.networkSocket = null;

            this.serverEndPoint = new IPEndPoint(IPAddress.Any, serverPort);
            this.clientEndPoint = new IPEndPoint(IPAddress.Parse(clientIpAddress), clientPort);

            this.transformer = new Transformer();

            this.serverSocket.NoDelay = true;
            this.serverSocket.Blocking = false;
        }

        public void Start()
        {
            this.serverSocket.Bind(this.serverEndPoint);
            this.serverSocket.Listen(1);

            this.serverSocket.BeginAccept(this.ServerAcceptCallback, null);
        }

        private void ServerAcceptCallback(IAsyncResult asyncResult)
        {
            this.networkSocket = this.serverSocket.EndAccept(asyncResult);
            byte[] buffer = new byte[2];

            if (this.networkSocket == null)
                throw new NullReferenceException(nameof(this.networkSocket));

            this.clientSocket.BeginConnect(this.clientEndPoint, this.ClientConnectCallback, null);
            this.networkSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, this.ServerReceiveCallback, buffer);
        }

        private void ServerReceiveCallback(IAsyncResult asyncResult)
        {
            byte[] buffer = (byte[]) asyncResult.AsyncState;

            int receivedBytes = this.networkSocket.EndReceive(asyncResult);

            if (receivedBytes != buffer.Length)
                return;

            ushort packetLength = BitConverter.ToUInt16(buffer, 0);

            if (packetLength > 0)
            {
                byte[] data = new byte[packetLength];
                int bytesRead = buffer.Length;

                Buffer.BlockCopy(buffer, 0, data, 0, bytesRead);

                while (bytesRead < packetLength)
                {
                    int bytesReceived = Task<int>.Factory.FromAsync(
                        this.networkSocket.BeginReceive(data, bytesRead, packetLength - bytesRead, SocketFlags.None, null, null),
                        this.networkSocket.EndReceive).Result;

                    bytesRead += bytesReceived;

                    if (bytesReceived <= 0)
                        break;
                }

                byte[] dataCopy = (byte[]) data.Clone();

                if (dataCopy[2] == 1)
                    this.transformer.Transform(ref dataCopy, 3);

                this.OnClientPacket?.Invoke(this, new Packet(dataCopy));

                this.clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, null, null);
                this.networkSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, this.ServerReceiveCallback, buffer);
            }
        }

        private void ClientConnectCallback(IAsyncResult asyncResult)
        {
            byte[] buffer = new byte[2];

            this.clientSocket.EndConnect(asyncResult);
            this.clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, this.ClientReceiveCallback, buffer);
        }

        private void ClientReceiveCallback(IAsyncResult asyncResult)
        {
            byte[] buffer = (byte[]) asyncResult.AsyncState;

            int receivedBytes = this.clientSocket.EndReceive(asyncResult);

            if (receivedBytes != buffer.Length)
                return;

            ushort packetLength = BitConverter.ToUInt16(buffer, 0);

            if (packetLength > 0)
            {
                byte[] data = new byte[packetLength];
                int bytesRead = buffer.Length;

                Buffer.BlockCopy(buffer, 0, data, 0, bytesRead);

                while (bytesRead < packetLength)
                {
                    int bytesReceived = Task<int>.Factory.FromAsync(
                        this.clientSocket.BeginReceive(data, bytesRead, packetLength - bytesRead, SocketFlags.None, null, null),
                        this.clientSocket.EndReceive).Result;

                    bytesRead += bytesReceived;

                    if (bytesReceived <= 0)
                        break;
                }

                byte[] dataCopy = (byte[]) data.Clone();

                if (dataCopy[2] == 1)
                    this.transformer.Transform(ref dataCopy, 3);

                this.OnServerPacket?.Invoke(this, new Packet(dataCopy));

                if (BitConverter.ToUInt16(dataCopy, 3) == 0x03eb)
                    this.transformer.UpdateServerInformation(dataCopy.Skip(5).ToArray());

                this.networkSocket.BeginSend(data, 0, data.Length, SocketFlags.None, null, null);
                this.clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, this.ClientReceiveCallback, buffer);
            }
        }

    }

}
