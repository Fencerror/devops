using DevOpsPollApp.Services;
using Xunit;

namespace DevOpsPollApp.Tests.Services
{
    public class InMemoryPollServiceTests
    {
        [Fact]
        public void GetActivePoll_ReturnsPollWithOptions()
        {
            var service = new InMemoryPollService();

            var poll = service.GetActivePoll();

            Assert.NotNull(poll);
            Assert.Equal(1, poll.Id);
            Assert.False(string.IsNullOrWhiteSpace(poll.Question));
            Assert.NotNull(poll.Options);
            Assert.NotEmpty(poll.Options);
        }

        [Fact]
        public void Vote_AddsVoteForOption()
        {
            var service = new InMemoryPollService();
            var poll = service.GetActivePoll();
            var optionId = poll.Options[0].Id;

            service.Vote("user1", optionId);
            var results = service.GetResults();

            var opt = results.Options.Find(o => o.Id == optionId);
            Assert.NotNull(opt);
            Assert.Equal(1, opt!.Votes);
        }

        [Fact]
        public void Vote_SameUserChangesVote_DecrementsPrevious()
        {
            var service = new InMemoryPollService();
            var poll = service.GetActivePoll();
            var first = poll.Options[0].Id;
            var second = poll.Options[1].Id;

            service.Vote("user1", first);
            service.Vote("user1", second);

            var results = service.GetResults();
            Assert.Equal(0, results.Options.Find(o => o.Id == first)!.Votes);
            Assert.Equal(1, results.Options.Find(o => o.Id == second)!.Votes);
        }

        [Fact]
        public void Vote_IgnoresEmptyUserName()
        {
            var service = new InMemoryPollService();
            var poll = service.GetActivePoll();
            var optionId = poll.Options[0].Id;

            service.Vote("", optionId);
            service.Vote("   ", optionId);

            var results = service.GetResults();
            Assert.Equal(0, results.Options.Find(o => o.Id == optionId)!.Votes);
        }

        [Fact]
        public void Vote_IgnoresInvalidOptionId()
        {
            var service = new InMemoryPollService();

            service.Vote("user1", -1);
            service.Vote("user2", 9999);

            var results = service.GetResults();
            foreach (var opt in results.Options)
            {
                Assert.Equal(0, opt.Votes);
            }
        }

        [Fact]
        public void GetResults_ReturnsCopy()
        {
            var service = new InMemoryPollService();
            var poll = service.GetActivePoll();
            service.Vote("user1", poll.Options[0].Id);

            var r1 = service.GetResults();
            var r2 = service.GetResults();

            Assert.NotSame(r1, r2);
            Assert.NotSame(r1.Options, r2.Options);
        }
    }
}
