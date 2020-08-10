using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPlatformSolutionImport_ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string AppId = "your-app-id";
                string ClientSecret = "your-app-secret";
                string orgUrl = "your-environment-url";

                string connectionString = $"AuthType = ClientSecret;url = {orgUrl};ClientId = {AppId};ClientSecret = {ClientSecret}"; //form the connection string. Notice that AuthType is ClientSecret for client credentials
                Console.WriteLine($"Connecting to environment {orgUrl}...");
                CrmServiceClient serviceClient = new CrmServiceClient(connectionString);
                if (serviceClient.IsReady)
                {
                    Console.WriteLine($"Connection is now ready. Starting import of solution into environment {orgUrl}");

                    byte[] fileBytes = File.ReadAllBytes(solutionFilePath); //path to your solution zip file

                    ImportSolutionRequest impSolReq = new ImportSolutionRequest()
                    {
                        OverwriteUnmanagedCustomizations = true,
                        CustomizationFile = fileBytes
                    };

                    serviceClient.Execute(impSolReq);
                    Console.WriteLine("Solution imported successfully");

                }
                else
                {
                    Console.WriteLine($"Connection failed");
                    throw serviceClient.LastCrmException;

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Something went wrong: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            Console.ReadKey();
           
        }
    }
}
