using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotnetCoreManageLinux.Web.Controllers
{
    public class SqlController : Controller
    {
        public IActionResult SaveConStr()
        {
            return View();
        }

        public IActionResult RunQuery()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveConStr(string model)
        {
            HttpContext.Session.SetString("dbconStr", model);


            string Server = model;

            SqlConnection con = new SqlConnection(Server);
            List<MyTree> m = new List<MyTree>();

            try
            {
                con.Open();
                ViewData["Message"] = "connection open success";
                DataTable dt = GetData(con, CommandType.Text, "SELECT name FROM master.dbo.sysdatabases", null);

                string sonuc = "";
                int i = 0;
                foreach (DataRow item in dt.Rows)
                {
                    MyTree m1 = new MyTree();
                    m1.folder = true;
                    m1.key = item[0].ToString();
                    m1.title = item[0].ToString();
                    m1.lazy = true;
                    m.Add(m1);
                    sonuc += "<br>" + item[0].ToString();
                }
                ViewData["RESULT"] = sonuc;
                ViewData["tre"] = Json(m);
            }
            catch (Exception e)
            {
                ViewData["Message"] = e.Message;
            }
            finally
            {
                con.Close();
            }
            return View();
        }

        [HttpPost]
        public ActionResult RunQuery(string txtSql)
        {
            string Server = HttpContext.Session.GetString("dbconStr");
            //string Server = "Server=.\\SQLEXPRESS;Database=master;User Id=sa; Password=123456;";

            SqlConnection con = new SqlConnection(Server);

            string sonuc = "";
            try
            {
                con.Open();
                DataTable dt = GetData(con, CommandType.Text, txtSql, null);

                sonuc = ConvertDataTableToHTML(dt);
                //foreach (DataRow item in dt.Rows)
                //{
                //    sonuc += "<br>" + item[0].ToString();
                //}

            }
            catch (Exception e)
            {

            }
            finally
            {
                con.Close();
            }

            ResponseModel m = new ResponseModel();
            m.IsSuccess = true;
            m.Object = sonuc;
            return Json(m);
        }

        public static string ConvertDataTableToHTML(DataTable dt)
        {
            string html = "<table class=\"table table-bordered\">";
            //add header row
            html += "<tr>";
            for (int i = 0; i < dt.Columns.Count; i++)
                html += "<td>" + dt.Columns[i].ColumnName + "</td>";
            html += "</tr>";
            //add rows
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                html += "<tr>";
                for (int j = 0; j < dt.Columns.Count; j++)
                    html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                html += "</tr>";
            }
            html += "</table>";
            return html;
        }

        public static DataTable GetData(SqlConnection conn, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            System.Data.DataTable dt = new DataTable();
            System.Data.SqlClient.SqlDataAdapter da = new SqlDataAdapter(cmdText, conn);
            da.Fill(dt);
            return dt;
        }

        [HttpGet]
        public ActionResult getDbJson(string key)
        {
            string Server = HttpContext.Session.GetString("dbconStr");
            //string Server = "Server=.\\SQLEXPRESS;Database=TosiaCashDB;User Id=sa; Password=123456;";

            SqlConnection con = new SqlConnection(Server);
            List<MyTree> m = new List<MyTree>();

            try
            {
                con.Open();
                DataTable dt = GetData(con, CommandType.Text, "SELECT name FROM master.dbo.sysdatabases", null);

                string sonuc = "";
                int i = 0;

                MyTree mMaster = new MyTree();
                mMaster.folder = true;
                mMaster.key = con.Database.ToString();
                mMaster.title = con.DataSource.ToString() + "." + con.Database.ToString();
                mMaster.lazy = true;
                m.Add(mMaster);

                foreach (DataRow item in dt.Rows)
                {
                    MyTree m1 = new MyTree();

                    m1.folder = true;
                    m1.key = item[0].ToString();
                    m1.title = item[0].ToString();
                    m1.lazy = true;
                    mMaster.children = m1;
                    m.Add(m1);
                    sonuc += "<br>" + item[0].ToString();
                }

            }
            catch (Exception e)
            {

            }
            finally
            {
                con.Close();
            }

            return Json(m);
        }

        [HttpGet]
        public ActionResult getDbTablesJson(string key)
        {
            string Server = HttpContext.Session.GetString("dbconStr");

            //string Server = "Server=.\\SQLEXPRESS;Database="+ key + ";User Id=sa; Password=123456;";

            SqlConnection con = new SqlConnection(Server);
            List<MyTree> m = new List<MyTree>();

            try
            {
                con.Open();
                DataTable dt = GetData(con, CommandType.Text, "USE  "+key+"; SELECT TABLE_SCHEMA,TABLE_NAME,* FROM INFORMATION_SCHEMA.TABLES", null);

                string sonuc = "";
                int i = 0;
                foreach (DataRow item in dt.Rows)
                {
                    MyTree m1 = new MyTree();
                    m1.folder = false;
                    m1.key = item[1].ToString();
                    m1.title = item[0].ToString() + '.' + item[1].ToString();
                    m1.lazy = false;
                    m1.checkbox = false;
                    m.Add(m1);
                    sonuc += "<br>" + item[0].ToString();
                }

            }
            catch (Exception e)
            {

            }
            finally
            {
                con.Close();
            }

            return Json(m);
        }

    }
}