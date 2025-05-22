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

            var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var newUserId = reader.GetInt64(0);
                return Ok(new { message = "User successfully added.", userId = newUserId });
            }
        }
        catch (MySqlException ex)
        {
            return BadRequest(ex.Message);
        }

        return BadRequest("User could not be added.");
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

    [HttpDelete("{userId:int}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        if (userId <= 0)
        {
            return BadRequest("Invalid user ID.");
        }

        try
        {
            using var command = await _context.CreateStoredProcedureCommandAsync(
                "CALL DeleteUser(@userID)"
            );

            command.Parameters.Add(new MySqlParameter("@userID", userId));

            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var status = reader.GetString(0);
                return Ok(new { message = status });
            }
        }
        catch (MySqlException ex)
        {
            return BadRequest(ex.Message);
        }

        return BadRequest("Failed to delete user.");
    }
}
