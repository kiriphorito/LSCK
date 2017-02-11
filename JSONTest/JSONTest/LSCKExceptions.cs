using System;

namespace JSONTest
{
    public class LSCKExceptions
    {
        public class InvalidThemeException : System.Exception
        {
            public InvalidThemeException()
                : base("You have entered an invalid Ace Editor theme!") { }
        }
    }
}
