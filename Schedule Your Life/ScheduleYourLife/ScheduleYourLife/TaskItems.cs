using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleYourLife
{
    public enum Quadrant
    {
        Q1_doFirst,
        Q2_schedule,
        Q3_delegate,
        Q4_delete
    }
    public class TaskItems
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Quadrant AssignedQuadrant { get; set; } = Quadrant.Q2_schedule;

        public string? AiReasoning { get; set; }
    }
}
