using Microsoft.AspNetCore.Mvc;
using cs_web_voting.Singletons;
using cs_web_voting.Data;
using cs_web_voting.Models;
using System.Text.Json;
namespace cs_web_voting.Controllers;

[ApiController]
[Route("[controller]")]
public class UservoteController : ControllerBase
{
    private readonly CsWebVotingDbContext _dbContext;

    public UservoteController(CsWebVotingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public IActionResult PostData([FromBody] JsonDocument requestData)
    {
        try
        {
            string jsonString = requestData.RootElement.ToString();
            var voteToEdit = _dbContext.votes?.FirstOrDefault(m => m.Name == jsonString);
            if (voteToEdit != null)
            {
                voteToEdit.VoteAmount += 1;
                _dbContext.SaveChanges();
            }
            var votes = (_dbContext.votes?.ToList()) ?? new List<Votes>();
            var scoredVotes = votes.OrderByDescending(obj => obj.VoteAmount).Take(3).ToList();
            // Return a success response
            return Ok(new {
                Message = "Data received successfully",
                mapData = votes, //object from VotedMaps as mapData;
                votedMaps = scoredVotes
                });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = $"Error: {ex.Message}"});
        }
    }

}