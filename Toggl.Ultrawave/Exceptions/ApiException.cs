﻿using System;
namespace Toggl.Ultrawave.Exceptions
{
    public class ApiException : Exception
    {
        private readonly string errorMessage;

        public ApiException(string errorMessage)
        {
            this.errorMessage = errorMessage;
        }

        public override string ToString() => errorMessage;
    }
}