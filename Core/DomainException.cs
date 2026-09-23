using System;

namespace Core
{
    // Custom exception class to handle business logic errors
    public class DomainException : Exception
    {

        /*
         * The task of this constructor is simply to accept data (the error text) 
         * and forward it to the base Exception class so that C# knows 
         * what kind of error it is and what its text is.
         * The parent Exception class does all the system work, 
         * so there is simply nothing to write inside the { } code block.
         */

        // Default constructor
        public DomainException() : base()
        {
        }

        // Constructor with an error message
        public DomainException(string message) : base(message)
        {
        }

        // Constructor with a message and an inner exception
        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}