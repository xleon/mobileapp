using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.DTOs;
using Toggl.Core.Extensions;
using Toggl.Core.Helper;
using Toggl.Core.Interactors;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Core.Sync;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Extensions.Reactive;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class ShareTimeEntryViewModel : ViewModelWithInput<SharePayload>
    {
        private BehaviorSubject<SharePayload> payloadSubject = new BehaviorSubject<SharePayload>(null);

        public IObservable<string> Description { get; private set; }
        public IObservable<string> ProjectClient { get; private set; }
        public IObservable<string> Tags { get; private set; }

        public IObservable<DateTimeOffset> StartTime { get; private set; }
        public IObservable<DateTimeOffset?> StopTime { get; private set; }

        public IObservable<bool> IsBillable { get; private set; }

        // ****************************************************

        private bool hasMissingProject;
        private bool hasMissingClient;
        private bool hasMissingTags;

        private BehaviorSubject<bool> shouldCreateProjectElementsSubject = new BehaviorSubject<bool>(false);
        public IObservable<bool> ShouldCreateProjectElements { get; private set; }
        public IObservable<string> CreateProjectElementsLabel { get; private set; }

        private BehaviorSubject<bool> shouldCreateTagsSubject = new BehaviorSubject<bool>(false);
        public IObservable<bool> ShouldCreateTags { get; private set; }
        public IObservable<string> CreateTagsLabel { get; private set; }

        private readonly ITimeService timeService;
        private readonly ITogglDataSource dataSource;
        private readonly IInteractorFactory interactorFactory;
        private readonly IAnalyticsService analyticsService;
        private readonly ISyncManager syncManager;
        private readonly ISchedulerProvider schedulerProvider;
        private readonly IRxActionFactory actionFactory;

        public UIAction ToggleProjectElementsCreation { get; private set; }
        public UIAction ToggleTagsCreation { get; private set; }

        public UIAction Save { get; private set; }

        public ShareTimeEntryViewModel(
            ITimeService timeService,
            ITogglDataSource dataSource,
            ISyncManager syncManager,
            IInteractorFactory interactorFactory,
            INavigationService navigationService,
            IOnboardingStorage onboardingStorage,
            IAnalyticsService analyticsService,
            IRxActionFactory actionFactory,
            ISchedulerProvider schedulerProvider)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(syncManager, nameof(syncManager));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(actionFactory, nameof(actionFactory));

            this.dataSource = dataSource;
            this.syncManager = syncManager;
            this.timeService = timeService;
            this.interactorFactory = interactorFactory;
            this.analyticsService = analyticsService;
            this.schedulerProvider = schedulerProvider;
            this.actionFactory = actionFactory;

            Description = payloadSubject
                .Select(p => p.Description)
                .DistinctUntilChanged()
                .AsDriver("", schedulerProvider);

            ProjectClient = payloadSubject
                .Select(projectElementsString)
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            Tags = payloadSubject
               .Select(tagsString)
               .DistinctUntilChanged()
               .AsDriver(schedulerProvider);

            StartTime = payloadSubject
                .Select(p => p.Start)
                .DistinctUntilChanged()
                .AsDriver(DateTimeOffset.Now, schedulerProvider);

            StopTime = payloadSubject
                .Select(p => p.Stop)
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            IsBillable = payloadSubject
                .Select(p => p.IsBillable)
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            ShouldCreateProjectElements = shouldCreateProjectElementsSubject
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            CreateProjectElementsLabel = Observable.CombineLatest(
                    payloadSubject.Select(p => p.ProjectId),
                    shouldCreateProjectElementsSubject,
                    missingProjectElementsString)
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            ShouldCreateTags = shouldCreateTagsSubject
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            CreateTagsLabel = Observable.CombineLatest(
                payloadSubject.Select(p => p.Tags.Select(tag => tag.Id).ToArray()),
                shouldCreateTagsSubject,
                missingTagsString)
             .DistinctUntilChanged()
             .AsDriver(schedulerProvider);

            ToggleProjectElementsCreation = actionFactory.FromAction(toggleProjectElementsCreation);
            ToggleTagsCreation = actionFactory.FromAction(toggleTagsCreation);

            Save = actionFactory.FromAsync(save);
        }

        private string tagsString(SharePayload payload)
        {
            if (payload.Tags.Length == 0)
                return null;

            return string.Join(", ", payload.Tags.Select(tag => tag.Name));
        }

        private string projectElementsString(SharePayload payload)
        {
            if (!payload.ProjectId.HasValue)
                return null;

            var client = payload.ClientId.HasValue
                ? $" ({payload.ClientName})"
                : "";

            return $"{payload.ProjectName}{client}";
        }

        private void toggleProjectElementsCreation()
        {
            var newValue = !shouldCreateProjectElementsSubject.Value;
            shouldCreateProjectElementsSubject.OnNext(newValue);
        }

        private void toggleTagsCreation()
        {
            var newValue = !shouldCreateTagsSubject.Value;
            shouldCreateTagsSubject.OnNext(newValue);
        }

        public override async Task Initialize(SharePayload payload)
        {
            await base.Initialize(payload);

            await calculate(payload);

            var tags = await dataSource.Tags.GetAll();

            tags.OrderBy(t => t.Id).ForEach(t => System.Diagnostics.Debug.WriteLine($"{t.Id} -> {t.Name}"));

            payloadSubject.OnNext(payload);
        }

        private async Task save()
        {
            var payload = payloadSubject.Value;
            var shouldCreateProjectAndClient = shouldCreateProjectElementsSubject.Value;
            var shouldCreateTags = shouldCreateTagsSubject.Value;

            // determine the destination workspace
            var workspace = await getWorkspace(payload.WorkspaceId);
            var project = (IThreadSafeProject)null;
            var client = (IThreadSafeClient)null;
            var tagsIds = new List<long>();

            if (payload.ProjectId.HasValue)
            {
                if (payload.ClientId.HasValue)
                {
                    client = await getClient(payload.ClientId.Value);

                    if (shouldCreateProjectAndClient && client == null)
                        client = await interactorFactory.CreateClient(payload.ClientName, payload.ClientId.Value).Execute();
                }

                project = await getProject(payload.ProjectId.Value);

                if (shouldCreateProjectAndClient && project == null)
                {
                    var projectDTO = new CreateProjectDTO
                    {
                        ClientId = client?.Id,
                        IsPrivate = true,
                        WorkspaceId = workspace.Id,
                        Name = payload.ProjectName,
                        Color = Colors.DefaultProjectColors.RandomElement(),
                        Billable = false
                    };

                    project = await interactorFactory.CreateProject(projectDTO).Execute();
                }
            }

            if (payload.Tags.Length > 0)
            {
                var sharedTags = payload.Tags;
                var sharedTagsIds = sharedTags.Select(t => t.Id);

                var dbTags = (await getTags(sharedTagsIds)).ToDictionary(t => t.Id);

                foreach (var tag in sharedTags)
                {
                    var dbTag = dbTags.ContainsKey(tag.Id)
                        ? dbTags[tag.Id]
                        : null;

                    if (shouldCreateTags && dbTag == null)
                    {
                        dbTag = await interactorFactory.CreateTag(tag.Name, workspace.Id).Execute();
                    }

                    if (dbTag != null)
                        tagsIds.Add(dbTag.Id);
                }
            }

            var duration = payload.Stop.HasValue
                ? (payload.Stop - payload.Start)
                : null;

            var timeEntry = payload.Description.AsTimeEntryPrototype(
                payload.Start,
                workspace.Id,
                duration,
                project?.Id,
                null,
                tagsIds.ToArray(),
                payload.IsBillable);

            await interactorFactory.CreateTimeEntry(timeEntry, TimeEntryStartOrigin.WifiShared).Execute();

            syncManager.InitiatePushSync();

            Close();
        }

        private void close(EditViewCloseReason reason)
        {
            Close();
        }

        private string missingProjectElementsString(long? projectId, bool shouldCreate)
        {
            if (!projectId.HasValue)
                return null;

            if (!hasMissingProject && !hasMissingClient)
                return null;

            var missingElementsList = new List<string>();
            if (hasMissingProject)
                missingElementsList.Add("project");
            if (hasMissingClient)
                missingElementsList.Add("client");
            var missingElements = string.Join(" and ", missingElementsList);

            return shouldCreate
                ? $"Missing {missingElements} will be created."
                : $"Missing {missingElements} will be ignored.";
        }

        private string missingTagsString(long[] tags, bool shouldCreate)
        {
            if (tags.Length == 0)
                return null;

            if (!hasMissingTags)
                return null;

            return shouldCreate
                 ? "Missing tags will be created."
                 : "Missing tags will be ignored.";
        }

        private async Task<IThreadSafeWorkspace> getWorkspace(long id)
        {
            var workspace = await interactorFactory.GetWorkspaceById(id)
                .Execute()
                .OnErrorResumeNext(Observable.Return<IThreadSafeWorkspace>(null));

            if (workspace == null)
                workspace = await interactorFactory.GetDefaultWorkspace().Execute();

            return workspace;
        }

        private async Task calculate(SharePayload payload)
        {
            if (payload.ProjectId.HasValue)
            {
                var project = await getProject(payload.ProjectId.Value);

                hasMissingProject = project == null;

                if (payload.ClientId.HasValue)
                {
                    var client = await getClient(payload.ClientId.Value);
                    hasMissingClient = client == null;
                }
            }

            if (payload.Tags.Length > 0)
            {
                var tagsIds = payload.Tags.Select(tag => tag.Id).ToArray();

                var dbTags = await getTags(tagsIds);
                hasMissingTags = tagsIds.Length == dbTags.Count();
            }
        }

        private async Task<IThreadSafeProject> getProject(long id)
            => await interactorFactory.GetProjectById(id)
            .Execute()
            .OnErrorResumeNext(Observable.Return<IThreadSafeProject>(null));

        private async Task<IThreadSafeClient> getClient(long id)
            => await interactorFactory.GetClientById(id)
            .Execute()
            .OnErrorResumeNext(Observable.Return<IThreadSafeClient>(null));

        private async Task<IEnumerable<IThreadSafeTag>> getTags(IEnumerable<long> ids)
            => await interactorFactory.GetMultipleTagsById(ids.ToArray())
            .Execute()
            .OnErrorResumeNext(Observable.Return(new IThreadSafeTag[0]));

    }
}
