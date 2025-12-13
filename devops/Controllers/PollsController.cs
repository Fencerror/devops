using System;
using System.Linq;
using System.Threading.Tasks;
using DevOpsPollApp.Data;
using DevOpsPollApp.Entities;
using DevOpsPollApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevOpsPollApp.Controllers
{
    public class PollsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PollsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var polls = await _db.Polls
                .OrderByDescending(p => p.CreatedAtUtc)
                .Select(p => new PollListItemViewModel
                {
                    PublicId = p.PublicId,
                    Title = p.Title,
                    CreatedAtUtc = p.CreatedAtUtc,
                    TotalVotes = p.Options.SelectMany(o => o.Votes).Count()
                })
                .ToListAsync();

            return View(new PollIndexViewModel { Polls = polls });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new PollCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PollCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var options = model.OptionsRaw
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (options.Count < 2)
            {
                ModelState.AddModelError(nameof(model.OptionsRaw), "Нужно минимум 2 варианта ответа (каждый с новой строки).");
                return View(model);
            }

            var poll = new Poll
            {
                PublicId = Guid.NewGuid(),
                Title = model.Title.Trim(),
                CreatedAtUtc = DateTime.UtcNow,
                Options = options.Select(o => new PollOption { Text = o }).ToList()
            };

            _db.Polls.Add(poll);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Share), new { publicId = poll.PublicId });
        }

        [HttpGet("p/{publicId:guid}")]
        public async Task<IActionResult> Vote(Guid publicId)
        {
            var poll = await _db.Polls
                .Include(p => p.Options)
                .FirstOrDefaultAsync(p => p.PublicId == publicId);

            if (poll == null)
            {
                return NotFound();
            }

            var vm = new PollVoteViewModel
            {
                PublicId = poll.PublicId,
                Title = poll.Title,
                Options = poll.Options
                    .OrderBy(o => o.Id)
                    .Select(o => new PollVoteOptionViewModel { Id = o.Id, Text = o.Text })
                    .ToList()
            };

            return View(vm);
        }

        [HttpPost("p/{publicId:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(Guid publicId, PollVoteViewModel model)
        {
            var option = await _db.PollOptions
                .Include(o => o.Poll)
                .FirstOrDefaultAsync(o => o.Id == model.SelectedOptionId && o.Poll.PublicId == publicId);

            if (option == null)
            {
                return NotFound();
            }

            _db.Votes.Add(new Vote
            {
                PollOptionId = option.Id,
                CreatedAtUtc = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Results), new { publicId });
        }

        [HttpGet("p/{publicId:guid}/results")]
        public async Task<IActionResult> Results(Guid publicId)
        {
            var poll = await _db.Polls
                .Include(p => p.Options)
                .ThenInclude(o => o.Votes)
                .FirstOrDefaultAsync(p => p.PublicId == publicId);

            if (poll == null)
            {
                return NotFound();
            }

            var total = poll.Options.Sum(o => o.Votes.Count);

            var rows = poll.Options
                .Select(o =>
                {
                    var votes = o.Votes.Count;
                    var pct = total == 0 ? 0 : (int)Math.Round((double)votes * 100 / total);
                    return new PollResultsRowViewModel
                    {
                        OptionText = o.Text,
                        Votes = votes,
                        Percentage = pct
                    };
                })
                .OrderByDescending(r => r.Votes)
                .ThenBy(r => r.OptionText)
                .ToList();

            var vm = new PollResultsViewModel
            {
                PublicId = poll.PublicId,
                Title = poll.Title,
                TotalVotes = total,
                Rows = rows
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Share(Guid publicId)
        {
            var exists = await _db.Polls.AnyAsync(p => p.PublicId == publicId);
            if (!exists)
            {
                return NotFound();
            }

            ViewData["PublicId"] = publicId;
            return View();
        }
    }
}
