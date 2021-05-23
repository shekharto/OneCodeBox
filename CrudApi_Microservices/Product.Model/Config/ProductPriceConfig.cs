using CRUD.Transaction.CRUDApi.Core.Entities;
using CRUD.Transaction.Product.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
 using Microsoft.EntityFrameworkCore;
 

namespace CRUD.Transaction.Product.Model.Config
{
    public class ProductPriceConfig : ModelEntityBaseConfig<ProductPrice>
    {
        public override void Configure(EntityTypeBuilder<ProductPrice> builder)
        {
            builder.ToTable("ProductPrice");
            builder.HasKey(m => new { m.Id });
            /// parent object reference
            builder.HasOne(r => r.Product).WithMany(t => t.ProductPrices).HasForeignKey(fk => fk.ProductId);
            base.Configure(builder);
        }
    }
}
