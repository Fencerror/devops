using System;

namespace DevOpsPollApp.Entities
{
    public class Vote
    {
        public int Id { get; set; }
        public int PollOptionId { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public PollOption PollOption { get; set; } = null!;
    }
}
