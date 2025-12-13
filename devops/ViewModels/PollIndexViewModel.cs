using System;
using System.Collections.Generic;

namespace DevOpsPollApp.ViewModels
{
    public class PollIndexViewModel
    {
        public List<PollListItemViewModel> Polls { get; set; } = new();
    }

    public class PollListItemViewModel
    {
        public Guid PublicId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public int TotalVotes { get; set; }
    }
}
