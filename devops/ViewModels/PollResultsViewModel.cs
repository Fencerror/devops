using System;
using System.Collections.Generic;

namespace DevOpsPollApp.ViewModels
{
    public class PollResultsViewModel
    {
        public Guid PublicId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TotalVotes { get; set; }
        public List<PollResultsRowViewModel> Rows { get; set; } = new();
    }

    public class PollResultsRowViewModel
    {
        public string OptionText { get; set; } = string.Empty;
        public int Votes { get; set; }
        public int Percentage { get; set; }
    }
}
