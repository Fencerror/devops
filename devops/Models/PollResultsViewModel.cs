namespace DevOpsPollApp.Models
{
    public class PollResultsViewModel
    {
        public Poll Poll { get; set; } = null!;
        public int TotalVotes { get; set; }
    }
}
