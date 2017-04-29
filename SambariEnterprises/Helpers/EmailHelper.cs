using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using SambariEnterprises.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace SambariEnterprises.Helpers
{
    public class EmailHelper
    {
        public static bool SendUserMail(string toName, string toEmail)
        {
            string emailBody = string.Empty;
            // This to supress warnings from razor engine template
            IRazorEngineService razorService = RazorEngineService.Create(new TemplateServiceConfiguration
            {
                DisableTempFileLocking = false,
                CachingProvider = new DefaultCachingProvider(t => { })
            });

            Engine.Razor = razorService;

            var model = new UserWelcomeEmailViewModel { ToName = toName };

            string templatePath = HttpContext.Current.Server.MapPath("~/Content/EmailTemplates/UserEmailTemplate.cshtml");

            using (var reader = new StreamReader(templatePath))
            {
                var template = reader.ReadToEnd();
                Engine.Razor.AddTemplate("UserWelcomeEmailTemplate", template);
                var emailBodyHtml = Engine.Razor.RunCompile("UserWelcomeEmailTemplate", typeof(UserWelcomeEmailViewModel), model);
                emailBody = emailBodyHtml;
            }

            MailMessage mail = new MailMessage(ConfigurationManager.AppSettings["FromMail"], toEmail);
            SmtpClient client = new SmtpClient();
            client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = ConfigurationManager.AppSettings["SmtpHost"];
            mail.Subject = "Welcome to Sambari Enterprises";
            mail.Body = emailBody;
            client.Send(mail);

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

            MailMessage mail = new MailMessage(ConfigurationManager.AppSettings["FromMail"], ConfigurationManager.AppSettings["AdminEmail"]);
            SmtpClient client = new SmtpClient();
            client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = ConfigurationManager.AppSettings["SmtpHost"];
            mail.Subject = "New registration in to Sambari Enterprises";
            mail.Body = emailBody;
            mail.IsBodyHtml = true;
            client.Send(mail);

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

            MailMessage mail = new MailMessage(ConfigurationManager.AppSettings["FromMail"], memberViewModel.Email);
            SmtpClient client = new SmtpClient();
            client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = ConfigurationManager.AppSettings["SmtpHost"];
            mail.Subject = "Sambari Enterprises - Login Details";
            mail.Body = emailBody;
            client.Send(mail);

            return true;
        }
    }
}