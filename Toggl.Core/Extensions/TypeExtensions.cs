using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.Extensions
{
    public static class TypeExtensions
    {
        public static string GetSafeTypeName<T>(this T item) 
            => item?.GetType().Name ?? "[null]";
    }
}
