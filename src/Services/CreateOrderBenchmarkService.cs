using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using client.Externals;
using client.Externals.Models;
using client.Interfaces;
using client.Models;
using Refit;

namespace client.Services
{
    public class CreateOrderBenchmarkService : IBenchmarkService
    {
        private const int SAMPLE_SIZE = 1000;

        public async Task<List<Result>> TestAsync()
        {
            var orderService = RestService.For<IOrderApi>(new HttpClient 
            {
                BaseAddress = new Uri("http://localhost:5192/api/Order"),
                Timeout = TimeSpan.FromHours(24)
            });
            
            var faker = new Faker();
            var results = new List<Result>();


            for (int i = 0; i < 10; i++)
            {
                var seed = faker.Random.Int();

                var postgresResult = await orderService.Create(SupportedDb.Postgres, SAMPLE_SIZE, seed);
                var mySqlResult = await orderService.Create(SupportedDb.MySQL, SAMPLE_SIZE, seed);
                var mongoDbResult = await orderService.Create(SupportedDb.MongoDb, SAMPLE_SIZE, seed);
                var cassndraResult = await orderService.Create(SupportedDb.Cassandra, SAMPLE_SIZE, seed);

                results.Add(new Result {
                    N = i,
                    Postgres = postgresResult.ToTimeSpan(),
                    MySql = mySqlResult.ToTimeSpan(),
                    MongoDb = mongoDbResult.ToTimeSpan(),
                    Cassandra = cassndraResult.ToTimeSpan()             
                });
            }

            return results;
        }
    }
}