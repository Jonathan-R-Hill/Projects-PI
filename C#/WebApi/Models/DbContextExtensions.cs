using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Threading.Tasks;

public static class DbContextExtensions
{
    public static async Task<DbCommand> CreateStoredProcedureCommandAsync(this DbContext context, string procedureSql)
    {
        var connection = context.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = procedureSql;
        command.CommandType = System.Data.CommandType.Text;

        return command;
    }
}
