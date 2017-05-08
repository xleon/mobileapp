﻿using Toggl.Multivac.Models;

namespace Toggl.PrimeRadiant.Models
{
    public interface IDatabaseClient : IClient, IDatabaseSyncable { }
    public interface IDatabaseProject : IProject, IDatabaseSyncable { }
    public interface IDatabaseTag : ITag, IDatabaseSyncable { }
    public interface IDatabaseTask : ITask, IDatabaseSyncable { }
    public interface IDatabaseTimeEntry : ITimeEntry, IDatabaseSyncable { }
    public interface IDatabaseUser : IUser, IDatabaseSyncable { }
    public interface IDatabaseWorkspace : IWorkspace, IDatabaseSyncable { }
}