using System;
using System.Threading.Tasks;

namespace Toggl.Core.UI.Services
{
    public interface IUrlHandler
    {
        Task<bool> Handle(Uri uri);
    }
}
