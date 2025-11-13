using Domain;

namespace Repository
{
    public interface IPromptRepository
    {
        Task<Prompt> AddPromptAsync(Prompt prompt);

        Task<IEnumerable<Prompt>> GetAllPromptsAsync();

        Task DeletePromptAsync(int id);

        Task UpdatePromptAsync(Prompt prompt);
    }
}