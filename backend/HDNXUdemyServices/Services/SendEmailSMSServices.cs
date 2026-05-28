using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyServices.IServices;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace HDNXUdemyServices.Services
{
    public class SendEmailSMSServices : ISendEmailSMSServices
    {
        private readonly ClientSecrets _clientSecretsGmailOAuth;

        public SendEmailSMSServices()
        {
            _clientSecretsGmailOAuth = new ClientSecrets()
            {
                ClientId = ProjectConfig.ClientId,
                ClientSecret = ProjectConfig.ClientSecret,
            };
        }

        public async Task<bool> SendEmailToSingleEmail(string toEmail, string subjectEmail, string bodyEmail)
        {
            try
            {
                var (senderEmail, hostName) = GetEmailConfiguration();
                var refreshToken = new TokenResponse { RefreshToken = ProjectConfig.RefreshToken };
                UserCredential getUserCredential = new UserCredential(
                    new GoogleAuthorizationCodeFlow(
                                new GoogleAuthorizationCodeFlow.Initializer
                                {
                                    ClientSecrets = _clientSecretsGmailOAuth,
                                }), senderEmail, refreshToken);

                if (getUserCredential.Token.IsStale)
                {
                    bool isRefreshEmailToken = await getUserCredential.RefreshTokenAsync(CancellationToken.None);
                    if (!isRefreshEmailToken)
                    {
                        throw new ProjectException(Messenger.LoginWithEmailGoogleNotSuccess);
                    }
                    else
                    {
                        var emailConfig = new MimeMessage
                        {
                            Sender = MailboxAddress.Parse(senderEmail),
                            Subject = subjectEmail,
                        };

                        var builderBody = new BodyBuilder { HtmlBody = bodyEmail };
                        emailConfig.To.Add(MailboxAddress.Parse(toEmail));
                        emailConfig.Body = builderBody.ToMessageBody();
                        using var smtpClient = new SmtpClient();
                        await smtpClient.ConnectAsync(hostName, ProjectConfig.Port, SecureSocketOptions.StartTls);
                        var oauth2 = new SaslMechanismOAuth2(senderEmail, getUserCredential.Token.AccessToken);
                        await smtpClient.AuthenticateAsync(oauth2);
                        await smtpClient.SendAsync(emailConfig);
                        await smtpClient.DisconnectAsync(true);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new ProjectException(ex.InnerException?.Message ?? string.Empty, ex);
            }
        }

        public async Task<bool> SendEmailToListEmail(List<string> toEmail, string subjectEmail, string bodyEmail)
        {
            try
            {
                var (senderEmail, hostName) = GetEmailConfiguration();
                var refreshToken = new TokenResponse { RefreshToken = ProjectConfig.RefreshToken };
                UserCredential getUserCredential = new UserCredential(
                    new GoogleAuthorizationCodeFlow(
                                new GoogleAuthorizationCodeFlow.Initializer
                                {
                                    ClientSecrets = _clientSecretsGmailOAuth
                                }), senderEmail, refreshToken);
                if (getUserCredential.Token.IsStale)
                {
                    bool isRefreshEmailToken = await getUserCredential.RefreshTokenAsync(CancellationToken.None);
                    if (!isRefreshEmailToken)
                    {
                        throw new ProjectException(Messenger.LoginWithEmailGoogleNotSuccess);
                    }
                    else
                    {
                        var emailConfig = new MimeMessage
                        {
                            Sender = MailboxAddress.Parse(senderEmail),
                            Subject = subjectEmail,
                        };

                        var resultData = new List<InternetAddress>();
                        for (int i = 0; i < toEmail.Count; i++)
                        {
                            var resultDataFor = MailboxAddress.Parse(toEmail[i]);
                            resultData.Add(resultDataFor);
                        }

                        var builderBody = new BodyBuilder { HtmlBody = bodyEmail };
                        emailConfig.To.AddRange(resultData);
                        emailConfig.Body = builderBody.ToMessageBody();
                        using var smtpClient = new SmtpClient();
                        await smtpClient.ConnectAsync(hostName, ProjectConfig.Port, SecureSocketOptions.StartTls);
                        var oauth2 = new SaslMechanismOAuth2(senderEmail, getUserCredential.Token.AccessToken);
                        smtpClient.Authenticate(oauth2);
                        await smtpClient.SendAsync(emailConfig);
                        await smtpClient.DisconnectAsync(true);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new ProjectException(ex.InnerException?.Message ?? string.Empty, ex);
            }
        }

        private static (string SenderEmail, string HostName) GetEmailConfiguration()
        {
            if (string.IsNullOrWhiteSpace(ProjectConfig.SenderEmail))
            {
                throw new ProjectException(nameof(ProjectConfig.SenderEmail));
            }

            if (string.IsNullOrWhiteSpace(ProjectConfig.HostName))
            {
                throw new ProjectException(nameof(ProjectConfig.HostName));
            }

            return (ProjectConfig.SenderEmail, ProjectConfig.HostName);
        }
    }
}
