using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HITO2_IPO_NUEVO
{
    internal class Ruta
    {
        public string Nombre { set; get; }
        public String Origen { set; get; }
        public String Destino { set; get; }
        public String Provincia { set; get; }
        public DateTime Fecha { get; set; }
        public String Dificultad { set; get; }
        public int PlazasDisponibles { set; get; }
        public String MaterialNecesario { set; get; }
        public int NumeroDeRealizaciones { set; get; }
        public Guia Guia { set; get; }
        public Uri URL_RUTA { set; get; }
        //public Uri URL_INTERES { set; get; }

        public Ruta(string nombre, string origen, string destino, string provincia, DateTime fecha, string dificultad, int plazasDisponibles, string material, int numRealizaciones,Guia guia)
        {
            Nombre = nombre;
            Origen = origen;
            Destino = destino;
            Provincia = provincia;
            Fecha = fecha;
            Dificultad = dificultad;
            PlazasDisponibles = plazasDisponibles;
            MaterialNecesario = material;
            NumeroDeRealizaciones = numRealizaciones;
            Guia Guia = guia;

        }
    }

}

