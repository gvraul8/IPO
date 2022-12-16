using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HITO2_IPO_NUEVO
{
    internal class PuntoInteres
    {
        public string Nombre { set; get; }
        public string Descripcion { set; get; }
        public Ruta Ruta { set; get; }
        public Uri URL_IMAGEN { set; get; }

        public PuntoInteres(string nombre, string descripcion, Ruta ruta)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            Ruta = ruta;
        }
    }
}
