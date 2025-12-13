using System;

namespace DevOpsPollApp.Models
{
    public class Vote
    {
        public string UserName { get; set; } = string.Empty;
        public int PollId { get; set; }
        public int OptionId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
