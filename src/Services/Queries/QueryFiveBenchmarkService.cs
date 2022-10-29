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
    public class QueryFiveBenchmarkService : IBenchmarkService
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
                var orderItemMinNetValue = faker.Finance.Amount(1, 500, 2);
                var gender = faker.PickRandom<Gender>();

                var postgresResult = await queryService.QueryFive(SupportedDb.Postgres, orderItemMinNetValue, gender);
                var mySqlResult = await queryService.QueryFive(SupportedDb.MySQL, orderItemMinNetValue, gender);
                var mongoDbResult = await queryService.QueryFive(SupportedDb.MongoDb, orderItemMinNetValue, gender);
                var cassndraResult = await queryService.QueryFive(SupportedDb.Cassandra, orderItemMinNetValue, gender);

                results.Add(new Result {
                    N = i,
                    Postgres = postgresResult.Performance.ToTimeSpan(),
                    MySql = mySqlResult.Performance.ToTimeSpan(),
                    MongoDb = mongoDbResult.Performance.ToTimeSpan(),
                    Cassandra = cassndraResult.Performance.ToTimeSpan()             
                });

                var numberOfOrders = new List<decimal>()
                {
                    postgresResult.NumerOfOrders,
                    mySqlResult.NumerOfOrders,
                    // mariaDbResult.NumerOfOrders,
                    mongoDbResult.NumerOfOrders,
                    cassndraResult.NumerOfOrders
                };

                if (!numberOfOrders.All(x => x == postgresResult.NumerOfOrders))
                {
                    Console.WriteLine(JsonConvert.SerializeObject(numberOfOrders));
                    throw new Exception("Db's returned different data");
                };
            }

            return results;
        }
    }
}