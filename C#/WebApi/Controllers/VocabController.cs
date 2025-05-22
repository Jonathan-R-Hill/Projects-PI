using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

    [HttpPost]
    public async Task<IActionResult> AddVocabulary([FromBody] AddVocabRequest request)
    {
        if (request.UserID <= 0 || 
            string.IsNullOrWhiteSpace(request.KnownWord) || 
            string.IsNullOrWhiteSpace(request.TargetWord))
        {
            return BadRequest("User ID, KnownWord, and TargetWord are required.");
        }

        try
        {
            using var command = await _context.CreateStoredProcedureCommandAsync("CALL AddVocabulary(@userID, @knownWord, @targetWord)");

            command.Parameters.Add(new MySqlParameter("@userID", request.UserID));
            command.Parameters.Add(new MySqlParameter("@knownWord", request.KnownWord));
            command.Parameters.Add(new MySqlParameter("@targetWord", request.TargetWord));

            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var newVocabId = reader.GetInt64(0);
                return Ok(new { message = "Vocabulary added", vocabId = newVocabId });
            }
        }
        catch (MySqlException ex)
        {
            return BadRequest(ex.Message);
        }

        return BadRequest("Failed to add vocabulary.");
    }

    [HttpGet("all/{userId}")]
    public async Task<IActionResult> GetAllVocabulary(int userId)
    {
        try
        {
            using var command = await _context.CreateStoredProcedureCommandAsync("CALL GetAllVocabulary(@userID)");
            command.Parameters.Add(new MySqlParameter("@userID", userId));

            var reader = await command.ExecuteReaderAsync();
            var result = new List<object>();

            while (await reader.ReadAsync())
            {
                result.Add(new
                {
                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                    KnownWord = reader.GetString(reader.GetOrdinal("KnownLanguage-Word")),
                    TargetWord = reader.GetString(reader.GetOrdinal("TargetLanguage-Word")),
                    Learnt = reader.GetBoolean(reader.GetOrdinal("Learnt"))
                });
            }

            return Ok(result);
        }
        catch (MySqlException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{userId}/{vocabId}")]
    public async Task<IActionResult> GetVocabularyByID(int userId, int vocabId)
    {
        try
        {
            using var command = await _context.CreateStoredProcedureCommandAsync("CALL GetVocabularyByID(@userID, @vocabID)");
            command.Parameters.Add(new MySqlParameter("@userID", userId));
            command.Parameters.Add(new MySqlParameter("@vocabID", vocabId));

            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Ok(new
                {
                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                    KnownWord = reader.GetString(reader.GetOrdinal("KnownLanguage-Word")),
                    TargetWord = reader.GetString(reader.GetOrdinal("TargetLanguage-Word")),
                    Learnt = reader.GetBoolean(reader.GetOrdinal("Learnt"))
                });
            }

            return NotFound("Vocabulary item not found.");
        }
        catch (MySqlException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("unknown/{userId}")]
    public async Task<IActionResult> GetUnknownVocabulary(int userId)
    {
        return await GetVocabularyWithLearntStatus(userId, learnt: false);
    }

    [HttpGet("known/{userId}")]
    public async Task<IActionResult> GetKnownVocabulary(int userId)
    {
        return await GetVocabularyWithLearntStatus(userId, learnt: true);
    }

    private async Task<IActionResult> GetVocabularyWithLearntStatus(int userId, bool learnt)
    {
        var procedure = learnt ? "GetKnownVocabulary" : "GetUnknownVocabulary";

        try
        {
            using var command = await _context.CreateStoredProcedureCommandAsync($"CALL {procedure}(@userID)");
            command.Parameters.Add(new MySqlParameter("@userID", userId));

            var reader = await command.ExecuteReaderAsync();
            var result = new List<object>();

            while (await reader.ReadAsync())
            {
                result.Add(new
                {
                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                    KnownWord = reader.GetString(reader.GetOrdinal("KnownLanguage-Word")),
                    TargetWord = reader.GetString(reader.GetOrdinal("TargetLanguage-Word")),
                    Learnt = reader.GetBoolean(reader.GetOrdinal("Learnt"))
                });
            }

            return Ok(result);
        }
        catch (MySqlException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{userId}/{vocabId}")]
    public async Task<IActionResult> DeleteVocabulary(int userId, int vocabId)
    {
        try
        {
            using var command = await _context.CreateStoredProcedureCommandAsync("CALL DeleteVocabulary(@userID, @vocabID)");

            command.Parameters.Add(new MySqlParameter("@userID", userId));
            command.Parameters.Add(new MySqlParameter("@vocabID", vocabId));

            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var status = reader.GetString(0);
                return Ok(new { message = status });
            }

            return BadRequest("Deletion failed.");
        }
        catch (MySqlException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
