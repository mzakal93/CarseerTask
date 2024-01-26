using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

public class CarMakeCsvHelper : ICarMakeCsvHelper
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<CarMakeCsvHelper> _logger;

    public CarMakeCsvHelper(
        IConfiguration configuration,
        IWebHostEnvironment environment,
        ILogger<CarMakeCsvHelper> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public int? GetMakeIdByMakeName(string makeName)
    {
        try
        {
            var filePath = Path.Combine(_environment.ContentRootPath, _configuration["ApiSetting:CsvFilePath"]);
            var carMakes = ReadCsv<CarMake>(filePath);

            var result = carMakes.FirstOrDefault(m => string.Equals(m.make_name, makeName, StringComparison.OrdinalIgnoreCase));

            return result?.make_id;
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, $"Error reading CSV file: {ex.Message}");
            return null;
        }
    }

    private List<T> ReadCsv<T>(string filePath)
    {
        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            return csv.GetRecords<T>().ToList();
        }
    }
}