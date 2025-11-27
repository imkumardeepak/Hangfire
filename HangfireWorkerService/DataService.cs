using Npgsql;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace HangfireWorkerService;

public class DataService(ILogger<DataService> logger, IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("PostgreSqlConnection") ?? string.Empty;

    public async Task ProcessData()
    {
        try
        {
            logger.LogInformation("Starting data processing at {Time}", DateTime.UtcNow);
            
            // Ensure the sample table exists
            await CreateSampleTable();
            
            // Insert sample data
            await InsertSampleData();
            
            // Read data from PostgreSQL
            var data = await ReadDataFromDatabase();
            
            // Wait for 2 seconds before printing
            await Task.Delay(2000);
            
            // Print data to console
            logger.LogInformation("Data retrieved: {Data}", data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing data");
        }
    }

    private async Task CreateSampleTable()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        using var cmd = new NpgsqlCommand(@"
            CREATE TABLE IF NOT EXISTS sample_data (
                id SERIAL PRIMARY KEY,
                name VARCHAR(100),
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            )", connection);
        
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task InsertSampleData()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        using var cmd = new NpgsqlCommand(@"
            INSERT INTO sample_data (name) 
            VALUES (@name)", connection);
        
        cmd.Parameters.AddWithValue("name", $"Sample Data {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task<string> ReadDataFromDatabase()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        // Read the latest 5 records
        using var cmd = new NpgsqlCommand(@"
            SELECT id, name, created_at 
            FROM sample_data 
            ORDER BY created_at DESC 
            LIMIT 5", connection);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        var results = new List<string>();
        
        while (await reader.ReadAsync())
        {
            var id = reader.GetInt32("id");
            var name = reader.GetString("name");
            var createdAt = reader.GetDateTime("created_at");
            results.Add($"ID: {id}, Name: {name}, Created: {createdAt}");
        }
        
        return results.Count > 0 ? string.Join("; ", results) : "No data found";
    }
}