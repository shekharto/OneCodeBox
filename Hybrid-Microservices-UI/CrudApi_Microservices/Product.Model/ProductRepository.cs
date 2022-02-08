using CRUD.Transaction.CRUDApi.Core.Common;
using CRUD.Transaction.CRUDApi.Core.Context;
using CRUD.Transaction.CRUDApi.Core.Interface;
using CRUD.Transaction.CRUDApi.Core.Repository;
using CRUD.Transaction.Product.Entities;
using CRUD.Transaction.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.Product.Model
{
    public class ProductRepository : DbContextRepository<Entities.Product>
    {
        private readonly ProductContext _context;
        private readonly  IConnectionConfig _config;
        private readonly string _approot;

        public ProductRepository(IConnectionConfig config, IAsyncContext<Entities.Product> context, string appRoot) : base(context)
        {
            _config = config;
            _context = (ProductContext)context;
            _approot = appRoot;
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


        /// <summary>
        /// Overall this function read the compressed xml file and decompress it. If the assigned zip file size
        /// is exceed (imaxQRDAFileSizeMB) then it will create individual zip file  as per size and club it into single zip file. 
        /// so in one single zip file you will find another zip files as per size mention.
        /// There UI will the data in byte array... and generate a single zip file
        /// Else, the final zip file contain plain xml files.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public async Task<Tuple<byte[], string>> ExportDecompressFileAsync(long Id, string fileType)
        {
            List<string> lstPatientDims = new List<string>();
            List<byte[]> arrays = new List<byte[]>();
            string strContent = string.Empty;
            string strFilename = string.Empty;
            int imaxQRDAFileSizeMB = 1; // default size
             double MaximumZipFileSize = imaxQRDAFileSizeMB * 1024 * 1024;
          //  double MaximumZipFileSize = imaxQRDAFileSizeMB * 1024;
            AppSettings app = new AppSettings(_config.ConnectionString);
                  string strQRDAFileSizeMB = app.GetSetting(AppConfigSettingKey.QRDAFileSize);

            if (!string.IsNullOrEmpty(strQRDAFileSizeMB))
                imaxQRDAFileSizeMB = Convert.ToInt32(strQRDAFileSizeMB);

            // this will read the column (DecompressXml) which is compressed/encrypted form.
            List<DecompressFile> decompressData = (from item in _context.Set<DecompressFile>().Where(p => p.FileType == fileType)
                                                   select item).ToList();

            byte[] data = new byte[0];
            if (decompressData != null)
            { 
                int index = 0;
                #region while loop to collect the xml
                while (index < decompressData.Count)
                {
                    if (index == 0)
                    {
                        // read the decrrypted xml and load it
                        strContent = decompressData[0].DecompressXml();  // decrypt and load the xml
                        if (fileType.ToUpper().Equals("HTML".ToString().ToUpper())) // this is for HTML
                        {
                            decompressData[0].DecompressedFileGenerationXml = strContent;
                            decompressData[0].GenerateTransformedXml(_approot);
                            strContent = decompressData[0].TransformedXml;
                        }
                    }                     

                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
                        {
                            do
                            {
                                DecompressFile detail = decompressData[index];
                                {
                                    if (fileType.ToUpper().Equals("HTML".ToString().ToUpper()))  // this is for HTML
                                    {
                                        lstPatientDims.Add(detail.Id.ToString());
                                        strFilename = (detail.Id > 0 ? String.Format("{0}.html", detail.Id) : String.Format("Patient{0}.html", index));
                                        ZipArchiveEntry entry = archive.CreateEntry(strFilename);
                                        using (Stream entryStream = entry.Open())
                                        using (StreamWriter writer = new StreamWriter(entryStream))
                                            writer.Write(strContent);

                                        index++;

                                        if (index < decompressData.Count)
                                        {
                                            decompressData[index].DecompressedFileGenerationXml = decompressData[index].DecompressXml();
                                            decompressData[index].GenerateTransformedXml(_approot);
                                            strContent = decompressData[index].TransformedXml;
                                        }
                                        else
                                            strContent = string.Empty;
                                    }
                                    else
                                    {
                                        lstPatientDims.Add(detail.Id.ToString());
                                        // update xml with xml filename and zip it again
                                        strFilename = (detail.Id > 0 ? String.Format("{0}.xml", detail.Id) : String.Format("Patient{0}.xml", index));
                                        ZipArchiveEntry entry = archive.CreateEntry(strFilename);
                                        using (Stream entryStream = entry.Open())
                                        using (StreamWriter writer = new StreamWriter(entryStream))
                                            writer.Write(strContent);

                                        index++;
                                        // Can only call this once, second time returns null
                                        strContent = (index < decompressData.Count ? decompressData[index].DecompressXml() : String.Empty);
                                    }
                                }

                                long val1 = stream.Length + strContent.Length;
                            } while ((stream.Length + strContent.Length < MaximumZipFileSize) && (index < decompressData.Count));
                        }
                        stream.Position = 0;
                        arrays.Add(stream.ToArray());
                    }
                }
                #endregion

                if (arrays.Count == 1)
                    data = arrays.First();   // compress the other zip file if size exceeds...
                else
                {
                    using (MemoryStream result = new MemoryStream())
                    {
                        using (ZipArchive combined = new ZipArchive(result, ZipArchiveMode.Create, true))
                        {
                            int arrayNo = 1;
                            foreach (byte[] array in arrays)
                            {
                                ZipArchiveEntry entry = combined.CreateEntry(String.Format("Segment{0}.zip", arrayNo++));
                                using (Stream entryStream = entry.Open())
                                {
                                    entryStream.Write(array, 0, array.Length);
                                }
                            }
                        }
                        result.Position = 0;
                        data = result.ToArray();
                    }
                }
            } 
                return new Tuple<byte[], string>(data, String.Join(",", lstPatientDims.ToArray()));
             
        }


        public async Task<IEnumerable<Entities.Product>> GetAllProducts()
        {
            var result = await this.GetAllChildEntitiesAsync();
            return result;
        }

    }
     
}
