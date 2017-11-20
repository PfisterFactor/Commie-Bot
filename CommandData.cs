using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommieBot
{
    // Contains a user message formatted into a command.
    // The constructor trys to find a command name, and any arguments.
    // Usually created by typing something in chat starting with Command.PREFIX
    public class CommandData
    {
        public const string PREFIX = "::";
        public static readonly CommandData NONE = new CommandData();

        public readonly String CommandName;
        public readonly String[] Arguments;
        public readonly Server Server;
        public readonly Channel Channel;
        public readonly User User;
        public CommandData(MessageEventArgs msg)
        {
            this.Server = msg.Server;
            this.Channel = msg.Channel;
            this.User = msg.User;

            var startIndex = msg.Message.Text.IndexOf(" ");
            startIndex = startIndex == -1 ? msg.Message.Text.Length : startIndex;
            CommandName = msg.Message.Text.Substring(PREFIX.Length, startIndex - PREFIX.Length);

            if (startIndex != msg.Message.Text.Length)
            {
                int index = msg.Message.Text.IndexOf(" ", startIndex + 1);
                index = index == -1 ? msg.Message.Text.Length - 1 : index;

                List<String> args = new List<string>();
                bool endOfMsgFlag = false;
                while (index != -1)
                {
                    String argument = msg.Message.Text.Substring(startIndex + 1, index - startIndex).Trim(' ');
                    args.Add(argument);
                    startIndex = index;
                    index = msg.Message.Text.IndexOf(" ", startIndex + 1);
                    if (index == -1 && !endOfMsgFlag)
                    {
                        index = msg.Message.Text.Length - 1;
                        endOfMsgFlag = true;
                    }
                }

                // Trims the arguments if any have quotes
                for (int i = 1; i < args.Count; i++)
                {
                    if (args[i-1].StartsWith("\"") && args[i].EndsWith("\""))
                    {
                        String args1 = args[i - 1];
                        String args2 = args[i];
                        args[i - 1] = (args1 + " " + args2).Substring(1,(args1+ " " +args2).Length-2);
                        args.RemoveAt(i);
                    }
                }
                Arguments = args.ToArray();
            }
            else
                Arguments = new String[] { };

            
        }

        public CommandData()
        {
            this.CommandName = "";
            this.Arguments = new String[]{ };
            this.Server = null;
            this.Channel = null;
            this.User = null;
        }
    }
}
