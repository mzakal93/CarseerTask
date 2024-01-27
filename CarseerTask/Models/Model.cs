namespace CarseerTask.Models
{
    public class ModelResult
    {
        public string[] Models { get; set; }
    }

    public class RootObject
    {
        public int Count { get; set; }
        public string Message { get; set; }
        public string SearchCriteria { get; set; }
        public CarsResult[] Results { get; set; }
    }

    public class CarsResult
    {
        public int Make_ID { get; set; }
        public string Make_Name { get; set; }
        public int Model_ID { get; set; }
        public string Model_Name { get; set; }
    }
}