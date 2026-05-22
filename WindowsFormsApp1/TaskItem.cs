using System;

namespace TaskManager
{
    public class TaskItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string Priority { get; set; }
        public bool IsDone { get; set; }

        public override string ToString()
        {
            return $"{Title} | {Description} | {Priority} | {Deadline:dd.MM.yyyy}";
        }
    }
}