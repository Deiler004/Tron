using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Tron
{
    public class Ventana : Form
    {
        private System.Windows.Forms.Timer enemigoMovimientoTimer;
        private System.Windows.Forms.Timer cambioDireccionTimer;
        private System.Windows.Forms.Timer fuegoTimer;
        private System.Windows.Forms.Timer juegoTimer;
        private List<PictureBox> enemigos;
        private List<PictureBox> llamas;
        private List<Point> direcciones;
        private PictureBox jugadorMoto;
        private Point direccionJugador;
        private int enemigoVelocidad;
        private int duracionLlama = 7000; // Duración de la llama en milisegundos
        private Random random;
        private Label lblVelocidad;
        private Label lblTiempo;
        private int tiempoJuego;
        private List<Point> posicionesPrevias;
        private List<PictureBox> llamasRecientes;
        private List<Point> nodos;
        private List<PictureBox> bombas;  // Lista de bombas


        public Ventana()
        {
            InitializeComponent();
            this.Width = 1080;
            this.Height = 720;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            llamasRecientes = new List<PictureBox>();

            random = new Random();
            enemigos = new List<PictureBox>();
            llamas = new List<PictureBox>();
            direcciones = new List<Point>();
            posicionesPrevias = new List<Point>();

            enemigoVelocidad = random.Next(1, 11);
            bombas = new List<PictureBox>();



            var bombaTimer = new System.Windows.Forms.Timer();
            bombaTimer.Interval = 10000; // Cada 10 segundos, ajusta este valor según sea necesario
            bombaTimer.Tick += (s, e) => ColocarBomba();
            bombaTimer.Start();





            // Inicializar el jugador
            jugadorMoto = new PictureBox();
            jugadorMoto.Image = Image.FromFile("moto_derecha.png");
            jugadorMoto.SizeMode = PictureBoxSizeMode.StretchImage;
            jugadorMoto.Size = new Size(20, 20);
            jugadorMoto.Location = new Point(this.ClientSize.Width / 2, this.ClientSize.Height / 2);
            this.Controls.Add(jugadorMoto);
            direccionJugador = new Point(1, 0); // Inicia moviéndose a la derecha

            for (int i = 0; i < 4; i++)
            {
                PictureBox enemigoPictureBox = new PictureBox();
                enemigoPictureBox.Image = Image.FromFile("Enemigo_abajo.png");
                enemigoPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                enemigoPictureBox.Size = new Size(20, 20);
                enemigoPictureBox.Location = new Point(random.Next(0, this.ClientSize.Width - 50),
                                                      random.Next(0, this.ClientSize.Height - 50));

                enemigos.Add(enemigoPictureBox);
                posicionesPrevias.Add(enemigoPictureBox.Location);
                this.Controls.Add(enemigoPictureBox);

                direcciones.Add(new Point(0, 0));
                CambiarDireccion(i);
            }

            lblVelocidad = new Label();
            lblVelocidad.Text = $"Velocidad: {enemigoVelocidad}";
            lblVelocidad.Font = new Font("Arial", 12, FontStyle.Bold);
            lblVelocidad.Location = new Point(10, 10);
            lblVelocidad.AutoSize = true;
            this.Controls.Add(lblVelocidad);

            lblTiempo = new Label();
            lblTiempo.Text = "Tiempo: 0s";
            lblTiempo.Font = new Font("Arial", 12, FontStyle.Bold);
            lblTiempo.Location = new Point(200, 10);
            lblTiempo.AutoSize = true;
            this.Controls.Add(lblTiempo);

            enemigoMovimientoTimer = new System.Windows.Forms.Timer();
            enemigoMovimientoTimer.Interval = 50;
            enemigoMovimientoTimer.Tick += new EventHandler(MoverEnemigos);
            enemigoMovimientoTimer.Start();

            cambioDireccionTimer = new System.Windows.Forms.Timer();
            cambioDireccionTimer.Interval = 3000;
            cambioDireccionTimer.Tick += new EventHandler((sender, e) => {
                for (int i = 0; i < enemigos.Count; i++)
                {
                    CambiarDireccion(i);
                }
            });
            cambioDireccionTimer.Start();

            juegoTimer = new System.Windows.Forms.Timer();
            juegoTimer.Interval = 1000;
            juegoTimer.Tick += new EventHandler(ActualizarTiempo);
            juegoTimer.Start();

            fuegoTimer = new System.Windows.Forms.Timer();
            fuegoTimer.Interval = duracionLlama;
            fuegoTimer.Tick += new EventHandler((sender, e) => {

            });
            fuegoTimer.Start();

            // Inicializar nodos del mapa
            nodos = new List<Point>();

            int gridSize = 100; // Tamaño de la cuadrícula
            for (int x = 0; x < this.ClientSize.Width; x += gridSize)
            {
                for (int y = 0; y < this.ClientSize.Height; y += gridSize)
                {
                    nodos.Add(new Point(x, y));
                }
            }

            // Añadir el evento de teclado para controlar el jugador
            this.KeyDown += new KeyEventHandler(Ventana_KeyDown);
        }




        // Función para encontrar el nodo más cercano a una posición actual
        private Point NodoMasCercano(Point posicionActual)
        {
            Point nodoCercano = nodos[0];
            double distanciaMinima = DistanciaEntrePuntos(posicionActual, nodos[0]);

            foreach (var nodo in nodos)
            {
                double distancia = DistanciaEntrePuntos(posicionActual, nodo);
                if (distancia < distanciaMinima)
                {
                    nodoCercano = nodo;
                    distanciaMinima = distancia;
                }
            }

            return nodoCercano;
        }

        // Función para calcular la distancia entre dos puntos
        private double DistanciaEntrePuntos(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }


        private void ColocarBomba()
        {
            PictureBox bomba = new PictureBox();
            bomba.Image = Image.FromFile("bomba.png"); // Ruta de la imagen de la bomba
            bomba.SizeMode = PictureBoxSizeMode.StretchImage;
            bomba.Size = new Size(20, 20);

            // Colocar en una ubicación aleatoria
            bomba.Location = new Point(random.Next(0, this.ClientSize.Width - bomba.Width), random.Next(0, this.ClientSize.Height - bomba.Height));

            bombas.Add(bomba);
            this.Controls.Add(bomba);

            // Timer para eliminar la bomba después de un tiempo si no se toca
            var eliminarBombaTimer = new System.Windows.Forms.Timer();
            eliminarBombaTimer.Interval = 5000; // 5 segundos, ajusta este valor según sea necesario
            eliminarBombaTimer.Tick += (s, e) =>
            {
                this.Controls.Remove(bomba);
                bombas.Remove(bomba);
                eliminarBombaTimer.Stop();
                eliminarBombaTimer.Dispose();
            };
            eliminarBombaTimer.Start();
        }



        private void ColocarLlama(Point posicion, bool esMovimientoVertical)
        {
            var delayTimer = new System.Windows.Forms.Timer();
            delayTimer.Interval = 100;
            delayTimer.Tick += (s, e) =>
            {
                PictureBox llamaPictureBox = new PictureBox();

                if (esMovimientoVertical)
                {
                    llamaPictureBox.Image = Image.FromFile("LlamaVertical.png");
                }
                else
                {
                    llamaPictureBox.Image = Image.FromFile("LlamaHorizontal.png");
                }

                llamaPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                llamaPictureBox.Size = new Size(8, 8);

                llamaPictureBox.Location = posicion;

                llamas.Add(llamaPictureBox);
                llamasRecientes.Add(llamaPictureBox);
                this.Controls.Add(llamaPictureBox);
                llamaPictureBox.SendToBack();

                var removerLlamaRecienteTimer = new System.Windows.Forms.Timer();
                removerLlamaRecienteTimer.Interval = 500;
                removerLlamaRecienteTimer.Tick += (s2, e2) =>
                {
                    llamasRecientes.Remove(llamaPictureBox);
                    removerLlamaRecienteTimer.Stop();
                    removerLlamaRecienteTimer.Dispose();
                };
                removerLlamaRecienteTimer.Start();

                var eliminarLlamaTimer = new System.Windows.Forms.Timer();
                eliminarLlamaTimer.Interval = duracionLlama;
                eliminarLlamaTimer.Tick += (s3, e3) =>
                {
                    this.Controls.Remove(llamaPictureBox);
                    llamas.Remove(llamaPictureBox);
                    eliminarLlamaTimer.Stop();
                    eliminarLlamaTimer.Dispose();
                };
                eliminarLlamaTimer.Start();

                delayTimer.Stop();
                delayTimer.Dispose();
            };
            delayTimer.Start();
        }

        private void MoverEnemigos(object sender, EventArgs e)
        {
            List<int> enemigosAEliminar = new List<int>();

            for (int i = 0; i < enemigos.Count; i++)
            {
                PictureBox enemigo = enemigos[i];
                Point direccion = direcciones[i];

                Point nuevaPosicion = new Point(
                    enemigo.Left + direccion.X * enemigoVelocidad,
                    enemigo.Top + direccion.Y * enemigoVelocidad
                );

                Rectangle nuevoRect = new Rectangle(nuevaPosicion, enemigo.Size);

                if (this.ClientRectangle.Contains(nuevoRect))
                {
                    enemigo.Location = nuevaPosicion;

                    bool esMovimientoVertical = direccion.Y != 0;

                    ColocarLlama(enemigo.Location, esMovimientoVertical);

                    for (int j = 0; j < enemigos.Count; j++)
                    {
                        if (i != j && enemigo.Bounds.IntersectsWith(enemigos[j].Bounds))
                        {
                            enemigosAEliminar.Add(i);
                            enemigosAEliminar.Add(j);
                        }
                    }

                    foreach (var llama in llamas)
                    {
                        if (!llamasRecientes.Contains(llama) && enemigo.Bounds.IntersectsWith(llama.Bounds))
                        {
                            enemigosAEliminar.Add(i);
                            break;
                        }
                    }
                }
                else
                {
                    CambiarDireccion(i); // Cambia la dirección si el enemigo está fuera del área visible
                }
            }

            MoverJugador(); // Asegúrate de que el jugador se mueve en cada tick

            enemigosAEliminar = enemigosAEliminar.Distinct().ToList();
            enemigosAEliminar.Sort((a, b) => b.CompareTo(a));

            foreach (int index in enemigosAEliminar)
            {
                if (index >= 0 && index < enemigos.Count) // Asegúrate de que el índice sea válido
                {
                    this.Controls.Remove(enemigos[index]);
                    enemigos.RemoveAt(index);
                    direcciones.RemoveAt(index);
                }
            }

            this.Invalidate();
        }

        private void MoverJugador()
        {
            Point posicionAnterior = jugadorMoto.Location;

            Point nuevaPosicion = new Point(
                jugadorMoto.Left + direccionJugador.X * enemigoVelocidad,
                jugadorMoto.Top + direccionJugador.Y * enemigoVelocidad
            );

            Rectangle nuevoRect = new Rectangle(nuevaPosicion, jugadorMoto.Size);

            if (this.ClientRectangle.Contains(nuevoRect))
            {
                jugadorMoto.Location = nuevaPosicion;

                bool esMovimientoVertical = direccionJugador.Y != 0;

                ColocarLlama(posicionAnterior, esMovimientoVertical);

                List<PictureBox> enemigosAEliminar = new List<PictureBox>();
                List<PictureBox> llamasAEliminar = new List<PictureBox>();
                List<PictureBox> bombasAEliminar = new List<PictureBox>(); // Lista para bombas

                foreach (var enemigo in enemigos)
                {
                    if (jugadorMoto.Bounds.IntersectsWith(enemigo.Bounds))
                    {
                        enemigosAEliminar.Add(enemigo);
                    }
                }

                foreach (var llama in llamas)
                {
                    if (!llamasRecientes.Contains(llama) && jugadorMoto.Bounds.IntersectsWith(llama.Bounds))
                    {
                        llamasAEliminar.Add(llama);
                    }
                }

                foreach (var bomba in bombas) // Verificar colisión con bombas
                {
                    if (jugadorMoto.Bounds.IntersectsWith(bomba.Bounds))
                    {
                        bombasAEliminar.Add(bomba);
                        MessageBox.Show("¡Bomba explotada! Fin del juego.");
                        Application.Exit(); // Cerrar la aplicación, puedes reemplazarlo con la lógica que prefieras
                    }
                }

                if (enemigosAEliminar.Count > 0 || llamasAEliminar.Count > 0 || bombasAEliminar.Count > 0)
                {
                    // Detener el juego y mostrar un solo mensaje de colisión
                    enemigoMovimientoTimer.Stop();
                    cambioDireccionTimer.Stop();
                    fuegoTimer.Stop();
                    juegoTimer.Stop();

                    // Eliminar enemigos, llamas y bombas colisionadas
                    foreach (var enemigo in enemigosAEliminar)
                    {
                        this.Controls.Remove(enemigo);
                        enemigos.Remove(enemigo);
                    }

                    foreach (var llama in llamasAEliminar)
                    {
                        this.Controls.Remove(llama);
                        llamas.Remove(llama);
                    }

                    foreach (var bomba in bombasAEliminar)
                    {
                        this.Controls.Remove(bomba);
                        bombas.Remove(bomba);
                    }

                    Application.Exit(); // Cerrar la aplicación
                }
            }
        }

        private void CambiarDireccion(int index)
        {
            int nuevaDireccion;
            int direccionActual = ObtenerDireccionActual(direcciones[index]);

            do
            {
                nuevaDireccion = random.Next(4);
            } while (nuevaDireccion == DireccionOpuesta(direccionActual));

            ActualizarDireccion(index, nuevaDireccion);
        }

        private void Ventana_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    direccionJugador = new Point(0, -1);
                    jugadorMoto.Image = Image.FromFile("moto_arriba.png");
                    break;
                case Keys.Down:
                    direccionJugador = new Point(0, 1);
                    jugadorMoto.Image = Image.FromFile("moto_abajo.png");
                    break;
                case Keys.Left:
                    direccionJugador = new Point(-1, 0);
                    jugadorMoto.Image = Image.FromFile("moto_izquierda.png");
                    break;
                case Keys.Right:
                    direccionJugador = new Point(1, 0);
                    jugadorMoto.Image = Image.FromFile("moto_derecha.png");
                    break;
            }
        }

        private int ObtenerDireccionActual(Point direccion)
        {
            if (direccion.X == 1 && direccion.Y == 0)
                return 0; // Derecha
            if (direccion.X == -1 && direccion.Y == 0)
                return 1; // Izquierda
            if (direccion.X == 0 && direccion.Y == 1)
                return 2; // Abajo
            return 3; // Arriba
        }

        private int DireccionOpuesta(int direccion)
        {
            if (direccion == 0) return 1;
            if (direccion == 1) return 0;
            if (direccion == 2) return 3;
            return 2;
        }

        private void ActualizarDireccion(int index, int nuevaDireccion)
        {
            switch (nuevaDireccion)
            {
                case 0: // Derecha
                    direcciones[index] = new Point(1, 0);
                    enemigos[index].Image = Image.FromFile("Enemigo_derecha.png");
                    break;
                case 1: // Izquierda
                    direcciones[index] = new Point(-1, 0);
                    enemigos[index].Image = Image.FromFile("Enemigo_izquierda.png");
                    break;
                case 2: // Abajo
                    direcciones[index] = new Point(0, 1);
                    enemigos[index].Image = Image.FromFile("Enemigo_abajo.png");
                    break;
                case 3: // Arriba
                    direcciones[index] = new Point(0, -1);
                    enemigos[index].Image = Image.FromFile("Enemigo_arriba.png");
                    break;
            }
        }

        private void ActualizarTiempo(object sender, EventArgs e)
        {
            tiempoJuego++;
            lblTiempo.Text = $"Tiempo: {tiempoJuego}s";
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "Ventana";
            this.Load += new System.EventHandler(this.Ventana_Load);
            this.ResumeLayout(false);
        }

        private void Ventana_Load(object sender, EventArgs e)
        {

        }
    }
}




