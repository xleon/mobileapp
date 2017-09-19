using Realms;
using System.Linq;
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

        public static RealmWorkspaceFeature FindOrCreate(IDatabaseWorkspaceFeature entity, Realms.Realm realm)
            => find(entity, realm) ?? create(entity, realm);

        private static RealmWorkspaceFeature find(IDatabaseWorkspaceFeature entity, Realms.Realm realm)
            => realm.All<RealmWorkspaceFeature>()
                .SingleOrDefault(x => x.FeatureIdInt == (int)entity.FeatureId && x.Enabled == entity.Enabled);

        private static RealmWorkspaceFeature create(IDatabaseWorkspaceFeature entity, Realms.Realm realm)
            => realm.Add(new RealmWorkspaceFeature(entity, realm));
    }
}
