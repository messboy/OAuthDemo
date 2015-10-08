using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Web.Mvc;

namespace OAuthDemo.Controllers
{
    public class AnalyticsController : Controller
    {
        // GET: Analytics
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

            //取得token
            string AuthenticationKey = "";
            if (credential.RequestAccessTokenAsync(CancellationToken.None).Result)
            {

                AuthenticationKey = credential.Token.AccessToken;
            }


            ViewBag.auth = AuthenticationKey;
            return View();
        }

    }
}