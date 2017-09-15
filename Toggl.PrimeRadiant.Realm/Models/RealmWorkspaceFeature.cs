using Realms;
using Toggl.Multivac;
using Toggl.PrimeRadiant.Models;

namespace Toggl.PrimeRadiant.Realm
{
    internal partial class RealmWorkspaceFeature : RealmObject, IDatabaseWorkspaceFeature
    {
        [Ignored]
        public WorkspaceFeatureId FeatureId
        {
            get => (WorkspaceFeatureId)FeatureIdInt;
            set => FeatureIdInt = (int)value;
        }

        public int FeatureIdInt { get; set; }
        
        public bool Enabled { get; set; }
    }
}
