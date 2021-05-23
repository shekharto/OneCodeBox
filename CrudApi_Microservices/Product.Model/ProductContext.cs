using CRUD.Transaction.CRUDApi.Core.Context;
using CRUD.Transaction.CRUDApi.Core.Interface;
using CRUD.Transaction.Product.Model.Config;
using Microsoft.EntityFrameworkCore;

namespace CRUD.Transaction.Product.Model
{
    public class ProductContext: Context<Entities.Product>
    {

        public ProductContext(IConnectionConfig config) : base(config) { }
        public ProductContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductConfig());
            modelBuilder.ApplyConfiguration(new ProductPriceConfig());
            base.OnModelCreating(modelBuilder);
        }

    }
}
