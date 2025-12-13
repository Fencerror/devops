using DevOpsPollApp.Models;
using System.Collections.Generic;

namespace DevOpsPollApp.Entities
{
    public class PollOption
    {
        public int Id { get; set; }
        public int PollId { get; set; }
        public string Text { get; set; } = string.Empty;

        public Poll Poll { get; set; } = null!;
        public List<Vote> Votes { get; set; } = new();
    }
}
