namespace Ticketer.Api
{
    public class EmailSender : IEmailSender
    {
        /// <inheritdoc/>
        public Task SendEmailAsync(string to, string subject, string htmlBody) => Task.CompletedTask;
    }
}
