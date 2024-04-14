using cs_web_voting.Data;
using cs_web_voting.Functions;
using cs_web_voting.Singletons;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs;

public class Counter : IHostedService
{
    private readonly IHubContext<RoomHub> _hubContext;
    private readonly IServiceScopeFactory _scopeFactory;

    public Counter(IHubContext<RoomHub> hubContext, IServiceScopeFactory scopeFactory)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Start the background task when the application starts
        Task.Run(() => UpdateCountdowns(cancellationToken), cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Perform cleanup or finalization if needed
        return Task.CompletedTask;
    }
    
    private void UpdateCountdown(string roomname, CsWebVotingDbContext dbContext)
    {
        //Print the mode of sessions into the console.
        //Console.WriteLine(SharedData.countdowns[roomname].Mode.ToString());
        // If the session isn't paused
        if (SharedData.countdowns[roomname].Mode == 1)
        {
            // Check if the countdown has reached zero
            if (SharedData.countdowns[roomname].Countdown <= 0)
            {
                //If so, forward the stage
                CommonFunctions.ForwardStage(dbContext, _hubContext, roomname);
            }
            else
            {
                //Else keep counting down
                SharedData.countdowns[roomname].Countdown -= 1;
            }
        }
    }

    // Background task to update countdowns
    private async Task UpdateCountdowns(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CsWebVotingDbContext>();

                if (SharedData.countdowns.Keys.Count() >= 1)
                {
                    foreach (var roomname in SharedData.countdowns.Keys.ToList())
                    {
                        UpdateCountdown(roomname, dbContext);
                    }
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }
}