using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace client.Models
{
    public class Result
    {
        public int N { get; set; }
        public TimeSpan Postgres { get; set; }
        public TimeSpan MySql{ get; set; }
        // public TimeSpan MariaDb { get; set; }
        public TimeSpan MongoDb { get; set; }
        public TimeSpan Cassandra { get; set; }
    }
}