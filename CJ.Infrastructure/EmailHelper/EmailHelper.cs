using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace CJ.Infrastructure.EmailHelper
{
    public class EmailHelper
    {
        private readonly static string SmtpServer = "smtp.163.com";//smtp服务器
        private readonly static int SmtpServerPort = 465;
        private readonly static bool SmtpEnableSsl = true;
        private readonly static string SmtpUsername = "xucanjie1071@163.com";//发件人邮箱地址
        private readonly static string SmtpDisplayName = "xucanjie";//发件人昵称
        private readonly static string SmtpUserPassword = "xucanjie88";//授权码

        /// <summary>
        /// 发送邮件到指定收件人
        /// </summary>
        /// <param name="to">收件人地址</param>
        /// <param name="subject">主题</param>
        /// <param name="mailBody">正文内容（支持HTML）</param>
        /// <returns>是否发送成功</returns>
        public static void Send(string to, string subject, string mailBody)
        {

            var message = new MimeMessage();

            //  发件人
            message.From.Add(new MailboxAddress(SmtpDisplayName, SmtpUsername));

            //  收件人
            message.To.Add(new MailboxAddress(to));

            //  邮件主题
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                //  邮件内容
                Text = $@"{mailBody}"
            };

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(SmtpServer, SmtpServerPort, SmtpEnableSsl);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(SmtpUsername, SmtpUserPassword);

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
