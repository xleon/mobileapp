using System;
using Foundation;
using Intents;
using ObjCRuntime;

namespace Toggl.Daneel.Intents
{
    // @interface StopTimerIntent : INIntent
    [BaseType(typeof(INIntent))]
    interface StopTimerIntent
    {
        // @property (readwrite, copy, nonatomic) INObject * _Nullable time_entry;
        [NullAllowed, Export("time_entry", ArgumentSemantic.Copy)]
        INObject Time_entry { get; set; }
    }

    // @protocol StopTimerIntentHandling <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface StopTimerIntentHandling
    {
        // @required -(void)handleStopTimer:(StopTimerIntent * _Nonnull)intent completion:(void (^ _Nonnull)(StopTimerIntentResponse * _Nonnull))completion;
        [Abstract]
        [Export("handleStopTimer:completion:")]
        void HandleStopTimer(StopTimerIntent intent, Action<StopTimerIntentResponse> completion);

        // @optional -(void)confirmStopTimer:(StopTimerIntent * _Nonnull)intent completion:(void (^ _Nonnull)(StopTimerIntentResponse * _Nonnull))completion;
        [Export("confirmStopTimer:completion:")]
        void ConfirmStopTimer(StopTimerIntent intent, Action<StopTimerIntentResponse> completion);
    }

    // @interface StopTimerIntentResponse : INIntentResponse
    [BaseType(typeof(INIntentResponse))]
    [DisableDefaultCtor]
    interface StopTimerIntentResponse
    {
        // -(instancetype _Nonnull)initWithCode:(StopTimerIntentResponseCode)code userActivity:(NSUserActivity * _Nullable)userActivity __attribute__((objc_designated_initializer));
        [Export("initWithCode:userActivity:")]
        [DesignatedInitializer]
        IntPtr Constructor(StopTimerIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

        // +(instancetype _Nonnull)successIntentResponseWithTime_entry:(INObject * _Nonnull)time_entry;
        [Static]
        [Export("successIntentResponseWithTime_entry:")]
        StopTimerIntentResponse SuccessIntentResponseWithTime_entry(INObject time_entry);

        // @property (readwrite, copy, nonatomic) INObject * _Nullable time_entry;
        [NullAllowed, Export("time_entry", ArgumentSemantic.Copy)]
        INObject Time_entry { get; set; }

        // @property (readonly, nonatomic) StopTimerIntentResponseCode code;
        [Export("code")]
        StopTimerIntentResponseCode Code { get; }
    }
}
