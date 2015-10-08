using Facebook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
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
                }
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}