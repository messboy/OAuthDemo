using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OAuthDemo.Controllers
{
    public class TokenController : Controller
    {
        // GET: Token
        public ActionResult Index()
        {
            string _oauthUrl = string.Format("https://accounts.google.com/o/oauth2/auth?" + "scope={0}&state={1}&redirect_uri={2}&response_type=code&client_id={3}&approval_prompt=force",
                        HttpUtility.UrlEncode("https://www.googleapis.com/auth/youtube https://www.googleapis.com/auth/youtube.readonly https://www.googleapis.com/auth/youtube.upload https://www.googleapis.com/auth/youtubepartner"),
                        "",
                        HttpUtility.UrlEncode("http://localhost:7115/"),
                        HttpUtility.UrlEncode("902116665022-co28khijsesjpmqoadulnbke2vmkc3hb.apps.googleusercontent.com"));

            return Redirect(_oauthUrl);

            byte[] bs = Encoding.ASCII.GetBytes(_oauthUrl);
            string result = string.Empty;

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://accounts.google.com/o/oauth2/auth?");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bs.Length;

            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
            }
            using (WebResponse wr = req.GetResponse())
            {
                //在這裡對接收到的頁面內容進行處理
                using (StreamReader myStreamReader = new StreamReader(wr.GetResponseStream()))
                {
                    result = myStreamReader.ReadToEnd();

                    //TODO 驗證aud & email 驗證都正確才算通過
                }

            }


            return Json(result, JsonRequestBehavior.AllowGet);


            return Redirect(_oauthUrl);
        }

        public ActionResult callback()
        {
            string queryStringFormat = @"code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code";
            string postcontents = string.Format(queryStringFormat
                                               , HttpUtility.UrlEncode(Request["Code"])
                                               , HttpUtility.UrlEncode(ConfigurationSettings.AppSettings.Get("GoogleClientID"))
                                               , HttpUtility.UrlEncode("ygcZU7FckhWa75-G53zDoogc")
                                               , HttpUtility.UrlEncode("http://localhost:1077/Default.aspx"));
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://accounts.google.com/o/oauth2/token");
            request.Method = "POST";
            byte[] postcontentsArray = Encoding.UTF8.GetBytes(postcontents);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postcontentsArray.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(postcontentsArray, 0, postcontentsArray.Length);
                requestStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string responseFromServer = reader.ReadToEnd();
                    reader.Close();
                    responseStream.Close();
                    response.Close();
                    //SerializeToken(responseFromServer);
                }
            }

            return View();
        }
    }
}