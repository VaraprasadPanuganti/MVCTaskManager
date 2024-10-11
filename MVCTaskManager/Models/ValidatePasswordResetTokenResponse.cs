namespace MVCTaskManager.Models
{
    public class ValidatePasswordResetTokenResponse
    {
        public string? Message { get; set; }
        public bool isvalidToken { get; set; }
        public string? UserId { get; set; }
    }
}
