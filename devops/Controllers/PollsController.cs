using System.Linq;
using DevOpsPollApp.Models;
using DevOpsPollApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevOpsPollApp.Controllers
{
    public class PollsController : Controller
    {
        private readonly IPollService _pollService;

        public PollsController(IPollService pollService)
        {
            _pollService = pollService;
        }

        [HttpGet]
        public IActionResult Index(string userName)
        {
            var poll = _pollService.GetActivePoll();
            var model = new PollVoteViewModel
            {
                Poll = poll,
                UserName = userName ?? string.Empty
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Vote(PollVoteViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.UserName) && model.SelectedOptionId != 0)
            {
                _pollService.Vote(model.UserName, model.SelectedOptionId);
            }

            return RedirectToAction("Results");
        }

        [HttpGet]
        public IActionResult Results()
        {
            var poll = _pollService.GetResults();
            var total = poll.Options.Sum(o => o.Votes);
            var model = new PollResultsViewModel
            {
                Poll = poll,
                TotalVotes = total
            };
            return View(model);
        }
    }
}
