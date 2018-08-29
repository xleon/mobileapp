//
// ShowReportIntent.m
//
// This file was automatically generated and should not be edited.
//

#import "ShowReportIntent.h"

@implementation ShowReportIntent

@dynamic period;

@end

@interface ShowReportIntentResponse ()

@property (readwrite, NS_NONATOMIC_IOSONLY) ShowReportIntentResponseCode code;

@end

@implementation ShowReportIntentResponse

@synthesize code = _code;

@dynamic period;

- (instancetype)initWithCode:(ShowReportIntentResponseCode)code userActivity:(nullable NSUserActivity *)userActivity {
    self = [super init];
    if (self) {
        _code = code;
        self.userActivity = userActivity;
    }
    return self;
}

+ (instancetype)successIntentResponseWithPeriod:(ShowReportReportPeriod)period {
    ShowReportIntentResponse *intentResponse = [[ShowReportIntentResponse alloc] initWithCode:ShowReportIntentResponseCodeSuccess userActivity:nil];
    intentResponse.period = period;
    return intentResponse;
}

@end
