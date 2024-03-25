using Crestron.SimplSharp.CrestronSockets;
using Crestron.SimplSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M24DriversDemo
{
    public static class Console
    {
        static List<ConsoleCommand> _consoleCommands;        
        static string _command;
        static TCPServer _server;
        static int _portNumber;
        static char[] charsToTrim = { '\n', '\r', ' ' };

        public delegate void commandDelegate(string parms);

        public static void Initialize(int portNumber)
        {
            _portNumber = portNumber;
            _server = new TCPServer(IPAddress.Any.ToString(), _portNumber, 15000, EthernetAdapterType.EthernetUnknownAdapter, 10);
            _server.SocketSendOrReceiveTimeOutInMs = 100000;
            _server.WaitForConnectionAsync(ClientConnected);
            _consoleCommands = new List<ConsoleCommand>();
            _consoleCommands.Add(new ConsoleCommand("help", "Lists help for all console commands.", Help));
            _consoleCommands.Add(new ConsoleCommand("clear", "Clears the screen", Clear));
        }

        static void Clear(string args)
        {
            for (int i = 0; i < 41; i++)
            {
                SendLine("");
            }
        }

        static public void Disconnect()
        {
            Send("Console shutdown");
            _server.DisconnectAll();
            _server.Stop();
        }

        static public void Send(string str)
        {
            if (CrestronEnvironment.DevicePlatform == eDevicePlatform.Appliance)
            {
                CrestronConsole.Print(str);
            }
            byte[] b = str.ToString().ToCharArray().Select(c => (byte)c).ToArray();
            _server.SendDataAsync(b, b.Length, sentCallback);
        }

        static public void SendLine(string str)
        {
            if (CrestronEnvironment.DevicePlatform == eDevicePlatform.Appliance)
            {
                CrestronConsole.PrintLine(str);
            }
            str = str + "\r\n";
            byte[] b = str.ToString().ToCharArray().Select(c => (byte)c).ToArray();
            _server.SendDataAsync(b, b.Length, sentCallback);
        }

        static private void sentCallback(TCPServer server, uint clientindex, int size)
        {

        }

        static private void ClientConnected(TCPServer s, uint index)
        {
            SendLine("Debug Console\n");
            Send("\n\r " + _portNumber + " >");            
            _server.ReceiveDataAsync(ReceiveData);
        }

        static private void ReceiveData(TCPServer s, uint index, int bytes)
        {
            string IncommingRx = Encoding.Default.GetString(s.GetIncomingDataBufferForSpecificClient(index), 0, bytes);
            _command = _command + IncommingRx;
            if (_command.Contains("\n"))
            {
                ProcessCommand(_command.ToLower());
                _command = "";
            }
            _server.ReceiveDataAsync(ReceiveData);
        }

        static private void ProcessCommand(string cmd)
        {
            bool commandfound = false;
            string _command = cmd.Substring(0, cmd.Length - 1);
            foreach (ConsoleCommand _cmd in _consoleCommands)
            {
                if (_command.Contains(_cmd.CommandName))
                {
                    _cmd.CommandCall(cmd);
                    commandfound = true;
                    break;
                }
            }
            if (!commandfound && (_command.Length > 0))
            {
                SendLine("Command not found.\n\r Type 'help' for a list of available commands\r");
            }
            Send("\n\r " + _portNumber + " >");

        }

        static private void Help(string args)
        {
            SendLine("\n");
            SendLine("Command Help");
            SendLine("====================================================");
            foreach (ConsoleCommand _cmd in _consoleCommands)
            {
                SendLine(_cmd.PrintHelp());
            }
            SendLine("====================================================\n");
        }

        static public void AddConsoleCommand(string consolecommand, string helptext, commandDelegate CommandHandler)
        {
            _consoleCommands.Add(new ConsoleCommand(consolecommand, helptext, CommandHandler));
            if (CrestronEnvironment.DevicePlatform == eDevicePlatform.Appliance)
            {
                CrestronConsole.AddNewConsoleCommand(x => CommandHandler(x), consolecommand, helptext, ConsoleAccessLevelEnum.AccessAdministrator);
            }
        }

        class ConsoleCommand
        {

            public commandDelegate CommandEvent;
            public string CommandName;
            string HelpText;

            public ConsoleCommand(string commandname, string helptext, commandDelegate CommandHandler)
            {
                CommandName = commandname.ToLower();
                HelpText = helptext;
                CommandEvent += CommandHandler;
            }

            public string PrintHelp()
            {
                return (CommandName + " : \t" + HelpText);
            }

            public void CommandCall(string commandstring)
            {
                var args = commandstring.Remove(0, commandstring.IndexOf(' ') + 1);
                CommandEvent(args);
            }
        }

    }
}