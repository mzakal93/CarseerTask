using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CarseerTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarseerTaskController : ControllerBase
    {
        private readonly ILogger<CarseerTaskController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CarseerTaskController(ILogger<CarseerTaskController> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet(Name = "GetModelsByMakeNameAndYear")]
        public async Task<IActionResult> GetModelsByMakeNameAndYear([FromQuery] int modelYear, [FromQuery] string make)
        {
            try
            { 
                // Retrieve the base URL of the Models API from configuration
                var modelsApiBaseUrl = _configuration["ApiSetting:GetModelsForMakeIdYearAPI"];

                using (HttpClient httpClient = _httpClientFactory.CreateClient())
                {
                    // Make a GET request to the Models API
                    HttpResponseMessage response = await httpClient.GetAsync($"{modelsApiBaseUrl}/makeId/{make}/modelyear/{modelYear}?format=json");

                    // Check if the request was successful (status code 200 OK)
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the response as a string
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        // return the result 
                        return Ok(apiResponse);
                    }
                    else
                    {
                        // If the request was not successful, return an appropriate error response
                        return StatusCode((int)response.StatusCode, $"Error: {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the request
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
