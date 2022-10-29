using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using client.Externals.Models;
using Refit;

namespace client.Externals
{
    public interface IQueryApi
    {
        [Get("/queryone")]
        Task<QueryOneResult> QueryOne([Query] SupportedDb db, string country);

        [Get("/querytwo")]
        Task<QueryTwoResult> QueryTwo([Query] SupportedDb db, DateTime dateFrom, DateTime dateTo);

        [Get("/querythree")]
        Task<QueryTwoResult> QueryThree([Query] SupportedDb db, int itemCount, DeliveryMethod deliveryMethod);

        [Get("/queryfour")]
        Task<QueryTwoResult> QueryFour([Query] SupportedDb db, string firstNameLetter, Gender gender);

        [Get("/queryfive")]
        Task<QueryTwoResult> QueryFive([Query] SupportedDb db, decimal orderItemMinNetVAlue, Gender gender);

        [Get("/querysix")]
        Task<QuerySixResult> QuerySix([Query] SupportedDb db, int minNumberOfOrders);
    }

    public class QueryOneResult
    {
        public decimal OrderSum { get; set; }
        public PerformanceResult Performance { get; set; }
    }
    public class QueryTwoResult
    {
        public int NumerOfOrders { get; set; }
        public PerformanceResult Performance { get; set; }
    }

    public class QuerySixResult
    {
        public int NumberOfCustomers { get; set; }
        public PerformanceResult Performance { get; set; }
    }
}