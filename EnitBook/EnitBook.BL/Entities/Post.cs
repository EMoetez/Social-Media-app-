
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnitBook.BL.Entities
{
    public class Post
    {
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int Likes { get ; set; } 
        public DateTime PublishedDateTime { get; set; }
        public User? User { get; set; }
        public byte[]? PostImage { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }

}
    