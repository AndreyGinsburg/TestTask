using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Excel = Microsoft.Office.Interop.Excel;
using System.Web.Mvc;
using System.IO;
using WebApplication31.Models;

namespace WebApplication31.Controllers
{
    public class HomeController : Controller
    {
        AccContext db = new AccContext();
        public ActionResult Index()
        {
            string appDataPath = System.Web.HttpContext.Current.Server.MapPath(@"~/App_Data");
            string fileName = "File.xls";
            string absolutePathToFile = Path.Combine(appDataPath, fileName);
            Models.Excel excel = new Models.Excel(absolutePathToFile, 1);
            int i = 9;
            int classCondition = 1;
            int accCondition = 1;
            bool newAcc = true;
            while (true)
            {
                int n;
                if (excel.ws.Cells[i, 1].Value2 == null)
                {
                    break;
                }
                else if (!Int32.TryParse(excel.ws.Cells[i, 1].Value2.ToString(), out n))
                {
                    string s = excel.ws.Cells[i, 1].Value2;
                    string[] str = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (str[0] == "КЛАСС")
                    {
                        Class _class = new Class();
                        _class.title = s;
                        _class.id = classCondition;
                        classCondition++;
                        db.classes.Add(_class);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (n < 100)
                    {
                        newAcc = true;
                    }
                    else
                    {
                        if (newAcc)
                        {
                            Account acc = new Account();
                            acc.id = accCondition;
                            accCondition++;
                            acc.number = n / 100;
                            db.accounts.Add(acc);
                            db.SaveChanges();
                            newAcc = false;
                        }
                        InBal inBal = new InBal();
                        inBal.number = n;
                        inBal._account = db.accounts.Find(accCondition - 1);
                        inBal.act = Double.Parse(excel.ws.Cells[i, "B"].Value2.ToString());
                        inBal.pass = Double.Parse(excel.ws.Cells[i, "C"].Value2.ToString());
                        db.accounts.Find(accCondition - 1).inBals.Add(inBal);
                        db.classes.Find(classCondition - 1).inBals.Add(inBal);
                        Rev rev = new Rev();
                        rev.number = n;
                        rev._account = db.accounts.Find(accCondition - 1);
                        rev.act = Double.Parse(excel.ws.Cells[i, "D"].Value2.ToString());
                        rev.pass = Double.Parse(excel.ws.Cells[i, "E"].Value2.ToString());
                        db.accounts.Find(accCondition - 1).revs.Add(rev);
                        db.classes.Find(classCondition - 1).revs.Add(rev);
                        OutBal outBal = new OutBal();
                        outBal.number = n;
                        outBal._account = db.accounts.Find(accCondition - 1);
                        outBal.act = Double.Parse(excel.ws.Cells[i, "F"].Value2.ToString());
                        outBal.pass = Double.Parse(excel.ws.Cells[i, "G"].Value2.ToString());
                        db.accounts.Find(accCondition - 1).outBals.Add(outBal);
                        db.classes.Find(classCondition - 1).outBals.Add(outBal);
                        db.SaveChanges();
                    }
                }
                i++;
            }
            return View(db.classes.ToList());
        }
    }
}