//
// StopTimerIntent.m
//
// This file was automatically generated and should not be edited.
//

#import "StopTimerIntent.h"

@implementation StopTimerIntent

@dynamic time_entry;

@end

@interface StopTimerIntentResponse ()

@property (readwrite, NS_NONATOMIC_IOSONLY) StopTimerIntentResponseCode code;

@end

@implementation StopTimerIntentResponse

@synthesize code = _code;

@dynamic time_entry;

- (instancetype)initWithCode:(StopTimerIntentResponseCode)code userActivity:(nullable NSUserActivity *)userActivity {
    self = [super init];
    if (self) {
        _code = code;
        self.userActivity = userActivity;
    }
    return self;
}

+ (instancetype)successIntentResponseWithTime_entry:(INObject *)time_entry {
    StopTimerIntentResponse *intentResponse = [[StopTimerIntentResponse alloc] initWithCode:StopTimerIntentResponseCodeSuccess userActivity:nil];
    intentResponse.time_entry = time_entry;
    return intentResponse;
}

@end
