using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;

namespace OAuthDemo.Controllers
{
    public class AnaController : Controller
    {

        static string ApplicationName = "Google Calendar API .NET Quickstart";

        // GET: Analysis 可使用三種權限取得analysis資訊
        public ActionResult Index()
        {

            //自己service 版本
            //將自己的驗證json檔放到app_data
            string[] scopes = new string[] { AnalyticsService.Scope.AnalyticsReadonly }; // view and manage your Google Analytics data

            var keyFilePath = HttpContext.Server.MapPath("~/App_Data/6c7504a5781f.p12");    // Downloaded from https://console.developers.google.com
            var serviceAccountEmail = "385316856762-bn5n205nd5cn1tmalnsjn1gs0nsm756t@developer.gserviceaccount.com";  // found https://console.developers.google.com

            //loading the Key file
            var certificate = new X509Certificate2(keyFilePath, "notasecret", X509KeyStorageFlags.Exportable);
            var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(serviceAccountEmail)
            {
                Scopes = scopes
            }.FromCertificate(certificate));

            //需要授權的版本
            //string[] scopes = new string[] {
            //    AnalyticsService.Scope.Analytics,               // view and manage your Google Analytics data
            //    AnalyticsService.Scope.AnalyticsEdit,           // Edit and manage Google Analytics Account
            //    AnalyticsService.Scope.AnalyticsManageUsers,    // Edit and manage Google Analytics Users
            //    AnalyticsService.Scope.AnalyticsReadonly};      // View Google Analytics Data

            //var clientId = "385316856762-m2rkgg70628r406pqbv9qvkb9s345c2f.apps.googleusercontent.com";      // From https://console.developers.google.com
            //var clientSecret = "mhZFbaucc40BdXushfG6KIC9";          // From https://console.developers.google.com
            //                                   // here is where we Request the user to give us access, or use the Refresh Token that was previously stored in %AppData%
            //var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
            //{
            //    ClientId = clientId,
            //    ClientSecret = clientSecret
            //},
            //scopes,
            //Environment.UserName,
            //CancellationToken.None,
            //new FileDataStore("Daimto.GoogleAnalytics.Auth.Store")).Result;

            var service = new AnalyticsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Analytics API Sample",
            });


            //AnalyticsService service = new AnalyticsService(new BaseClientService.Initializer()
            //{
            //    ApiKey = "AIzaSyD8BPsvUGtt_OMt_DTAvEkcnyYyAIrVQPQ",  // from https://console.developers.google.com (Public API access)
            //    ApplicationName = "Analytics API Sample",
            //});

            ManagementResource.AccountSummariesResource.ListRequest list = service.Management.AccountSummaries.List();
            list.MaxResults = 1000;  // Maximum number of Account Summaries to return per request. 

            AccountSummaries feed = list.Execute();
            List<AccountSummary> allRows = new List<AccountSummary>();

            //// Loop through until we arrive at an empty page
            while (feed.Items != null)
            {
                allRows.AddRange(feed.Items);

                // We will know we are on the last page when the next page token is
                // null.
                // If this is the case, break.
                if (feed.NextLink == null)
                {
                    break;
                }

                // Prepare the next page of results             
                list.StartIndex = feed.StartIndex + list.MaxResults;
                // Execute and process the next page request
                feed = list.Execute();

            }

            feed.Items = allRows;  // feed.Items not contains all of the rows even if there are more then 1000 


            //Get account summary and display them.
            foreach (AccountSummary account in feed.Items)
            {
                // Account
                Console.WriteLine("Account: " + account.Name + "(" + account.Id + ")");
                foreach (WebPropertySummary wp in account.WebProperties)
                {
                    // Web Properties within that account
                    Console.WriteLine("\tWeb Property: " + wp.Name + "(" + wp.Id + ")");

                    //Don't forget to check its not null. Believe it or not it could be.
                    if (wp.Profiles != null)
                    {
                        foreach (ProfileSummary profile in wp.Profiles)
                        {
                            // Profiles with in that web property.
                            Console.WriteLine("\t\tProfile: " + profile.Name + "(" + profile.Id + ")");
                        }
                    }
                }
            }

            return View();
        }
    }
}