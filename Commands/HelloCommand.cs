using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommieBot.Commands
{
    class HelloCommand : Command
    {
        public HelloCommand() : base("hello") { }

        public override bool doCommand(CommandData cmd)
        {
            if (!base.doCommand(cmd))
                return false;

            switch (cmd.User.Id)
            {
                // Jareth
                case 162007058831507456:
                    Command.SimulateTypingAndSend(cmd.Channel, "i don't talk to weebs.");
                    break;
                // Sav
                case 149912605555818497:
                    Command.SimulateTypingAndSend(cmd.Channel, "omg i love u rawr x3");
                    break;
                // Eric
                case 172837909760573442:
                    Command.SimulateTypingAndSend(cmd.Channel, "hello god");
                    break;
                // Cory
                case 123618415385509888:
                    Command.SimulateTypingAndSend(cmd.Channel, "who the fuck do you think you are.");
                    Command.SimulateTypingAndSend(cmd.Channel, "you think you can just waltz in here and talk to me?");
                    Command.SimulateTypingAndSend(cmd.Channel, "fuckin naw son, you don't get a hello");
                    Command.SimulateTypingAndSend(cmd.Channel, "whore");
                    Command.SimulateTypingAndSend(cmd.Channel, "_play fuck you cory");
                    break;
                // Troy
                case 191369262026326017:
                    Command.SimulateTypingAndSend(cmd.Channel, "hi troy");
                    Command.SimulateTypingAndSend(cmd.Channel, "you're my favorite");
                    break;
                // Joel
                case 135560019985956864:
                    Command.SimulateTypingAndSend(cmd.Channel, "JOEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
                    Command.SimulateTypingAndSend(cmd.Channel, "EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
                    Command.SimulateTypingAndSend(cmd.Channel, "EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
                    Command.SimulateTypingAndSend(cmd.Channel, "EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
                    Command.SimulateTypingAndSend(cmd.Channel, "EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
                    Command.SimulateTypingAndSend(cmd.Channel, "EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
                    Command.SimulateTypingAndSend(cmd.Channel, "EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
                    Command.SimulateTypingAndSend(cmd.Channel, "EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
                    Command.SimulateTypingAndSend(cmd.Channel, "EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEELLLLLLLL");
                    break;
                // Patryk
                case 172133073364844545:
                    Command.SimulateTypingAndSend(cmd.Channel, "i'm dead inside");
                    Command.SimulateTypingAndSend(cmd.Channel, "and so are you");
                    //Command.SimulateTypingAndSend(cmd.Channel, ":CommieCat: :CommieCat: :CommieCat:");
                    break;
                case 161880791750672389:
                    Command.SimulateTypingAndSend(cmd.Channel, "hi carlos =)");
                    Command.SimulateTypingAndSend(cmd.Channel, "https://i.imgur.com/szHW27D.jpg");
                    break;
                // Everyone else
                default:
                    Command.SimulateTypingAndSend(cmd.Channel, "what's up my nigga");
                    break;
            }
            return true;
        }
    }
}
