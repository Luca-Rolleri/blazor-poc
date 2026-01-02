using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CounterController(ICounterRepository _counterRepository) : ControllerBase
    {
        [HttpGet]
        public CounterData Get()
        {
            var count = _counterRepository.GetCount();
            return new CounterData(count);
        }

        [HttpPost]
        public void Post([FromBody] int count)
        {
            _counterRepository.SaveCount(count);
        }

        public record CounterData(int Counter);
    }
}
