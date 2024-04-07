using Microsoft.AspNetCore.Mvc;
using cs_web_voting.Data;
using System.Text.Json;
using cs_web_voting.Models;

namespace cs_web_voting.Controllers;

[ApiController]
[Route("[controller]")]
public class MapsController : ControllerBase
{
    private readonly CsWebVotingDbContext _dbContext;

    public MapsController(CsWebVotingDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    //[HttpGet("{imageName}")]
    [HttpGet]
    public IActionResult GetImage()
    {
        
       var maps = _dbContext.maps?.ToList();
        if (maps == null)
        {
            maps = new List<Maps>();
        }
        string jsonContent = JsonSerializer.Serialize(maps);
        return Content(jsonContent, "application/json");
        
    }
}