﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Foundation.DTOs;
using Toggl.Foundation.Models;
using Toggl.Multivac;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    public sealed class PreferencesDataSource : IPreferencesSource
    {
        private readonly ISingleObjectStorage<IDatabasePreferences> databaseStorage;
        private readonly ISubject<IDatabasePreferences> currentPreferencesSubject;
    
        public IObservable<IDatabasePreferences> Current { get; }

        public PreferencesDataSource(ISingleObjectStorage<IDatabasePreferences> databaseStorage)
        {
            Ensure.Argument.IsNotNull(databaseStorage, nameof(databaseStorage));

            this.databaseStorage = databaseStorage;

            IDatabasePreferences preferences;
            try
            {
                preferences = Get().Wait();
            }
            catch
            {
                preferences = Preferences.DefaultPreferences;
            }

            currentPreferencesSubject = new BehaviorSubject<IDatabasePreferences>(preferences);

            Current = currentPreferencesSubject.AsObservable();
        }

        public IObservable<IDatabasePreferences> Get()
            => databaseStorage.Single().Select(Preferences.From);

        public IObservable<IDatabasePreferences> Update(EditPreferencesDTO dto)
            => databaseStorage
                .Single()
                .Select(preferences => updatedPreferences(preferences, dto))
                .SelectMany(databaseStorage.Update)
                .Select(Preferences.From)
                .Do(currentPreferencesSubject.OnNext);

        public IObservable<IEnumerable<IConflictResolutionResult<IDatabasePreferences>>> BatchUpdate(
            IEnumerable<(long Id, IDatabasePreferences Entity)> entities,
            Func<IDatabasePreferences, IDatabasePreferences, ConflictResolutionMode> conflictResolution,
            IRivalsResolver<IDatabasePreferences> rivalsResolver = null)
            => databaseStorage.BatchUpdate(entities, conflictResolution, rivalsResolver)
                .Do(processConflictResultionResult);

        public IObservable<IDatabasePreferences> GetById(long id)
            => databaseStorage.GetById(id);

        public IObservable<IDatabasePreferences> Create(IDatabasePreferences entity)
            => databaseStorage.Create(entity)
                .Do(currentPreferencesSubject.OnNext);

        public IObservable<IDatabasePreferences> Update(long id, IDatabasePreferences entity)
            => databaseStorage.Update(id, entity)
                .Do(currentPreferencesSubject.OnNext);

        public IObservable<Unit> Delete(long id)
        {
            throw new InvalidOperationException("Preferences cannot be deleted.");
        }

        public IObservable<IEnumerable<IDatabasePreferences>> GetAll()
            => databaseStorage.GetAll();

        public IObservable<IEnumerable<IDatabasePreferences>> GetAll(Func<IDatabasePreferences, bool> predicate)
            => databaseStorage.GetAll(predicate);

        private void processConflictResultionResult(IEnumerable<IConflictResolutionResult<IDatabasePreferences>> batchResult)
        {
            var preferences = batchResult.FirstOrDefault();

            if (preferences == null) return;

            switch (preferences)
            {
                case CreateResult<IDatabasePreferences> created:
                    currentPreferencesSubject.OnNext(created.Entity);
                    break;

                case UpdateResult<IDatabasePreferences> updated:
                    currentPreferencesSubject.OnNext(updated.Entity);
                    break;
            }
        }

        private IDatabasePreferences updatedPreferences(IDatabasePreferences existing, EditPreferencesDTO dto)
            => Preferences.Builder
                .FromExisting(existing)
                .SetDateFormat(dto.DateFormat)
                .Build();
    }
}
