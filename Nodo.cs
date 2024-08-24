using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tron
{
    public class Nodo
    {
        public Nodo Arriba { get; set; }
        public Nodo Abajo { get; set; }
        public Nodo Izquierda { get; set; }
        public Nodo Derecha { get; set; }
        public Point Posicion { get; set; }

        public Nodo(int x, int y)
        {
            Posicion = new Point(x, y);
        }
    }
}
