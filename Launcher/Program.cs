using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using Launcher.Network;
using Launcher.Util;

namespace Launcher
{

    public class Program
    {

        private static readonly string AppName = "Oasis Launcher";
        private static readonly string ConfigFile = "config.json";

        private static readonly string AuthTokenUrl = "https://www.blackdesertonline.com/launcher/l/api/Login.json";
        private static readonly string PlayTokenUrl = "https://www.blackdesertonline.com/launcher/l/api/CreatePlayToken.json";
        private static readonly string AuthUsernameKey = "email";
        private static readonly string AuthPasswordKey = "password";
        private static readonly string AuthTokenKey = "token";

        private Config config;

        public Program()
        {
            this.config = new Config();

            Console.Title = AppName;
        }

        public void Run()
        {
            if (!File.Exists(ConfigFile))
            {
                using (FileStream fileStream = new FileStream(ConfigFile, FileMode.CreateNew, FileAccess.Write, FileShare.Read))
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                using (JsonTextWriter jsonTextWriter = new JsonTextWriter(streamWriter))
                    new JsonSerializer() { Formatting = Formatting.Indented }.Serialize(jsonTextWriter, this.config);

                Console.WriteLine($"Couldn't find `{ConfigFile}`. Created a empty one.");
                Console.WriteLine($"Please edit the `{ConfigFile}` file and restart the launcher.");
                Console.ReadLine();

                return;
            }

            using (FileStream fileStream = new FileStream(ConfigFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader streamReader = new StreamReader(fileStream))
            using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                this.config = new JsonSerializer().Deserialize<Config>(jsonTextReader);

            if (String.IsNullOrEmpty(this.config.PathToGameExecutable) || String.IsNullOrEmpty(this.config.Username) || String.IsNullOrEmpty(this.config.Password))
            {
                Console.WriteLine($"`{ConfigFile}` is corrupted. Please delete it and rerun the launcher.");
                Console.ReadLine();

                return;
            }

            if (!File.Exists(this.config.PathToGameExecutable) || Path.GetExtension(this.config.PathToGameExecutable) != ".exe")
            {
                Console.WriteLine("Please check the path to your game executable.");
                Console.ReadLine();

                return;
            }

            StringBuilder passwordStringBuilder = new StringBuilder();

            for (int i = 0; i < this.config.Password.Length; i++)
                passwordStringBuilder.Append("*");

            Console.WriteLine($"Read `{ConfigFile}` file.");
            Console.WriteLine($"Path to game executable: {this.config.PathToGameExecutable}");
            Console.WriteLine($"Username: {this.config.Username}");
            Console.WriteLine($"Password: {passwordStringBuilder}");

            Console.WriteLine("Generating auth token..");

            string authToken = this.GenerateAuthToken();

            if (String.IsNullOrEmpty(authToken))
            {
                Console.WriteLine("Couldn't generate auth token.");
                Console.ReadLine();

                return;
            }

            Console.WriteLine($"Done: `{authToken}`.");

            Console.WriteLine("Generating play token..");

            string playToken = this.GeneratePlayToken(authToken);

            if (String.IsNullOrEmpty(playToken))
            {
                Console.WriteLine("Couldn't generate play token.");
                Console.ReadLine();

                return;
            }

            Console.WriteLine($"Done: `{playToken}`.");

            Process gameProcess = new Process();

            gameProcess.StartInfo.FileName = this.config.PathToGameExecutable;
            gameProcess.StartInfo.Arguments = playToken;
            gameProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(this.config.PathToGameExecutable);

            Console.WriteLine($"Starting game with play token `{playToken}`.");

            gameProcess.Start();

            int waitTime = 5;
            DateTime startDate = DateTime.Now;

            Console.WriteLine($"Closing launcher in {waitTime} seconds. See you next time!");

            while ((DateTime.Now - startDate).TotalSeconds < waitTime) { }
        }

        private string GenerateAuthToken()
        {
            using (WebRequest webRequest = new WebRequest(AuthTokenUrl))
            {
                Dictionary<string, string> requestData = new Dictionary<string, string>()
                {
                    { AuthUsernameKey, this.config.Username },
                    { AuthPasswordKey, this.config.Password }
                };

                dynamic responseData = webRequest.JsonPost(requestData, true);

                return DynamicConvert.To<string>(responseData.result.token);
            }
        }

        private string GeneratePlayToken(string authToken)
        {
            using (WebRequest webRequest = new WebRequest(PlayTokenUrl))
            {
                Dictionary<string, string> requestData = new Dictionary<string, string>()
                {
                    { AuthTokenKey, authToken }
                };

                dynamic responseData = webRequest.JsonPost(requestData, true);

                return DynamicConvert.To<string>(responseData.result.token);
            }
        }

        public static void Main(string[] args) => new Program().Run();

    }

}
