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
    public class QueryThreeBenchmarkService : IBenchmarkService
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
                var itemCount = faker.Random.Int(1, 10);
                var deliveryMethod = faker.PickRandom<DeliveryMethod>();

                var postgresResult = await queryService.QueryThree(SupportedDb.Postgres, itemCount, deliveryMethod);
                var mySqlResult = await queryService.QueryThree(SupportedDb.MySQL, itemCount, deliveryMethod);
                var mongoDbResult = await queryService.QueryThree(SupportedDb.MongoDb, itemCount, deliveryMethod);
                var cassndraResult = await queryService.QueryThree(SupportedDb.Cassandra, itemCount, deliveryMethod);

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