using System;
using System.Collections.Generic;
using System.Text;

namespace BEonTime.Data.Attributes
{
    public class BsonCollectionAttribute : Attribute
    {
        public string CollectionName { get; }

        public BsonCollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}
