using System.ComponentModel.DataAnnotations;

namespace MVCTaskManager.Models
{
    public class Project
    {
        [Key]        
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public DateTime DateOfStart { get; set; }
        public int  TeamSize { get; set; }
        public bool Active { get; set; }
        public string? Status { get; set; }
        public string? ClientLocation { get; set; }
        public string? ClientLocationId { get; set; }
    }
}
