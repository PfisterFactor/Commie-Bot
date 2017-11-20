using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommieBot.Commands
{
    public class EmojiCommand : Command
    {
        public EmojiCommand() : base("emoji")
        {

        }

        public override bool doCommand(CommandData cmd)
        {
            if (!base.doCommand(cmd))
                return false;

            Command.SimulateTypingAndSend(cmd.Channel, cmd.Arguments.Last() + " *** " + cmd.Arguments.First() + " *** " +cmd.Arguments.Last());
            return true;
        }
    }
}
