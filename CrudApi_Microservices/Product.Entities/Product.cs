using CRUD.Transaction.CRUDApi.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;


namespace CRUD.Transaction.Product.Entities
{
    [KnownType(typeof(ModelEntityBase))]
    public partial class Product : ModelEntityBase
    {

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


        #endregion

    }
}
