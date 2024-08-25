﻿using System;
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
        private List<PictureBox> llamas; // Lista para las llamas
        private List<Point> direcciones;
        private int enemigoVelocidad;
        private int duracionLlama = 7000; // Duración de la llama en milisegundos
        private Random random;
        private Label lblVelocidad;
        private Label lblTiempo;
        private int tiempoJuego; // Tiempo de juego en segundos
        private List<Point> posicionesPrevias; // Lista para almacenar posiciones previas de los enemigos
        private List<PictureBox> llamasRecientes;

        public Ventana()
        {
            InitializeComponent();
            this.Width = 1080;
            this.Height = 720;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Inicializar la lista de llamas recientes
            llamasRecientes = new List<PictureBox>();

            random = new Random();
            enemigos = new List<PictureBox>();
            llamas = new List<PictureBox>();
            direcciones = new List<Point>();
            posicionesPrevias = new List<Point>(); // Inicializar la lista de posiciones previas

            enemigoVelocidad = random.Next(1, 11);

            for (int i = 0; i < 4; i++)
            {
                PictureBox enemigoPictureBox = new PictureBox();
                enemigoPictureBox.Image = Image.FromFile("Enemigo_abajo.png");
                enemigoPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                enemigoPictureBox.Size = new Size(20, 20);
                enemigoPictureBox.Location = new Point(random.Next(0, this.ClientSize.Width - 50),
                                                      random.Next(0, this.ClientSize.Height - 50));

                enemigos.Add(enemigoPictureBox);
                posicionesPrevias.Add(enemigoPictureBox.Location); // Guardar la posición inicial
                this.Controls.Add(enemigoPictureBox);

                direcciones.Add(new Point(0, 0));
                CambiarDireccion(i);
            }

            // Configurar los Labels
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

            // Timer para mover los enemigos
            enemigoMovimientoTimer = new System.Windows.Forms.Timer();
            enemigoMovimientoTimer.Interval = 50;
            enemigoMovimientoTimer.Tick += new EventHandler(MoverEnemigos);
            enemigoMovimientoTimer.Start();

            // Timer para cambiar la dirección de los enemigos
            cambioDireccionTimer = new System.Windows.Forms.Timer();
            cambioDireccionTimer.Interval = 3000;
            cambioDireccionTimer.Tick += new EventHandler((sender, e) => {
                for (int i = 0; i < enemigos.Count; i++)
                {
                    CambiarDireccion(i);
                }
            });
            cambioDireccionTimer.Start();

            // Timer para manejar el tiempo de juego
            juegoTimer = new System.Windows.Forms.Timer();
            juegoTimer.Interval = 1000; // 1 segundo
            juegoTimer.Tick += new EventHandler(ActualizarTiempo);
            juegoTimer.Start();

            // Timer para manejar las llamas
            fuegoTimer = new System.Windows.Forms.Timer();
            fuegoTimer.Interval = duracionLlama;
            fuegoTimer.Tick += new EventHandler((sender, e) => {

            });
            fuegoTimer.Start();
        }
        private void ColocarLlama(Point posicion, bool esMovimientoVertical)
        {
            var delayTimer = new System.Windows.Forms.Timer();
            delayTimer.Interval = 100; // Retraso en milisegundos (ajusta este valor según sea necesario)
            delayTimer.Tick += (s, e) =>
            {
                PictureBox llamaPictureBox = new PictureBox();

                // Cambiar la imagen según la dirección del movimiento
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

                // Colocar la llama en la posición proporcionada
                llamaPictureBox.Location = posicion;

                llamas.Add(llamaPictureBox);
                llamasRecientes.Add(llamaPictureBox); // Agregar la llama a la lista de llamas recientes
                this.Controls.Add(llamaPictureBox);
                llamaPictureBox.SendToBack(); // Asegurar que la llama quede detrás del enemigo

                // Eliminar la llama de las recientes después de un intervalo
                var removerLlamaRecienteTimer = new System.Windows.Forms.Timer();
                removerLlamaRecienteTimer.Interval = 500; // Tiempo para considerar la llama como "no reciente"
                removerLlamaRecienteTimer.Tick += (s2, e2) =>
                {
                    llamasRecientes.Remove(llamaPictureBox);
                    removerLlamaRecienteTimer.Stop();
                    removerLlamaRecienteTimer.Dispose();
                };
                removerLlamaRecienteTimer.Start();

                // Configurar temporizador para eliminar la llama
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



        private void Ventana_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < enemigos.Count; i++)
            {
                // Dibujar la imagen del enemigo directamente en la ventana
                e.Graphics.DrawImage(enemigos[i].Image, enemigos[i].Location);
            }
        }







        private void MoverEnemigos(object sender, EventArgs e)
        {
            // Crear una lista temporal para almacenar los enemigos que deben ser eliminados
            List<int> enemigosAEliminar = new List<int>();

            for (int i = 0; i < enemigos.Count; i++)
            {
                PictureBox enemigo = enemigos[i];
                Point direccion = direcciones[i];

                // Guardar la posición actual del enemigo antes de moverlo
                Point posicionAnterior = enemigo.Location;

                // Calcular la nueva posición
                Point nuevaPosicion = new Point(
                    enemigo.Left + direccion.X * enemigoVelocidad,
                    enemigo.Top + direccion.Y * enemigoVelocidad
                );

                Rectangle nuevoRect = new Rectangle(nuevaPosicion, enemigo.Size);

                if (this.ClientRectangle.Contains(nuevoRect))
                {
                    enemigo.Location = nuevaPosicion;

                    // Determinar si el movimiento es vertical u horizontal
                    bool esMovimientoVertical = direccion.Y != 0;

                    // Colocar la llama en la posición anterior del enemigo
                    ColocarLlama(posicionAnterior, esMovimientoVertical);

                    // Verificar colisión con otros enemigos
                    for (int j = 0; j < enemigos.Count; j++)
                    {
                        if (i != j && enemigo.Bounds.IntersectsWith(enemigos[j].Bounds))
                        {
                            enemigosAEliminar.Add(i);
                            enemigosAEliminar.Add(j);
                        }
                    }

                    // Verificar colisión con llamas, excluyendo las llamas recientes
                    foreach (var llama in llamas)
                    {
                        if (!llamasRecientes.Contains(llama) && enemigo.Bounds.IntersectsWith(llama.Bounds))
                        {
                            enemigosAEliminar.Add(i);
                            break; // No es necesario seguir comprobando más llamas si ya hay una colisión
                        }
                    }
                }
                else
                {
                    CambiarDireccion(i);
                }
            }

            // Eliminar enemigos que colisionaron
            enemigosAEliminar = enemigosAEliminar.Distinct().ToList(); // Eliminar duplicados
            enemigosAEliminar.Sort((a, b) => b.CompareTo(a)); // Ordenar de mayor a menor para evitar problemas al eliminar

            foreach (int index in enemigosAEliminar)
            {
                this.Controls.Remove(enemigos[index]);
                enemigos.RemoveAt(index);
                direcciones.RemoveAt(index);
            }

            this.Invalidate(); // Forzar repintado del formulario
        }

        private void CambiarDireccion(int index)
        {
            int nuevaDireccion;
            int direccionActual = ObtenerDireccionActual(direcciones[index]);

            do
            {
                nuevaDireccion = random.Next(4); // 0: Derecha, 1: Izquierda, 2: Abajo, 3: Arriba
            } while (nuevaDireccion == DireccionOpuesta(direccionActual));

            ActualizarDireccion(index, nuevaDireccion);
        }

        private int ObtenerDireccionActual(Point direccion)
        {
            if (direccion.X == 1 && direccion.Y == 0) return 0; // Derecha
            if (direccion.X == -1 && direccion.Y == 0) return 1; // Izquierda
            if (direccion.X == 0 && direccion.Y == 1) return 2; // Abajo
            if (direccion.X == 0 && direccion.Y == -1) return 3; // Arriba
            return -1;
        }

        private int DireccionOpuesta(int direccion)
        {
            switch (direccion)
            {
                case 0: return 1; // Derecha => Opuesta es Izquierda
                case 1: return 0; // Izquierda => Opuesta es Derecha
                case 2: return 3; // Abajo => Opuesta es Arriba
                case 3: return 2; // Arriba => Opuesta es Abajo
                default: return -1;
            }
        }

        private void ActualizarDireccion(int index, int nuevaDireccion)
        {
            switch (nuevaDireccion)
            {
                case 0:
                    direcciones[index] = new Point(1, 0); // Derecha
                    enemigos[index].Image = Image.FromFile("Enemigo_derecha.png");
                    break;
                case 1:
                    direcciones[index] = new Point(-1, 0); // Izquierda
                    enemigos[index].Image = Image.FromFile("Enemigo_izquierda.png");
                    break;
                case 2:
                    direcciones[index] = new Point(0, 1); // Abajo
                    enemigos[index].Image = Image.FromFile("Enemigo_abajo.png");
                    break;
                case 3:
                    direcciones[index] = new Point(0, -1); // Arriba
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
            this.ClientSize = new System.Drawing.Size(1080, 720);
            this.Name = "Tron";
            this.Load += new System.EventHandler(this.Ventana_Load);
            this.ResumeLayout(false);
        }

        private void Ventana_Load(object sender, EventArgs e)
        {
        }
    }
}



