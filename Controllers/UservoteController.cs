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

    [HttpPost] // This attribute specifies that this method handles HTTP POST requests
    public IActionResult PostData([FromBody] JsonDocument requestData)
    {
        try
        {
            //Increase the voted amount based in jsonString
            //Send updated votes as map data
            //Generate top 3 from votes
            //Send that as votedMaps
            /*
            string jsonString = requestData.RootElement.ToString();
            
            Console.WriteLine(jsonString);
            //Add vote to a List in SharedData
            ProcessJsonString(jsonString);

            foreach (var item in SharedData.VotedMaps.OfType<VoteObject>())
            {
                Console.WriteLine($"Name: {item.Name}, Count: {item.Count}");
            }
            //Add vote to a List in SharedData
            //Take the List in SharedData, sort out top 3
            //Send back down in OK:
                //object from VotedMaps as mapData;
                //object from TopMaps as votedMaps;
            
            var scoredList = SharedData.VotedMaps
                .OfType<VoteObject>()
                .OrderByDescending(obj => obj.Count)
                .Take(3)
                .ToList();
            */
            string jsonString = requestData.RootElement.ToString();
            var voteToEdit = _dbContext.votes.FirstOrDefault(m => m.Name == jsonString);
            if (voteToEdit != null)
            {
                // Step 2: Modify the property values of the retrieved record
                voteToEdit.VoteAmount += 1;
                // ... modify other properties
                // Step 3: Save changes to the database
                _dbContext.SaveChanges();
            }
            var votes = _dbContext.votes?.ToList();
            if (votes == null)
            {
                votes = new List<Votes>();
            }

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

            // Log or handle exceptions as needed
            return BadRequest(new { Message = $"Error: {ex.Message}"});
        }
    }
    private void ProcessJsonString(string jsonString)
    {   
        
        // Check if an object with the same string already exists in VotedMaps
        var existingObject = SharedData.VotedMaps.OfType<VoteObject>().FirstOrDefault(obj => obj?.Name == jsonString);
        Console.WriteLine(existingObject?.Name + " object name");
        if (existingObject != null)
        {
            // If it exists, increment the count field
            IncrementCount(existingObject);
            Console.WriteLine("kek1");
        }
        else
        {
            // If it doesn't exist, add a new object to VotedMaps
            AddNewObject(jsonString);
            Console.WriteLine("kek2");
        }
    }

    private void IncrementCount(object existingObject)
    {
        // Assuming you have a count field in your object
        // You need to cast the object to the appropriate type to modify its properties
        if (existingObject is VoteObject myObject)
        {
            myObject.Count++;
            Console.WriteLine(myObject?.Count + " object count");
        }
    }

    private void AddNewObject(string jsonString)
    {
        // Assuming you have a constructor for your object
        var newObject = new VoteObject
        {
            Name = jsonString,
            Count = 1
        };

        SharedData.VotedMaps.Add(newObject);
    }

}