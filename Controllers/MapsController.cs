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

    [HttpGet]
    public IActionResult GetMaps()
    {
        
        var maps = (_dbContext.maps?.ToList()) ?? new List<Maps>();
        string jsonContent = JsonSerializer.Serialize(maps);
        return Content(jsonContent, "application/json");
        
    }
}