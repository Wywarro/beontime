using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BEonTime.Data.Models
{
    public class MongoDBOptions
    {
        public string ConnectionString;
        public string Database;
        public CollectionOptions User;
        public CollectionOptions Role;
    }

    public class CollectionOptions
    {
        public string CollectionName { get; set; }
        public bool ManageIndicies { get; set; }
    }
}
