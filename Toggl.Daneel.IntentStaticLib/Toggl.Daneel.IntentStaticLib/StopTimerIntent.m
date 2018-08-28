//
// StopTimerIntent.m
//
// This file was automatically generated and should not be edited.
//

#import "StopTimerIntent.h"

@implementation StopTimerIntent



@end

@interface StopTimerIntentResponse ()

@property (readwrite, NS_NONATOMIC_IOSONLY) StopTimerIntentResponseCode code;

@end

@implementation StopTimerIntentResponse

@synthesize code = _code;

@dynamic entry_description, entry_duration;

- (instancetype)initWithCode:(StopTimerIntentResponseCode)code userActivity:(nullable NSUserActivity *)userActivity {
    self = [super init];
    if (self) {
        _code = code;
        self.userActivity = userActivity;
    }
    return self;
}

+ (instancetype)successIntentResponseWithEntry_description:(NSString *)entry_description entry_duration:(NSString *)entry_duration {
    StopTimerIntentResponse *intentResponse = [[StopTimerIntentResponse alloc] initWithCode:StopTimerIntentResponseCodeSuccess userActivity:nil];
    intentResponse.entry_description = entry_description;
    intentResponse.entry_duration = entry_duration;
    return intentResponse;
}

@end
