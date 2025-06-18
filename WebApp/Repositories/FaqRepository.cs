using Dapper;

using System.Data;

using WebApp.IRepository;
using WebApp.Models;

namespace WebApp.Repositories
{
    public class FaqRepository:IFaqRepository
    {
        private readonly IDbConnection _db;

        public FaqRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Faq>> GetAllAsync()
        {
            var sql = "SELECT Id, Question, Answer FROM Faqs ORDER BY Id";
            return await _db.QueryAsync<Faq>(sql);
        }

        public async Task<Faq?> GetByIdAsync(int id)
        {
            var sql = "SELECT Id, Question, Answer FROM Faqs WHERE Id = @Id";
            return await _db.QuerySingleOrDefaultAsync<Faq>(sql, new { Id = id });
        }

        public async Task AddAsync(Faq faq)
        {
            var sql = "INSERT INTO Faqs (Question, Answer) VALUES (@Question, @Answer)";
            await _db.ExecuteAsync(sql, faq);
        }

        public async Task UpdateAsync(Faq faq)
        {
            var sql = "UPDATE Faqs SET Question = @Question, Answer = @Answer WHERE Id = @Id";
            await _db.ExecuteAsync(sql, faq);
        }

        public async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM Faqs WHERE Id = @Id";
            await _db.ExecuteAsync(sql, new { Id = id });
        }
    }
}
