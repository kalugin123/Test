using Microsoft.AspNet.WebHooks;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading;

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

                    var mailMessage = new MailMessage
                    {
                        Subject = "false",
                        Body = "false"
                    };

                    if (modifiedFiles.Contains("1.txt"))
                    {
                        mailMessage = new MailMessage
                        {
                            Subject = "true",
                            Body = "true"
                        };
                    }

                    mailMessage.To.Add("kalugin123@yandex.ru");

                    var smtpClient = new SmtpClient();

                    try
                    {
                        smtpClient.Send(mailMessage);
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        for (var i = 0; i <= ex.InnerExceptions.Length; i++)
                        {
                            var status = ex.InnerExceptions[i].StatusCode;
                            if ((status == SmtpStatusCode.MailboxBusy) | (status == SmtpStatusCode.MailboxUnavailable))
                            {
                                Thread.Sleep(5000);
                                smtpClient.Send(mailMessage);
                            }
                        }
                    }
                }
            }

            return Task.FromResult(true);
        }
    }
}