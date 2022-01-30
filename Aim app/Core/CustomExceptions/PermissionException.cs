using System;

namespace Core.CustomExceptions
{
    public class PermissionException : Exception
    {
        public PermissionException() : base("You do not have permission to perform this action") { }
        
        public PermissionException(string message) : base(message) {}
        
        public PermissionException(string message, Exception inner) : base(message, inner) { }
    }
}
