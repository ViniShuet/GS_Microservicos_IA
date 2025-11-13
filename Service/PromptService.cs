using Domain;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class PromptService : IPromptService
    {
        private readonly IPromptRepository _promptRepository;

        public PromptService(IPromptRepository promptRepository)
        {
            _promptRepository = promptRepository;
        }

        public async Task<IEnumerable<Prompt>> GetAllAsync()
        {
            var prompts = await _promptRepository.GetAllPromptsAsync();
            return prompts ?? new List<Prompt>();
        }

        public async Task<Prompt> AddAsync(Prompt prompt)
        {
            if (prompt == null)
                throw new ArgumentException("Dados do prompt são obrigatórios");

            if (string.IsNullOrWhiteSpace(prompt.Nome) || string.IsNullOrWhiteSpace(prompt.Descricao))
                throw new ArgumentException("Nome e descrição são campos obrigatórios");

            var newPrompt = await _promptRepository.AddPromptAsync(prompt);
            if (newPrompt == null)
                throw new InvalidOperationException("Erro interno ao criar prompt");

            return newPrompt;
        }

        public async Task UpdateAsync(int id, Prompt prompt)
        {
            if (id <= 0)
                throw new ArgumentException("ID do prompt deve ser maior que zero");

            if (prompt == null)
                throw new ArgumentException("Dados do prompt são obrigatórios");

            if (string.IsNullOrWhiteSpace(prompt.Nome) || string.IsNullOrWhiteSpace(prompt.Descricao))
                throw new ArgumentException("Nome e descrição são campos obrigatórios");

            prompt.Id = id;
            await _promptRepository.UpdatePromptAsync(prompt);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID do prompt deve ser maior que zero");

            await _promptRepository.DeletePromptAsync(id);
        }
    }
}
