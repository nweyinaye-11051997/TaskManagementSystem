﻿namespace TaskManagementSystem.ViewModel
{
    public class CreateTaskVM
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Priority { get; set; }

        public string Status { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
