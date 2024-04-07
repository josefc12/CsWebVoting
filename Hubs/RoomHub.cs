using Microsoft.AspNetCore.SignalR;
using cs_web_voting.Data;
using cs_web_voting.Singletons;
using cs_web_voting.Models;
using System.Text.Json;
using cs_web_voting.Functions;

namespace SignalRChat.Hubs
{
    public class RoomHub : Hub
    {   

        private readonly IHubContext<RoomHub> _hubContext;
        private readonly CsWebVotingDbContext _dbContext;

        public RoomHub(CsWebVotingDbContext dbContext, IHubContext<RoomHub> hubContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }
        
        //Client requested to join a group
        //This happens when user submits their Nickname and Roomname. After that they are moved to /Room
        //Client waits for this to finish, before moving to /Room
        public async Task JoinGroup(string user, string roomname)
        {
            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(roomname))
            {
                //Add them to the group in SignalR
                await Groups.AddToGroupAsync(Context.ConnectionId, roomname);

                //Check if such group already exists
                Sessions newSession = null;
                //If it doesn't exist, add it:
                if(!_dbContext.sessions.Any(n => n.Name == roomname)){
                    newSession = new Sessions
                        {
                            Stage = 0,
                            Name = roomname,
                        };
                    //Also add a new countdown entry:
                    // Assuming initial values for Mode and Countdown
                    int initialModeValue = 1;
                    int initialCountdownValue = 120;
                    // Initialize the dictionary entry for the room
                    SharedData.countdowns[roomname] = new RoomData { Mode = initialModeValue, Countdown = initialCountdownValue };
                    
                };
                if (newSession != null)
                {
                    _dbContext.sessions.Add(newSession);
                };
                _dbContext.SaveChanges();

                //Add the user into the voters table.
                var record = _dbContext.sessions.FirstOrDefault(s => s.Name == roomname);
                Voters newVoter = null;
                //If there isn't a user with the same nickname yet:
                if(!_dbContext.voters.Any(n => n.Name == user && n.SessionID == record.SessionID)){
                    newVoter = new Voters
                        {
                            SessionID = record.SessionID,
                            ConnectionID = Context.ConnectionId,
                            Name = user,
                            Admin = 0,
                        };
                };
                if (newVoter != null)
                {
                    _dbContext.voters.Add(newVoter);
                };
                _dbContext.SaveChanges();
                //If this is a first user in the group:
                if (_dbContext.voters.Where(n => n.SessionID == record.SessionID).ToList().Count == 1){
                    _dbContext.voters.FirstOrDefault(n => n.SessionID == record.SessionID).Admin = 1;
                    _dbContext.SaveChanges();
                }

                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveConnection", "Connection established");
                //Update other's in the group:
                var logString = string.Format("{0} JOINED the room.",user.ToString());
                await Clients.Group(roomname).SendAsync("UpdateVoters", GetVoters(record.SessionID), logString);
            }
        }

        //Sends current voting pool and it's stats to the client
        //This pool is created from nominations when forwarding to stage 2
        public async Task RequestVotingPool()
        {
            var voter = _dbContext.voters.FirstOrDefault(s => s.ConnectionID == Context.ConnectionId);
            var sessionID = voter.SessionID;

            var initPool = _dbContext.votes?.Where(h => h.SessionId == sessionID).ToList();
            if (initPool == null)
            {
                initPool = new List<Votes>();
            }
            var votingPoolJson = JsonSerializer.Serialize(initPool);
            await Clients.Client(Context.ConnectionId).SendAsync("UpdateVotingPool", votingPoolJson);
        }

        public async Task RequestTop3()
        {
            var voter = _dbContext.voters.FirstOrDefault(s => s.ConnectionID == Context.ConnectionId);
            var sessionID = voter.SessionID;

            var scoredVotes = _dbContext.votes?.Where(h => h.SessionId == sessionID).ToList().OrderByDescending(obj => obj.VoteAmount).Take(3).ToList();
            var topVotesJson = JsonSerializer.Serialize(scoredVotes);
            await Clients.Client(Context.ConnectionId).SendAsync("UpdateTopVotes", topVotesJson);
        }

        //Client requested room information
        // /Room automatically asks for this information
        public async Task ServeRoomInformation()
        {
            var user = _dbContext.voters.FirstOrDefault(s => s.ConnectionID == Context.ConnectionId);
            //Gather session information
            var record = _dbContext.sessions.FirstOrDefault(s => s.SessionID == user.SessionID);

            CurrentSession currentSession = null;
            if(record != null){
                currentSession = new CurrentSession 
                {
                    SessionID = record.SessionID,
                    Stage = record.Stage,
                    Name = record.Name,
                    Timeleft = SharedData.countdowns[record.Name].Countdown,
                    MyName = user.Name,
                };
            }
            Console.WriteLine(SharedData.countdowns[record.Name].ToString() + " amount of count down");
            foreach (var key in SharedData.countdowns.Keys.ToList())
            {
                Console.WriteLine(key);
            }
            Console.WriteLine(currentSession.Timeleft);
            //Send back to the connection:
            var currentSessionJson = JsonSerializer.Serialize(currentSession);
            Console.WriteLine(currentSessionJson);

            var adCheck = false;
            if (_dbContext.voters.FirstOrDefault(s => s.ConnectionID == Context.ConnectionId).Admin == 1){
                adCheck = true;
            } else {
                adCheck = false;
            }
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveAdminStatus", adCheck);
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveRoomInformation", currentSessionJson, GetVoters(record.SessionID));
            if(record.Stage == 1 && _dbContext.nominations.Any(b => b.SessionId == record.SessionID)){
                var nominations = _dbContext.nominations.Where(b => b.SessionId == record.SessionID).ToList();
                var currentNominationsJson = JsonSerializer.Serialize(nominations);
                await Clients.Client(Context.ConnectionId).SendAsync("UpdateNominations", currentNominationsJson);
            }
        }

        public async Task RequestWinner()
        {

            var voter = _dbContext.voters.FirstOrDefault(s => s.ConnectionID == Context.ConnectionId);
            var sessionID = voter.SessionID;

            var winner = _dbContext.votes.Where(b => b.SessionId == sessionID).OrderByDescending(obj => obj.VoteAmount).Take(1).ToList();
            var winnerJson = JsonSerializer.Serialize(winner);
            await Clients.Client(Context.ConnectionId).SendAsync("UpdateWinner", winnerJson);

        }

        //Receive signal to forward a stage.
        public async Task RequestForwardStage()
        {
            var voter = _dbContext.voters.FirstOrDefault(s => s.ConnectionID == Context.ConnectionId);
            var sessionID = voter.SessionID;
            var session = _dbContext.sessions.FirstOrDefault(s => s.SessionID == sessionID);
            var roomname = session.Name;
            if (voter.Admin == 1 && session.Stage != 3){
                CommonFunctions.ForwardStage(_dbContext, _hubContext, roomname);
            }
            
        }
        
        public async Task RequestStagePlayback(bool mode)
        {
            var voter = _dbContext.voters.FirstOrDefault(s => s.ConnectionID == Context.ConnectionId);
            var voterName = voter.Name;
            var sessionID = voter.SessionID;
            var session = _dbContext.sessions.FirstOrDefault(s => s.SessionID == sessionID);
            var roomname = session.Name;
            var msgMode = "";

            int trMode = new();

            if (mode){
                trMode = 0;
                msgMode = "PAUSED";
            } else {
                trMode = 1;
                msgMode = "RESUMED";
            }
            //If mode is true, resume
            //If mode is false, pause 
            if (SharedData.countdowns[roomname].Mode != trMode){

                SharedData.countdowns[roomname].Mode = trMode;
                Console.WriteLine(SharedData.countdowns[roomname].Mode.ToString() + "UPDATED MODE OF SESSION");
                //Pause the clients
                var logString = string.Format("{0} {1} the stage.",voterName.ToString(),msgMode);
                await Clients.Group(roomname).SendAsync("UpdatePlaybackTimer", SharedData.countdowns[roomname].Mode, logString);
            }

        }
        
        public async Task SubmitAdminPassword(string password)
        {
            var voter = _dbContext.voters.FirstOrDefault(s => s.ConnectionID == Context.ConnectionId); 
            //Is the password correct?
            if (_dbContext.passwords.Any(s => s.Password == password)){
                voter.Admin = 1;
                _dbContext.SaveChanges();
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveAdminStatus", true);
            } else {
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveAdminStatus", false);
            }
        }
        public async Task SubmitVote(string singleVote)
        {
            var voter = _dbContext.voters.FirstOrDefault(s => s.ConnectionID == Context.ConnectionId);
            var voterName = voter.Name;
            var sessionID = voter.SessionID;
            var session = _dbContext.sessions.FirstOrDefault(s => s.SessionID == sessionID);
            var roomname = session.Name;
            if(session.Stage == 2 && voter.VtAmnt <1)
            {
                //Check if the voted map is actually in the nomiated maps
                //If so, increase it's count
                //and trigger update for all users:
                    //voting pool
                    //top 3 pool
                if(_dbContext.votes.Any(n => n.Name == singleVote && n.SessionId == sessionID))
                {
                    _dbContext.votes.FirstOrDefault(n => n.Name == singleVote && n.SessionId == sessionID).VoteAmount += 1;

                    voter.VtAmnt +=1;
                    _dbContext.SaveChanges();

                    //Send the whole thing (for now)
                    var initPool = _dbContext.votes?.Where(h => h.SessionId == sessionID).ToList();
                    var votingPoolJson = JsonSerializer.Serialize(initPool);
                    var logString = string.Format("{0} VOTED for {1}.",voterName.ToString(),singleVote);
                    await Clients.Group(roomname).SendAsync("UpdateVotingPool", votingPoolJson, logString);

                    //Send top 3
                    var scoredVotes = _dbContext.votes?.Where(h => h.SessionId == sessionID).ToList().OrderByDescending(obj => obj.VoteAmount).Take(3).ToList();
                    var topVotesJson = JsonSerializer.Serialize(scoredVotes);
                    await Clients.Group(roomname).SendAsync("UpdateTopVotes", topVotesJson);

                    //If everyone has voted
                    if(!_dbContext.voters.Any(s => s.VtAmnt == 0 && s.SessionID == sessionID)){
                        //Forward stage
                        CommonFunctions.ForwardStage(_dbContext, _hubContext, roomname);
                    }
                }
            }
        }
        public async Task SubmitNominations(string[] nominatedMaps)
        {
            var voter = _dbContext.voters.FirstOrDefault(s => s.ConnectionID == Context.ConnectionId);
            var voterName = voter.Name;
            var sessionID = voter.SessionID;
            var session = _dbContext.sessions.FirstOrDefault(s => s.SessionID == sessionID);
            var roomname = session.Name;

            if (nominatedMaps.Length > 3)
            {
                nominatedMaps = new string[3];
                Array.Copy(nominatedMaps, nominatedMaps, 3);
            }

            if(session.Stage == 1 && voter.NmntAmnt <1){
                List<string> nominatedMapsList = nominatedMaps.ToList();
                //Save into database for user logging mid/nomination:
                //First, filter out those that have already been nominated:
                nominatedMapsList = nominatedMapsList.Where(map => !_dbContext.nominations.Any(n => n.Name == map && n.SessionId == sessionID)).ToList();
                //nominatedMaps have been reduced. Add them to the nominated table: 
                foreach (var map in nominatedMapsList)
                {   
                    Nominations newNomination = null;

                    newNomination = new Nominations
                    {
                        SessionId = sessionID,
                        Name = map,
                    };
                    if (newNomination != null)
                    {
                        _dbContext.nominations.Add(newNomination);
                    }
                }
                voter.NmntAmnt +=1;
                _dbContext.SaveChanges();
                var currentNominationsJson = JsonSerializer.Serialize(_dbContext.nominations.Where(s => s.SessionId == sessionID).ToList());
                //Hub send out an update with these nominations:
                var logString = string.Format("{0} NOMINATED: {1}.",voterName.ToString(), string.Join(", ", nominatedMaps));
                await Clients.Group(roomname).SendAsync("UpdateNominations", currentNominationsJson, logString);
                
                //If everyone has nominated
                if(!_dbContext.voters.Any(s => s.NmntAmnt == 0 && s.SessionID == sessionID)){
                    //Forward stage
                    CommonFunctions.ForwardStage(_dbContext, _hubContext, roomname);
                }
            }
        }
        
        public async Task RemoveVoter(string name){
            var voter = _dbContext.voters.FirstOrDefault(s => s.ConnectionID == Context.ConnectionId);
            var sessionID = voter.SessionID;
            var session = _dbContext.sessions.FirstOrDefault(s => s.SessionID == sessionID);
            var roomname = session.Name;
            if(voter.Admin == 1){
                var voterToRemove = _dbContext.voters.FirstOrDefault(s => s.Name == name && s.SessionID == sessionID);
                //Remove from SignalR group
                await Groups.RemoveFromGroupAsync(voterToRemove.ConnectionID, roomname);
                await Clients.Client(voterToRemove.ConnectionID).SendAsync("ReceiveKickAlert");
                //Remove from database.
                _dbContext.voters.Remove(voterToRemove);
                _dbContext.SaveChanges();
                //Ammounce kick to the room:
                var logString = string.Format("{0} was KICKED.",voterToRemove.Name.ToString());
                await Clients.Group(roomname).SendAsync("UpdateVoters", GetVoters(sessionID), logString);
            }
        }

        public async Task RemoveNomination(string nomination)
        {
            Console.WriteLine("test");
            var voter = _dbContext.voters.FirstOrDefault(s => s.ConnectionID == Context.ConnectionId);
            var voterName = voter.Name;
            var sessionID = voter.SessionID;
            var session = _dbContext.sessions.FirstOrDefault(s => s.SessionID == sessionID);
            var roomname = session.Name;
            //If user is admin and the stage is nomination stage
            if(voter.Admin == 1 && session.Stage == 1){
                //Remove the nomination from the nominating table
                
                var nomToRemove = _dbContext.nominations.FirstOrDefault(s => s.Name == nomination && s.SessionId == sessionID);
                _dbContext.nominations.Remove(nomToRemove);
                _dbContext.SaveChanges();
                //Push UpdateNominations
                var currentNominationsJson = JsonSerializer.Serialize(_dbContext.nominations.Where(s => s.SessionId == sessionID).ToList());
                var logString = string.Format("Admin {0} REMOVED nomination: {1}.",voterName.ToString(),nomination.ToString());
                await Clients.Group(roomname).SendAsync("UpdateNominations", currentNominationsJson, logString);
            }
            
            
        }
        //When someone disconnects, delete them from voters
        //If they were last voter with the sessionID, delete the session from the sessions table.
        //Don't have to remove this user from SignalR groups or delete the SignalR group, that's done automatically
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //Do only if the connection is actually in a group.
            // Perform tasks when a client disconnects
            // For example, remove the user from a group or update user status

            // Access the connection ID of the disconnected client
            var connectionId = Context.ConnectionId;
            //This voter's record
            var thisVoter = _dbContext.voters.FirstOrDefault(s => s.ConnectionID == connectionId);
            var voterName = thisVoter.Name;
            var currentSessionID = thisVoter.SessionID;
            var getVoters = _dbContext.voters.Where(record => record.SessionID == currentSessionID).ToList();
            //If there's one or less voters with the same sessionID
            if(getVoters.Count <= 1 ){
                //Delete the session.
                var sessionToRemove = _dbContext.sessions.FirstOrDefault(s => s.SessionID == thisVoter.SessionID);
                _dbContext.sessions.Remove(sessionToRemove);
                //Delete it's counter as well:
                SharedData.countdowns.Remove(sessionToRemove.Name);
                //Delte nominated maps:
                var nomsToRemove = _dbContext.nominations.Where(record => record.SessionId == currentSessionID);
                _dbContext.nominations.RemoveRange(nomsToRemove);
                //Delte votes maps:
                var votsToRemove = _dbContext.votes.Where(record => record.SessionId == currentSessionID);
                _dbContext.votes.RemoveRange(votsToRemove);
            }
            //Inform the group about this user leaving.
            //here
            Console.WriteLine("Disconnected");
            //Remove the voter record.
            _dbContext.voters.Remove(thisVoter);
            var logString = string.Format("{0} DISCONNECTED.",voterName.ToString());
            //Save
            _dbContext.SaveChanges();
            await Clients.Group(_dbContext.sessions.FirstOrDefault(s => s.SessionID == currentSessionID).Name).SendAsync("UpdateVoters", GetVoters(currentSessionID), logString);
            await base.OnDisconnectedAsync(exception);
        }
        private List<string> GetVoters (int sessionID) {
            List<string> currentVoters = new List<string>();
            var getVoters = _dbContext.voters.Where(voter => voter.SessionID == sessionID).ToList();
            foreach (var voter in getVoters)
            {   
                currentVoters.Add(voter.Name.ToString());
            }
            return currentVoters;
        }

        
    }

    
    [Serializable]
    class CurrentSession
    {
        public int SessionID { get; set; }
        public int Stage { get; set; }
        public string Name { get; set; }
        public int Timeleft {get;set;}
        public string MyName {get;set;}

    };
}