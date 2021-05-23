using CRUD.Transaction.CRUDApi.Core.ApiResult;
using CRUD.Transaction.CRUDApi.Core.Controllers;
using CRUD.Transaction.CRUDApi.Core.Helper;
using CRUD.Transaction.CRUDApi.Core.Repository;
using CRUD.Transaction.Product.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUD.Transaction.Product.Api.Controllers
{
    [Route("Product")]
    [ApiController]
    public class ProductController : ApiAsyncController<Entities.Product>
    {
        ProductRepository productRepository;
        public ProductController(IRepositoryAsync<Entities.Product> repository) : base(repository)
        {
            productRepository = (ProductRepository)repository;
        }

        [HttpPost]
        [Route("PostProducts")]
        public async Task<IApiResult> PostEnumerableAsync(IEnumerable<Entities.Product> products)
        {


            return await PostResultsAsync(productRepository.PostProductsAsync, products);

            //StopWatch watch = StopWatch.StartNew();
  
            //await productRepository.SaveAsync(products);
            //watch.Stop();
            //return await GetResultAsync(GetAllProducts);
        }

        //[HttpGet]
        //[Route("GetAllProducts")]
        //public async Task<IApiResult> GetAllProducts(string name, int version)
        //{
        //    return await ApiResultGenerator<Entities.Product>.GetResultsAsync(productRepository.GetAllProducts);
        //}

        private async Task<IEnumerable<Entities.Product>> GetAllProducts()
        {
            return await productRepository.GetAllAsync();
        }
    }
}
