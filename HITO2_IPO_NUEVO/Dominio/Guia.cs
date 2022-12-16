using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HITO2_IPO_NUEVO
{
    internal class Guia : Usuario
    {
        public string Disponibilidad { set; get; }
        public string Idiomas { set; get; }
        public List<Ruta> RutasRealizadas { set; get; }
        public List<Ruta> RutasFuturas { set; get; }

        public Guia(string user, string pass, string name, string lastName, string email, DateTime lastLogin, string telefono,string idiomas, string disponibilidad, List<Ruta> rutasrealizadas, List<Ruta> rutasfuturas) : base(user, pass, name, lastName, email, lastLogin, telefono)
        {
            Disponibilidad = disponibilidad; 
            Idiomas = idiomas;
            RutasRealizadas = rutasrealizadas;
            RutasFuturas = rutasfuturas;
        }
    }
}
