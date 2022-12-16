using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HITO2_IPO_NUEVO
{
    internal class Excursionista : Usuario
    {

        public int Edad { set; get; }
        public bool Notificaciones { set; get; }
        public List<Ruta> RutasRealizadas { set; get; }
        public List<Ruta> RutasFuturas { set; get; }

        public Excursionista(string user, string pass, string name, string lastName, string email, DateTime lastLogin, string telefono, int edad, bool notificaciones, List<Ruta> rutasrealizadas, List<Ruta> rutasfuturas) : base(user, pass, name, lastName, email, lastLogin, telefono)
        {
            Edad = edad;
            Notificaciones = notificaciones;
            RutasRealizadas = rutasrealizadas;
            RutasFuturas = rutasfuturas;
        }
    }
}
