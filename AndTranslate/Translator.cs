using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using AndTranslate.net.bing.api;

namespace AndTranslate
{

    class Translator
    {
        private static string APP_ID = "A416E9E5DFE936E4EAD0CDEFEE44885B7B2276AC";


        public static string Translate(string fromLang, string toLang, string query)
        {
            // BingService implements IDisposable.
            using (BingService service = new BingService())
            {
                try
                {
                    SearchRequest request = BuildRequest(fromLang, toLang, query);

                    // Send the request; display the response.
                    SearchResponse response = service.Search(request);
                  return   ParseTranslation(response);
                }
                catch (System.Web.Services.Protocols.SoapException ex)
                {
                    // A SOAP Exception was thrown. Display error details.
                    DisplayErrors(ex.Detail);
                }
                catch (System.Net.WebException ex)
                {
                    // An exception occurred while accessing the network.
                    Console.WriteLine(ex.Message);
                }
            }
            return "|ERROR - no translation";

        }

                static SearchRequest BuildRequest(string  fromLang, string toLang, string query)
        {
            SearchRequest request = new SearchRequest();

            // Common request fields (required)
            request.AppId = APP_ID;
            request.Query =query;
            request.Sources = new SourceType[] { SourceType.Translation };

            // SourceType-specific fields (required)
            request.Translation = new TranslationRequest();
            request.Translation.SourceLanguage = fromLang;
            request.Translation.TargetLanguage = toLang;

            // Common request fields (optional)
            request.Version = "2.2";

            return request;
        }


                static string ParseTranslation(SearchResponse response)
        {
            // Display the results header.
            //Console.WriteLine("Bing API Version " + response.Version);
            //Console.WriteLine("Translation results for " + response.Query.SearchTerms);
            //Console.WriteLine();

            // Display the Translation results.
            //foreach (TranslationResult result in response.Translation.Results)
            //{
            //    Console.WriteLine(result.TranslatedTerm);
            //}
            if (response.Translation != null)
            {
                return response.Translation.Results.First().TranslatedTerm;
            }
            return "|ERROR - no translation|";
        }



        static void DisplayErrors(XmlNode errorDetails)
        {
            // Add the default namespace to the namespace manager.
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(
                errorDetails.OwnerDocument.NameTable);
            nsmgr.AddNamespace(
                "api",
                "http://schemas.microsoft.com/LiveSearch/2008/03/Search");

            XmlNodeList errors = errorDetails.SelectNodes(
                "./api:Errors/api:Error",
                nsmgr);

            if (errors != null)
            {
                // Iterate over the list of errors and display error details.
                Console.WriteLine("Errors:");
                Console.WriteLine();
                foreach (XmlNode error in errors)
                {
                    foreach (XmlNode detail in error.ChildNodes)
                    {
                        Console.WriteLine(detail.Name + ": " + detail.InnerText);
                    }

                    Console.WriteLine();
                }
            }
        }
    }
}
