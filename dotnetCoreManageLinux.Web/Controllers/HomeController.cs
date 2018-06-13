using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using dotnetCoreManageLinux.Web.Models;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace dotnetCoreManageLinux.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public static string RunShellCommand(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunCommand([Bind("ShellCommand,Params,Result")] ShellCommandViewModel model)
        {
            if (string.IsNullOrEmpty(model.ShellCommand))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string result = RunShellCommand(model.ShellCommand);

                    model.Result = result;

                    ViewData["Command"] = model.ShellCommand;
                    ViewData["Result"] = result;

                    return View(model);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else
            {
                return View(model);
            }
        }

        public IActionResult RunCommand()
        {
            ViewData["Message"] = "Your application description page.";

            string cmd = "docker ps";
            string result = ""; // RunShellCommand(cmd);

            ViewData["Command"] = cmd;
            ViewData["Result"] = result;

            return View();
        }

        public static DataTable GetData(SqlConnection conn, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            System.Data.DataTable dt = new DataTable();
            System.Data.SqlClient.SqlDataAdapter da = new SqlDataAdapter(cmdText, conn);
            da.Fill(dt);
            return dt;
        }

        public class menuTree
        {
            // these are needed to set up the fancytree widget
            public string title { get; set; }
            public string key { get; set; }
            public bool folder { get; set; }
            public List<menuTree> children { get; set; }
            public List<menuTree> under { get; set; }
        }

        public class GenMenuList
        {
            // independant key for db record
            public int keyno { get; set; }

            // tree label name
            public string mdisplay { get; set; }

            // what level of the tree it's on, 0 being topmost parent
            public int levelno { get; set; }

            // if under !=0, under=key of parent it's nested under
            public int under { get; set; }

            // folder icon true or false for front end tree
            public bool folder { get; set; }

            // create list for children
            public List<GenMenuList> children { get; set; }

            public GenMenuList nest { get; set; }
            public GenMenuList(int keynoc, string mdisplayc, int levelnoc, int underc)
            {
                this.keyno = keynoc;
                this.mdisplay = mdisplayc;
                this.levelno = levelnoc;
                this.under = underc;
            }
        }

        public List<menuTree> GetGenMenu(List<GenMenuList> list)
        {
            // create new menu tree
            List<menuTree> mlist = new List<menuTree>();

            // hardcode manual example
            string genser = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            Console.WriteLine("First List (nested correctly):");
            Console.WriteLine(genser);


            foreach (GenMenuList dto in list)
            {
                // make a new branch of tree
                menuTree m1 = new menuTree();
                m1.title = dto.mdisplay;
                m1.key = dto.keyno.ToString();
                m1.children = new List<menuTree>();

                // this is a parent folder
                if (dto.levelno == 0)
                {
                    mlist.Add(m1);
                }

                else
                {
                    Traverse(mlist, dto, m1);
                }

            }

            string chkmlist = Newtonsoft.Json.JsonConvert.SerializeObject(mlist);

            Console.WriteLine("\nSecond List (catching the parents):");
            Console.WriteLine(chkmlist);

            return mlist;
            //return "returned";
            //return Json(mlist, JsonRequestBehavior.AllowGet);
        }

        public void Traverse(List<menuTree> mlist, GenMenuList dto, menuTree m1)
        {
            foreach (var i in mlist)
            {
                // if this is a matching child
                if (i.key == dto.under.ToString())
                {
                    // add this as a child
                    i.children.Add(m1);
                }
                i.folder = i.children.Count != 0;
                Traverse(i.children, dto, m1);
            }
        }

        [HttpGet]
        public ActionResult getJsonMlist()
        {
            List<GenMenuList> list = new List<GenMenuList>();
            GenMenuList item1 = new GenMenuList(1, "Parent Folder 1", 0, 0);
            //item1.folder = true;

            list.Add(item1);

            GenMenuList item2 = new GenMenuList(2, "Parent Folder 2", 1, 1);
            list.Add(item2);

            GenMenuList item3 = new GenMenuList(3, "Parent Folder 3", 0, 1);
            list.Add(item3);

            GenMenuList item5 = new GenMenuList(5, "(1) Nested Folder", 3, 3);
            item1.nest = item5;

            GenMenuList key12 = new GenMenuList(12, "(1) Nested Nested File", 5, 5);
            item5.nest = key12;




            return Json(GetGenMenu(list));
        }

        [HttpGet]
        public ActionResult getjson2(string key)
        {
            //string j = "[ {\"title\": \"Sub item\", \"lazy\": true }, {\"title\": \"Sub folder\", \"folder\": true, \"lazy\": true } ]";
            List<MyTree> m = new List<MyTree>();
            MyTree m1 = new MyTree();
            m1.folder = true;
            m1.key = "1";
            m1.title = "s1";
            m1.lazy = true;
            m1.icon = "http://icons.iconarchive.com/icons/gakuseisean/ivista-2/48/Misc-Database-3-icon.png";

            MyTree m2 = new MyTree();
            m2.folder = true;
            m2.key = "2";
            m2.title = "s2";
            m2.lazy = true;
            m2.icon = "";

            m.Add(m1);
            m.Add(m2);

            //var pathToJson = Path.Combine("test.json");
            //var r = new StreamReader(pathToJson);
            //var myJson = r.ReadToEnd();

            return Json(m);
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
                DataTable dt = GetData(con, CommandType.Text, "SELECT TABLE_SCHEMA,TABLE_NAME,* FROM INFORMATION_SCHEMA.TABLES", null);

                string sonuc = "";
                int i = 0;
                foreach (DataRow item in dt.Rows)
                {
                    MyTree m1 = new MyTree();
                    m1.folder = false;
                    m1.key = item[1].ToString();
                    m1.title = item[0].ToString()+'.'+ item[1].ToString();
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

        public IActionResult About()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> About(string model)
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


        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

