using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

namespace Task1.Models
{
    [Table("users")]
    public class UserModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("avatar_url")]
        public string AvatarUrl { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}