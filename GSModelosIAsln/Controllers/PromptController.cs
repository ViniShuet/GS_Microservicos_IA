using Domain;
using Microsoft.AspNetCore.Mvc;
using Service;
using System;
using System.Net;
using System.Threading.Tasks;

namespace GSModelosIAsln.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromptController : ControllerBase
    {
        private readonly IPromptService _promptService;

        public PromptController(IPromptService promptService)
        {
            _promptService = promptService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var prompts = await _promptService.GetAllAsync();
                return Ok(prompts);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = ex.Message, timestamp = DateTime.UtcNow });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Prompt prompt)
        {
            try
            {
                var newPrompt = await _promptService.AddAsync(prompt);
                return CreatedAtAction(nameof(Get), new { id = newPrompt.Id }, newPrompt);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message, timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = ex.Message, timestamp = DateTime.UtcNow });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Prompt prompt)
        {
            try
            {
                await _promptService.UpdateAsync(id, prompt);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message, timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = ex.Message, timestamp = DateTime.UtcNow });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _promptService.DeleteAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message, timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = ex.Message, timestamp = DateTime.UtcNow });
            }
        }
    }
}
