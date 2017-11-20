using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.API;
using Discord.Net;
using System.Runtime.InteropServices;
using CommieBot.Commands;

namespace CommieBot
{
    public class Program
    {
        public static DiscordClient client;

        public static Dictionary<String, Command> commands = new Dictionary<string, Command>() {
            { "hello", new HelloCommand() },
            { "vote",  new VoteCommand() },
            { "emoji", new EmojiCommand() }
        };

        public static void runCommand(CommandData command)
        {
            if (commands.ContainsKey(command.CommandName))
                Task.Run(() => commands[command.CommandName].doCommand(command));
        }
        public static void OnLogMessage(object sender, LogMessageEventArgs args)
        {
            Console.WriteLine(args.Message);
        }
        public static void OnMessageRecieved(object sender, MessageEventArgs args)
        {
            CommandData command = Command.GetCommand(args);
            if (command != CommandData.NONE)
            {
                runCommand(command);
            }

        }

        public static void OnServerAvailable(object sender, ServerEventArgs args)
        {
            ((VoteCommand)commands["vote"]).registerVoteCommandForServer(args.Server);
        }
        public static void OnServerDisconnect(object sender, ServerEventArgs args)
        {
            ((VoteCommand)commands["vote"]).unregisterVoteCommandForServer(args.Server);
        }
        static void Main(string[] args)
        {
            // Logs out and disconnects bot on console window close
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);

            client = new DiscordClient(x =>
            {
                x.AppName = "CommieBot";
                x.AppUrl = "";
                x.MessageCacheSize = 0;
                x.UsePermissionsCache = true;
                x.EnablePreUpdateEvents = true;
                x.LogLevel = LogSeverity.Debug;
                x.LogHandler = OnLogMessage;
            });
            client.MessageReceived += OnMessageRecieved;
            client.ServerAvailable += OnServerAvailable;
            client.LeftServer += OnServerDisconnect;
            VoteCommand.VoteTimer.Start();
            client.ExecuteAndWait(async () =>
            {
                while (true)
                {
                    try
                    {
                        await client.Connect("MzAwODY0NjM0OTEzNTU0NDMy.C9LdVQ.hLuMb7iAPhnCleLK-eGXKyJbckQ", TokenType.Bot);
                        client.SetGame("Communism Simulator 2017");
                        break;
                    }
                    catch (Exception e)
                    {
                        client.Log.Error($"Login Failed", e);
                        await Task.Delay(client.Config.FailedReconnectDelay);
                    }
                }
            });
        }

        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                client.Disconnect();
                client.Dispose();
            }
            return false;
        }
        static ConsoleEventDelegate handler;

        // Pinvoke
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
    }
}
