using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.Tests.Mocks;
using Toggl.PrimeRadiant.Models;
using Xunit;

namespace Toggl.Foundation.Tests.Interactors
{
    public sealed class GetDefaultWorkspaceInteractorTests
    {
        public sealed class TheGetDefaultWorkspaceInteractor : BaseInteractorTests
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsUsersDefaultWorkspace()
            {
                const long workspaceId = 11;
                var workspace = new MockWorkspace { Id = workspaceId };
                var user = new MockUser { DefaultWorkspaceId = workspaceId };
                Database.User.Single().Returns(Observable.Return(user));
                Database.Workspaces.GetById(workspaceId).Returns(Observable.Return(workspace));

                var defaultWorkspace = await InteractorFactory.GetDefaultWorkspace().Execute();

                defaultWorkspace.ShouldBeEquivalentTo(workspace, options => options.IncludingProperties());
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsTheWorkspaceWithSmallestIdWhenUsersDefaultWorkspaceIsNull()
            {
                const long workspaceId = 11;
                var workspaces = new[] { new MockWorkspace { Id = workspaceId + 2 }, new MockWorkspace { Id = workspaceId }, new MockWorkspace { Id = workspaceId + 1 } };
                var user = new MockUser { DefaultWorkspaceId = null };
                Database.User.Single().Returns(Observable.Return(user));
                Database.Workspaces.GetAll(Arg.Any<Func<IDatabaseWorkspace, bool>>())
                    .Returns(Observable.Return(workspaces));

                var defaultWorkspace = await InteractorFactory.GetDefaultWorkspace().Execute();

                defaultWorkspace.Id.Should().Be(workspaceId);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsTheWorkspaceWithSmallestIdWhenUsersDefaultWorkspaceIsNotInTheDatabase()
            {
                const long workspaceId = 11;
                var workspaces = new[] { new MockWorkspace { Id = workspaceId + 2 }, new MockWorkspace { Id = workspaceId }, new MockWorkspace { Id = workspaceId + 1 } };
                var user = new MockUser { DefaultWorkspaceId = workspaceId + 3 };
                Database.User.Single().Returns(Observable.Return(user));
                Database.Workspaces.GetById(user.DefaultWorkspaceId.Value)
                    .Returns(Observable.Throw<IDatabaseWorkspace>(new InvalidOperationException()));
                Database.Workspaces.GetAll(Arg.Any<Func<IDatabaseWorkspace, bool>>())
                    .Returns(Observable.Return(workspaces));

                var defaultWorkspace = await InteractorFactory.GetDefaultWorkspace().Execute();

                defaultWorkspace.Id.Should().Be(workspaceId);
            }
        }
    }
}
