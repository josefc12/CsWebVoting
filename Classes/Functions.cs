using cs_web_voting.Data;
using cs_web_voting.Models;
using cs_web_voting.Singletons;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs;

namespace cs_web_voting.Functions;
public static class CommonFunctions
{
    public static void ForwardStage(CsWebVotingDbContext database, IHubContext<RoomHub> hub, string roomname)
    {
        //Check current stage
        var currentSession = database.sessions?.FirstOrDefault(s => s.Name == roomname);
        var currentStage = currentSession?.Stage;

        if (currentStage < 3 && currentSession is not null){
            
            if(currentStage == 1){
                //Stage is going to be 2 - voting stage, initialize voting pool.
                var nominatedMaps = database.nominations?.Where(h => h.SessionId == currentSession.SessionID).ToList();
                
                if (nominatedMaps is not null){
                    foreach (var map in nominatedMaps)
                    {
                        Votes? newInit = null;
                        if (database.votes is not null){
                            if (!database.votes.Any(n => n.Name == map.Name && n.SessionId == currentSession.SessionID)){
                                newInit = new Votes
                                {
                                    SessionId = currentSession.SessionID,
                                    Name = map.Name,
                                    VoteAmount = 0,

                                };
                            }
                            if (newInit != null)
                            {
                                database.votes.Add(newInit);
                            }
                        }
                    }
                }
            }

            currentSession.Stage += 1;
            //Set new countdown
            SharedData.countdowns[roomname].Countdown = 120;
            //Push both new stage and countdown to the clients in the group
            database.SaveChanges();
            hub.Clients.Group(roomname).SendAsync("ReceiveForwardStage", currentSession.Stage, SharedData.countdowns[roomname].Countdown);
            
        } else if (currentStage == 3 && currentSession is not null){
            //The session has completely ended
            var seshVoters = database.voters?.Where(record => record.SessionID == currentSession.SessionID);

            if (seshVoters is not null){
                foreach (var voter in seshVoters.ToList()) {
                    //Disconnect voter from group
                    if (voter.ConnectionID is not null){
                        hub.Groups.RemoveFromGroupAsync(voter.ConnectionID, roomname);
                    }
                }
            }
            //Delete voters from database
            database.voters?.RemoveRange(seshVoters);
            //Delete nominated maps
            var nomsToRemove = database.nominations?.Where(record => record.SessionId == currentSession.SessionID);
            if (nomsToRemove is not null){
                database.nominations?.RemoveRange(nomsToRemove);
            }
            //Delte votes maps:
            var votsToRemove = database.votes?.Where(record => record.SessionId == currentSession.SessionID);
            if (votsToRemove is not null){
                database.votes?.RemoveRange(votsToRemove);
            }
            //Remove session from database
            database.sessions?.Remove(currentSession);
            database.SaveChanges();
            //Remove the countdown
            SharedData.countdowns.Remove(roomname);
        }
    }
}