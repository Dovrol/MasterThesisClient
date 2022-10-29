using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using client.Externals;
using client.Externals.Models;
using client.Interfaces;
using client.Models;
using CountryData.Bogus;
using Newtonsoft.Json;
using Refit;

namespace client.Services.Queries
{
    public class QuerySixBenchmarkService : IBenchmarkService
    {
        public async Task<List<Result>> TestAsync()
        {
            var queryService = RestService.For<IQueryApi>(new HttpClient 
            {
                BaseAddress = new Uri("http://localhost:5192/api/Query"),
                Timeout = TimeSpan.FromHours(24)
            });
            
            var faker = new Faker();
            var results = new List<Result>();
            

            for (int i = 0; i < 10; i++)
            {
                var minNumberOfOrders = faker.Random.Int(1, 10);

                var postgresResult = await queryService.QuerySix(SupportedDb.Postgres, minNumberOfOrders);
                var mySqlResult = await queryService.QuerySix(SupportedDb.MySQL, minNumberOfOrders);
                var mongoDbResult = await queryService.QuerySix(SupportedDb.MongoDb, minNumberOfOrders);
                // _cassandraResults.Add(cassndraResult.Performance.ToTimeSpan());

                results.Add(new Result {
                    N = i,
                    Postgres = postgresResult.Performance.ToTimeSpan(),
                    MySql = mySqlResult.Performance.ToTimeSpan(),
                    MongoDb = mongoDbResult.Performance.ToTimeSpan(),
                    // Cassandra = cassndraResult.Performance.ToTimeSpan()             
                });

                var numberOfCustomers = new List<decimal>()
                {
                    postgresResult.NumberOfCustomers,
                    mySqlResult.NumberOfCustomers,
                    mongoDbResult.NumberOfCustomers,
                    // cassndraResult.NumberOfCustomers
                };

                if (!numberOfCustomers.All(x => x == postgresResult.NumberOfCustomers))
                {
                    Console.WriteLine(JsonConvert.SerializeObject(numberOfCustomers));
                    throw new Exception("Db's returned different data");
                };
            }

            return results;
        }
    }
}