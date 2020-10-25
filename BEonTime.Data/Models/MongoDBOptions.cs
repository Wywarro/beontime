using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BEonTime.Data.Models
{
    public class MongoDBOptions
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public CollectionOptions User { get; set; } = new CollectionOptions();
        public CollectionOptions Role { get; set; } = new CollectionOptions();
    }

    public class CollectionOptions
    {
        public string CollectionName { get; set; }
        public bool ManageIndicies { get; set; }
    }
}
