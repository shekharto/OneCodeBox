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

        /// <summary>
        /// TODO: comment
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
      [HttpGet]
        [Route("Export/Id/{Id:long}/FileType/{fileType}")]
        public async Task<IApiResult> GetDecompressFile(long Id, string fileType)
        {
            try
            {
                Tuple<byte[], string> result = null;
                StopWatch watch = StopWatch.StartNew();
                result = await productRepository.ExportDecompressFileAsync(Id, fileType);
                if (result.Item2 != null)
                {
                    //await AuditEventAsync($"QRDA1 - PHI Exported. The following PHI was exported - {QRDA1_MESSAGE}{Environment.NewLine}Patient ID:{result.Item2}", 
                    //    typeof(GenerationRepository).GetMethod("ExportCategoryOneDetailsAsync").ToString());
                }

                watch.Stop();

                return ApiResult<byte>.Ok(result.Item1, watch.Elapsed);
            }
            catch (Exception ex)
            {
                return  await HandleError1Async(ex, "Export");
            }
        }

    }
}
