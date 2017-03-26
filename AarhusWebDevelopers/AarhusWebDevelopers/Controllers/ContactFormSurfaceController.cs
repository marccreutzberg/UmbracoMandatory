using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using AarhusWebDevelopers.ViewModels;
using System.Net.Mail;
using Umbraco.Core.Models;


namespace AarhusWebDev.Controllers
{
    public class ContactFormSurfaceController : SurfaceController    {
        // GET: ContactFormSurface
        public ActionResult Index()
        {
            return PartialView("ContactForm", new ContactForm());
        }

        [HttpPost]
        public ActionResult HandleFormSubmit(ContactForm model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            //creating a new mail object to send a email
            MailMessage message = new MailMessage();
            message.To.Add("mail@mcreutzberg.dk");
            message.Subject = model.Subject;
            message.From = new MailAddress(model.Email, model.Name);
            message.Body = model.Message;


            //creating a new umbraco node
            IContent comment = Services.ContentService.CreateContent(model.Subject, CurrentPage.Id, "message");

            comment.SetValue("messageName", model.Name);
            comment.SetValue("email", model.Email);
            comment.SetValue("subject", model.Subject);
            comment.SetValue("messageContent", model.Message);

           //saving umbraco Node
            Services.ContentService.Save(comment);


            //For Save and publish
            //Services.ContentService.SaveAndPublishWithStatus(comment);

            //Create smtp and sending email
            using (SmtpClient smtp = new SmtpClient())
            {
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential("Creutzberq@gmail.com", "tsjuofiawkctjanz");
                smtp.EnableSsl = true;
                // send mail
                smtp.Send(message);
            }

            //setting temp data = true
            TempData["success"] = true;

            return RedirectToCurrentUmbracoPage();
        }

    }
}