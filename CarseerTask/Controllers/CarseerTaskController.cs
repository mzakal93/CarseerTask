using CarseerTask.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[ApiController]
[Route("[controller]")]
public class CarseerTaskController : ControllerBase
{
    private readonly ILogger<CarseerTaskController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ICarMakeCsvHelper _carMakeCsvHelper;

    public CarseerTaskController(
        ILogger<CarseerTaskController> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ICarMakeCsvHelper carMakeCsvHelper)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _carMakeCsvHelper = carMakeCsvHelper;
    }

    [HttpGet]
    [Route("api/models")]
    public async Task<IActionResult> GetModelsByMakeNameAndYearAsync([FromQuery] int modelYear, [FromQuery] string make)
    {
        try
        {
            var carMakeId = _carMakeCsvHelper.GetMakeIdByMakeName(make);
            if (!carMakeId.HasValue)
            {
                return BadRequest($"Car name '{make}' is not valid.");
            }
            var modelsApiBaseUrl = _configuration["ApiSetting:GetModelsForMakeIdYearAPI"];

            using (HttpClient httpClient = _httpClientFactory.CreateClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{modelsApiBaseUrl}/makeId/{carMakeId}/modelyear/{modelYear}?format=json");

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var deserializedResult = JsonConvert.DeserializeObject<RootObject>(apiResponse);

                    var res = new ModelResult { Models = deserializedResult.Results.Select(r => r.Model_Name).ToArray() };
                    return Ok(res);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, $"Error: {response.ReasonPhrase}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during the request");
            return StatusCode(500, "Internal Server Error");
        }
    }
}