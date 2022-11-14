using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using Hrms.Helper.Models.Dto;
using Hrms.Helper.Settings;
using Hrms.Helper.StaticClasses;

namespace Hrms.Helper.EmailHelper
{
    public static class EmailSender
    {
        private static void SendEmail(EmailDto emailToSend)
        {
            try
            {
                using (var client =
                    new System.Net.Mail.SmtpClient(emailToSend.SmtpServer, Int32.Parse(emailToSend.Port)))
                {
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = false;
                    if (!emailToSend.SmtpServer.Contains("kubota"))
                    {
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential(emailToSend.SmtpUsername, emailToSend.SmtpPassword);
                    }

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(emailToSend.From.Email, emailToSend.From.Name);
                    foreach (var mail in emailToSend.Messages)
                    {
                        foreach (var to in mail.To)
                        {
                            mailMessage.To.Add(new MailAddress(to.Email, to.Name));
                        }

                        if (mail.Bcc != null)
                        {
                            foreach (var bcc in mail.Bcc)
                            {
                                mailMessage.Bcc.Add(new MailAddress(bcc.Email, bcc.Name));
                            }
                        }

                        mailMessage.IsBodyHtml = true;
                        mailMessage.Body = mail.Content;
                        mailMessage.Subject = mail.Subject;

                        try
                        {
                            Console.WriteLine("Attempting to send email...");
                            client.Send(mailMessage);
                            Console.WriteLine("Email sent!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("The email was not sent.");
                            Console.WriteLine("Error message: " + ex.Message);
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SendRegisteredEmail(string name, string emailId, string tempPassword,
            ApplicationSettings settings)
        {
            var emailContent = string.Empty;
            string executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            using (var reader =
                new StreamReader(Path.Combine(executableLocation, "EmailHelper/Templates/Register.html")))
            {
                emailContent = reader.ReadToEnd();
            }

            var emailSubject = "Added on Kubtoa HRMS.";

            emailContent = emailContent
                .Replace("{{name}}", name)
                .Replace("{{tempPassword}}", tempPassword)
                .Replace("{{appUrl}}", settings.AppUrl);

            var emailDto = new EmailDto
            {
                From = new EmailAddress
                {
                    Email = settings.EmailSettings.FromEmailAddress.Email,
                    Name = settings.EmailSettings.FromEmailAddress.Name
                },
                SmtpServer = settings.EmailSettings.SmtpServer,
                SmtpUsername = settings.EmailSettings.Username,
                SmtpPassword = settings.EmailSettings.Password,
                Port = settings.EmailSettings.Port,
                Messages = new List<EmailMessage>
                {
                    new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent,
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = emailId,
                                Name = name
                            }
                        },
                        Bcc = settings.EmailSettings.BccEmailAddresses
                            .Select(var => new EmailAddress {Email = var.Email, Name = var.Name}).ToList()
                    }
                }
            };

            SendEmail(emailDto);
        }

        public static void SendForgotPasswordEmail(string name, string emailId, string tempPassword,
            ApplicationSettings settings)
        {
            var emailContent = string.Empty;
            string executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            using (var reader =
                new StreamReader(Path.Combine(executableLocation, "EmailHelper/Templates/ForgotPassword.html")))
            {
                emailContent = reader.ReadToEnd();
            }

            var emailSubject = "Temporary password request.";

            emailContent = emailContent
                .Replace("{{name}}", name)
                .Replace("{{tempPassword}}", tempPassword)
                .Replace("{{appUrl}}", settings.AppUrl);

            var emailDto = new EmailDto
            {
                From = new EmailAddress
                {
                    Email = settings.EmailSettings.FromEmailAddress.Email,
                    Name = settings.EmailSettings.FromEmailAddress.Name
                },
                SmtpServer = settings.EmailSettings.SmtpServer,
                SmtpUsername = settings.EmailSettings.Username,
                SmtpPassword = settings.EmailSettings.Password,
                Port = settings.EmailSettings.Port,
                Messages = new List<EmailMessage>
                {
                    new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent,
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = emailId,
                                Name = name
                            }
                        },
                        Bcc = settings.EmailSettings.BccEmailAddresses
                            .Select(var => new EmailAddress {Email = var.Email, Name = var.Name}).ToList()
                    }
                }
            };

            SendEmail(emailDto);
        }

        public static void SendOTPPasswordEmail(string name, string emailId, string tempPassword,
            ApplicationSettings settings)
        {
            var emailContent = string.Empty;
            string executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            using (var reader =
                new StreamReader(Path.Combine(executableLocation, "EmailHelper/Templates/OTPPassword.html")))
            {
                emailContent = reader.ReadToEnd();
            }

            var emailSubject = "OTP password request.";

            emailContent = emailContent
                .Replace("{{name}}", name)
                .Replace("{{tempPassword}}", tempPassword)
                .Replace("{{appUrl}}", settings.AppUrl);

            var emailDto = new EmailDto
            {
                From = new EmailAddress
                {
                    Email = settings.EmailSettings.FromEmailAddress.Email,
                    Name = settings.EmailSettings.FromEmailAddress.Name
                },
                SmtpServer = settings.EmailSettings.SmtpServer,
                SmtpUsername = settings.EmailSettings.Username,
                SmtpPassword = settings.EmailSettings.Password,
                Port = settings.EmailSettings.Port,
                Messages = new List<EmailMessage>
                {
                    new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent,
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = emailId,
                                Name = name
                            }
                        },
                        Bcc = settings.EmailSettings.BccEmailAddresses
                            .Select(var => new EmailAddress {Email = var.Email, Name = var.Name}).ToList()
                    }
                }
            };

            SendEmail(emailDto);
        }

        public static void SendSelfAppraisalFilledEmail(string name, string emailId, string managerName,
            ApplicationSettings settings, string appraisalMode)
        {
            var emailContent = string.Empty;
            string executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            using (var reader =
                new StreamReader(Path.Combine(executableLocation, "EmailHelper/Templates/SelfAppraisal.html")))
            {
                emailContent = reader.ReadToEnd();
            }

            var emailSubject = appraisalMode + " submitted.";

            emailContent = emailContent
                .Replace("{{name}}", name)
                .Replace("{{managerName}}", managerName)
                .Replace("{{appUrl}}", settings.AppUrl)
                .Replace("{{appraisalMode}}", appraisalMode);

            var emailDto = new EmailDto
            {
                From = new EmailAddress
                {
                    Email = settings.EmailSettings.FromEmailAddress.Email,
                    Name = settings.EmailSettings.FromEmailAddress.Name
                },
                SmtpServer = settings.EmailSettings.SmtpServer,
                SmtpUsername = settings.EmailSettings.Username,
                SmtpPassword = settings.EmailSettings.Password,
                Port = settings.EmailSettings.Port,
                Messages = new List<EmailMessage>
                {
                    new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent,
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = emailId,
                                Name = name
                            }
                        },
                        Bcc = settings.EmailSettings.BccEmailAddresses
                            .Select(var => new EmailAddress {Email = var.Email, Name = var.Name}).ToList()
                    }
                }
            };

            SendEmail(emailDto);
        }


        public static void SendRmAppraisalFilledEmail(string name, string emailId, string managerName,
            ApplicationSettings settings, string appraisalMode, string l2ManagerName)
        {
            var emailContent = string.Empty;
            string executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            using (var reader =
                new StreamReader(Path.Combine(executableLocation, "EmailHelper/Templates/RmAppraisal.html")))
            {
                emailContent = reader.ReadToEnd();
            }

            var emailSubject = "Manager " + appraisalMode + " completed.";

            emailContent = emailContent
                .Replace("{{name}}", name)
                .Replace("{{managerName}}", managerName)
                .Replace("{{appUrl}}", settings.AppUrl)
                .Replace("{{appraisalMode}}", appraisalMode)
                .Replace("{{l2manager}}", l2ManagerName);

            var emailDto = new EmailDto
            {
                From = new EmailAddress
                {
                    Email = settings.EmailSettings.FromEmailAddress.Email,
                    Name = settings.EmailSettings.FromEmailAddress.Name
                },
                SmtpServer = settings.EmailSettings.SmtpServer,
                SmtpUsername = settings.EmailSettings.Username,
                SmtpPassword = settings.EmailSettings.Password,
                Port = settings.EmailSettings.Port,
                Messages = new List<EmailMessage>
                {
                    new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent,
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = emailId,
                                Name = name
                            }
                        },
                        Bcc = settings.EmailSettings.BccEmailAddresses
                            .Select(var => new EmailAddress {Email = var.Email, Name = var.Name}).ToList()
                    }
                }
            };

            SendEmail(emailDto);
        }


        public static void SendL2AppraisalFilledEmail(string name, string emailId, string l2ManagerName,
            ApplicationSettings settings, string appraisalMode)
        {
            var emailContent = string.Empty;
            string executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            using (var reader =
                new StreamReader(Path.Combine(executableLocation, "EmailHelper/Templates/L2Appraisal.html")))
            {
                emailContent = reader.ReadToEnd();
            }

            var emailSubject = "L2 Manager " + appraisalMode + " completed.";

            emailContent = emailContent
                .Replace("{{name}}", name)
                .Replace("{{appUrl}}", settings.AppUrl)
                .Replace("{{appraisalMode}}", appraisalMode)
                .Replace("{{l2manager}}", l2ManagerName);

            var emailDto = new EmailDto
            {
                From = new EmailAddress
                {
                    Email = settings.EmailSettings.FromEmailAddress.Email,
                    Name = settings.EmailSettings.FromEmailAddress.Name
                },
                SmtpServer = settings.EmailSettings.SmtpServer,
                SmtpUsername = settings.EmailSettings.Username,
                SmtpPassword = settings.EmailSettings.Password,
                Port = settings.EmailSettings.Port,
                Messages = new List<EmailMessage>
                {
                    new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent,
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = emailId,
                                Name = name
                            }
                        },
                        Bcc = settings.EmailSettings.BccEmailAddresses
                            .Select(var => new EmailAddress {Email = var.Email, Name = var.Name}).ToList()
                    }
                }
            };

            SendEmail(emailDto);
        }


        public static void SendHrAppraisalFilledEmail(string name, string emailId,
            ApplicationSettings settings, string appraisalMode)
        {
            var emailContent = string.Empty;
            string executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            using (var reader =
                new StreamReader(Path.Combine(executableLocation, "EmailHelper/Templates/HrAppraisal.html")))
            {
                emailContent = reader.ReadToEnd();
            }

            var emailSubject = "HR " + appraisalMode + " completed. Your rating is set !!";

            emailContent = emailContent
                .Replace("{{name}}", name)
                .Replace("{{appUrl}}", settings.AppUrl)
                .Replace("{{appraisalMode}}", appraisalMode);

            var emailDto = new EmailDto
            {
                From = new EmailAddress
                {
                    Email = settings.EmailSettings.FromEmailAddress.Email,
                    Name = settings.EmailSettings.FromEmailAddress.Name
                },
                SmtpServer = settings.EmailSettings.SmtpServer,
                SmtpUsername = settings.EmailSettings.Username,
                SmtpPassword = settings.EmailSettings.Password,
                Port = settings.EmailSettings.Port,
                Messages = new List<EmailMessage>
                {
                    new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent,
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = emailId,
                                Name = name
                            }
                        },
                        Bcc = settings.EmailSettings.BccEmailAddresses
                            .Select(var => new EmailAddress {Email = var.Email, Name = var.Name}).ToList()
                    }
                }
            };

            SendEmail(emailDto);
        }


        public static void SendHrObjectiveFilledEmail(string name, string emailId,
            ApplicationSettings settings)
        {
            var emailContent = string.Empty;
            string executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            using (var reader =
                new StreamReader(Path.Combine(executableLocation, "EmailHelper/Templates/HrObjective.html")))
            {
                emailContent = reader.ReadToEnd();
            }

            var emailSubject = "HR Objective completed.";

            emailContent = emailContent
                .Replace("{{name}}", name)
                .Replace("{{appUrl}}", settings.AppUrl);

            var emailDto = new EmailDto
            {
                From = new EmailAddress
                {
                    Email = settings.EmailSettings.FromEmailAddress.Email,
                    Name = settings.EmailSettings.FromEmailAddress.Name
                },
                SmtpServer = settings.EmailSettings.SmtpServer,
                SmtpUsername = settings.EmailSettings.Username,
                SmtpPassword = settings.EmailSettings.Password,
                Port = settings.EmailSettings.Port,
                Messages = new List<EmailMessage>
                {
                    new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent,
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = emailId,
                                Name = name
                            }
                        },
                        Bcc = settings.EmailSettings.BccEmailAddresses
                            .Select(var => new EmailAddress {Email = var.Email, Name = var.Name}).ToList()
                    }
                }
            };

            SendEmail(emailDto);
        }


        public static void SendErrorEmail(string errorMessage,
            ApplicationSettings settings)
        {
            var emailContent = string.Empty;
            string executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            using (var reader =
                new StreamReader(Path.Combine(executableLocation, "EmailHelper/Templates/Error.html")))
            {
                emailContent = reader.ReadToEnd();
            }

            var emailSubject = "Error in Kubota HRMS";

            emailContent = emailContent
                .Replace("{{error}}", errorMessage)
                .Replace("{{date}}", DateTime.Now.ToCustomDateTimeFormat(0));

            var emailDto = new EmailDto
            {
                From = new EmailAddress
                {
                    Email = settings.EmailSettings.FromEmailAddress.Email,
                    Name = settings.EmailSettings.FromEmailAddress.Name
                },
                SmtpServer = settings.EmailSettings.SmtpServer,
                SmtpUsername = settings.EmailSettings.Username,
                SmtpPassword = settings.EmailSettings.Password,
                Port = settings.EmailSettings.Port,
                Messages = new List<EmailMessage>
                {
                    new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent,
                        To = settings.ErrorHandlingSettings.ToEmailAddress
                            .Select(var => new EmailAddress {Email = var.Email, Name = var.Name}).ToList(),
                        Bcc = settings.ErrorHandlingSettings.BccEmailAddresses
                            .Select(var => new EmailAddress {Email = var.Email, Name = var.Name}).ToList()
                    }
                }
            };

            SendEmail(emailDto);
        }

        public static void SendTicketCreatedEmail(string name, string emailId, string title, string date, string guid, string category, string createdBy, ApplicationSettings settings)
        {
            var emailContent = string.Empty;
            string executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            using (var reader =
                new StreamReader(Path.Combine(executableLocation, "EmailHelper/Templates/TicketCreated.html")))
            {
                emailContent = reader.ReadToEnd();
            }

            var emailSubject = "New ticket logged under - " + category;

            emailContent = emailContent
                .Replace("{{name}}", name)
                .Replace("{{title}}", title)
                .Replace("{{category}}", category)
                .Replace("{{createdBy}}", createdBy)
                .Replace("{{date}}", date)
                .Replace("{{url}}", string.Concat(settings.AppUrl, "/tickets/", guid));

            var emailDto = new EmailDto
            {
                From = new EmailAddress
                {
                    Email = settings.EmailSettings.FromEmailAddress.Email,
                    Name = settings.EmailSettings.FromEmailAddress.Name
                },
                SmtpServer = settings.EmailSettings.SmtpServer,
                SmtpUsername = settings.EmailSettings.Username,
                SmtpPassword = settings.EmailSettings.Password,
                Port = settings.EmailSettings.Port,
                Messages = new List<EmailMessage>
                {
                    new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent,
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = emailId,
                                Name = name
                            }
                        },
                        Bcc = settings.EmailSettings.BccEmailAddresses
                            .Select(var => new EmailAddress {Email = var.Email, Name = var.Name}).ToList()
                    }
                }
            };

            SendEmail(emailDto);
        }

        public static void SendTrainingConfirmedEmail(string name, string emailId, TrainingEmailDto training,
            ApplicationSettings settings)
        {
            var emailContent = string.Empty;
            string executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            using (var reader =
                new StreamReader(Path.Combine(executableLocation, "EmailHelper/Templates/TrainingConfirmed.html")))
            {
                emailContent = reader.ReadToEnd();
            }

            var emailSubject = "HRMS Training Acceptance";

            emailContent = emailContent
                .Replace("{{name}}", name)
                .Replace("{{trainingName}}", training.TrainingName)
                .Replace("{{trainingDate}}", training.Date)
                .Replace("{{trainingTime}}", training.Time)
                .Replace("{{trainingLocations}}", string.IsNullOrWhiteSpace(training.Locations) ? "All" : training.Locations)
                .Replace("{{trainingMode}}", training.Mode)
                .Replace("{{trainingAcceptDate}}", training.AcceptanceLastDate)
                .Replace("{{trainingDescription}}", training.TrainingDescription);

            var emailDto = new EmailDto
            {
                From = new EmailAddress
                {
                    Email = settings.EmailSettings.FromEmailAddress.Email,
                    Name = settings.EmailSettings.FromEmailAddress.Name
                },
                SmtpServer = settings.EmailSettings.SmtpServer,
                SmtpUsername = settings.EmailSettings.Username,
                SmtpPassword = settings.EmailSettings.Password,
                Port = settings.EmailSettings.Port,
                Messages = new List<EmailMessage>
                {
                    new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent,
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = emailId,
                                Name = name
                            }
                        },
                        Bcc = settings.EmailSettings.BccEmailAddresses
                            .Select(var => new EmailAddress {Email = var.Email, Name = var.Name}).ToList()
                    }
                }
            };

            SendEmail(emailDto);
        }
        public static void SendTrainingConfirmedManagerEmail(string name, string emailId, TrainingEmailDto training,
            ApplicationSettings settings)
        {
            var emailContent = string.Empty;
            string executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            using (var reader =
                new StreamReader(Path.Combine(executableLocation, "EmailHelper/Templates/TrainingConfirmedManager.html")))
            {
                emailContent = reader.ReadToEnd();
            }

            var emailSubject = "HRMS Training Nomination- Approval";

            emailContent = emailContent
                .Replace("{{name}}", name)
                .Replace("{{trainingName}}", training.TrainingName)
                .Replace("{{trainingDate}}", training.Date)
                .Replace("{{trainingTime}}", training.Time)
                .Replace("{{trainingDepartments}}", string.IsNullOrWhiteSpace(training.Departments) ? "All" : training.Departments)
                .Replace("{{trainingDesignations}}", string.IsNullOrWhiteSpace(training.Designations) ? "All" : training.Designations)
                .Replace("{{trainingGrades}}", string.IsNullOrWhiteSpace(training.Grades) ? "All" : training.Grades)
                .Replace("{{trainingLocations}}", string.IsNullOrWhiteSpace(training.Locations) ? "All" : training.Locations)
                .Replace("{{trainingMode}}", training.Mode)
                .Replace("{{trainingAcceptDate}}", training.AcceptanceLastDate)
                .Replace("{{trainingDescription}}", training.TrainingDescription);

            var emailDto = new EmailDto
            {
                From = new EmailAddress
                {
                    Email = settings.EmailSettings.FromEmailAddress.Email,
                    Name = settings.EmailSettings.FromEmailAddress.Name
                },
                SmtpServer = settings.EmailSettings.SmtpServer,
                SmtpUsername = settings.EmailSettings.Username,
                SmtpPassword = settings.EmailSettings.Password,
                Port = settings.EmailSettings.Port,
                Messages = new List<EmailMessage>
                {
                    new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent,
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = emailId,
                                Name = name
                            }
                        },
                        Bcc = settings.EmailSettings.BccEmailAddresses
                            .Select(var => new EmailAddress {Email = var.Email, Name = var.Name}).ToList()
                    }
                }
            };

            SendEmail(emailDto);
        }
        public static void SendTrainingCompletedEmail(string name, string emailId, string trainingTitle,
            ApplicationSettings settings)
        {
            var emailContent = string.Empty;
            string executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            using (var reader =
                new StreamReader(Path.Combine(executableLocation, "EmailHelper/Templates/TrainingCompleted.html")))
            {
                emailContent = reader.ReadToEnd();
            }

            var emailSubject = "Training completed.";

            emailContent = emailContent
                .Replace("{{name}}", name)
                .Replace("{{trainingName}}", trainingTitle);

            var emailDto = new EmailDto
            {
                From = new EmailAddress
                {
                    Email = settings.EmailSettings.FromEmailAddress.Email,
                    Name = settings.EmailSettings.FromEmailAddress.Name
                },
                SmtpServer = settings.EmailSettings.SmtpServer,
                SmtpUsername = settings.EmailSettings.Username,
                SmtpPassword = settings.EmailSettings.Password,
                Port = settings.EmailSettings.Port,
                Messages = new List<EmailMessage>
                {
                    new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent,
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = emailId,
                                Name = name
                            }
                        },
                        Bcc = settings.EmailSettings.BccEmailAddresses
                            .Select(var => new EmailAddress {Email = var.Email, Name = var.Name}).ToList()
                    }
                }
            };

            SendEmail(emailDto);
        }

        public static void SendExitRequestEmail(string employeeName, string managerName, string seniorManagerName, string hrName, string employeeEmailId, string managerEmailId, string seniorManagerEmailId, string hrEmailId, string position, ApplicationSettings settings)
        {
            List<EmailMessage> emailMessages = new List<EmailMessage>();
            StringBuilder emailContent = new StringBuilder();
            var emailSubject = string.Empty;

            // employee mail
            emailSubject = "HRMS Resignation submitted";

            emailContent.AppendLine("<p>Dear " + employeeName + ",</p>");
            emailContent.AppendLine("<p>Your resignation is submitted successfully.</p>");
            if (!String.IsNullOrEmpty(managerName))
            {
                emailContent.AppendLine("<p>It is now pending with -" + managerName + " for processing.</p>");
            }

            emailContent.AppendLine("<p>You will be intimated on the progress of your request.</p>");

            emailMessages.Add(new EmailMessage
            {
                Subject = emailSubject,
                Content = emailContent.ToString(),
                To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = employeeEmailId,
                                Name = employeeName
                            }
                        },
                Bcc = settings.EmailSettings.BccEmailAddresses
                            .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
            });

            // manager email
            if (!String.IsNullOrEmpty(managerEmailId))
            {
                emailSubject = employeeName + " : Resignation requested";
                emailContent.Clear();
                emailContent.AppendLine("<p>Dear " + managerName + ", </p>");
                emailContent.AppendLine("<p>Your reportee, Employee " + employeeName + " have initiated the resignation from the position of " + position + " on " + DateTime.Today.Date + " .</p>");
                emailContent.AppendLine("<p>You need to confirm the resignation in the HRMS Portal on or before " + DateTime.Today.Date.AddDays(7) + " </p>");
                emailContent.AppendLine("<p>You will be intimated on the progress of this request.</p>");

                emailMessages.Add(new EmailMessage
                {
                    Subject = emailSubject,
                    Content = emailContent.ToString(),
                    To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = managerEmailId,
                                Name = managerName
                            }
                        },
                    Bcc = settings.EmailSettings.BccEmailAddresses
                        .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                });
            }

            // senior manager email
            if (!String.IsNullOrEmpty(seniorManagerEmailId))
            {
                emailSubject = employeeName + " : Resignation requested";
                emailContent.Clear();
                emailContent.AppendLine("<p>Hello " + seniorManagerName + ", </p>");
                emailContent.AppendLine("<p>Employee " + employeeName + " reporting to " + managerName + " have initiated the resignation from the position of " + position + " on " + DateTime.Today.Date + "</p>");
                emailContent.AppendLine("<p>You need to Confirm the resignation after Reporting Manager " + managerName + " confirmation, on or before " + DateTime.Today.Date.AddDays(7) + ",</p>");
                emailContent.AppendLine("<p>You will be intimated on the progress of this request.</p>");

                emailMessages.Add(new EmailMessage
                {
                    Subject = emailSubject,
                    Content = emailContent.ToString(),
                    To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = seniorManagerEmailId,
                                Name = seniorManagerName
                            }
                        },
                    Bcc = settings.EmailSettings.BccEmailAddresses
                        .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                });
            }

            //HR Email id
            if (!string.IsNullOrEmpty(hrEmailId))
            {
                emailSubject = employeeName + " : Resignation requested";
                emailContent.Clear();
                emailContent.AppendLine("<p>Dear " + hrName + ",</p >");
                emailContent.AppendLine("<p>Employee " + employeeName + " reporting to " + managerName + " have initiated the resignation from the position of " + position + " on " + DateTime.Today.Date + ".</p>");
                //emailContent.AppendLine("<p>It is now pending with " + managerName + " for processing</p>");
                emailContent.AppendLine("<p>You will be intimated on the progress of this request.</p>");

                string[] multiemail = hrEmailId.Split(',');
                foreach (string hremail in multiemail)
                {
                    emailMessages.Add(new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent.ToString(),
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = hremail,
                                Name = hrName
                            }
                        }
                        //, Bcc = settings.EmailSettings.BccEmailAddresses
                        //    .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                    });
                }
            }

            var emailDto = new EmailDto
            {
                From = new EmailAddress
                {
                    Email = settings.EmailSettings.FromEmailAddress.Email,
                    Name = settings.EmailSettings.FromEmailAddress.Name
                },
                SmtpServer = settings.EmailSettings.SmtpServer,
                SmtpUsername = settings.EmailSettings.Username,
                SmtpPassword = settings.EmailSettings.Password,
                Port = settings.EmailSettings.Port,
                Messages = emailMessages
            };

            SendEmail(emailDto);
        }

        public static void SendExitUpdateEmail(string status, string employeeName, string managerName, string seniorManagerName, string hrName, string employeeEmailId, string managerEmailId, string seniorManagerEmailId, string hrEmailId, string rejectedReason, string rejectedReasonOthers, DateTime resignedDate, string position, string confirmedrelivingdate, ApplicationSettings settings)
        {
            List<EmailMessage> emailMessages = new List<EmailMessage>();
            StringBuilder emailContent = new StringBuilder();
            var emailSubject = string.Empty;


            if (status == "L1-Approved")
            {
                //employee  email
                emailSubject = employeeName + " : Resignation approved by " + managerName;

                emailContent.AppendLine("<p>Dear " + employeeName + ",</p>");
                emailContent.AppendLine("<p>Your resignation dated " + resignedDate + " has been accepted by your reporting manager " + managerName + ".</p>");

                if (!String.IsNullOrEmpty(seniorManagerName))
                {
                    emailContent.AppendLine("<p>It is now pending with - " + seniorManagerName + " for processing.</p>");
                }

                emailContent.AppendLine("<p>You will be intimated on the progress of your request.</p>");

                emailMessages.Add(new EmailMessage
                {
                    Subject = emailSubject,
                    Content = emailContent.ToString(),
                    To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = employeeEmailId,
                                Name = employeeName
                            }
                        }
                    //,
                    //Bcc = settings.EmailSettings.BccEmailAddresses
                    //   .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                });

                //L2 email

                if (!String.IsNullOrEmpty(seniorManagerEmailId))
                {
                    emailSubject = employeeName + " : Resignation approved by " + managerName;
                    emailContent.Clear();
                    emailContent.AppendLine("<p>Dear " + seniorManagerName + ",</p>");
                    emailContent.AppendLine("<p>Resignation of employee " + employeeName + " is accepted by the reporting manager " + managerName + " on " + DateTime.Today.Date + ".</p>");
                    emailContent.AppendLine("<p>Please review and process the request on or before " + resignedDate.Date.AddDays(7) + "</p>");
                    emailContent.AppendLine("<p>You will be intimated on the progress of this request.</p>");

                    emailMessages.Add(new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent.ToString(),
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = seniorManagerEmailId,
                                Name = seniorManagerName
                            }
                        }
                        //,
                        //Bcc = settings.EmailSettings.BccEmailAddresses
                        //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                    });
                }
                //HR Email
                if (!String.IsNullOrEmpty(hrEmailId))
                {
                    emailSubject = employeeName + " : Resignation approved by " + managerName;
                    emailContent.Clear();
                    emailContent.AppendLine("<p>Dear " + hrName + ",</p>");
                    emailContent.AppendLine("<p>Resignation of employee " + employeeName + " is accepted by the reporting manager " + managerName + " on " + DateTime.Today.Date + ".</p>");
                    emailContent.AppendLine("<p>You will be intimated on the progress of this request.</p>");

                    emailMessages.Add(new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent.ToString(),
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = hrEmailId,
                                Name = hrName
                            }
                        }
                        //,
                        //Bcc = settings.EmailSettings.BccEmailAddresses
                        //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                    });
                }
            }
            else if (status == "L2-Approved")
            {
                emailSubject = employeeName + " : Resignation approved by " + seniorManagerName;

                emailContent.AppendLine("<p>Dear " + employeeName + ", </p> ");
                emailContent.AppendLine("<p>Your resignation dated " + resignedDate + " has been accepted by your L2 manager " + seniorManagerName + ".</p>");
                emailContent.AppendLine("<p>It is now pending with HR for processing.</p>");
                emailContent.AppendLine("<p>You will be intimated on the progress of your request.</p>");

                emailMessages.Add(new EmailMessage
                {
                    Subject = emailSubject,
                    Content = emailContent.ToString(),
                    To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = employeeEmailId,
                                Name = employeeName
                            }
                        }
                    //,
                    //Bcc = settings.EmailSettings.BccEmailAddresses
                    //   .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                });

                //L1 email
                if (!String.IsNullOrEmpty(managerEmailId))
                {
                    emailSubject = employeeName + " : Resignation approved by " + seniorManagerName;
                    emailContent.Clear();
                    emailContent.AppendLine("<p>Dear " + managerName + ", </p>");
                    emailContent.AppendLine("<p>resignation of employee " + employeeName + " reporting to you has been approved by " + seniorManagerName + ".</p>");
                    emailContent.AppendLine("<p>It is now pending with HR for processing.</p>");
                    emailContent.AppendLine("<p>You will be intimated on the progress of this request.</p>");

                    emailMessages.Add(new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent.ToString(),
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = managerEmailId,
                                Name = managerName
                            }
                        }
                        //,
                        //Bcc = settings.EmailSettings.BccEmailAddresses
                        //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                    });
                }

                //HR Email
                if (!String.IsNullOrEmpty(hrEmailId))
                {
                    emailSubject = employeeName + " : Resignation approved by " + seniorManagerName;
                    emailContent.Clear();
                    emailContent.AppendLine("<p>Dear " + hrName + ",</p>");
                    emailContent.AppendLine("<p>Resignation of employee " + employeeName + " is accepted by the L2 manager " + seniorManagerName + " on " + DateTime.Today.Date + ".</p>");
                    emailContent.AppendLine("<p>Please review and process the request on or before " + resignedDate.Date.AddDays(7) + "</p>");
                    emailContent.AppendLine("<p>You will be intimated on the progress of this request.</p>");

                    string[] multiemail = hrEmailId.Split(',');
                    foreach (string hremail in multiemail)
                    {
                        emailMessages.Add(new EmailMessage
                        {
                            Subject = emailSubject,
                            Content = emailContent.ToString(),
                            To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = hremail,
                                Name = hrName
                            }
                        }
                            //,
                            //Bcc = settings.EmailSettings.BccEmailAddresses
                            //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                        });
                    }
                }
            }
            else if (status == "HR-Approved")
            {
                emailSubject = employeeName + " : Resignation approved by " + hrName;

                emailContent.AppendLine("<p>Dear " + employeeName + ",</p>");
                emailContent.AppendLine("<p>This is in further to your resignation letter dated " + confirmedrelivingdate + " you will be relieved from your position of " + position + " with effect from " + resignedDate + ".</p>");
                emailContent.AppendLine("<p>You will be further intimated, once clearance is initiated by HR.</p>");

                emailMessages.Add(new EmailMessage
                {
                    Subject = emailSubject,
                    Content = emailContent.ToString(),
                    To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = employeeEmailId,
                                Name = employeeName
                            }
                        }
                    //,
                    //Bcc = settings.EmailSettings.BccEmailAddresses
                    //   .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                });

                //L1 email
                if (!String.IsNullOrEmpty(managerEmailId))
                {
                    emailSubject = employeeName + " : Resignation approved by " + hrName;
                    emailContent.Clear();
                    emailContent.AppendLine("<p>Dear " + managerName + ", </p>");
                    emailContent.AppendLine("<p>Further to your resignation acceptance for employee " + employeeName + ", his/her last working day will be " + confirmedrelivingdate + ".</p>");
                    emailContent.AppendLine("<p>You will be further intimated, once clearance is initiated by HR.</p>");

                    emailMessages.Add(new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent.ToString(),
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = managerEmailId,
                                Name = managerName
                            }
                        }
                        //,
                        //Bcc = settings.EmailSettings.BccEmailAddresses
                        //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                    });
                }

                //L2 email

                if (!String.IsNullOrEmpty(seniorManagerEmailId))
                {
                    emailSubject = employeeName + " : Resignation approved by" + hrName;
                    emailContent.Clear();
                    emailContent.AppendLine("<p>Dear " + seniorManagerName + ",</p>");
                    emailContent.AppendLine("<p>The last working day of employee " + employeeName + ", will be " + confirmedrelivingdate + ".</p>");

                    emailMessages.Add(new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent.ToString(),
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = seniorManagerEmailId,
                                Name = seniorManagerName
                            }
                        }
                        //,
                        //Bcc = settings.EmailSettings.BccEmailAddresses
                        //   .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                    });
                }
            }

            if (status == "L1-Rejected")
            {
                emailSubject = employeeName + " : Resignation rejected by " + managerName;
                emailContent.AppendLine("<p>Dear " + employeeName + ",</p>");
                emailContent.AppendLine("<p>Your resignation has been rejected by your Reporting Manager " + managerName + ".</p>");

                emailMessages.Add(new EmailMessage
                {
                    Subject = emailSubject,
                    Content = emailContent.ToString(),
                    To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = employeeEmailId,
                                Name = employeeName
                            }
                        }
                    //,
                    //Bcc = settings.EmailSettings.BccEmailAddresses
                    //   .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                });

                //L1 email

                //if (!String.IsNullOrEmpty(managerEmailId))
                //{
                //    emailSubject = employeeName + " : Resignation rejected by " + managerName;
                //    emailContent.Clear();
                //    emailContent.AppendLine("<p>Dear " + managerName + ",</p>");
                //    emailContent.AppendLine("<p>The Withdrawal of resignation by Employee " + employeeName + " has been accepted by HR.</p>");

                //    emailMessages.Add(new EmailMessage
                //    {
                //        Subject = emailSubject,
                //        Content = emailContent.ToString(),
                //        To = new List<EmailAddress>
                //        {
                //            new EmailAddress
                //            {
                //                Email = managerEmailId,
                //                Name = managerName
                //            }
                //        }
                //        //,
                //        //Bcc = settings.EmailSettings.BccEmailAddresses
                //          //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                //    });
                //}
                //L2 email

                if (!String.IsNullOrEmpty(seniorManagerEmailId))
                {
                    emailSubject = employeeName + " : Resignation rejected by " + managerName;
                    emailContent.Clear();
                    emailContent.AppendLine("<p>Dear " + seniorManagerName + ",</p>");
                    emailContent.AppendLine("<p>Based on our discussions, we have revoked the resignation acceptance given to Employe name " + employeeName + " wef " + DateTime.Today.Date + " .</p>");

                    emailMessages.Add(new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent.ToString(),
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = seniorManagerEmailId,
                                Name = seniorManagerName
                            }
                        }
                        //,
                        //Bcc = settings.EmailSettings.BccEmailAddresses
                        //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                    });
                }

                //HR Email
                if (!String.IsNullOrEmpty(hrEmailId))
                {
                    emailSubject = employeeName + " : Resignation rejected by " + managerName;
                    emailContent.Clear();
                    emailContent.AppendLine("<p>Dear " + hrName + ",</p>");
                    emailContent.AppendLine("<p>The resignation of Employee " + employeeName + " is rejected by Reporting Manager " + managerName + " .</p>");

                    emailMessages.Add(new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent.ToString(),
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = seniorManagerEmailId,
                                Name = seniorManagerName
                            }
                        }
                        //,
                        //Bcc = settings.EmailSettings.BccEmailAddresses
                        //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                    });
                }
            }
            else if (status == "L2-Rejected")
            {
                emailSubject = employeeName + " : Resignation rejected by " + seniorManagerName;
                emailContent.AppendLine("<p>Dear " + employeeName + ",</p>");
                emailContent.AppendLine("<p>Your resignation has been rejected by " + seniorManagerName + ".</p>");
                emailMessages.Add(new EmailMessage
                {
                    Subject = emailSubject,
                    Content = emailContent.ToString(),
                    To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = employeeEmailId,
                                Name = employeeName
                            }
                        }
                    //,
                    //Bcc = settings.EmailSettings.BccEmailAddresses
                    //   .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                });

                //L1 email
                if (!String.IsNullOrEmpty(managerEmailId))
                {
                    emailSubject = employeeName + " : Resignation rejected by " + seniorManagerName;
                    emailContent.Clear();
                    emailContent.AppendLine("<p>Dear " + managerName + ",</p>");
                    emailContent.AppendLine("<p>Based on our discussions, we have revoked the resignation acceptance given to Employee " + employeeName + " wef " + DateTime.Today.Date + ".</p>");
                    emailMessages.Add(new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent.ToString(),
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = managerEmailId,
                                Name = managerName
                            }
                        }
                        //,
                        //Bcc = settings.EmailSettings.BccEmailAddresses
                        //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                    });
                }
                //L2 email

                //if (!String.IsNullOrEmpty(seniorManagerEmailId))
                //{
                //    emailSubject = employeeName + " : Resignation rejected by " + seniorManagerName;
                //    emailContent.Clear();
                //    emailContent.AppendLine("<p>Dear " + seniorManagerName + ",</p>");
                //    emailContent.AppendLine("<p>The Withdrawal of resignation by Employee " + employeeName + " has been accepted by HR </p>");

                //    emailMessages.Add(new EmailMessage
                //    {
                //        Subject = emailSubject,
                //        Content = emailContent.ToString(),
                //        To = new List<EmailAddress>
                //        {
                //            new EmailAddress
                //            {
                //                Email = seniorManagerEmailId,
                //                Name = seniorManagerName
                //            }
                //        }
                //        //,
                //        //Bcc = settings.EmailSettings.BccEmailAddresses
                //          //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                //    });
                //}

                //HR Email
                if (!String.IsNullOrEmpty(hrEmailId))
                {
                    emailSubject = employeeName + " : Resignation rejected by " + seniorManagerName;
                    emailContent.Clear();
                    emailContent.AppendLine("<p>Dear " + hrName + ",</p>");
                    emailContent.AppendLine("<p>The resignation of Employee " + employeeName + " is rejected by L2 Manager " + seniorManagerName + " .</p>");

                    emailMessages.Add(new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent.ToString(),
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = seniorManagerEmailId,
                                Name = seniorManagerName
                            }
                        }
                        //,
                        //Bcc = settings.EmailSettings.BccEmailAddresses
                        //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                    });
                }
            }
            else if (status == "HR-Rejected")
            {
                emailSubject = employeeName + " : Resignation rejected by " + hrName;
                emailContent.AppendLine("<p>Dear " + employeeName + ",</p>");
                emailContent.AppendLine("<p>Thank You for reconsidering your resignation.</p>");
                emailContent.AppendLine("<p>We hereby accepting your Withdrawal of Resignation dated - " + resignedDate + "</p>");
                emailContent.AppendLine("<p>We look forward your continued contribution for the growth of the company </p>");
                emailContent.AppendLine("<p><b>Wishing you all the Best</b></p>");
                emailMessages.Add(new EmailMessage
                {
                    Subject = emailSubject,
                    Content = emailContent.ToString(),
                    To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = employeeEmailId,
                                Name = employeeName
                            }
                        }
                    //,
                    //Bcc = settings.EmailSettings.BccEmailAddresses
                    //   .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                });

                //L1 email
                if (!String.IsNullOrEmpty(managerEmailId))
                {
                    emailSubject = employeeName + " : Resignation rejected by " + hrName;
                    emailContent.Clear();
                    emailContent.AppendLine("<p>Dear " + managerName + ",</p>");
                    emailContent.AppendLine("<p>The Withdrawal of resignation by Employee " + employeeName + " has been accepted by HR</p>");
                    emailMessages.Add(new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent.ToString(),
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = managerEmailId,
                                Name = managerName
                            }
                        }
                        //,
                        //Bcc = settings.EmailSettings.BccEmailAddresses
                        //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                    });
                }

                //L2 email

                if (!String.IsNullOrEmpty(seniorManagerEmailId))
                {
                    emailSubject = employeeName + " : Resignation rejected by" + hrName;
                    emailContent.Clear();
                    emailContent.AppendLine("<p>Dear " + seniorManagerName + ",</p>");
                    emailContent.AppendLine("<p>Based on our discussions, we have revoked the resignation acceptance given to Employe name " + employeeName + " wef " + DateTime.Today.Date + ".</p>");
                    emailMessages.Add(new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent.ToString(),
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = seniorManagerEmailId,
                                Name = seniorManagerName
                            }
                        }
                        //,
                        //Bcc = settings.EmailSettings.BccEmailAddresses
                        //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                    });
                }
            }
            else if (status == "Exit-Processing")
            {
                emailSubject = employeeName + " - Exit Clearance form initiated by " + hrName;

                emailContent.AppendLine("<p>Dear " + employeeName + ", </p>");
                emailContent.AppendLine("<p>We have enabled your Exit Clearance, Please return all KAI property on or before your last day of work," + confirmedrelivingdate + "</p>");
                emailContent.AppendLine("<p>As part of your Exit Clearance, you are hereby informed to fill the Exit feedback Form on or before " + confirmedrelivingdate + "</p>");

                emailMessages.Add(new EmailMessage
                {
                    Subject = emailSubject,
                    Content = emailContent.ToString(),
                    To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = employeeEmailId,
                                Name = employeeName
                            }
                        }
                    //,
                    //Bcc = settings.EmailSettings.BccEmailAddresses
                    //   .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                });

                //L1 email
                if (!String.IsNullOrEmpty(managerEmailId))
                {
                    emailSubject = employeeName + " - Exit Clearance form initiated by " + hrName;

                    emailContent.AppendLine("<p>Dear " + managerName + ", </p>");
                    emailContent.Clear();
                    emailContent.AppendLine("<p>We have enabled the Exit Clearance for Employee " + employeeName + " You may clear the formalities as per the Policy and update asset clearance section in the HRMS on or before" + confirmedrelivingdate + "</p>");
                    emailContent.AppendLine("<p>As part of Exit Clearance process, you are hereby informed to fill the RM feedback Form for " + employeeName + " on or before " + confirmedrelivingdate + "</p>");


                    emailMessages.Add(new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent.ToString(),
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = managerEmailId,
                                Name = managerName
                            }
                        }
                        //,
                        //Bcc = settings.EmailSettings.BccEmailAddresses
                        //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                    });
                }

                //L2 email

                if (!String.IsNullOrEmpty(seniorManagerEmailId))
                {
                    emailSubject = employeeName + " - Exit Clearance form initiated by " + hrName;

                    emailContent.AppendLine("<p>Dear " + seniorManagerName + ", </p>");
                    emailContent.Clear();
                    emailContent.AppendLine("<p>We have enabled the Exit Clearance for Employee " + employeeName + " You may clear the formalities as per the Policy and update asset clearance section in the HRMS on or before" + confirmedrelivingdate + "</p>");
                    emailContent.AppendLine("<p>As part of Exit Clearance process, you are hereby informed to fill the L2 feedback Form for " + employeeName + " on or before " + confirmedrelivingdate + "</p>");


                    emailMessages.Add(new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent.ToString(),
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = seniorManagerEmailId,
                                Name = seniorManagerName
                            }
                        }
                        //,
                        //Bcc = settings.EmailSettings.BccEmailAddresses
                        //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                    });
                }
                //HR email

                if (!String.IsNullOrEmpty(hrEmailId))
                {
                    emailSubject = employeeName + " - Exit Clearance form initiated by " + hrName;

                    emailContent.AppendLine("<p>Dear " + hrName + ", </p>");
                    emailContent.Clear();
                    emailContent.AppendLine("<p>Exit Interview for Employee " + employeeName + " is due on " + confirmedrelivingdate + "</p>");


                    emailMessages.Add(new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent.ToString(),
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = hrEmailId,
                                Name = hrName
                            }
                        }
                        //,
                        //Bcc = settings.EmailSettings.BccEmailAddresses
                        //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                    });
                }
            }
            else if (status == "Completed")
            {
                emailSubject = employeeName + " : Resignation clearance completed by " + hrName;

                emailContent.AppendLine("<p>Hello " + employeeName + ", Your resignation clearance has been completed by " + hrName + ".</p>");

                emailMessages.Add(new EmailMessage
                {
                    Subject = emailSubject,
                    Content = emailContent.ToString(),
                    To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = employeeEmailId,
                                Name = employeeName
                            }
                        }
                    //,
                    //Bcc = settings.EmailSettings.BccEmailAddresses
                    //   .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                });

                //L1 email
                //if (!String.IsNullOrEmpty(managerEmailId))
                //{
                //    emailSubject = employeeName + " : Resignation clearance completed by " + hrName;
                //    emailContent.Clear();
                //    emailContent.AppendLine("<p>Hello " + managerName + ",  resignation clearance of employee " + employeeName + " reporting to you has been completed by " + hrName + ".</p>");

                //    emailMessages.Add(new EmailMessage
                //    {
                //        Subject = emailSubject,
                //        Content = emailContent.ToString(),
                //        To = new List<EmailAddress>
                //        {
                //            new EmailAddress
                //            {
                //                Email = managerEmailId,
                //                Name = managerName
                //            }
                //        }
                //        //,
                //        //Bcc = settings.EmailSettings.BccEmailAddresses
                //          //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                //    });
                //}

                //L2 email

                //if (!String.IsNullOrEmpty(seniorManagerEmailId))
                //{
                //    emailSubject = employeeName + " : Resignation clearance completed by " + hrName;
                //    emailContent.Clear();
                //    emailContent.AppendLine("<p>Hello " + seniorManagerName + ", resignation clearance of employee " + employeeName + " reporting to " + managerName + " has been completed by " + hrName + ".</p>");

                //    emailMessages.Add(new EmailMessage
                //    {
                //        Subject = emailSubject,
                //        Content = emailContent.ToString(),
                //        To = new List<EmailAddress>
                //        {
                //            new EmailAddress
                //            {
                //                Email = seniorManagerEmailId,
                //                Name = seniorManagerName
                //            }
                //        }
                //        //,
                //        //Bcc = settings.EmailSettings.BccEmailAddresses
                //          //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                //    });
                //}
                //HR Email
                if (!String.IsNullOrEmpty(hrEmailId))
                {
                    emailSubject = employeeName + " : Resignation clearance completed by " + hrName;
                    emailContent.Clear();
                    emailContent.AppendLine("<p>Hello " + hrName + ", resignation clearance of employee " + employeeName + " reporting to " + managerName + " has been completed by " + hrName + ".</p>");

                    emailMessages.Add(new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent.ToString(),
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = hrEmailId,
                                Name = hrName
                            }
                        }
                        //,
                        //Bcc = settings.EmailSettings.BccEmailAddresses
                        //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                    });
                }
            }

            else if (status == "Revoked")
            {
                emailSubject = employeeName + " : Resignation revoked by " + hrName;
                emailContent.AppendLine("<p>Thank You for reconsidering your resignation.</p>");
                emailContent.AppendLine("<p>We hereby accepting your Withdrawal of Resignation dated - " + resignedDate + "</p>");
                emailContent.AppendLine("<p>We look forward your continued contribution for the growth of the company </p>");
                emailContent.AppendLine("<p><b>Wishing you all the Best</b></p>");

                emailMessages.Add(new EmailMessage
                {
                    Subject = emailSubject,
                    Content = emailContent.ToString(),
                    To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = employeeEmailId,
                                Name = employeeName
                            }
                        }
                    //,
                    //Bcc = settings.EmailSettings.BccEmailAddresses
                    //   .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                });

                //L1 email
                if (!String.IsNullOrEmpty(managerEmailId))
                {
                    emailSubject = employeeName + " : Resignation revoked by " + hrName;
                    emailContent.Clear();
                    emailContent.AppendLine("<p>Dear " + managerName + ",</p>");
                    emailContent.AppendLine("<p>The Withdrawal of resignation by Employee " + employeeName + " has been accepted by HR.</p>");
                    emailMessages.Add(new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent.ToString(),
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = managerEmailId,
                                Name = managerName
                            }
                        }
                        //,
                        //Bcc = settings.EmailSettings.BccEmailAddresses
                        //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                    });
                }

                //L2 email

                if (!String.IsNullOrEmpty(seniorManagerEmailId))
                {
                    emailSubject = employeeName + " : Resignation revoked by" + hrName;
                    emailContent.Clear();
                    emailContent.AppendLine("<p>Dear " + seniorManagerName + ",</p>");
                    emailContent.AppendLine("<p>Based on our discussions, we have revoked the resignation acceptance given to Employe name " + employeeName + " wef " + DateTime.Today.Date + ".</p>");
                    emailMessages.Add(new EmailMessage
                    {
                        Subject = emailSubject,
                        Content = emailContent.ToString(),
                        To = new List<EmailAddress>
                        {
                            new EmailAddress
                            {
                                Email = seniorManagerEmailId,
                                Name = seniorManagerName
                            }
                        }
                        //,
                        //Bcc = settings.EmailSettings.BccEmailAddresses
                        //  .Select(var => new EmailAddress { Email = var.Email, Name = var.Name }).ToList()
                    });
                }
            }



            var emailDto = new EmailDto
            {
                From = new EmailAddress
                {
                    Email = settings.EmailSettings.FromEmailAddress.Email,
                    Name = settings.EmailSettings.FromEmailAddress.Name
                },
                SmtpServer = settings.EmailSettings.SmtpServer,
                SmtpUsername = settings.EmailSettings.Username,
                SmtpPassword = settings.EmailSettings.Password,
                Port = settings.EmailSettings.Port,
                Messages = emailMessages
            };

            SendEmail(emailDto);
        }
    }
}