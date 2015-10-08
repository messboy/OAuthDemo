using Facebook;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace OAuthDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Demo()
        {
            return View();
        }

        public JsonResult FacebookLogin(string token)
        {
            var client = new FacebookClient(token);
            dynamic result = client.Get("me", new { fields = "name,id,email" });
            string name = result.name;
            string id = result.id;
            return Json(name, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GoogleLogin(string token)
        {
            string param = string.Format("id_token={0}", token);
            byte[] bs = Encoding.ASCII.GetBytes(param);
            string result = string.Empty;

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://www.googleapis.com/oauth2/v3/tokeninfo");
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

                    //    "iss": "accounts.google.com",
                    //     "at_hash": "Dn3kUsH2goJMfq5N6TqReA",
                    //     "aud": "902116665022-co28khijsesjpmqoadulnbke2vmkc3hb.apps.googleusercontent.com",
                    //     "sub": "105928066147029190178",
                    //     "email_verified": "true",
                    //     "azp": "902116665022-co28khijsesjpmqoadulnbke2vmkc3hb.apps.googleusercontent.com",
                    //     "hd": "brightideas.com.tw",
                    //     "email": "simon@brightideas.com.tw",
                    //     "iat": "1444283819",
                    //     "exp": "1444287419",
                    //     "name": "陳--",
                    //     "given_name": "昱元",
                    //     "family_name": "陳",
                    //     "locale": "zh-TW",
                    //     "alg": "RS256",
                    //     "kid": "9015759ea37707cb6d325cca00e6299231b7f72f"
}
                
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}