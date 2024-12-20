using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace ReportesWH.Controllers
{
    /// <summary>
    /// Summary description for Letmeout
    /// </summary>
    public class Letmeout : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string json = "{";

            HttpCookie cookie = context.Request.Cookies["_idu"];
            cookie.Expires = DateTime.Now.AddYears(-1);
            context.Response.Cookies.Add(cookie);
            context.Request.Cookies.Remove("_idu");
            FormsAuthentication.SignOut();

            json += "\"result\":\"true\"}";

            context.Response.ContentType = "text/plain";
            context.Response.Write(json);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}