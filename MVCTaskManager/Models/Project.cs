﻿using System.ComponentModel.DataAnnotations;

namespace MVCTaskManager.Models
{
    public class Project
    {
        [Key]        
        public int ProjectId { get; set; }
        public string? PorjectName { get; set; }
        public DateTime DateOfStart { get; set; }
        public int  TeamSize { get; set; }
    }
}
