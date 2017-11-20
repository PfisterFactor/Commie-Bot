using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CommieBot.Commands
{
    class VoteCommand : Command
    {
        private const String VOTE_DIRECTORY = @"..\CommieBot_data\";

        public static Timer VoteTimer = new Timer(60000); // 1 minute

        private Dictionary<Server,DateTime> lastVoteHeld;

        private Dictionary<Server, Dictionary<User, User>> voteDictionary;

        private Dictionary<Server, bool> doNewVote;

        public void OnVoteTimerTick(object sender, ElapsedEventArgs args)
        {
            this.QueryNewVote();
        }
        public VoteCommand() : base("vote")
        {
            voteDictionary = new Dictionary<Server, Dictionary<User, User>>();
            lastVoteHeld = new Dictionary<Server, DateTime>();
            doNewVote = new Dictionary<Server, bool>();
            VoteTimer.Elapsed += OnVoteTimerTick;
        }

        public void unregisterVoteCommandForServer(Server s)
        {
            if (s == null) return;
            if (voteDictionary.Keys.Where(x => x.Id == s.Id).Count() == 0) return;
            lastVoteHeld.Remove(s);
            doNewVote.Remove(s);
            voteDictionary.Remove(s);
        }
        public void registerVoteCommandForServer(Server s)
        {
            if (s == null) return;
            if (voteDictionary.Keys.Where(x => x.Id == s.Id).Count() != 0) return;
            lastVoteHeld.Add(s, DateTime.Now);
            doNewVote.Add(s, false);
            Dictionary<User, User> votes = new Dictionary<User, User>();
            try
            {
                using (StreamReader file = new StreamReader(VOTE_DIRECTORY + s.Id + ".vote"))
                {
                    // Get last time the vote was held
                    String dateLine = file.ReadLine();
                    lastVoteHeld[s] = DateTime.Parse(dateLine.Substring(dateLine.IndexOf(':') + 1));
                    //

                    // Get the votes for each user in the server
                    String nextLine;
                    do
                    {
                        nextLine = file.ReadLine();
                        String voterID = nextLine.Substring(nextLine.IndexOf(":") + 1);

                        User voter = s.GetUser(ulong.Parse(voterID));

                        nextLine = file.ReadLine();
                        String voteeID = nextLine.Substring(nextLine.IndexOf(":") + 1);
                        User votee = s.GetUser(ulong.Parse(voteeID));

                        if (votee == null || voter == null)
                            continue;

                        votes.Add(voter, votee);

                    } while (file.Peek() != -1);
                    voteDictionary.Add(s, votes);

                    //
                }
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
                {
                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(VOTE_DIRECTORY));
                        using (StreamWriter file = new StreamWriter(VOTE_DIRECTORY + s.Id + ".vote"))
                        {
                            file.WriteLine("#LastVote: " + lastVoteHeld[s].Date);
                            foreach (User u in s.Users.Where(u => !u.HasRole(s.FindRoles("Robots",true).First())))
                            {
                                file.WriteLine("#Voter: " + u.Id);
                                file.WriteLine("##Votee: " + s.Owner.Id);
                                votes.Add(u, s.Owner);
                            }
                            voteDictionary.Add(s, votes);
                        }
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine("Access to data directory denied.");
                        Console.WriteLine("Vote command disabled");
                        isDisabled = true;
                        return;
                    }
                    return;
                }
                isDisabled = true;
                throw;

            }
            
        }

        private void writeVoteToFile(User voter, User votee)
        {
            try
            {
                String votePath = VOTE_DIRECTORY + voter.Server.Id + ".vote";

                String contents = File.ReadAllText(votePath);

                String indexSearch = "#Voter: " + voter.Id + Environment.NewLine + "##Votee: ";
                int indexOfThing = contents.IndexOf(indexSearch) + indexSearch.Length;
                if (indexOfThing != -1)
                {
                    String previousVote = contents.Substring(indexOfThing, contents.IndexOf(Environment.NewLine, indexOfThing) == -1 ? contents.Length-1 : contents.IndexOf(Environment.NewLine,indexOfThing) - indexOfThing).Trim();

                    contents = contents.Replace("#Voter: " + voter.Id + Environment.NewLine + "##Votee: " + previousVote, "#Voter: " + voter.Id + Environment.NewLine + "##Votee: " + votee.Id);
                }
                else
                {
                    contents += Environment.NewLine + "#Voter: " + voter.Id + Environment.NewLine + "##Votee: " + votee.Id;
                }
                

                using (StreamWriter file = new StreamWriter(votePath))
                    file.Write(contents);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("fuck.");
                throw;
            }
        }

        public override bool doCommand(CommandData cmd)
        {
            if (!base.doCommand(cmd))
                return false;

            if (!voteDictionary.ContainsKey(cmd.Server))
            {
                Console.WriteLine("voteDictionary does not contain server: " + cmd.Server.Name);
                return false;
            }

            if (cmd.Arguments.Length < 1)
                return false;

            User voter = cmd.User;
            User votee = cmd.Server.FindUsers(cmd.Arguments.First(),true).FirstOrDefault();
            if (votee == null)
            {
                
                foreach (User u in cmd.Server.Users)
                {
                    if (u.Nickname == cmd.Arguments.First())
                    {
                        votee = u;
                        break;
                    }
                }
                if (votee == null)
                {
                    Command.SimulateTypingAndSend(cmd.Channel, "Can't vote for " + cmd.Arguments.First() + " cause he or she does not exist");
                    return false;
                }
            }
            if (votee.IsBot)
            {
                Command.SimulateTypingAndSend(cmd.Channel, "A bot cannot be Premier you dunce");
                return false;
            }

            voteDictionary[cmd.Server][voter] = votee;
            writeVoteToFile(voter, votee);

            Command.SimulateTypingAndSend(cmd.Channel, voter.Name + " casted a vote for " + votee.Name + " for new Premier!");
            
            return true;
        }

        public async void QueryNewVote()
        {
            List<Server> serversToUpdate = new List<Server>();
            foreach (Server s in lastVoteHeld.Keys)
            {
                TimeSpan diff = DateTime.Today.Subtract(lastVoteHeld[s]);
                if (diff.TotalDays >= 1)
                {
                    Console.WriteLine(diff);
                    Console.WriteLine(DateTime.Today);
                    Tuple<int,User> voteQuery = getVoteWinner(s);
                    Command.SimulateTypingAndSend(s.DefaultChannel, "It has been a day, and the vote tallying for new Premier shall commence!");
                    Command.SimulateTypingAndSend(s.DefaultChannel, voteQuery.Item2.Name + " wins with " + voteQuery.Item1 + " votes!");

                    Role Premier = s.FindRoles("Premier", true).FirstOrDefault();
                    Premier.Members.FirstOrDefault()?.RemoveRoles(Premier);

                    await voteQuery.Item2.AddRoles(s.FindRoles("Premier", true).FirstOrDefault());

                    serversToUpdate.Add(s);
                }
            }
            foreach (Server s in serversToUpdate)
            {
                lastVoteHeld[s] = DateTime.Today.Date;
                String path = VOTE_DIRECTORY + s.Id + ".vote";
                String voteFileContents = File.ReadAllText(path);
                voteFileContents = voteFileContents.Remove(11, voteFileContents.IndexOf(Environment.NewLine)-11);
                voteFileContents = voteFileContents.Insert(11, DateTime.Today.Date.ToString());
                using (StreamWriter file = new StreamWriter(path))
                {
                    file.Write(voteFileContents);
                }
            }
        }

        private Tuple<int,User> getVoteWinner(Server s)
        {
            IEnumerable<User> ordered = this.voteDictionary[s].Values.OrderBy(x => x.Name);
            int run = 0;
            User runUser = ordered.First();
            int max = 0;
            User maxUser = ordered.First();
            foreach (User u in ordered)
            {
                if (u == runUser)
                {
                    run++;
                }
                else
                {
                    run = 1;
                    runUser = u;
                }
                if (run > max)
                {
                    maxUser = runUser;
                    max = run;
                }
                
            }
            return new Tuple<int, User>(max,maxUser);

        }
    }

}
