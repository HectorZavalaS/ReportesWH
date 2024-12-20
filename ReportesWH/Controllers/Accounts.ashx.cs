using ReportesWH.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace ReportesWH.Controllers
{

    public class Accounts : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            HttpCookie cookie = new HttpCookie("_idU");
            string json = "{";
            String user = context.Request.Form["user"].ToString();
            String pass = context.Request.Form["pass"].ToString();
            var ac = new AccountController();
            string respond = ac.Validate(user, pass);

            if (respond.Equals("WAR_ADMIN") || respond.Equals("IT_ADMIN"))
            {
                cookie.Value = Convert.ToString(respond);
                //cookie.Secure = true;
                context.Response.Cookies.Add(cookie);
                FormsAuthentication.SetAuthCookie(user, true);
                json += "\"result\":\"true\"}";
                
            }
            else
            {
                json += "\"result\":\"false\"}";
            }
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