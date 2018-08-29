using System;
using Foundation;
using Intents;
using ObjCRuntime;

namespace Toggl.Daneel.Intents
{
	// @interface ShowReportIntent : INIntent
	[BaseType (typeof(INIntent))]
	interface ShowReportIntent
	{
		// @property (assign, readwrite, nonatomic) ShowReportReportPeriod period;
		[Export ("period", ArgumentSemantic.Assign)]
		ShowReportReportPeriod Period { get; set; }
	}

	// @protocol ShowReportIntentHandling <NSObject>
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface ShowReportIntentHandling
	{
		// @required -(void)handleShowReport:(ShowReportIntent * _Nonnull)intent completion:(void (^ _Nonnull)(ShowReportIntentResponse * _Nonnull))completion;
		[Abstract]
		[Export ("handleShowReport:completion:")]
		void HandleShowReport (ShowReportIntent intent, Action<ShowReportIntentResponse> completion);

		// @optional -(void)confirmShowReport:(ShowReportIntent * _Nonnull)intent completion:(void (^ _Nonnull)(ShowReportIntentResponse * _Nonnull))completion;
		[Export ("confirmShowReport:completion:")]
		void ConfirmShowReport (ShowReportIntent intent, Action<ShowReportIntentResponse> completion);
	}

	// @interface ShowReportIntentResponse : INIntentResponse
	[BaseType (typeof(INIntentResponse))]
	[DisableDefaultCtor]
	interface ShowReportIntentResponse
	{
		// -(instancetype _Nonnull)initWithCode:(ShowReportIntentResponseCode)code userActivity:(NSUserActivity * _Nullable)userActivity __attribute__((objc_designated_initializer));
		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (ShowReportIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		// +(instancetype _Nonnull)successIntentResponseWithPeriod:(ShowReportReportPeriod)period;
		[Static]
		[Export ("successIntentResponseWithPeriod:")]
		ShowReportIntentResponse SuccessIntentResponseWithPeriod (ShowReportReportPeriod period);

		// @property (assign, readwrite, nonatomic) ShowReportReportPeriod period;
		[Export ("period", ArgumentSemantic.Assign)]
		ShowReportReportPeriod Period { get; set; }

		// @property (readonly, nonatomic) ShowReportIntentResponseCode code;
		[Export ("code")]
		ShowReportIntentResponseCode Code { get; }
	}

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
