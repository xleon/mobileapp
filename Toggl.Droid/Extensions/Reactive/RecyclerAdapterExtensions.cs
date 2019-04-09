using System;
using System.Collections.Generic;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Reactive;
using Toggl.Droid.Adapters;

namespace Toggl.Droid.Extensions.Reactive
{
    public static class RecyclerAdapterExtensions
    {
        public static Action<IList<T>> Items<T>(this IReactive<BaseRecyclerAdapter<T>> reactive) where T : IEquatable<T>
            => collection => reactive.Base.Items = collection;

        public static Action<IList<SectionModel<TSection, TItem>>> Items<TSection, TItem>(this IReactive<BaseSectionedRecyclerAdapter<TSection, TItem>> reactive)
            where TSection : IEquatable<TSection>
            where TItem : IEquatable<TItem>
            => collection => reactive.Base.Items = collection;
    }
}
