using CRUD.Transaction.CRUDApi.Core.Context;
using CRUD.Transaction.CRUDApi.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.Product.Model
{
    public class ProductRepository : DbContextRepository<Entities.Product>
    {
        private readonly ProductContext _context;

        public ProductRepository(IAsyncContext<Entities.Product> context) : base(context)
        {
            _context = (ProductContext)context;
        }
    }
     
}
