using System.Globalization;
using CsvHelper;
using client.Models;
using client.Services.Queries;
using client.Services;
using client.Interfaces;
using System.Reflection;

void Save(List<Result> result, string path)
{
    using (var writer = new StreamWriter(path))
    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
    {
        csv.WriteRecords(result);
    }
}

var operation = args.AsQueryable().FirstOrDefault();
var outputPath = args.AsQueryable().Skip(1).FirstOrDefault() ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);;

Console.WriteLine($"Starting operation: {operation}");
Console.WriteLine($"Output path: {outputPath}");

IBenchmarkService benchmarkService = operation switch {
    "create" => new CreateOrderBenchmarkService(),
    "update" => new UpdateOrderBenchmarkService(),
    "delete" => new DeleteOrderBenchmarkService(),
    "queryOne" => new QueryOneBenchmarkService(),
    "queryTwo" => new QueryTwoBenchmarkService(),
    "queryThree" => new QueryThreeBenchmarkService(),
    "queryFour" => new QueryFourBenchmarkService(),
    "queryFive" => new QueryFiveBenchmarkService(),
    _ => throw new Exception($"Operation no supported: {operation}")
};

var benchmarkResult = await benchmarkService.TestAsync();
Save(benchmarkResult, outputPath);