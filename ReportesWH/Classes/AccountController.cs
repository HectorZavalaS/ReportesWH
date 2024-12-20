using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Ajax.Utilities;
using ReportesWH.Models;

namespace ReportesWH.Classes
{
    public class AccountController
    {
        public string Validate(string user, string pass)
        {
            string pc = "";
            var db = new siixsem_main_dbEntities();
            pass = GetSHA1(pass);
            var vu = db.validate_user(user, pass).ToList();
            if (vu.First().RESULT == 1 && (vu.First().se_level == 1 && (vu.First().code.Equals("WAR_ADMIN")||vu.First().code.Equals("IT_ADMIN"))))
            {
                pc = vu.First().code;
            }
            return pc;
        }
       
        public static string GetSHA1(String texto)
        {
            SHA1 sha1 = SHA1CryptoServiceProvider.Create();
            Byte[] textOriginal = ASCIIEncoding.Default.GetBytes(texto);
            Byte[] hash = sha1.ComputeHash(textOriginal);
            StringBuilder cadena = new StringBuilder();
            foreach (byte i in hash)
            {
                cadena.AppendFormat("{0:x2}", i);
            }
            return cadena.ToString();
        }
    }
}