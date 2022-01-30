using System;
using System.Linq.Expressions;

namespace Core.CustomExceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException() : base("Entity was not found") { }
        
        public EntityNotFoundException(string message) : base(message) {}
        
        public EntityNotFoundException(string message, Exception inner) : base(message, inner) { }
        
        public EntityNotFoundException(BaseEntity baseEntity) : base($"{baseEntity} not found"){}
    }
}
