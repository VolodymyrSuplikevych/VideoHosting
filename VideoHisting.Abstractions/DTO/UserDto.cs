using System;
using System.Collections.Generic;

namespace VideoHosting.Abstractions.Dto
{
    public class UserDto
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Group { get; set; }

        public bool DoSubscribed { get; set; }

        public int Subscribers { get; set; }

        public string PhotoPath { get; set; }

        public string PhotoName { get; set; }

        public DateTime DateOfCreation { get; set; }

        public int Subscriptions { get; set; }

        public IList<string> Roles { get; set; }

    }
}
