using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using System;
using System.Collections.Generic;

namespace copyFromBlobToCosmosDB
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // Set variables
            string tenantID = "ab8614a3-38fa-4410-81cc-2cff09afdee1";
            string subscriptionId = "5392a7fd-63df-43e1-94c6-b7bf0a669805";
            string applicationId = "e5ab87d7-2f02-4dfc-a076-b1ef78a29ee7";
            string authenticationKey = "13R9bAF3M+thkO3wnQOd6R1sidWlnZFQoGR8TmbTv5A=";
            string resourceGroup = "shpdatafactory-rg";
            string region = "East US";
            string dataFactoryName = "BlobToCosDBDataFactory"; //must be globally unique 

            // Specify the source Azure Blob information  
            string storageAccount = "datasfactorytorage";
            string storageKey = "zJZ6LxEAQepM73iX/S5Xc1XTBas5p8XfbpP703Js36ceNgjl+eKkkViy6zKW44CBZlcD9E7ayV9T7Pywaz6p/A==";
            string inputBlobPath = "datafactorycontainer/";
            string inputBlobName = "employee.csv";

        // Specify the Azure Cosmos DB information 

            string azureCosmosDBConnString = "AccountEndpoint=https://datafactorydosmosdb.documents.azure.com:443/;AccountKey=9I4km9joJAlOwtD3yIfCfFTAJxhbUAYypwlPeseU2U7Ano9UEXyPxHoib85pfQSgoLJ7QUxrvOX9yjQXxc3hBA==;Database=cosmosdbemployee"; 
            string azureCosmosDBCollection = "employee";

            //Specify the Linked Service Names and Dataset Names
            string blobStorageLinkedServiceName = "AzureBlobStorageLinkedService";
            string cosmosDbLinkedServiceName = "AzureCosmosDbLinkedService";
            string blobDatasetName = "BlobDataset";
            string cosmosDbDatasetName = "CosmosDbDataset";
            string pipelineName = "SHPADFBlobToCosmosDbCopy";

            // Authenticate and create a data factory management client  
            var context = new AuthenticationContext("https://login.windows.net/" + tenantID);
            ClientCredential cc = new ClientCredential(applicationId, authenticationKey);
            AuthenticationResult result = context.AcquireTokenAsync("https://management.azure.com/", cc).Result;
            ServiceClientCredentials cred = new TokenCredentials(result.AccessToken);
            var client = new DataFactoryManagementClient(cred) { SubscriptionId = subscriptionId };

            // Create data factory 
            Console.WriteLine("Creating data factory " + dataFactoryName + "...");
            Factory dataFactory = new Factory
            {
                Location = region,
                Identity = new FactoryIdentity()
            };

            
            client.Factories.CreateOrUpdate(resourceGroup, dataFactoryName, dataFactory);
            Console.WriteLine(SafeJsonConvert.SerializeObject(dataFactory, client.SerializationSettings));
            while (client.Factories.Get(resourceGroup, dataFactoryName).ProvisioningState == "PendingCreation")
            {
                System.Threading.Thread.Sleep(1000);
            }

            // Create an Azure Blob Storage linked service
            Console.WriteLine("Creating linked service " + blobStorageLinkedServiceName + "...");
            LinkedServiceResource storageLinkedService = new LinkedServiceResource(
                new AzureStorageLinkedService
                {
                    ConnectionString = new SecureString("DefaultEndpointsProtocol=https;AccountName=" + storageAccount + ";AccountKey=" + storageKey)
                });
            client.LinkedServices.CreateOrUpdate(resourceGroup, dataFactoryName, blobStorageLinkedServiceName, storageLinkedService);
            Console.WriteLine(SafeJsonConvert.SerializeObject(storageLinkedService, client.SerializationSettings));

            // Create an Azure Cosmos DB linked service  
            Console.WriteLine("Creating linked service " + cosmosDbLinkedServiceName + "...");
            LinkedServiceResource cosmosDbLinkedService = new LinkedServiceResource(
                new CosmosDbLinkedService
                {
                    ConnectionString = new SecureString(azureCosmosDBConnString)
                });
            client.LinkedServices.CreateOrUpdate(resourceGroup, dataFactoryName, cosmosDbLinkedServiceName, cosmosDbLinkedService);
            Console.WriteLine(SafeJsonConvert.SerializeObject(cosmosDbLinkedService, client.SerializationSettings));

            // create an Azure Blob dtaset
            Console.WriteLine("Creating dataset " + blobDatasetName + "...");
            DatasetResource blobDataset = new DatasetResource(
                new AzureBlobDataset
                {
                    LinkedServiceName = new LinkedServiceReference
                    {
                        ReferenceName = blobStorageLinkedServiceName
                    },
                    FolderPath = inputBlobPath,
                    FileName = inputBlobName,
                    Format = new TextFormat { ColumnDelimiter = ",", TreatEmptyAsNull = true, FirstRowAsHeader = true },
                    Structure = new List<DatasetDataElement>
                    {
                        new DatasetDataElement
                        {
                            Name = "name",
                            Type = "String"
                        },
                          new DatasetDataElement
                        {
                            Name = "age",
                            Type = "Int32"
                        },
                            new DatasetDataElement
                        {
                            Name = "department",
                            Type = "String"
                        }
                    }
                });
            client.Datasets.CreateOrUpdate(resourceGroup, dataFactoryName, blobDatasetName, blobDataset);
            Console.WriteLine(SafeJsonConvert.SerializeObject(blobDataset, client.SerializationSettings));

            // Create a Cosmos DB Database dataset  
            Console.WriteLine("Creating dataset " + cosmosDbDatasetName + "...");
            DatasetResource cosmosDbDataset = new DatasetResource(
                new DocumentDbCollectionDataset
                {
                    LinkedServiceName = new LinkedServiceReference
                    {
                        ReferenceName = cosmosDbLinkedServiceName
                    },
                    CollectionName = azureCosmosDBCollection
                });
            client.Datasets.CreateOrUpdate(resourceGroup, dataFactoryName, cosmosDbDatasetName, cosmosDbDataset);
            Console.WriteLine(SafeJsonConvert.SerializeObject(cosmosDbDataset, client.SerializationSettings));

            // Create a Pipeline with Copy Activity  
            Console.WriteLine("Creating pipeline " + pipelineName + "...");
            PipelineResource pipeline = new PipelineResource
            {
                Activities = new List<Activity>
                {
                    new CopyActivity
                    {
                        Name = "CopyFromBlobToCosmosDB",
                        Inputs = new List<DatasetReference>
                        {
                            new DatasetReference()
                            {
                                ReferenceName = blobDatasetName
                            }
                        },
                        Outputs = new List<DatasetReference>
                        {
                            new DatasetReference
                            {
                                ReferenceName = cosmosDbDatasetName
                            }
                        },
                        Source = new BlobSource {},
                        Sink = new DocumentDbCollectionSink {}

                    }
                }
            };
            client.Pipelines.CreateOrUpdate(resourceGroup, dataFactoryName, pipelineName, pipeline);
            Console.WriteLine(SafeJsonConvert.SerializeObject(pipeline, client.SerializationSettings));

            // Create a Pipeline Run  
            Console.WriteLine("Creating Pipeline run...");
            CreateRunResponse runResponse = client.Pipelines.CreateRunWithHttpMessagesAsync(resourceGroup, dataFactoryName, pipelineName).Result.Body;
            Console.WriteLine("Pipeline run ID: " + runResponse.RunId);

            // Monitor the Pipeline Run  
            Console.WriteLine("Checking Pipeline Run Status...");
            PipelineRun pipelineRun;
            while (true)
            {
                pipelineRun = client.PipelineRuns.Get(resourceGroup, dataFactoryName, runResponse.RunId);
                Console.WriteLine("Status: " + pipelineRun.Status);
                if (pipelineRun.Status == "InProgress")
                    System.Threading.Thread.Sleep(15000);
                else
                    break;
            }

            // Check the Copy Activity Run Details  
            Console.WriteLine("Checking copy activity run details...");
            if (pipelineRun.Status == "Succeeded")
            {
                Console.WriteLine("Copy Activity Succeeded!");
            }
            else
            {
                Console.WriteLine("Copy Activity Failed!");
            }
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();


        }
    }
}
