using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using client.Externals.Models;
using Refit;

namespace client.Externals
{
    public interface IOrderApi
    {
        [Post("/create")]
        Task<PerformanceResult> Create([Query] SupportedDb db, int n, int seed);

        [Post("/update")]
        Task<UpdateResult> Update([Query] SupportedDb db, int itemCount, DeliveryMethod deliveryMethod, DateTime FulfillmentDate);

        [Post("/delete")]
        Task<DeleteResult> Delete([Query] SupportedDb db, int customerId);
    }


    public class UpdateResult
    {
        public int NumerOfOrders { get; set; }
        public PerformanceResult Performance { get; set; }
    }

    public class DeleteResult
    {
        public int DeletedNumerOfOrders { get; set; }
        public PerformanceResult Performance { get; set; }
    }
}