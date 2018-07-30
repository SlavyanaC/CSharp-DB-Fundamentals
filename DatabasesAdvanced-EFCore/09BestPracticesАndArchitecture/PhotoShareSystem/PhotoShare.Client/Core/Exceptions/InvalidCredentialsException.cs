namespace PhotoShare.Client.Core.Exceptions
{
    using System;

    class InvalidCredentialsException : InvalidOperationException
    {
        private const string MESSAGE = "Invalid credentials!";

        public InvalidCredentialsException() :
            base(MESSAGE)
        {
        }
    }
}
