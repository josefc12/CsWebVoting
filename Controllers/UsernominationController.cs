using Microsoft.AspNetCore.Mvc;
using cs_web_voting.Data;
using cs_web_voting.Models;
using System.Text.Json;
namespace cs_web_voting.Controllers;

[ApiController]
[Route("[controller]")]
public class UsernominationController : ControllerBase
{   
    private readonly CsWebVotingDbContext _dbContext;

    public UsernominationController(CsWebVotingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost] // This attribute specifies that this method handles HTTP POST requests
    public IActionResult PostData([FromBody] JsonDocument requestData)
    {
        
        try
        {
            /*
            string jsonString = requestData.RootElement.ToString();
            SharedData.JsonData = jsonString;
            var nominatedMaps = requestData.RootElement.EnumerateArray()
                .Select(element => element.GetString())
                .ToList();

            SharedData.NominatedMaps.AddRange(nominatedMaps.Except(SharedData.NominatedMaps).Where(map => map != null).Select(map => map!));

            foreach (var map in SharedData.NominatedMaps)
            {
                Console.WriteLine(map);
            }
            */
            //Save into database for user logging mid/nomination:
            //First, filter out those that have already been nominated:
            var nominatedMaps = requestData.RootElement.EnumerateArray().Select(element => element.GetString()).ToList();
            foreach (var map in nominatedMaps)
            {
                if (!_dbContext.nominations.Any(n => n.Name == map))
                {
                    nominatedMaps.Remove(map);
                }
            }
            //nominatedMaps have been reduced. Add the m 
            foreach (var map in nominatedMaps)
            {   
                Nominations newNomination = null;
                //CHECK FOR SESSION ID AS WELL
                if (!_dbContext.nominations.Any(n => n.Name == map))
                {
                    newNomination = new Nominations
                    {
                        SessionId = 1,
                        Name = map,
                    };
                }
                if (newNomination != null)
                {
                    _dbContext.nominations.Add(newNomination);
                }
            }
            _dbContext.SaveChanges();

            //Invoke Hub to send out an update with these nominations:


            var nominations = _dbContext.nominations?.ToList();
            if (nominations == null)
            {
                nominations = new List<Nominations>();
            }
            // Return a success response
            return Ok(new {
                Message = "Data received successfully",
                nominatedMaps = nominations,
                });
        }
        catch (Exception ex)
        {

            // Log or handle exceptions as needed
            return BadRequest(new { Message = $"Error: {ex.Message}"});
        }
    }
}