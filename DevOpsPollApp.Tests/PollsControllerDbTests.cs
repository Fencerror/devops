using System;
using System.Linq;
using System.Threading.Tasks;
using DevOpsPollApp.Controllers;
using DevOpsPollApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DevOpsPollApp.Tests
{
    public class PollsControllerDbTests
    {
        [Fact]
        public async Task Create_Post_CreatesPollAndOptions_AndRedirectsToShare()
        {
            var f = new TestDbFactory();
            try
            {
                var controller = new PollsController(f.Db);

                var vm = new PollCreateViewModel
                {
                    Title = "Test poll",
                    OptionsRaw = "A\nB\nC"
                };

                var result = await controller.Create(vm);

                var redirect = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Share", redirect.ActionName);
                Assert.NotNull(redirect.RouteValues);
                Assert.True(redirect.RouteValues!.ContainsKey("publicId"));

                var poll = f.Db.Polls.FirstOrDefault();
                Assert.NotNull(poll);
                Assert.Equal("Test poll", poll!.Title);
                Assert.Equal(3, f.Db.PollOptions.Count());
            }
            finally
            {
                f.Dispose();
            }
        }

        [Fact]
        public async Task Vote_Post_AddsVote_AndRedirectsToResults()
        {
            var f = new TestDbFactory();
            try
            {
                var controller = new PollsController(f.Db);

                var created = await controller.Create(new PollCreateViewModel
                {
                    Title = "Vote poll",
                    OptionsRaw = "X\nY"
                }) as RedirectToActionResult;

                var publicId = (Guid)created!.RouteValues!["publicId"]!;
                var optionId = f.Db.PollOptions.OrderBy(o => o.Id).First().Id;

                var voteVm = new PollVoteViewModel
                {
                    PublicId = publicId,
                    SelectedOptionId = optionId
                };

                var voteResult = await controller.Vote(publicId, voteVm);

                var redirect = Assert.IsType<RedirectToActionResult>(voteResult);
                Assert.Equal("Results", redirect.ActionName);
                Assert.Equal(publicId, redirect.RouteValues!["publicId"]);

                Assert.Equal(1, f.Db.Votes.Count());
            }
            finally
            {
                f.Dispose();
            }
        }

        [Fact]
        public async Task Results_ReturnsCorrectTotals()
        {
            var f = new TestDbFactory();
            try
            {
                var controller = new PollsController(f.Db);

                var created = await controller.Create(new PollCreateViewModel
                {
                    Title = "Results poll",
                    OptionsRaw = "One\nTwo"
                }) as RedirectToActionResult;

                var publicId = (Guid)created!.RouteValues!["publicId"]!;
                var opt1 = f.Db.PollOptions.OrderBy(o => o.Id).First().Id;
                var opt2 = f.Db.PollOptions.OrderBy(o => o.Id).Skip(1).First().Id;

                await controller.Vote(publicId, new PollVoteViewModel { PublicId = publicId, SelectedOptionId = opt1 });
                await controller.Vote(publicId, new PollVoteViewModel { PublicId = publicId, SelectedOptionId = opt1 });
                await controller.Vote(publicId, new PollVoteViewModel { PublicId = publicId, SelectedOptionId = opt2 });

                var view = await controller.Results(publicId) as ViewResult;
                var model = Assert.IsType<PollResultsViewModel>(view!.Model);

                Assert.Equal(3, model.TotalVotes);
                Assert.Equal(2, model.Rows.First(r => r.OptionText == "One").Votes);
                Assert.Equal(1, model.Rows.First(r => r.OptionText == "Two").Votes);
            }
            finally
            {
                f.Dispose();
            }
        }

        [Fact]
        public async Task Index_ShowsPollsWithTotalVotes()
        {
            var f = new TestDbFactory();
            try
            {
                var controller = new PollsController(f.Db);

                var created = await controller.Create(new PollCreateViewModel
                {
                    Title = "Index poll",
                    OptionsRaw = "A\nB"
                }) as RedirectToActionResult;

                var publicId = (Guid)created!.RouteValues!["publicId"]!;
                var optionId = f.Db.PollOptions.First().Id;

                await controller.Vote(publicId, new PollVoteViewModel { PublicId = publicId, SelectedOptionId = optionId });

                var view = await controller.Index() as ViewResult;
                var model = Assert.IsType<PollIndexViewModel>(view!.Model);

                Assert.Single(model.Polls);
                Assert.Equal(1, model.Polls[0].TotalVotes);
            }
            finally
            {
                f.Dispose();
            }
        }
    }
}
