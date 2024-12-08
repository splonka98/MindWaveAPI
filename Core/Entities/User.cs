﻿using System.Data;

namespace MindWaveApi.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; }
        public ICollection<Query> Queries { get; set; }
    }
}
