using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tron
{
    public class Mapa
    {
        public Nodo[,] Grid { get; private set; }
        private int ancho;
        private int alto;
        private int tamanoNodo;

        public Mapa(int anchoVentana, int altoVentana, int tamanoNodo)
        {
            this.ancho = anchoVentana / tamanoNodo;
            this.alto = altoVentana / tamanoNodo;
            this.tamanoNodo = tamanoNodo;
            Grid = new Nodo[ancho, alto];
            CrearMapa();
        }

        private void CrearMapa()
        {
            for (int x = 0; x < ancho; x++)
            {
                for (int y = 0; y < alto; y++)
                {
                    Grid[x, y] = new Nodo(x * tamanoNodo, y * tamanoNodo);
                }
            }

            // Establecer las conexiones entre nodos
            for (int x = 0; x < ancho; x++)
            {
                for (int y = 0; y < alto; y++)
                {
                    if (x > 0) Grid[x, y].Izquierda = Grid[x - 1, y];
                    if (x < ancho - 1) Grid[x, y].Derecha = Grid[x + 1, y];
                    if (y > 0) Grid[x, y].Arriba = Grid[x, y - 1];
                    if (y < alto - 1) Grid[x, y].Abajo = Grid[x, y + 1];
                }
            }
        }
    }
}
