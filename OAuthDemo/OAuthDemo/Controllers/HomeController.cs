﻿using Facebook;
using System.IO;
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

                    //TODO 驗證aud & email 驗證都正確才算通過
                }
                
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //oauth2callback
        public ActionResult oauth2callback()
        {
            string queryStringFormat = @"code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code";
            string postcontents = string.Format(queryStringFormat
                                               , HttpUtility.UrlEncode(Request["Code"])
                                               , HttpUtility.UrlEncode("902116665022-co28khijsesjpmqoadulnbke2vmkc3hb.apps.googleusercontent.com")
                                               , HttpUtility.UrlEncode("jJ469gR_acVPai4ClXuccLUi")
                                               , HttpUtility.UrlEncode("http://localhost:7115//Home/oauth2callback"));
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://accounts.google.com/o/oauth2/token");
            request.Method = "POST";
            byte[] postcontentsArray = Encoding.UTF8.GetBytes(postcontents);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postcontentsArray.Length;

            string responseFromServer = string.Empty;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(postcontentsArray, 0, postcontentsArray.Length);
                requestStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    responseFromServer = reader.ReadToEnd();
                    reader.Close();
                    responseStream.Close();
                    response.Close();
                    //SerializeToken(responseFromServer);
                }
            }

            return View(responseFromServer);

        }
    }
}