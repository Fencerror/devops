using DevOpsPollApp.Models;
using System;
using System.Collections.Generic;

namespace DevOpsPollApp.Entities
{
    public class Poll
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public List<PollOption> Options { get; set; } = new();
    }
}
