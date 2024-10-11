namespace MVCTaskManager.Services.Interfaces
{
    public interface IMailService
    {
        public Task SendMail(string userName, string url, string email);
    }
}
