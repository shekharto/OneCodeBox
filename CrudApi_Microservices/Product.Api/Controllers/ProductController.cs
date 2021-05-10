using CRUD.Transaction.CRUDApi.Core.Controllers;
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

    }
}
