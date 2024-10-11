namespace MVCTaskManager.Models
{
    public class LoginResponse
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? UserId { get; set; }
        public string? Role { get; set; }
        public string? token { get; set; }
        public string? SecurityStamp { get; set; }
    }
}
