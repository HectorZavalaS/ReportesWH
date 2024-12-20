using ReportesWH.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ReportesWH.Controllers
{
    public class getReport1 : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string[] own = {"OWH-Out","OWH-In"};
            string json = "{";
            int c = 0;
            int sumStoreIn = 0;
            int sumPicking = 0;
            int sumMaterialOut = 0;
            string test = "";

            try
            {

                var m_oracle = new COracle("192.168.0.23", "SEMPROD");

                String datestart = context.Request.Form["ds"].ToString();
                //String dateend = context.Request.Form["de"].ToString();
                test = "Oracle Conectado " + datestart + " " + m_oracle.conexion.ToString();
                DateTime ds;
                DateTime.TryParse(datestart, out ds);
                DateTime de = ds.AddMonths(1);
                //DateTime.TryParse(dateend, out de);

                DataTable rpsi = m_oracle.getReportSI(ds,de);
                DataTable rpmo = m_oracle.getReportMO(ds,de);
                DataTable rpfp = m_oracle.getReportFP(ds,de);
                DataTable rpiv = m_oracle.getReportIV(ds,de);
                DataTable rpsw = m_oracle.getReportSW(ds,de);
                DataTable rpfg = m_oracle.getReportFG(ds,de);

                string html = "";

                html += "<div><table id='report' class='table table-bordered table-sm'>" +
                "<thead><tr>" +
                //"<th>Process</th>" +
                "<th scope='col'>Transactions</th>" +
                "<th scope='col'>Batch number</th></tr></thead>";
                html += "<tr>";
                html += "<th colspan='2'>Store In</th>";
                html += "</tr>";
                foreach (DataRow row in rpsi.Rows)
                {
                    string description = row["DESCRIPTION"].ToString();
                    string count = row["QTY"].ToString();
                    test += "\n"+description +" "+count;
                    html += "<tr>";
                    html += "<td>" + description + "</td>";
                    html += "<td>" + count + "</td>";
                    html += "</tr>";
                    sumStoreIn += int.Parse(count);
                }
                html += "<tr>";
                html += "<td style='font-weight:bold;'>Total STORE IN</td><td style='font-weight:bold;'>" + sumStoreIn+"</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<th colspan='2'>Material Out</th>";
                html += "</tr>";
                foreach (DataRow row in rpmo.Rows)
                {
                    string description = row["DESCRIPTION"].ToString();
                    string count = row["QTY"].ToString();

                    html += "<tr>";
                    html += "<td>" + description + "</td>";
                    html += "<td>" + count + "</td>";
                    html += "</tr>";
                    sumPicking += int.Parse(count);
                }
                foreach (DataRow row in rpfp.Rows)
                {
                    string description = row["DESCRIPTION"].ToString();
                    string count = row["QTY"].ToString();

                    html += "<tr>";
                    html += "<td>" + description + "</td>";
                    html += "<td>" + count + "</td>";
                    html += "</tr>";
                    sumPicking += int.Parse(count);
                }
                html += "<tr>";
                html += "<td style='font-weight:bold;'>Total PICKING</td><td style='font-weight:bold;'>" + sumPicking + "</td>";
                html += "</tr>";
                foreach (DataRow row in rpiv.Rows)
                {
                    string count = row["QTY"].ToString();

                    html += "<tr>";
                    html += "<td>Tray CONS Transactions</td>";
                    html += "<td>" + count + "</td>";
                    html += "</tr>";
                    sumMaterialOut += int.Parse(count);
                }
                foreach (DataRow row in rpsw.Rows)
                {
                    string count = row["QTY"].ToString();

                    html += "<tr>";
                    html += "<td>RETURN SMT -> WH</td>";
                    html += "<td>" + count + "</td>";
                    html += "</tr>";
                    sumMaterialOut += int.Parse(count);
                }
                sumMaterialOut += sumPicking;
                html += "<tr>";
                html += "<td style='font-weight:bold;'>Total Material OUT</td><td style='font-weight:bold;'>" + sumMaterialOut + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<th colspan='2'>FG</th>";
                html += "</tr>";
                foreach (DataRow row in rpfg.Rows)
                {
                    string count = row["QTY"].ToString();
                    
                    html += "<tr>";
                    html += "<td>" + own[c] + "</td>";
                    html += "<td>" + count + "</td>";
                    html += "</tr>";
                    c++;
                }
                c = 0;
                test += " " + html;
                html += "</table></div>";
                json += "\"result\":\"true\",";
                json += "\"html\":\"" + html + "\"";
            }
            catch (Exception ex)
            {
                json += "\"result\":\"false\",";
                json += "\"MessageError\":\"" + ex.Message + " "+ test +"\"";
            }
            json += "}";
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