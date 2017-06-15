using System;
using System.Collections.Generic;
using Toggl.Ultrawave.Models;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;

namespace Toggl.Ultrawave.ApiClients
{
    internal sealed class TagsApi : BaseApi, ITagsApi
    {
        private readonly TagEndpoints endPoints;

        public TagsApi(TagEndpoints endPoints, IApiClient apiClient, IJsonSerializer serializer, Credentials credidentials)
            : base(apiClient, serializer, credidentials)
        {
            this.endPoints = endPoints;
        }

        public IObservable<List<Tag>> GetAll()
            => CreateObservable<List<Tag>>(endPoints.Get, AuthHeader);
    }
}
