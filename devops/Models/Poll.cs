using System.Collections.Generic;

namespace DevOpsPollApp.Models
{
    public class Poll
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public List<PollOption> Options { get; set; } = new List<PollOption>();
    }
}
