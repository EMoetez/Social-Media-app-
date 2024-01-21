using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnitBook.BL.Entities
{
    public class Profil
    {
        public int IdProfil {  get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
        public User User { get; set; }
        public string UserId { get; set; }
    }
}
