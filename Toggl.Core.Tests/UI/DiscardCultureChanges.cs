using System.Globalization;
using System.Reflection;
using System.Threading;
using Toggl.Core.UI.Helper;
using Xunit.Sdk;

namespace Toggl.Core.Tests.UI
{
    public class DiscardCultureChanges : BeforeAfterTestAttribute
    {
        private CultureInfo originalCultureInfo;
        private CultureInfo originalCultureInfoUI;
        private CultureInfo originalThreadCultureInfo;
        private CultureInfo originalThreadCultureInfoUI;
        private CultureInfo originalDateFormatCultureInfo; 
            
        public override void Before(MethodInfo methodUnderTest)
        {
            originalCultureInfo = CultureInfo.DefaultThreadCurrentCulture;
            originalCultureInfoUI = CultureInfo.DefaultThreadCurrentUICulture;
            originalThreadCultureInfo = Thread.CurrentThread.CurrentCulture;
            originalThreadCultureInfoUI = Thread.CurrentThread.CurrentUICulture;
            originalDateFormatCultureInfo = DateFormatCultureInfo.CurrentCulture;
        }
            
        public override void After(MethodInfo methodUnderTest)
        {
            CultureInfo.DefaultThreadCurrentCulture = originalCultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = originalCultureInfoUI;
            Thread.CurrentThread.CurrentCulture = originalThreadCultureInfo;
            Thread.CurrentThread.CurrentUICulture = originalThreadCultureInfoUI;
            DateFormatCultureInfo.CurrentCulture = originalDateFormatCultureInfo;
        }
    }
}