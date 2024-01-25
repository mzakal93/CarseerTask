using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

public class CarMakeCsvHelper : ICarMakeCsvHelper
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    public CarMakeCsvHelper(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    public int? GetMakeIdByMakeName(string makeName)
    {
        try
        {
            var carMakes = ReadCsv<CarMake>(Path.Combine(_environment.ContentRootPath,_configuration["ApiSetting:CsvFilePath"]));
            var result = carMakes.FirstOrDefault(m => m.make_name.ToUpper() == makeName.ToUpper());

            return result?.make_id;
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            Console.WriteLine($"Error reading CSV file: {ex.Message}");
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