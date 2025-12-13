using System.Collections.Generic;
using System.Linq;
using DevOpsPollApp.Models;

namespace DevOpsPollApp.Services
{
    public class InMemoryPollService : IPollService
    {
        private readonly object _lock = new object();
        private readonly Poll _poll;
        private readonly Dictionary<string, int> _userVotes = new Dictionary<string, int>();

        public InMemoryPollService()
        {
            _poll = new Poll
            {
                Id = 1,
                Question = "Какой ваш любимый язык программирования?",
                Options = new List<PollOption>
                {
                    new PollOption { Id = 1, Text = "C#" },
                    new PollOption { Id = 2, Text = "Python" },
                    new PollOption { Id = 3, Text = "JavaScript" },
                    new PollOption { Id = 4, Text = "C++" }
                }
            };
        }

        public Poll GetActivePoll()
        {
            return _poll;
        }

        public void Vote(string userName, int optionId)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return;
            }

            lock (_lock)
            {
                var option = _poll.Options.FirstOrDefault(o => o.Id == optionId);
                if (option == null)
                {
                    return;
                }

                if (_userVotes.TryGetValue(userName, out var previousOptionId))
                {
                    if (previousOptionId == optionId)
                    {
                        return;
                    }

                    var previousOption = _poll.Options.FirstOrDefault(o => o.Id == previousOptionId);
                    if (previousOption != null && previousOption.Votes > 0)
                    {
                        previousOption.Votes--;
                    }
                }

                option.Votes++;
                _userVotes[userName] = optionId;
            }
        }

        public Poll GetResults()
        {
            lock (_lock)
            {
                var copy = new Poll
                {
                    Id = _poll.Id,
                    Question = _poll.Question,
                    Options = _poll.Options
                        .Select(o => new PollOption
                        {
                            Id = o.Id,
                            Text = o.Text,
                            Votes = o.Votes
                        })
                        .ToList()
                };

                return copy;
            }
        }
    }
}
