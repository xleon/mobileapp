using System;
using System.Collections.Generic;
using System.Reactive;
using Toggl.Foundation.Autocomplete.Suggestions;
using Toggl.Foundation.DTOs;
using Toggl.Foundation.Models;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Foundation.Suggestions;

namespace Toggl.Foundation.Interactors
{
    public interface IInteractorFactory
    {
        #region Time Entries

        IInteractor<IObservable<IThreadSafeTimeEntry>> CreateTimeEntry(ITimeEntryPrototype prototype);

        IInteractor<IObservable<IThreadSafeTimeEntry>> StartSuggestion(Suggestion suggestion);

        IInteractor<IObservable<IThreadSafeTimeEntry>> ContinueTimeEntry(ITimeEntryPrototype prototype);

        IInteractor<IObservable<IThreadSafeTimeEntry>> ContinueMostRecentTimeEntry();

        IInteractor<IObservable<Unit>> DeleteTimeEntry(long id);

        IInteractor<IObservable<IEnumerable<IThreadSafeTimeEntry>>> GetAllNonDeletedTimeEntries();

        IInteractor<IObservable<IThreadSafeTimeEntry>> UpdateTimeEntry(EditTimeEntryDto dto);

        IInteractor<IObservable<IThreadSafeTimeEntry>> StopTimeEntry(DateTimeOffset currentDateTime);

        #endregion

        #region Projects

        IInteractor<IObservable<bool>> ProjectDefaultsToBillable(long projectId);

        IInteractor<IObservable<bool>> IsBillableAvailableForProject(long projectId);

        IInteractor<IObservable<IThreadSafeProject>> CreateProject(CreateProjectDTO dto);

        #endregion

        #region Workspaces

        IInteractor<IObservable<IThreadSafeWorkspace>> GetDefaultWorkspace();

        IInteractor<IObservable<IEnumerable<IThreadSafeWorkspace>>> GetAllWorkspaces();

        IInteractor<IObservable<IThreadSafeWorkspace>> GetWorkspaceById(long workspaceId);

        IInteractor<IObservable<bool?>> AreProjectsBillableByDefault(long workspaceId);

        IInteractor<IObservable<bool>> WorkspaceAllowsBillableRates(long workspaceId);

        IInteractor<IObservable<bool>> AreCustomColorsEnabledForWorkspace(long workspaceId);

        IInteractor<IObservable<bool>> IsBillableAvailableForWorkspace(long workspaceId);

        IInteractor<IObservable<Unit>> CreateDefaultWorkspace();

        #endregion

        #region Sync

        IInteractor<IObservable<IEnumerable<SyncFailureItem>>> GetItemsThatFailedToSync();

        #endregion

        #region Autocomplete Suggestions

        IInteractor<IObservable<IEnumerable<AutocompleteSuggestion>>> GetTimeEntriesAutocompleteSuggestions(
            IList<string> wordsToQuery);

        IInteractor<IObservable<IEnumerable<AutocompleteSuggestion>>> GetTagsAutocompleteSuggestions(
            IList<string> wordsToQuery);

        IInteractor<IObservable<IEnumerable<AutocompleteSuggestion>>> GetProjectsAutocompleteSuggestions(
            IList<string> wordsToQuery);

        #endregion

        #region Preferences

        IInteractor<IObservable<IThreadSafePreferences>> GetPreferences();

        IInteractor<IObservable<IThreadSafePreferences>> UpdatePreferences(EditPreferencesDTO dto);

        #endregion

        #region User

        IInteractor<IObservable<byte[]>> GetUserAvatar(string url);

        IInteractor<IObservable<IThreadSafeUser>> UpdateUser(EditUserDTO dto);

        IInteractor<IObservable<IThreadSafeUser>> UpdateDefaultWorkspace(long selectedWorkspaceId);

        #endregion

        #region Settings

        IInteractor<IObservable<Unit>> SendFeedback(string message);

        #endregion

        #region Clients

        IInteractor<IObservable<IThreadSafeClient>> CreateClient(string clientName, long workspaceId);

        IInteractor<IObservable<IEnumerable<IThreadSafeClient>>> GetAllClientsInWorkspace(long workspaceId);

        #endregion

        #region Tags

        IInteractor<IObservable<IThreadSafeTag>> CreateTag(string tagName, long workspaceId);

        #endregion
    }
}
