using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using SambariEnterprises.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace SambariEnterprises.Helpers
{
    public class EmailHelper
    {
        public static bool SendUserMail(RegistrationViewModel model)
        {
            string emailBody = string.Empty;
            // This to supress warnings from razor engine template
            IRazorEngineService razorService = RazorEngineService.Create(new TemplateServiceConfiguration
            {
                DisableTempFileLocking = false,
                CachingProvider = new DefaultCachingProvider(t => { })
            });

            Engine.Razor = razorService;

            string templatePath = HttpContext.Current.Server.MapPath("~/Content/EmailTemplates/UserRegistrationEmailTemplate.cshtml");

            using (var reader = new StreamReader(templatePath))
            {
                var template = reader.ReadToEnd();
                Engine.Razor.AddTemplate("AdminEmailTemplate", template);
                var emailBodyHtml = Engine.Razor.RunCompile("AdminEmailTemplate", typeof(RegistrationViewModel), model);
                emailBody = emailBodyHtml;
            }

            //MailMessage mail = new MailMessage(ConfigurationManager.AppSettings["FromMail"], model.ContactPersonEmail);
            //SmtpClient client = new SmtpClient();
            //client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false;
            //client.Host = ConfigurationManager.AppSettings["SmtpHost"];
            //mail.Subject = "New registration in to Sambari Enterprises";
            //mail.Body = emailBody;
            //mail.IsBodyHtml = true;
            //client.Send(mail);

            //SmtpClient client = new SmtpClient("in.mailjet.com ", 465)
            //{
            //    Credentials = new NetworkCredential("f57dfa92125cde464b72bd53233a8d01", "f8fc65bc7a4451411025894add71b2f1"),
            //    EnableSsl = true
            //};

            var mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.To.Add(model.Email);
            mailMessage.Subject = "New registration in to Sambari Enterprises";
            mailMessage.Body = emailBody;
            mailMessage.IsBodyHtml = true;

            var smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);

            return true;
        }

        public static bool SendAdminMail(RegistrationViewModel model)
        {
            string emailBody = string.Empty;
            // This to supress warnings from razor engine template
            IRazorEngineService razorService = RazorEngineService.Create(new TemplateServiceConfiguration
            {
                DisableTempFileLocking = false,
                CachingProvider = new DefaultCachingProvider(t => { })
            });

            Engine.Razor = razorService;

            string templatePath = HttpContext.Current.Server.MapPath("~/Content/EmailTemplates/UserRegistrationEmailTemplate.cshtml");

            using (var reader = new StreamReader(templatePath))
            {
                var template = reader.ReadToEnd();
                Engine.Razor.AddTemplate("AdminEmailTemplate", template);
                var emailBodyHtml = Engine.Razor.RunCompile("AdminEmailTemplate", typeof(RegistrationViewModel), model);
                emailBody = emailBodyHtml;
            }

            //MailMessage mail = new MailMessage(ConfigurationManager.AppSettings["FromMail"], ConfigurationManager.AppSettings["AdminEmail"]);
            //SmtpClient client = new SmtpClient();
            //client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false;
            //client.Host = ConfigurationManager.AppSettings["SmtpHost"];
            //mail.Subject = "New registration in to Sambari Enterprises";
            //mail.Body = emailBody;
            //mail.IsBodyHtml = true;
            //client.Send(mail);

            var mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.To.Add(ConfigurationManager.AppSettings["FromMail"]);
            mailMessage.Subject = "New registration in to Sambari Enterprises";
            mailMessage.Body = emailBody;
            mailMessage.IsBodyHtml = true;

            var smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);

            return true;
        }

        public static bool SendUserDetailsMail(MemberViewModel memberViewModel)
        {
            string emailBody = string.Empty;
            // This to supress warnings from razor engine template
            IRazorEngineService razorService = RazorEngineService.Create(new TemplateServiceConfiguration
            {
                DisableTempFileLocking = false,
                CachingProvider = new DefaultCachingProvider(t => { })
            });

            Engine.Razor = razorService;

            string templatePath = HttpContext.Current.Server.MapPath("~/Content/EmailTemplates/UserDetailsEmailTemplate.cshtml");

            using (var reader = new StreamReader(templatePath))
            {
                var template = reader.ReadToEnd();
                Engine.Razor.AddTemplate("UserDetailsEmailTemplate", template);
                var emailBodyHtml = Engine.Razor.RunCompile("UserDetailsEmailTemplate", typeof(MemberViewModel), memberViewModel);
                emailBody = emailBodyHtml;
            }

            //MailMessage mail = new MailMessage(ConfigurationManager.AppSettings["FromMail"], memberViewModel.Email);
            //SmtpClient client = new SmtpClient();
            //client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false;
            //client.Host = ConfigurationManager.AppSettings["SmtpHost"];
            //mail.Subject = "Sambari Enterprises - Login Details";
            //mail.Body = emailBody;
            //client.Send(mail);

            var mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.To.Add(memberViewModel.Email);
            mailMessage.Subject = "Sambari Enterprises - Login Details";
            mailMessage.Body = emailBody;
            mailMessage.IsBodyHtml = true;

            var smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);

            return true;
        }

        public static bool SendResetPasswordMail(string email, ForgotPasswordViewModel forgotPasswordViewModel)
        {
            string emailBody = string.Empty;
            // This to supress warnings from razor engine template
            IRazorEngineService razorService = RazorEngineService.Create(new TemplateServiceConfiguration
            {
                DisableTempFileLocking = false,
                CachingProvider = new DefaultCachingProvider(t => { })
            });

            Engine.Razor = razorService;

            string templatePath = HttpContext.Current.Server.MapPath("~/Content/EmailTemplates/ForgotPasswordEmailTemplate.cshtml");

            using (var reader = new StreamReader(templatePath))
            {
                var template = reader.ReadToEnd();
                Engine.Razor.AddTemplate("ForgotPasswordEmailTemplate", template);
                var emailBodyHtml = Engine.Razor.RunCompile("ForgotPasswordEmailTemplate", typeof(ForgotPasswordViewModel), forgotPasswordViewModel);
                emailBody = emailBodyHtml;
            }

            //MailMessage mail = new MailMessage(ConfigurationManager.AppSettings["FromMail"], memberViewModel.Email);
            //SmtpClient client = new SmtpClient();
            //client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false;
            //client.Host = ConfigurationManager.AppSettings["SmtpHost"];
            //mail.Subject = "Sambari Enterprises - Login Details";
            //mail.Body = emailBody;
            //client.Send(mail);

            var mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.To.Add(email);
            mailMessage.Subject = "Sambari Enterprises - Reset Password";
            mailMessage.Body = emailBody;
            mailMessage.IsBodyHtml = true;

            var smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);

            return true;
        }
    }
}