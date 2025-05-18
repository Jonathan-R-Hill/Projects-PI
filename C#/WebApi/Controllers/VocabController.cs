using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MySqlConnector;


[ApiController]
[Route("api/[controller]")]
public class VocabController : ControllerBase
{
    private readonly AppDbContext _context;

    public VocabController(AppDbContext context)
    {
        _context = context;
    }



}

