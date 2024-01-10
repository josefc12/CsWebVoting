using Microsoft.AspNetCore.Mvc;
using cs_web_voting.Data;
using cs_web_voting.Models;
using cs_web_voting.Singletons;
using System.Text.Json;
namespace cs_web_voting.Controllers;

[ApiController]
[Route("[controller]")]
public class NominatedmapsController : ControllerBase
{
    private readonly CsWebVotingDbContext _dbContext;

    public NominatedmapsController(CsWebVotingDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    //[HttpGet("{imageName}")]
    [HttpGet]
    public IActionResult GetNominated()
    {
        //GET NOMINATED MAPS FROM NOMINATED TABLE AND INSERT EACH INTO VOTED TABLE WITH INITIAL 0 SCORE.
        //RETURN VOTED TABLE
        var nominatedMaps = _dbContext.nominations?.ToList();
        foreach (var map in nominatedMaps)
        {
            Votes newInit = null;
            //CHECK FOR SESSION ID AS WELL
            if (!_dbContext.votes.Any(n => n.Name == map.Name)){
                newInit = new Votes
                {
                    SessionId = 1,
                    Name = map.Name,
                    VoteAmount = 0,

                };
            }
            if (newInit != null)
            {
                _dbContext.votes.Add(newInit);
            } 
        }
        _dbContext.SaveChanges();
        var initNominations = _dbContext.votes?.ToList();
        if (initNominations == null)
        {
            initNominations = new List<Votes>();
        }
        /*
        // Assuming SharedData.NominatedMaps is a List<string>
        var nominatedNames = SharedData.NominatedMaps;
        // Create an array of objects with Id and Name fields
        var formattedArray = nominatedNames.Select((name, index) => new { Id = index + 1, Name = name }).ToArray();
        */
        //Take nominated array from the singleton and create an object out of it.
        string jsonContent = JsonSerializer.Serialize(initNominations);
        // Set the appropriate content type for the image
        return Content(jsonContent, "application/json");
    }
}