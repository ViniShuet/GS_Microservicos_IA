using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public interface IPromptService
    {
        Task<IEnumerable<Prompt>> GetAllAsync();
        Task<Prompt> AddAsync(Prompt prompt);
        Task UpdateAsync(int id, Prompt prompt);
        Task DeleteAsync(int id);
    }
}
