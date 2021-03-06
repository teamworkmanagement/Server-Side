using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Email;
using TeamApp.Application.Exceptions;
using TeamApp.Application.Interfaces;
using TeamApp.Domain.Settings;

namespace TeamApp.Infrastructure.Shared.Services
{
    public class EmailService : IEmailService
    {
        public MailSettings _mailSettings { get; }
        public ILogger<EmailService> _logger { get; }

        public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public Task SendAsync(EmailRequest request)
        {
            try
            {
                // create message
                MailMessage mail = new MailMessage("kdsoftverify@gmail.com", request.To, "Xác nhận OTP", request.Body);
                mail.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("smtp.gmail.com");
                client.Host = "smtp.gmail.com";
                client.UseDefaultCredentials = false;
                client.Port = 587;
                client.Credentials = new System.Net.NetworkCredential(_mailSettings.EmailFrom, _mailSettings.SmtpPass);
                client.EnableSsl = true;
                client.Send(mail);

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new ApiException(ex.Message);
            }
            return Task.CompletedTask;
        }

        public async Task SendAsyncAWS(EmailRequest request)
        {
            string FROM = "dung.thpt.hk@gmail.com";
            string FROMNAME = "KDSoftVerify";

            string TO = request.To;
            string SMTP_USERNAME = "AKIAYN6LHKCUUF33WPM2";
            string SMTP_PASSWORD = "BCLkHd89rdOOo/+5QhHBH8L0UdX4dM8rJuf5sdcxqDkJ";

            string CONFIGSET = "ConfigSet";
            string HOST = "email-smtp.ap-southeast-1.amazonaws.com";

            //int PORT = 587;

            int PORT = 25;
            // The subject line of the email
            String SUBJECT = request.Subject;

            // The body of the email
            String BODY = request.Body;

            // Create and build a new MailMessage object
            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;
            message.From = new MailAddress(FROM, FROMNAME);
            message.To.Add(new MailAddress(TO));
            message.Subject = SUBJECT;
            message.Body = BODY;

            /*using (var client = new System.Net.Mail.SmtpClient(HOST, PORT))
            {
                // Pass SMTP credentials
                client.Credentials =
                    new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                // Enable SSL encryption
                client.EnableSsl = true;

                // Try to send the message. Show status in console.
                try
                {
                    Console.WriteLine("Attempting to send email...");
                    client.Send(message);
                    Console.WriteLine("Email sent!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The email was not sent.");
                    Console.WriteLine("Error message: " + ex.Message);
                }
            }*/

            var options = new CredentialProfileOptions
            {
                AccessKey = "AKIAYN6LHKCU4EYT2EMT",
                SecretKey = "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"
            };
            var profile = new CredentialProfile("shared_profile", options);
            profile.Region = RegionEndpoint.APSoutheast1;
            var sharedFile = new SharedCredentialsFile();
            sharedFile.RegisterProfile(profile);


            CredentialProfile basicProfile;
            AWSCredentials awsCredentials;
            if (sharedFile.TryGetProfile("shared_profile", out basicProfile) &&
    AWSCredentialsFactory.TryGetAWSCredentials(basicProfile, sharedFile, out awsCredentials))
            {
                using (var client = new AmazonSimpleEmailServiceClient(awsCredentials, RegionEndpoint.APSoutheast1))
                {
                    var sendRequest = new SendEmailRequest
                    {
                        Source = FROM,
                        Destination = new Destination
                        {
                            ToAddresses =
                            new List<string> { TO }
                        },
                        Message = new Message
                        {
                            Subject = new Content(SUBJECT),
                            Body = new Body
                            {
                                Html = new Content
                                {
                                    Charset = "UTF-8",
                                    Data = BODY
                                },
                                /*Text = new Content
                                {
                                    Charset = "UTF-8",
                                    Data = BODY
                                }*/
                            }
                        },
                        // If you are not using a configuration set, comment
                        // or remove the following line 
                        //ConfigurationSetName = CONFIGSET,
                    };
                    try
                    {
                        Console.WriteLine("Sending email using Amazon SES...");
                        var response = await client.SendEmailAsync(sendRequest);
                        Console.WriteLine("The email was sent successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("The email was not sent.");
                        Console.WriteLine("Error message: " + ex.Message);

                    }
                }
            }


            //return Task.CompletedTask;
        }
    }
}
