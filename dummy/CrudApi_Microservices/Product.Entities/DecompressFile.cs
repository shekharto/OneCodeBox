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
    public partial class DecompressFile : ModelEntityBase
    {

        #region base properties
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string FileType { get; set; }
        [DataMember]
        public byte[] DecompressXml { get; set; }
        [DataMember] public byte[] DecompressedFileGeneration { get; set; }
        [DataMember] public string DecompressedFileGenerationXml { get; set; }
        #endregion

        #region Custom Properties
        [DataMember] public string TransformedXml { get; set; }
        #endregion

    }
}
