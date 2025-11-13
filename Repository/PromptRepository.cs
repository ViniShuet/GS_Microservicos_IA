using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using Domain;

namespace Repository
{
    public class PromptRepository : IPromptRepository
    {
        private readonly string _connectionString;

        public PromptRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Prompt> AddPromptAsync(Prompt prompt)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                INSERT INTO prompt (nome, descricao)
                VALUES (@Nome, @Descricao);
                SELECT LAST_INSERT_ID();
            ";

            int newId = await connection.QuerySingleAsync<int>(sql, prompt);
            prompt.Id = newId;
            return prompt;
        }

        public async Task<IEnumerable<Prompt>> GetAllPromptsAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            return await connection.QueryAsync<Prompt>("SELECT * FROM prompt");
        }
        public async Task UpdatePromptAsync(Prompt prompt)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                UPDATE prompt
                SET name = @Name, descricao = @Descricao, data = @Data 
                WHERE id = @Id;
            ";

            await connection.ExecuteAsync(sql, prompt);
        }

        public async Task DeletePromptAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "DELETE FROM prompt WHERE id = @Id;";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
