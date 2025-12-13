using System;
using System.Collections.Generic;

namespace DevOpsPollApp.ViewModels
{
    public class PollVoteViewModel
    {
        public Guid PublicId { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<PollVoteOptionViewModel> Options { get; set; } = new();
        public int SelectedOptionId { get; set; }
    }

    public class PollVoteOptionViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
