using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{

    public sealed class Config
    {

        public string PathToGameExecutable
        {
            get { return this.pathToGameExectuable; }
            set { this.pathToGameExectuable = value; }
        }

        public string Username
        {
            get { return this.username; }
            set { this.username = value; }
        }

        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        private string pathToGameExectuable;

        private string username;
        private string password;

        public Config()
        {
            this.pathToGameExectuable = String.Empty;

            this.username = String.Empty;
            this.password = String.Empty;
        }

    }

}
