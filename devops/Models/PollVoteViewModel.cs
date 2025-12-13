namespace DevOpsPollApp.Models
{
    public class PollVoteViewModel
    {
        public Poll Poll { get; set; } = null!;
        public string UserName { get; set; } = string.Empty;
        public int SelectedOptionId { get; set; }
    }
}
