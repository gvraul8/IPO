using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HITO2_IPO_NUEVO
{
    internal class Oferta
    {
        public int Id { get; set; }
        public Ruta Ruta { get; set; }
        public string Descripcion { get; set; }
        public Uri IMG_OFERTA { get; set; }

        public Oferta(int id, Ruta ruta, string descripcion)
        {
            Id = id;
            Descripcion = descripcion;
            Ruta = ruta;
        }
    }
}
