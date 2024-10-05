namespace MVCTaskManager.Models
{
    public class AddUserRequest
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? password { get; set; }
        public string? Role { get; set; }
    }
}
