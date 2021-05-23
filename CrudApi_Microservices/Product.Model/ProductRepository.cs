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

		public async Task<IEnumerable<Entities.Product>> PostProductsAsync(IEnumerable<Entities.Product> products)
		{
            IList<Entities.Product> result = null;

          if (await Context.SaveAsync(products) != -1)
            {
                result = await GetAllAsync();
            }
            return result;
        }


        public async Task<IEnumerable<Entities.Product>> GetAllProducts()
        {
            var result = await this.GetAllChildEntitiesAsync();
            return result;
        }

    }
     
}
