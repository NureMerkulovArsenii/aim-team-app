using System;
using System.Runtime.Intrinsics.Arm;

namespace Core
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
    }
}
