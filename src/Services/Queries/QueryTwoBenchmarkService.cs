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
    public class QueryTwoBenchmarkService : IBenchmarkService
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
                var dateFrom = faker.Date.Between(DateTime.Now.AddDays(-20), DateTime.Now.AddDays(-1)).ToUniversalTime();
                var dateTo = faker.Date.Between(dateFrom, DateTime.Now).ToUniversalTime();

                var postgresResult = await queryService.QueryTwo(SupportedDb.Postgres, dateFrom, dateTo);
                var mySqlResult = await queryService.QueryTwo(SupportedDb.MySQL, dateFrom, dateTo);
                var mongoDbResult = await queryService.QueryTwo(SupportedDb.MongoDb, dateFrom, dateTo);
                var cassndraResult = await queryService.QueryTwo(SupportedDb.Cassandra, dateFrom, dateTo);

                results.Add(new Result {
                    N = i,
                    Postgres = postgresResult.Performance.ToTimeSpan(),
                    MySql = mySqlResult.Performance.ToTimeSpan(),
                    MongoDb = mongoDbResult.Performance.ToTimeSpan(),
                    Cassandra = cassndraResult.Performance.ToTimeSpan()             
                });

                // var numberOfOrders = new List<decimal>()
                // {
                //     postgresResult.NumerOfOrders,
                //     mySqlResult.NumerOfOrders,
                //     // mariaDbResult.NumerOfOrders,
                //     mongoDbResult.NumerOfOrders,
                //     cassndraResult.NumerOfOrders
                // };

                // if (!numberOfOrders.All(x => x == postgresResult.NumerOfOrders))
                // {
                //     Console.WriteLine(JsonConvert.SerializeObject(numberOfOrders));
                //     throw new Exception("Db's returned different data");
                // };
            }

            return results;
        }
    }
}