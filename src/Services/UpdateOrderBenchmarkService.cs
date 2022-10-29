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
    public class UpdateOrderBenchmarkService : IBenchmarkService
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


            for (int i = 0; i < 10; i++)
            {
                var itemCount = faker.Random.Int(1, 10);
                var deliveryMethod = faker.PickRandom<DeliveryMethod>();
                var fulfillmentDate = faker.Date.Between(DateTime.Now.AddDays(-20), DateTime.Now);

                Console.WriteLine($"Item count: {itemCount}");
                Console.WriteLine($"Delivery method: {deliveryMethod}");
                Console.WriteLine($"FulfillmentDate: {fulfillmentDate}");

                var postgresResult = await queryService.Update(SupportedDb.Postgres, itemCount, deliveryMethod, fulfillmentDate);
                var mySqlResult = await queryService.Update(SupportedDb.MySQL, itemCount, deliveryMethod, fulfillmentDate);
                var mongoDbResult = await queryService.Update(SupportedDb.MongoDb, itemCount, deliveryMethod, fulfillmentDate);
                var cassandraResult = await queryService.Update(SupportedDb.Cassandra, itemCount, deliveryMethod, fulfillmentDate);

                results.Add(new Result {
                    N = i,
                    Postgres = postgresResult.Performance.ToTimeSpan(),
                    MySql = mySqlResult.Performance.ToTimeSpan(),
                    MongoDb = mongoDbResult.Performance.ToTimeSpan(),
                    Cassandra = cassandraResult.Performance.ToTimeSpan()             
                });


                var numberOfOrders = new List<decimal>()
                {
                    postgresResult.NumerOfOrders,
                    mySqlResult.NumerOfOrders,
                    mongoDbResult.NumerOfOrders,
                    cassandraResult.NumerOfOrders
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