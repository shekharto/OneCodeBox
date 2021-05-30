using CRUD.Transaction.CRUDApi.Core.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;


namespace CRUD.Transaction.Product.Entities
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(ModelEntityBase))]
    [KnownType(typeof(List<ProductPrice>))]
    public partial class Product : ModelEntityBase
    {
        public Product()
        {
            this.ProductPrices = new List<ProductPrice>();
    
        }

        #region base properties
        [DataMember]
        public long ProductId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public bool Active { get; set; }
        #endregion

        #region custom or navigation properties
         
        [DataMember]
        [ChildNavigationEntity]
        public List<ProductPrice> ProductPrices { get; set; }


        #endregion

    }
}
