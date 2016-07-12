using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Oasis.Network;
using Oasis.Util;

namespace Oasis
{

    public class Session
    {

        private string baseDirectory;
        private string packetsDirectory;

        private List<Proxy> proxies;

        private List<Tuple<Proxy, Packet>> proxyServerPackets;
        private List<Tuple<Proxy, Packet>> proxyClientPackets;

        public Session()
        {
            if (!Directory.Exists(Settings.SessionsDirectory))
                Directory.CreateDirectory(Settings.SessionsDirectory);

            this.baseDirectory = Path.Combine(Settings.SessionsDirectory, DateTime.Now.ToString("ddMMyyyyHHmmssffffff"));
            this.packetsDirectory = Path.Combine(this.baseDirectory, Settings.PacketsDirectory);

            this.proxies = new List<Proxy>();

            this.proxyServerPackets = new List<Tuple<Proxy, Packet>>();
            this.proxyClientPackets = new List<Tuple<Proxy, Packet>>();

            Directory.CreateDirectory(this.baseDirectory);
            Directory.CreateDirectory(this.packetsDirectory);
        }

        public void AddProxy(Proxy proxy)
        {
            this.proxies.ForEach((p) =>
            {
                if (p.Name == proxy.Name)
                    throw new Exception("Service object names must be unique!");
            });

            Directory.CreateDirectory(Path.Combine(this.packetsDirectory, proxy.Name));

            proxy.OnServerPacket += this.OnProxyServerPacket;
            proxy.OnClientPacket += this.OnProxyClientPacket;

            this.proxies.Add(proxy);
        }

        private void OnProxyServerPacket(object sender, Packet packet)
        {
            Proxy proxy = (Proxy) sender;

            this.proxyServerPackets.Add(new Tuple<Proxy, Packet>(proxy, packet));

            Program.WriteLine($"Packet: {proxy.Name}, Server, {packet.OpCode:x4}, {packet.Length}");

            this.SavePacketToFile("server", proxy, packet);
        }

        private void OnProxyClientPacket(object sender, Packet packet)
        {
            Proxy proxy = (Proxy) sender;

            this.proxyClientPackets.Add(new Tuple<Proxy, Packet>(proxy, packet));

            Program.WriteLine($"Packet: {proxy.Name}, Client, {packet.OpCode:x4}, {packet.Length}");

            this.SavePacketToFile("client", proxy, packet);
        }

        private void SavePacketToFile(string fileNamePrefix, Proxy proxy, Packet packet)
        {
            string fileName = Path.Combine(this.packetsDirectory, proxy.Name, $"{DateTime.Now.ToString("HHmmssffffff")}_{fileNamePrefix}_{packet.OpCode:X4}_{packet.Length}.txt");

            using (FileStream fileStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.Read))
                FormatUtil.FormatAsHexToStream(packet.Data, fileStream);
        }

    }

}
