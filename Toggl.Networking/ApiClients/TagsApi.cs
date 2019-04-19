using System;
using System.Collections.Generic;
using Toggl.Shared.Models;
using Toggl.Networking.Models;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;

namespace Toggl.Networking.ApiClients
{
    internal sealed class TagsApi : BaseApi, ITagsApi
    {
        private readonly TagEndpoints endPoints;

        public TagsApi(Endpoints endPoints, IApiClient apiClient, IJsonSerializer serializer, Credentials credidentials)
            : base(apiClient, serializer, credidentials, endPoints.LoggedIn)
        {
            this.endPoints = endPoints.Tags;
        }

        public IObservable<List<ITag>> GetAll()
            => SendRequest<Tag, ITag>(endPoints.Get, AuthHeader);

        public IObservable<List<ITag>> GetAllSince(DateTimeOffset threshold)
            => SendRequest<Tag, ITag>(endPoints.GetSince(threshold), AuthHeader);

        public IObservable<ITag> Create(ITag tag)
        {
            var endPoint = endPoints.Post(tag.WorkspaceId);
            var tagCopy = tag as Tag ?? new Tag(tag);
            var observable = SendRequest(endPoint, AuthHeader, tagCopy, SerializationReason.Post);
            return observable;
        }
    }
}
