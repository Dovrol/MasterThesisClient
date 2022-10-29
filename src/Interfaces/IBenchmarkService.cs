using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using client.Models;

namespace client.Interfaces
{
    public interface IBenchmarkService
    {
        Task<List<Result>> TestAsync();
    }
}