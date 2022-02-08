using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core.Entities
{
    public class ModelEntityBaseConfig<T> : IEntityTypeConfiguration<T> where T : ModelEntityBase
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
         
            builder.Ignore(c => c.ObjectState);
        }
    }
}
