using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AgileBits;
using Foundation;
using Toggl.Core.UI.Services;
using Toggl.Shared;
using LoginHandler = AgileBits.OnePasswordLoginDictionaryCompletionBlock;

namespace Toggl.Daneel.Services
{
    public sealed class OnePasswordServiceIos : NSObject, IPasswordManagerService
    {
        private NSObject sourceView;
        public bool IsAvailable => OnePasswordExtension.SharedExtension.IsAppExtensionAvailable;

        public void SetSourceView(NSObject view)
        {
            sourceView = view;
        }

        public IObservable<PasswordManagerResult> GetLoginInformation()
        {
            return Observable.Create<PasswordManagerResult>(observer =>
            {
                if (sourceView == null)
                {
                    return Disposable.Empty;
                }

                var presenter = IosDependencyContainer.Instance.ViewPresenter;

                OnePasswordExtension.SharedExtension.FindLoginForURLString(
                    "https://www.toggl.com",
                    presenter.MasterNavigationController,
                    sourceView,
                    getOnePasswordHandler(observer)
                );

                return Disposable.Empty;
            });
        }

        private LoginHandler getOnePasswordHandler(IObserver<PasswordManagerResult> observer) => (dict, ex) =>
        {
            if (dict != null && dict.Any())
            {
                var email = dict[AppExtensionLoginDictionarykeys.UsernameKey] as NSString;
                var password = dict[AppExtensionLoginDictionarykeys.PasswordKey] as NSString;

                observer.OnNext(new PasswordManagerResult(
                    Email.From(email), Password.From(password)));
            }
            else
            {
                observer.OnNext(PasswordManagerResult.None);
            }

            observer.OnCompleted();
        };
    }
}
