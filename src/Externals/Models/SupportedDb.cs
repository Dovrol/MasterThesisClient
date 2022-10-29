using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace client.Externals.Models
{
    public enum SupportedDb
    {
        Postgres,
        MySQL,
        // MariaDb,
        MongoDb,
        Cassandra
        // CouchDb
    }
}