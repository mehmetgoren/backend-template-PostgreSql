namespace Server.Models
{
    using System;

    public sealed class UserLocal
    {
        public string Name { get; set; }

        public Guid? Token { get; set; }
    }
}
