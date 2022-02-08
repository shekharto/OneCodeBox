using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ApiClient
{

    public class Product
    {
        public long ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public ObjectStateType ObjectState { get; set; }
    }

    public enum ObjectStateType
    {
        [EnumMember]
        Unchanged = 0x1,
        [EnumMember]
        Added = 0x2,
        [EnumMember]
        Modified = 0x4,
        [EnumMember]
        Deleted = 0x8
    };

    public interface IObjectState
    {
        ObjectStateType ObjectState { get; set; }
    }

    class Program
    {
        static HttpClient client = new HttpClient();
        private static string APIUrl = "http://localhost:1040/Product"; // change it accordingly...

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            // Create a new product
            Product product = new Product
            {
                Name = "Gizmo",
                Description = "Gizmo new",
                Active = true,
                ObjectState = ObjectStateType.Added
            };

            client.BaseAddress = new Uri(APIUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Get and display all product list
                var tt =  GetProductAsync(APIUrl);  // -- its working

                // Add new product
               tt = CreateProductAsync(APIUrl, product);  // -- its working


                //// Update the product
                //  Console.WriteLine("Updating description...");
                UpdateProductAsync(APIUrl);  // -- its working

                ///////////////////////////////////
                // here Patch is not-working

                //// Delete the product
                //   Console.WriteLine("Delete description...");
                //   DeleteProductAsync(APIUrl);  // -- its not working

                //// Delete the product
                //var statusCode = await DeleteProductAsync(product.Id);
                //Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        static async void DeleteProductAsync(string url)
        {
            // update the existing product ID

            Product product = new Product
            {
                ObjectState = ObjectStateType.Deleted,
                 ProductId = 5
            };

            url = url + "/5";

             string jsonData = JsonConvert.SerializeObject(product);
            //Needed to setup the body of the request
             StringContent data = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                var readTask = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var rawResponse = readTask.GetAwaiter().GetResult();
                Console.WriteLine(rawResponse);
            }

        }

        static async void UpdateProductAsync(string url)
        {
            // update/put any exisitng product
            Product updateProduct = new Product()
            {
                ProductId = 5,
                Name = "Gizmo",
                Description = "new testing gizmo",
                Active = true,
                ObjectState = ObjectStateType.Modified
            };
 
            string jsonData = JsonConvert.SerializeObject(updateProduct);
            //Needed to setup the body of the request
            StringContent data = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(url, data);
            response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var rawResponse = readTask.GetAwaiter().GetResult();
                    Console.WriteLine(rawResponse);
            }                 
            
        }

        static async Task<Product> CreateProductAsync(string url, Product product)
        {
            string jsonData = JsonConvert.SerializeObject(product);
            //Needed to setup the body of the request
            StringContent data = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, data);
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                var readTask = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var rawResponse = readTask.GetAwaiter().GetResult();
                Console.WriteLine(rawResponse);
            }
            return null;
        }

        static async Task<Product> GetProductAsync(string path)
        {
            Product product = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var readTask = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var rawResponse = readTask.GetAwaiter().GetResult();
                Console.WriteLine(rawResponse);
            }
            return product;
        }
 
    }
}
