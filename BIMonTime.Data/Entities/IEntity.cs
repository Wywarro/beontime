using System;
using System.Collections.Generic;
using System.Text;

namespace BIMonTime.Data.Entities
{
    public interface IEntity
    {
        int Id { get; set; }
        DateTime CreatedOn { get; set; }
        DateTime UpdatedOn { get; set; }
    }
}
