using Microsoft.AspNet.WebHooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Mail;
using System.Net;

namespace WebApplication1
{
    public class GitHubWebHookHandler : WebHookHandler
    {
        public override Task ExecuteAsync(string receiver, WebHookHandlerContext context)
        {
            var serviceName = "GitHub";

            if (serviceName.Equals(receiver, StringComparison.CurrentCultureIgnoreCase))
            {
                string action = context.Actions.First();
                JObject data = context.GetDataOrDefault<JObject>();
                var branch = data.Value<string>("ref").Replace("refs/heads/", String.Empty);

                if (branch.Equals("master"))
                {
                    var modifiedFiles = new List<string>();

                    foreach (var commit in data["commits"])
                    {
                        foreach (var mod in ((JObject)commit)["modified"])
                        {
                            modifiedFiles.Add((string)mod);
                        }
                    }

                    if (modifiedFiles.Contains("f1.txt"))
                    {
                     
                        using (var smtp = new SmtpClient())
                        {
                            smtp.Host = "smtp.yandex.ru";
                            smtp.Port = 465;
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.Credentials = new NetworkCredential("app.t@yandex.ru", "qwerty12345");
                            smtp.EnableSsl = true;

                            using (var mail = new MailMessage("app.t@yandex.ru", "mailvadimprokopchuk@gmail.com"))
                            {
                                mail.Subject = "Subject";
                                mail.Body = "f1.txt has been changed";

                                smtp.Send(mail);
                            }
                        }

                    }
                }
            }

            return Task.FromResult(true);
        }
    }
}