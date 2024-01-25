using CarseerTask.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarseerTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarseerTaskController : ControllerBase
    {
        private readonly ILogger<CarseerTaskController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly CarMakeCsvHelper _carMakeCsvHelper;

        public CarseerTaskController(ILogger<CarseerTaskController> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory, IWebHostEnvironment environment)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _environment = environment;
            _carMakeCsvHelper = new CarMakeCsvHelper(configuration, environment);
        }

        [HttpGet()]
        [Route("api/models")]
        public async Task<IActionResult> GetModelsByMakeNameAndYear([FromQuery] int modelYear, [FromQuery] string make)
        {
            try
            {
                // get carMakeId from Car Name
                var carMakeId = _carMakeCsvHelper.GetMakeIdByMakeName(make);

                // Retrieve the base URL of the Models API from configuration
                var modelsApiBaseUrl = _configuration["ApiSetting:GetModelsForMakeIdYearAPI"];

                using (HttpClient httpClient = _httpClientFactory.CreateClient())
                {
                    // Make a GET request to the Models API
                    HttpResponseMessage response = await httpClient.GetAsync($"{modelsApiBaseUrl}/makeId/{carMakeId}/modelyear/{modelYear}?format=json");

                    // Check if the request was successful (status code 200 OK)
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content of the response as a string
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        // Deserialize the API response into the RootObject
                        RootObject DeserializedResult = JsonConvert.DeserializeObject<RootObject>(apiResponse);
                        // Select The Model Name of Cars
                        var res = new ModelResult { Models = DeserializedResult.Results.Select(r => r.Model_Name).ToArray() };

                        // return the result
                        return Ok(res);
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