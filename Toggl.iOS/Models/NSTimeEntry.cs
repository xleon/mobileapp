using System;
using System.Collections.Generic;
using Foundation;
using Toggl.Core;
using Toggl.iOS.Extensions;
using Toggl.Shared.Models;

namespace Toggl.iOS.Models
{
    public class NSTimeEntry : NSCoding
    {
        public DateTimeOffset Start { get; }
        public string Description { get; }

        public NSTimeEntry(
            DateTimeOffset start,
            string description)
        {
            Start = start;
            Description = description;
        }

        [Export("initWithCoder:")]
        public NSTimeEntry(NSCoder decoder)
        {
            Start = ((DateTime) (NSDate) decoder.DecodeObject("Start")).ToLocalTime();
            Description = (string) (NSString) decoder.DecodeObject("Description");
        }

        public override void EncodeTo(NSCoder encoder)
        {
            encoder.Encode(Start.ToNSDate(), "Start");
            encoder.Encode((NSString)Description, "Description");
        }
    }
}
