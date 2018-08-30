using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.Mvc;
using SystemManagement.Models;

namespace SystemManagement.Controllers
{
    public class ReportController : Controller
    {
        public static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["SystemManagement"].ConnectionString;
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        private List<ReportTransferUserGold> GetData()
        {
            var DateFrom = "2018-08-01";
            var DateTo = "2018-08-10";
            var UserSearch = "11";
            var TypeSearch = 1;
            SqlCommand cmd = new SqlCommand();
            var list = new List<ReportTransferUserGold>();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                cmd.Connection = conn;
                cmd.CommandText = "usp_ReportTransferGold_GetData";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] p =
                {
                    new SqlParameter("DateFrom", DateFrom),
                    new SqlParameter("DateTo", DateTo),
                    new SqlParameter("UserSearch", UserSearch),
                    new SqlParameter("TypeSearch", TypeSearch),
                };
                foreach (SqlParameter parm in p)
                {
                    cmd.Parameters.Add(parm);
                }
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    var m = new ReportTransferUserGold();
                    m.TotalGoldSend = Convert.ToDecimal(dr["TotalGoldSend"]);
                    m.TotalGoldRecieve = Convert.ToDecimal(dr["TotalGoldRecieve"]);
                    m.DateSend = Convert.ToDateTime(dr["DateSend"]);
                    list.Add(m);
                }
                conn.Close();
            }
            return list;
        }

        public ActionResult UserTransferGold()
        {
            Highcharts columnChart = new Highcharts("columnchart");

            columnChart.InitChart(new Chart()
            {
                Type = ChartTypes.Column,
                BackgroundColor = new BackColorOrGradient(Color.AliceBlue),
                Style = "fontWeight: 'bold', fontSize: '17px'",
                BorderColor = Color.LightBlue,
                BorderRadius = 0,
                BorderWidth = 2

            });

            columnChart.SetTitle(new Title()
            {
                Text = "Sachin Vs Dhoni"
            });

            columnChart.SetSubtitle(new Subtitle()
            {
                Text = "Played 9 Years Together From 2004 To 2012"
            });

            var list = GetData();

            var categories = new string[list.Count];
            var send = new Object[list.Count];
            var recieve = new Object[list.Count];

            for(int i=0; i < list.Count; i++)
            {
                categories[i] = $"{list[i].DateSend.Day}/{list[i].DateSend.Month}";
                send[i] = list[i].TotalGoldSend;
                recieve[i] = list[i].TotalGoldRecieve;
            }


            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "Day", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = categories
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "Runs",
                    Style = "fontWeight: 'bold', fontSize: '17px'"
                },
                ShowFirstLabel = true,
                ShowLastLabel = true,
                Min = 0
            });

            columnChart.SetLegend(new Legend
            {
                Enabled = true,
                BorderColor = Color.CornflowerBlue,
                BorderRadius = 6,
                BackgroundColor = new BackColorOrGradient(ColorTranslator.FromHtml("#FFADD8E6"))
            });

            

            columnChart.SetSeries(new Series[]
            {
                new Series{

                    Name = "Send",
                    Data = new Data(send)
                },
                new Series()
                {
                    Name = "Recieve",
                    Data = new Data(recieve)
                }
            }
            );

            return View(columnChart);
        }
    }
}