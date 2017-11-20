using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommieBot.Commands
{
    public abstract class Command
    {
        protected Command(String name)
        {
            this.CommandName = name;
        }

        public readonly String CommandName;

        public bool isDisabled
        {
            get;
            protected set;
        }

        public virtual bool doCommand(CommandData cmd)
        {
            if (isDisabled)
            {
                SimulateTypingAndSend(cmd.Channel, "Command " + CommandName + " is disabled.");
            }
            return !isDisabled;
        }

        public static void SimulateTyping(Channel channel, int milliseconds = 1000)
        {
            channel.SendIsTyping();
            Thread.Sleep(milliseconds);
        }
        public static void SimulateTypingAndSend(Channel channel, String reply)
        {
            SimulateTyping(channel, 500);
            channel.SendMessage(reply);
        }

        public static CommandData GetCommand(MessageEventArgs msg)
        {
            if (msg.Message.Text.Length < CommandData.PREFIX.Length + 1) return CommandData.NONE;
            if (msg.Message.Text.Substring(0, CommandData.PREFIX.Length) != CommandData.PREFIX)
                return CommandData.NONE;

            return new CommandData(msg);
        }
    }
}
