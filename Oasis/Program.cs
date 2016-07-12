using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oasis.Network;

namespace Oasis
{

    public class Program
    {

        private Session session;

        private Proxy authProxy;

        public static string Title
        {
            get { return Console.Title; }
            set { Console.Title = value; }
        }

        public Program()
        {
            this.session = new Session();

            this.authProxy = new Proxy("AuthProxy", 8888, "5.79.111.36", 8888);

            this.session.AddProxy(this.authProxy);
        }

        public void Run()
        {
            this.authProxy.Start();

            while (true) { }
        }

        public static void Main(string[] args) => new Program().Run();

        public static void WriteLine(string text)
        {
            foreach (string line in text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
                Console.ResetColor();
                Console.Write("]");
                Console.Write(" ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(line);
                Console.ResetColor();
            }
        }

        public static string ReadLine() => Console.ReadLine();

    }

}
