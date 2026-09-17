using MiniWalletSystemApi.Interfaces.infrastructure.Email;

namespace MiniWalletSystemApi.Infrastructure.Email;

public sealed class EmailTemplate: IEmailTemplate
{
    private readonly IConfiguration _config;
    
    public EmailTemplate( IConfiguration configuration) 
        { 
            _config = configuration;
        }

        public (string emailSubject, string emailBody) BuildVerificationEmailContent(string? name, string? email, string token, string verificationLink)
        {

            var subject = "Welcome! Your Account Has Been Created";

            var body = $@"
                        <p>Hi {name ?? string.Empty},</p>

                        <p>We’re happy to inform you that your account has been successfully created by our admin team.</p>

                        <p>To activate your account and set your password, please click the button below:</p>

                        <p style='text-align: center;'>
                          <a href='{verificationLink}' style='display: inline-block; padding: 10px 20px; background-color: #28a745; color: white; text-decoration: none; border-radius: 5px;'>Set Your Password</a>
                        </p>

                        <p>If the button above doesn’t work, copy and paste this link into your browser:</p>
                        <p style='word-break: break-all;'>{verificationLink}</p><br/><br/>



                        <p>If you did not expect this email, please ignore it or contact our support team.</p>

                        <p>Need more help? Feel free to contact our support team.</p><br/><br/>

                        <p>Thank you<br/></p>
                        ";

            return (subject , body);
        }

        public (string emailSubject, string emailBody) BuildLoginOtpEmailContent(string otpCode, int expiredIn)
        {
            var subject = "Your OTP Code";

            var body = $@"
                            <p>This email is to provide the login code required to access the site.</p>
                            <p>Your one-time pin (OTP) is: <strong>&gt;&gt; {otpCode} &lt;&lt;</strong>. It will expire in {expiredIn} minute(s).</p>
                            <p>Please do not share it with anyone.<br/>
                            If you didn’t request this, please ignore this email or contact our support team immediately.</p>
                            <p>Stay secure,</p>";

            return (subject, body);
        }

        public (string emailSubject, string emailBody) BuildResetEmailContent(string? name, string? email, string token, string verificationLink, int expiredIn)
        {

            var subject = "Password Reset";

            var body = $@"
                        <p>Hi {name ?? string.Empty},</p>

                        
                        <p>We received a request to reset the password associated with your account. If you made this request, please click the button below to reset your password. This link is only valid for the next <strong>{expiredIn} minutes</strong>.</p>

                        <p style='text-align: center;'>
                          <a href='{verificationLink}' style='display: inline-block; padding: 10px 20px; background-color: #28a745; color: white; text-decoration: none; border-radius: 5px;'>Set Your Password</a>
                        </p>

                        <p>If the button above doesn’t work, copy and paste the following link into your browser:</p>
                        <p style='word-break: break-all;'>{verificationLink}</p><br/><br/>


                        <p><strong>For your security:</strong></p>
                        <ul>
                          <li>Do not share this link with anyone.</li>
                          <li>If you did not request a password reset, you can safely ignore this email. No changes will be made to your account.</li>
                          <li>This link will expire in {expiredIn} minutes to protect your account.</li>
                        </ul>

                        <p>Need more help? Feel free to contact our support team.</p>

                        <p>Thanks,<br/></p>
                        ";

            return (subject, body);
        }

}