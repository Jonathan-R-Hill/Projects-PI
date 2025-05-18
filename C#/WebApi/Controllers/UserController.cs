using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MySqlConnector;


[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] AddUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName) ||
            string.IsNullOrWhiteSpace(request.UserName))
        {
            return BadRequest("All fields are required");
        }

        try
        {
            using var command = await _context.CreateStoredProcedureCommandAsync(
                "CALL AddUser(@firstName, @lastName, @userName)"
            );

            command.Parameters.Add(new MySqlParameter("@firstName", request.FirstName));
            command.Parameters.Add(new MySqlParameter("@lastName", request.LastName));
            command.Parameters.Add(new MySqlParameter("@userName", request.UserName));

            await command.ExecuteNonQueryAsync();
        }
        catch (MySqlException ex)
        {
            return BadRequest(ex.Message);
        }

        return Ok(new { message = "User successfully added." });
    }

    [HttpGet("{userName}")]
    public async Task<IActionResult> GetUserId(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return BadRequest("User name is required");
        }

        try
        {
            using var command = await _context.CreateStoredProcedureCommandAsync(
                "CALL GetUserIdByUserName(@userName)"
            );

            command.Parameters.Add(new MySqlParameter("@userName", userName));

            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var userId = reader.GetInt32(0);
                return Ok(new { UserId = userId });
            }
        }
        catch (MySqlException ex)
        {
            return BadRequest(ex.Message);
        }

        return NotFound("User not found");
    }
}

