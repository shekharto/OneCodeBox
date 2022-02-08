using CRUD.Transaction.CRUDApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.Product.Entities
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(ModelEntityBase))]
    public partial class ProductPrice : ModelEntityBase
    {

        #region base properties
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public long ProductId { get; set; }
        [DataMember]
        public long Price { get; set; }
        #endregion

        #region custom or navigation properties
         

        public Product Product { get; set; }



        #endregion

    }

}
