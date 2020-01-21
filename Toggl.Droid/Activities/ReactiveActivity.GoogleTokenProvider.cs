using Android.App;
using Android.Content;
using Android.Gms.Auth;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Exceptions;
using Toggl.Shared.Extensions;
using Object = Java.Lang.Object;

namespace Toggl.Droid.Activities
{
    public abstract partial class ReactiveActivity<TViewModel>
    {
        private const int googleSignInResult = 123;
        private readonly object lockable = new object();
        private readonly string scope = $"oauth2:{Scopes.Profile}";

        private bool isLoggingIn;
        private GoogleApiClient googleApiClient;
        private Subject<string> loginSubject = new Subject<string>();

        public IObservable<string> GetGoogleToken()
        {
            ensureApiClientExists();

            return logOutIfNeeded()
                .Do(x => Console.WriteLine("DEBUG - ReactiveActivity: after logOutIfNeeded"))
                .SelectMany(getGoogleToken);

            IObservable<Unit> logOutIfNeeded()
            {
                if (!googleApiClient.IsConnected)
                {
                    Console.WriteLine("DEBUG - ReactiveActivity: login");
                    return Observable.Return(Unit.Default);
                }

                var logoutSubject = new Subject<Unit>();
                var logoutCallback = new LogOutCallback(() => logoutSubject.CompleteWith(Unit.Default));
                Auth.GoogleSignInApi
                    .SignOut(googleApiClient)
                    .SetResultCallback(logoutCallback);

                return logoutSubject.AsObservable();
            }

            IObservable<string> getGoogleToken(Unit _)
            {
                lock (lockable)
                {
                    if (isLoggingIn)
                    {
                        Console.WriteLine("DEBUG - ReactiveActivity: getGoogleToken if (isLoggingIn)");
                        return loginSubject.AsObservable();
                    }

                    isLoggingIn = true;
                    loginSubject = new Subject<string>();

                    if (googleApiClient.IsConnected)
                    {
                        Console.WriteLine("DEBUG - ReactiveActivity: getGoogleToken if (googleApiClient.IsConnected)");
                        login();
                        return loginSubject.AsObservable();
                    }

                    Console.WriteLine("DEBUG - ReactiveActivity: getGoogleToken end");
                    googleApiClient.Connect();
                    return loginSubject.AsObservable();
                }
            }
        }

        private void onGoogleSignInResult(Intent data)
        {
            lock (lockable)
            {
                Console.WriteLine("DEBUG - ReactiveActivity: onGoogleSignInResult");

                var signInData = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                if (!signInData.IsSuccess)
                {
                    Console.WriteLine("DEBUG - ReactiveActivity: if (!signInData.IsSuccess)");

                    loginSubject.OnError(new GoogleLoginException(signInData.Status.IsCanceled));
                    isLoggingIn = false;
                    return;
                }

                Task.Run(() =>
                {
                    try
                    {
                        Console.WriteLine("DEBUG - ReactiveActivity: onGoogleSignInResult try");

                        var token = GoogleAuthUtil.GetToken(Application.Context, signInData.SignInAccount.Account, scope);
                        loginSubject.OnNext(token);
                        loginSubject.OnCompleted();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("DEBUG - ReactiveActivity: onGoogleSignInResult catch" + e.Message);

                        loginSubject.OnError(e);
                    }
                    finally
                    {
                        Console.WriteLine("DEBUG - ReactiveActivity: onGoogleSignInResult finally");

                        isLoggingIn = false;
                    }
                });
            }
        }

        private void login()
        {
            Console.WriteLine("DEBUG - ReactiveActivity: login");

            if (!isLoggingIn)
            {
                Console.WriteLine("DEBUG - ReactiveActivity: !isLoggingIn");

                return;
            }

            if (!googleApiClient.IsConnected)
            {
                Console.WriteLine("DEBUG - ReactiveActivity: !googleApiClient.IsConnected");

                throw new GoogleLoginException(false);
            }

            var intent = Auth.GoogleSignInApi.GetSignInIntent(googleApiClient);
            StartActivityForResult(intent, googleSignInResult);
        }

        private void onError(ConnectionResult result)
        {
            lock (lockable)
            {
                Console.WriteLine("DEBUG - ReactiveActivity: onError");

                loginSubject.OnError(new GoogleLoginException(false));
                isLoggingIn = false;
            }
        }

        private void ensureApiClientExists()
        {
            Console.WriteLine("DEBUG - ReactiveActivity: ensureApiClientExists");

            if (googleApiClient != null)
            {
                Console.WriteLine("DEBUG - ReactiveActivity: googleApiClient != null");
                return;
            }

            var signInOptions = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                .RequestIdToken("{TOGGL_DROID_GOOGLE_SERVICES_CLIENT_ID}")
                .RequestEmail()
                .Build();

            googleApiClient = new GoogleApiClient.Builder(Application.Context)
                .AddConnectionCallbacks(login)
                .AddOnConnectionFailedListener(onError)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, signInOptions)
                .Build();
        }

        private class LogOutCallback : Object, IResultCallback
        {
            private Action callback;

            public LogOutCallback(Action callback)
            {
                this.callback = callback;
            }

            public void OnResult(Object result)
            {
                callback();
            }
        }
    }
}
