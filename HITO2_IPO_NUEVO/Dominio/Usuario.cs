using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HITO2_IPO_NUEVO
{
    public class Usuario
    {
        public String User { set; get; }
        public String Pass { set; get; }
        public String Name { set; get; }
        public String LastName { set; get; }
        public String Email { set; get; }
        public string Telefono { set; get; }
        public DateTime LastLogin { set; get; }
        public Uri ImgUrl { set; get; }

        public Usuario(string user, string pass, string name, string lastName, string email, DateTime lastLogin, string telefono)
        {
            User = user;
            Pass = pass;
            Name = name;
            LastName = lastName;
            Email = email;
            LastLogin = lastLogin;
            Telefono = telefono;
        }
    }
}
