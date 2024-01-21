using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EnitBook.BL.Entities
{
    public class User : IdentityUser
    {

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? UsernameChangeLimit { get; set; } = 10;
        public byte[]? ProfilePicture { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public Profil Profil { get; set; }

    }
}
