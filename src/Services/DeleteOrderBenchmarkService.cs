using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using client.Externals;
using client.Externals.Models;
using client.Interfaces;
using client.Models;
using Newtonsoft.Json;
using Refit;

namespace client.Services
{
    public class DeleteOrderBenchmarkService : IBenchmarkService
    {
        public async Task<List<Result>> TestAsync()
        {
            var queryService = RestService.For<IOrderApi>(new HttpClient 
            {
                BaseAddress = new Uri("http://localhost:5192/api/Order"),
                Timeout = TimeSpan.FromHours(24)
            });

            var faker = new Faker();
            var results = new List<Result>();


            var r = new Random();
            var customerIds = Enumerable.Range(1, 1000).OrderBy(x => r.Next()).Take(10);
            int i = 1;
            foreach (var id in customerIds)
            {
                var cassndraResult = await queryService.Delete(SupportedDb.Cassandra, id);
                var postgresResult = await queryService.Delete(SupportedDb.Postgres, id);
                var mySqlResult = await queryService.Delete(SupportedDb.MySQL, id);
                var mongoDbResult = await queryService.Delete(SupportedDb.MongoDb, id);

                results.Add(new Result {
                    N = i++,
                    Postgres = postgresResult.Performance.ToTimeSpan(),
                    MySql = mySqlResult.Performance.ToTimeSpan(),
                    MongoDb = mongoDbResult.Performance.ToTimeSpan(),
                    Cassandra = cassndraResult.Performance.ToTimeSpan()             
                });

                var numberOfOrders = new List<decimal>()
                {
                    postgresResult.DeletedNumerOfOrders,
                    mySqlResult.DeletedNumerOfOrders,
                    mongoDbResult.DeletedNumerOfOrders,
                    cassndraResult.DeletedNumerOfOrders
                };

                if (!numberOfOrders.All(x => x == postgresResult.DeletedNumerOfOrders))
                {
                    Console.WriteLine(JsonConvert.SerializeObject(numberOfOrders));
                    throw new Exception("Db's returned different data");
                };

                Console.WriteLine($"Deleted: {postgresResult.DeletedNumerOfOrders} of customer {id}");
            }
            
            return results;
        }
    }
}