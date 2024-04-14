using Microsoft.AspNetCore.Mvc;
using cs_web_voting.Data;
using System.Text.Json;

namespace cs_web_voting.Controllers;

[ApiController]
[Route("[controller]")]
public class GetwinnerController : ControllerBase
{
    private readonly CsWebVotingDbContext _dbContext;

    public GetwinnerController(CsWebVotingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult GetWinner()
    {
        var winner = _dbContext.votes?.OrderByDescending(obj => obj.VoteAmount).Take(1).ToList();
        // Create an array of objects with Id and Name fields
        if (winner is not null){
            foreach (var item in winner)
            {
                Console.WriteLine($"Name: {item.Name}, Count: {item.VoteAmount}");
            }
        }
        
        //Take nominated array from the singleton and create an object out of it.
        string jsonContent = JsonSerializer.Serialize(winner);
        // Set the appropriate content type for the image
        return Content(jsonContent, "application/json");
    }
}