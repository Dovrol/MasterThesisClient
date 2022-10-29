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
    public class QueryOneBenchmarkService : IBenchmarkService
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
                var country = faker.Country().Name();

                var postgresResult = await queryService.QueryOne(SupportedDb.Postgres, country);
                var mySqlResult = await queryService.QueryOne(SupportedDb.MySQL, country);
                var mongoDbResult = await queryService.QueryOne(SupportedDb.MongoDb, country);
                var cassndraResult = await queryService.QueryOne(SupportedDb.Cassandra, country);

                results.Add(new Result {
                    N = i,
                    Postgres = postgresResult.Performance.ToTimeSpan(),
                    MySql = mySqlResult.Performance.ToTimeSpan(),
                    MongoDb = mongoDbResult.Performance.ToTimeSpan(),
                    Cassandra = cassndraResult.Performance.ToTimeSpan()             
                });

                var orderSum = new List<decimal>()
                {
                    postgresResult.OrderSum,
                    mySqlResult.OrderSum,
                    // mariaDbResult.OrderSum,
                    mongoDbResult.OrderSum,
                    cassndraResult.OrderSum
                };

                if (!orderSum.All(x => x == postgresResult.OrderSum))
                {
                    Console.WriteLine(JsonConvert.SerializeObject(orderSum));
                    throw new Exception("Db's returned different data");
                };
            }

            return results;
        }
    }
}