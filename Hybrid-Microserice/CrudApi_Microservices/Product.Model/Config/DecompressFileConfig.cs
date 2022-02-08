using CRUD.Transaction.CRUDApi.Core.Entities;
using CRUD.Transaction.Product.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.Product.Model.Config
{
     public class DecompressFileConfig : ModelEntityBaseConfig<DecompressFile>
    {
        public override void Configure(EntityTypeBuilder<DecompressFile> builder)
        {
            builder.ToTable("DecompressFile");
            builder.HasKey(m => new { m.Id });
            builder.Ignore(m => m.DecompressedFileGenerationXml);
            builder.Ignore(m => m.DecompressedFileGeneration);
            builder.Ignore(c => c.TransformedXml);
            /// parent object reference
            //   builder.HasOne(r => r.Product).WithMany(t => t.ProductPrices).HasForeignKey(fk => fk.ProductId);
            base.Configure(builder);
        }
    }

}
