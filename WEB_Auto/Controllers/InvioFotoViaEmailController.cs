using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_Auto.Models;
using System.Net.Mail;
using System.Net;
using System.Data.SqlClient;

namespace WEB_Auto.Controllers
{
    public class InvioFotoViaEmailController : Controller
    {
        private wisedbEntities db = new wisedbEntities();
        // GET: InvioFotoViaEmail
        public ActionResult AllegaFoto()
        {
            //string filename = "";
            string path = System.IO.Path.Combine(Server.MapPath("~/DocumentiXTelai/Email/Foto"));


            var model = new Models.HomeModel();



            var myFoto = (from f in db.WEB_AUTO_FOTO_X_EMAIL
                          select f);
            model.WEB_AUTO_FOTO_X_EMAIL = myFoto.ToList();
            UpdateModel(myFoto);

            System.IO.DirectoryInfo di = new DirectoryInfo(path);

            //foreach (FileInfo file in di.GetFiles())
            //{
            //    file.Delete();
            //    var sql = @"DELETE FROM  WEB_AUTO_FOTO_X_EMAIL WHERE FileName = @FileName ";
            //    int noOfRowInserted = db.Database.ExecuteSqlCommand(sql, new SqlParameter("@FileName", filename));
            //}
            return View(myFoto);
        }

        public ActionResult UploadFoto(IEnumerable<HttpPostedFileBase> files, string myIDPerizia, string IDPerito, string IDSpedizione, string IDMeteo,
               string IDTP, string aIDTrasportatore, string aIDTipoRotabile, string aIDModelloCasa, string ErrMess = "", bool IsUpdate = false)
        {
            string filename = "";
            string path = System.IO.Path.Combine(Server.MapPath("~/DocumentiXTelai/Email/Foto"));

           System.IO.DirectoryInfo di = new DirectoryInfo(path);

            //foreach (FileInfo file in di.GetFiles())
            //{
            //    file.Delete();
            //    var sql = @"DELETE FROM  WEB_AUTO_FOTO_X_EMAIL WHERE FileName = @FileName ";
            //    int noOfRowInserted = db.Database.ExecuteSqlCommand(sql, new SqlParameter("@FileName", filename));
            //}

            foreach (var file in files)
            {
                if (file != null)
                {
                    filename = System.IO.Path.GetFileName(file.FileName);

                    path = System.IO.Path.Combine(Server.MapPath("~/DocumentiXTelai/Email/Foto"), filename);
                    if (file != null)
                    {
                        file.SaveAs(path);
                    }
                    int cnt = 0;
                    //try
                    //{
                    //    cnt = (int)(from m in db.WEB_AUTO_FOTO
                    //                where m.IDPerizia == myIDPerizia
                    //                select m.Prog).Max();
                    //}
                    //catch { }
                    cnt++;

                    var sql = @"INSERT INTO WEB_AUTO_FOTO_X_EMAIL ( FileName) Values (@FileName)";
                    int noOfRowInserted = db.Database.ExecuteSqlCommand(sql, new SqlParameter("@FileName", filename));


                }
            }

            //// INVIO MAIL !
            //MailMessage mail = new MailMessage();
            ////SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            //SmtpClient SmtpServer = new SmtpClient("mail.collabra.it");
            //System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            //try
            //{
            //    mail.From = new System.Net.Mail.MailAddress("system_actor@interconsult.it");
            //    mail.To.Add("maurizio.vigna@interconsult.it");

            //    mail.Subject = "Invio email MVC BOT";
            //    mail.Body = "TEST ! ";



            //    SmtpServer.Port = 587;
            //    SmtpServer.Credentials = new System.Net.NetworkCredential("system_actor@interconsult.it", "!nTerc0n$u1t");
            //    SmtpServer.EnableSsl = true;

            //    // Allego foto
            //    path = System.IO.Path.Combine(Server.MapPath("~/DocumentiXTelai/Email/Foto"));
            //    di = new DirectoryInfo(path);

            //    foreach (FileInfo file in di.GetFiles())
            //    {
            //       mail.Attachments.Add(new Attachment(System.IO.Path.Combine(Server.MapPath("~/DocumentiXTelai/Email/Foto/")+ file.Name)));
            //    }

            //    SmtpServer.Send(mail);
            //    mail.Dispose();

            //}
            //catch
            //{
            //    //MessageBox.Show(ex.Message);
            //}
            //finally
            //{
            //    mail = null;
            //    SmtpServer = null;
            //}

            var model = new Models.HomeModel();



            var myFoto = (from f in db.WEB_AUTO_FOTO_X_EMAIL
                          select f);
            model.WEB_AUTO_FOTO_X_EMAIL = myFoto.ToList();
            UpdateModel(myFoto);


            //ViewBag.myIDPerizia = myIDPerizia;
            //ViewBag.IDPerito = IDPerito;
            //ViewBag.IDSpedizione = IDSpedizione;
            //ViewBag.IDMeteo = IDMeteo;
            //ViewBag.IDTP = IDTP;
            //ViewBag.aIDTrasportatore = aIDTrasportatore;
            //ViewBag.aIDTipoRotabile = aIDTipoRotabile;
            //ViewBag.aIDModelloCasa = aIDModelloCasa;
            //ViewBag.ErrMess = ErrMess;
            //ViewBag.IsUpdate = IsUpdate;
            return View("AllegaFoto",myFoto);
        }

        public ActionResult CancellaDocumento(int? IDDocumento, string nomefile)
        {
            var sql = @"DELETE FROM WEB_AUTO_FOTO_X_EMAIL WHERE ID = @IDDocumento";
            int myRecordCounter = db.Database.ExecuteSqlCommand(sql, new SqlParameter("@IDDocumento", IDDocumento));

            string fullPath = Request.MapPath("~/DocumentiXTelai/Email/Foto/" + nomefile);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            var model = new Models.HomeModel();



            var myFoto = (from f in db.WEB_AUTO_FOTO_X_EMAIL
                          select f);
            model.WEB_AUTO_FOTO_X_EMAIL = myFoto.ToList();

            //ViewBag.IDTelaio = myIDPerizia;
            //return RedirectToAction("AllegaFoto", "InvioFotoViaEmail", null);

            return RedirectToAction("AllegaFoto");
        }

        public ActionResult InviaMail()
        {
            // INVIO MAIL !
            //string filename = "";
            string path = System.IO.Path.Combine(Server.MapPath("~/DocumentiXTelai/Email/Foto"));
            bool isSent = false;

            System.IO.DirectoryInfo di = new DirectoryInfo(path);

            MailMessage mail = new MailMessage();
            //SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpClient SmtpServer = new SmtpClient("mail.collabra.it");
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            try
            {
                mail.From = new System.Net.Mail.MailAddress("system_actor@interconsult.it");
                mail.To.Add("maurizio.vigna@astreaclaim.eu");

                string myIDPerito = Session["IDPeritoVero"].ToString();

                var myemail = (from f in db.AGR_Periti_WEB
                               where f.IDVero == myIDPerito
                               select f.EmailAddress).FirstOrDefault();

                if (!String.IsNullOrEmpty(myemail))
                    mail.To.Add("maurizio.vigna@astreaclaim.eu");

                mail.Subject = "Invio email MVC BOT";
                mail.Body = "TEST ! ";



                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("system_actor@interconsult.it", "!nTerc0n$u1t");
                SmtpServer.EnableSsl = true;

                // Allego foto
                path = System.IO.Path.Combine(Server.MapPath("~/DocumentiXTelai/Email/Foto"));
                di = new DirectoryInfo(path);

                foreach (FileInfo file in di.GetFiles())
                {
                    mail.Attachments.Add(new Attachment(System.IO.Path.Combine(Server.MapPath("~/DocumentiXTelai/Email/Foto/") + file.Name)));
                }

                SmtpServer.Send(mail);
                mail.Dispose();
                isSent = true;

            }
            catch
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                mail = null;
                SmtpServer = null;
            }

            if(isSent)
            {


                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                    
                }
                var sql = @"DELETE FROM WEB_AUTO_FOTO_X_EMAIL ";
                int myRecordCounter = db.Database.ExecuteSqlCommand(sql);


                
                var model = new Models.HomeModel();



                

                return RedirectToAction("AllegaFoto");
            }

            return RedirectToAction("AllegaFoto");
        }
    }
}