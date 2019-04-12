using System;
using System.Linq;
using System.Reactive.Linq;
using FluentAssertions;
using Toggl.Networking.Tests.Integration.BaseTests;
using Xunit;
using Toggl.Shared;
using Toggl.Networking.Models;

namespace Toggl.Networking.Tests.Integration
{
    public sealed class WorkspaceFeaturesIdEnumTests : EndpointTestBase
    {
        [Fact, LogTestInfo]
        public async void EnumValuesMatchBackendResponse()
        {
            var (togglClient, user) = await SetupTestUser();
            var enumFeatures = Enum
                .GetValues(typeof(WorkspaceFeatureId))
                .OfType<WorkspaceFeatureId>()
                .ToDictionary(wf => wf, wf => wf.ToString());

            var workspaceFeaturesCollections = await togglClient.WorkspaceFeatures.GetAll();
            var features = workspaceFeaturesCollections
                .First()
                .Features
                .Select(feature => (feature.FeatureId, toPascalCase((feature as WorkspaceFeature).Name)))
                .Distinct();

            features.Should().HaveCount(enumFeatures.Count);
            foreach (var tuple in features)
            {
                string enumName = enumFeatures[tuple.Item1];
                string responseName = tuple.Item2;

                enumName.Should().Be(responseName);
            }
        }
            
        private string toPascalCase(string snakeCasedString)
        {
            var segments = snakeCasedString
                .Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1));
    
            return string.Join("", segments);
        }
    }
}
