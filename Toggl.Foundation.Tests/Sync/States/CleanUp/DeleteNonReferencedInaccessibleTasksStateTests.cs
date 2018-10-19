using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NSubstitute;
using Toggl.Foundation.DataSources.Interfaces;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Foundation.Sync.States.CleanUp;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Xunit;

namespace Toggl.Foundation.Tests.Sync.States.CleanUp
{
    public sealed class DeleteNonReferencedInaccessibleTasksStateTests : DeleteNonReferencedInaccessibleEntityTests
    {
        private readonly DeleteNonReferencedInaccessibleTasksState state;

        private readonly IDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> timeEntriesDataSource =
            Substitute.For<IDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry>>();

        private readonly IDataSource<IThreadSafeTask, IDatabaseTask> tasksDataSource =
            Substitute.For<IDataSource<IThreadSafeTask, IDatabaseTask>>();

        public DeleteNonReferencedInaccessibleTasksStateTests()
        {
            state = new DeleteNonReferencedInaccessibleTasksState(tasksDataSource, timeEntriesDataSource);
        }

        [Fact, LogIfTooSlow]
        public async Task DeleteUnreferencedTasksInInaccessibleWorkspace()
        {
            var accessibleWorkspace = GetWorkspace(1000, isInaccessible: false);
            var inaccessibleWorkspace = GetWorkspace(2000, isInaccessible: true);

            var project1 = GetProject(101, accessibleWorkspace, SyncStatus.InSync);
            var project2 = GetProject(102, accessibleWorkspace, SyncStatus.RefetchingNeeded);
            var project3 = GetProject(201, inaccessibleWorkspace, SyncStatus.SyncFailed);
            var project4 = GetProject(202, inaccessibleWorkspace, SyncStatus.SyncNeeded);

            var task1 = GetTask(1001, accessibleWorkspace, project1, SyncStatus.InSync);
            var task2 = GetTask(1002, accessibleWorkspace, project2, SyncStatus.RefetchingNeeded);
            var task3 = GetTask(1003, accessibleWorkspace, project2, SyncStatus.SyncNeeded);
            var task4 = GetTask(2001, inaccessibleWorkspace, project3, SyncStatus.InSync);
            var task5 = GetTask(2002, inaccessibleWorkspace, project4, SyncStatus.RefetchingNeeded);
            var task6 = GetTask(2003, inaccessibleWorkspace, project3, SyncStatus.SyncNeeded);
            var task7 = GetTask(2003, inaccessibleWorkspace, project4, SyncStatus.InSync);
            var task8 = GetTask(2004, inaccessibleWorkspace, project4, SyncStatus.InSync);

            var te1 = GetTimeEntry(10001, accessibleWorkspace, SyncStatus.SyncNeeded, project: project1, task: task1);
            var te2 = GetTimeEntry(10002, accessibleWorkspace, SyncStatus.SyncNeeded, project: project1, task: task2);
            var te3 = GetTimeEntry(20001, inaccessibleWorkspace, SyncStatus.SyncNeeded, project: project3, task: task4);
            var te4 = GetTimeEntry(20002, inaccessibleWorkspace, SyncStatus.SyncNeeded, project: project4, task: task5);
            var te5 = GetTimeEntry(20002, inaccessibleWorkspace, SyncStatus.SyncNeeded, project: project4);
            var te6 = GetTimeEntry(20002, inaccessibleWorkspace, SyncStatus.SyncNeeded);

            var tasks = new[] { task1, task2, task3, task4, task5, task6, task7, task8 };
            var timeEntries = new[] { te1, te2, te3, te4, te5, te6 };

            var unreferencedTasks = new[] { task7, task8 };
            var neededTasks = tasks.Where(task => !unreferencedTasks.Contains(task));

            configureDataSource(tasks, timeEntries);

            await state.Start().SingleAsync();

            tasksDataSource.Received().DeleteAll(Arg.Is<IEnumerable<IThreadSafeTask>>(arg =>
                arg.All(task => unreferencedTasks.Contains(task)) &&
                arg.None(task => neededTasks.Contains(task))));
        }

        private void configureDataSource(IThreadSafeTask[] tasks, IThreadSafeTimeEntry[] timeEntries)
        {
            tasksDataSource
                .GetAll(Arg.Any<Func<IDatabaseTask, bool>>(), Arg.Is(true))
                .Returns(callInfo =>
                {
                    var predicate = callInfo[0] as Func<IDatabaseTask, bool>;
                    var filteredTasks = tasks.Where(predicate);
                    return Observable.Return(filteredTasks.Cast<IThreadSafeTask>());
                });

            timeEntriesDataSource
                .GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>(), Arg.Is(true))
                .Returns(callInfo =>
                {
                    var predicate = callInfo[0] as Func<IDatabaseTimeEntry, bool>;
                    var filteredTimeEntries = timeEntries.Where(predicate);
                    return Observable.Return(filteredTimeEntries.Cast<IThreadSafeTimeEntry>());
                });
        }
    }
}
