using Microsoft.AspNetCore.Mvc;
using cs_web_voting.Data;
using cs_web_voting.Models;
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

    [HttpGet]
    public IActionResult GetNominated()
    {
        //GET NOMINATED MAPS FROM NOMINATED TABLE AND INSERT EACH INTO VOTED TABLE WITH INITIAL 0 SCORE.
        //RETURN VOTED TABLE
        var nominatedMaps = _dbContext.nominations?.ToList();

        if(nominatedMaps is not null){
            foreach (var map in nominatedMaps)
            {
                Votes? newInit = null;
                //CHECK FOR SESSION ID AS WELL
                if (_dbContext.votes is not null){
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
                
            }
        }
        
        _dbContext.SaveChanges();
        var initNominations = (_dbContext.votes?.ToList()) ?? new List<Votes>();
      
        //Take nominated array from the singleton and create an object out of it.
        string jsonContent = JsonSerializer.Serialize(initNominations);
        // Set the appropriate content type for the image
        return Content(jsonContent, "application/json");
    }
}