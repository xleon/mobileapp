﻿using System;

namespace Toggl.Ultrawave.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}
