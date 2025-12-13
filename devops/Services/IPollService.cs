using DevOpsPollApp.Models;

namespace DevOpsPollApp.Services
{
    public interface IPollService
    {
        Poll GetActivePoll();
        void Vote(string userName, int optionId);
        Poll GetResults();
    }
}
