using System.Collections.Generic;

namespace Hrms.Helper.Settings
{
    public class ApplicationSettings
    {
        public string AppUrl { get; set; }
        public string Key { get; set; }
        public string ApiKeyFileName { get; set; }
        public string ProcessorFunction { get; set; }
        public string LogPath { get; set; }
        public string ResetPasswordPath { get; set; }
        public JwtSettings JwtSettings { get; set; }
        public EmailSettings EmailSettings { get; set; }
        public FileUploadSettings FileUploadSettings { get; set; }
        public ErrorHandlingSettings ErrorHandlingSettings { get; set; }
        public string CaptchaSecretKey { get; set; }
    }

    public class ErrorHandlingSettings
    {
        public string EmailTemplate { get; set; }
        public string SlackWebhook { get; set; }
        public List<EmailAddress> ToEmailAddress { get; set; }
        public List<EmailAddress> BccEmailAddresses { get; set; }
    }

    public class FileUploadSettings
    {
        public string BasePath { get; set; }
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public string Username { get; set; }    
        public string Password { get; set; }
        public string Port { get; set; }
        public EmailAddress FromEmailAddress { get; set; }
        public List<EmailAddress> BccEmailAddresses { get; set; }
    }

    public class EmailAddress
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string SigningKey { get; set; }
        public string EncryptionKey { get; set; }
        public string Audience { get; set; }
    }
}
