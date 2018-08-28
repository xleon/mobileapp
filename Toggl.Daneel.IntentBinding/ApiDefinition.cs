using System;
using Foundation;
using Intents;

namespace Toggl.Daneel.Intents
{
	// @interface StopTimerIntent : INIntent
	[BaseType (typeof(INIntent))]
	interface StopTimerIntent
	{
	}

	// @protocol StopTimerIntentHandling <NSObject>
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface StopTimerIntentHandling
	{
		// @required -(void)handleStopTimer:(StopTimerIntent * _Nonnull)intent completion:(void (^ _Nonnull)(StopTimerIntentResponse * _Nonnull))completion;
		[Abstract]
		[Export ("handleStopTimer:completion:")]
		void HandleStopTimer (StopTimerIntent intent, Action<StopTimerIntentResponse> completion);

		// @optional -(void)confirmStopTimer:(StopTimerIntent * _Nonnull)intent completion:(void (^ _Nonnull)(StopTimerIntentResponse * _Nonnull))completion;
		[Export ("confirmStopTimer:completion:")]
		void ConfirmStopTimer (StopTimerIntent intent, Action<StopTimerIntentResponse> completion);
	}

	// @interface StopTimerIntentResponse : INIntentResponse
	[BaseType (typeof(INIntentResponse))]
	[DisableDefaultCtor]
	interface StopTimerIntentResponse
	{
		// -(instancetype _Nonnull)initWithCode:(StopTimerIntentResponseCode)code userActivity:(NSUserActivity * _Nullable)userActivity __attribute__((objc_designated_initializer));
		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (StopTimerIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		// +(instancetype _Nonnull)successIntentResponseWithEntry_description:(NSString * _Nonnull)entry_description entry_duration:(NSString * _Nonnull)entry_duration;
		[Static]
		[Export ("successIntentResponseWithEntry_description:entry_duration:")]
		StopTimerIntentResponse SuccessIntentResponseWithEntry_description (string entry_description, string entry_duration);

		// @property (readwrite, copy, nonatomic) NSString * _Nullable entry_description;
		[NullAllowed, Export ("entry_description")]
		string Entry_description { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nullable entry_duration;
		[NullAllowed, Export ("entry_duration")]
		string Entry_duration { get; set; }

		// @property (readonly, nonatomic) StopTimerIntentResponseCode code;
		[Export ("code")]
		StopTimerIntentResponseCode Code { get; }
	}
}

