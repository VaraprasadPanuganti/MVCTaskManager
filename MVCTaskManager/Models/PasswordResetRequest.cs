namespace MVCTaskManager.Models
{
    public class PasswordResetRequest
    {
        public string? EmailId { get; set; }
        public string? Url { get; set; }
    }
}
