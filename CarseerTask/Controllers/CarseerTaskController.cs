using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CarseerTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarseerTaskController : ControllerBase
    {
        private readonly ILogger<CarseerTaskController> _logger;

        public CarseerTaskController(ILogger<CarseerTaskController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetModelsByMakeNameAndYear")]
        public ModelResult GetModelsByMakeNameAndYear(int modelyear, string make)
        {
            return new ModelResult { Models = ["Town Car", "Continental", "Mark"] };             
        }
    }
}
