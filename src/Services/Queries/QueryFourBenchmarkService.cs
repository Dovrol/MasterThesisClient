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
    public class QueryFourBenchmarkService : IBenchmarkService
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
                var firstNameLetter = faker.Random.Char('A', 'Z').ToString();
                var gender = faker.PickRandom<Gender>();

                var postgresResult = await queryService.QueryFour(SupportedDb.Postgres, firstNameLetter, gender);
                var mySqlResult = await queryService.QueryFour(SupportedDb.MySQL, firstNameLetter, gender);
                var mongoDbResult = await queryService.QueryFour(SupportedDb.MongoDb, firstNameLetter, gender);
                var cassndraResult = await queryService.QueryFour(SupportedDb.Cassandra, firstNameLetter, gender);

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