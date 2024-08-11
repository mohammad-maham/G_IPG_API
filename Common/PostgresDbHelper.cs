using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

public class PostgresDbHelper
{
    private readonly string _connectionString = ConfigurationManager.AppSetting["ConnectionStrings:GWalletDbContext"]!;

    public PostgresDbHelper()
    {
    }

    private NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }


    public DataTable RunQuery(string query)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = new NpgsqlCommand(query, connection))
            {
                using (var adapter = new NpgsqlDataAdapter(command))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    //Task.Run(() => adapter.Fill(dt));
                    return dt;
                }
            }
        }
    }

    public void Insert(string tableName, Dictionary<string, object> values)
    {
        if (string.IsNullOrEmpty(tableName) || values == null || values.Count == 0)
        {
            throw new ArgumentException("Invalid table name or values.");
        }

        string columns = string.Join(", ", values.Keys);
        string parameterNames = string.Join(", ", values.Keys.Select(k => "@" + k));

        string sql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameterNames})";

        try
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    foreach (var kvp in values)
                    {
                        command.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                    }

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            // Handle or log the exception as needed
            throw new Exception("An error occurred while inserting data.", ex);
        }
    }
}