using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnitBook.BL.Entities
{
    public class ProfilPostsViewModel
    {
        public List<Profil> Profiles { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Post> Posts { get; set; }
    }

}
