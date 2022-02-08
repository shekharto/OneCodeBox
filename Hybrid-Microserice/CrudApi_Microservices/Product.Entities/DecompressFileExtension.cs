using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace CRUD.Transaction.Product.Entities
{
    public static class DecompressFileExtension
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="GeneratedProfile"></param>
        /// <param name="rootPath"></param>
        public static void GenerateTransformedXml(this DecompressFile GeneratedProfile, string rootPath)
        {
            try
            {
                if (GeneratedProfile.DecompressedFileGenerationXml == null)
                    return;

                string output = String.Empty;

                // Load an Xml string into the XPathDocument.
                StringReader rdr = new StringReader(GeneratedProfile.DecompressedFileGenerationXml);
                XPathDocument myXPathDoc = new XPathDocument(rdr);

                XslCompiledTransform myXslTrans = new XslCompiledTransform();
                XsltSettings settings = new XsltSettings();
                settings.EnableScript = true;

                if (GeneratedProfile != null && GeneratedProfile.Id == -999999999)
                    myXslTrans.Load(Path.Combine(rootPath, "CDAGroup.xsl"), settings, null);
                else
                    myXslTrans.Load(Path.Combine(rootPath, "CDA.xsl"), settings, null);

                //create the output stream
                StringWriter sw = new StringWriter();
                XmlWriter xwo = XmlWriter.Create(sw);

                myXslTrans.Transform(myXPathDoc, null, xwo);
                output = sw.ToString();
                output = output.Replace("utf-16", "utf-8");
                GeneratedProfile.TransformedXml = output;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // which will read the compress data and decompress it or unzip it. and return the actual xml file string.
        public static string DecompressXml(this DecompressFile decompressFile)
        {
            string strOutputXml = "";
            if (decompressFile.DecompressXml == null)
                return strOutputXml;
            using (var inStream = new MemoryStream(decompressFile.DecompressXml))
            {
                using (var unZip = new GZipStream(inStream, CompressionMode.Decompress, true))
                {
                    const int BUFFER_SIZE = 1024;
                    byte[] buffer = new byte[BUFFER_SIZE];
                    using (MemoryStream outStream = new MemoryStream())
                    {
                        try
                        {
                            int n;
                            while ((n = unZip.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                outStream.Write(buffer, 0, n);
                            }
                            // reset to allow read from start
                            outStream.Seek(0, SeekOrigin.Begin);
                            var sr = new StreamReader(outStream, System.Text.Encoding.UTF8);
                            strOutputXml = sr.ReadToEnd();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
            decompressFile.DecompressXml = null;
            return strOutputXml;
        }

    }
}
