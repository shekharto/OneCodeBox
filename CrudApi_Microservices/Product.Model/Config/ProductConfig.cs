using CRUD.Transaction.CRUDApi.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRUD.Transaction.Product.Model.Config
{
    public class ProductConfig : ModelEntityBaseConfig<Entities.Product>
    {
        public override void Configure(EntityTypeBuilder<Entities.Product> builder)
        {
            builder.ToTable("Product");
            builder.HasKey(r => r.ProductId);
            base.Configure(builder);
        }


    }
}
