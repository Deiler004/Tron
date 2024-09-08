using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class Escudo
{
    private PictureBox escudo;
    private Random random;
    private Control parentControl;
    private List<PictureBox> escudos;
    private int duracion; // Duración en milisegundos que el escudo estará en el mapa

    public Escudo(Control parentControl, List<PictureBox> escudos, int duracion = 5000)
    {
        this.parentControl = parentControl;
        this.escudos = escudos;
        this.random = new Random();
        this.duracion = duracion;
        ColocarEscudo();
    }

    private void ColocarEscudo()
    {
        escudo = new PictureBox();
        escudo.Image = Image.FromFile("Escudo.png"); // Ruta de la imagen del escudo
        escudo.SizeMode = PictureBoxSizeMode.StretchImage;
        escudo.Size = new Size(20, 20);

        // Colocar en una ubicación aleatoria
        escudo.Location = new Point(random.Next(0, parentControl.ClientSize.Width - escudo.Width), random.Next(0, parentControl.ClientSize.Height - escudo.Height));

        escudos.Add(escudo);
        parentControl.Controls.Add(escudo);

        // Timer para eliminar el escudo después de un tiempo si no se toca
        var eliminarEscudoTimer = new System.Windows.Forms.Timer();
        eliminarEscudoTimer.Interval = duracion; // Duración en milisegundos
        eliminarEscudoTimer.Tick += (s, e) =>
        {
            parentControl.Controls.Remove(escudo);
            escudos.Remove(escudo);
            eliminarEscudoTimer.Stop();
            eliminarEscudoTimer.Dispose();
        };
        eliminarEscudoTimer.Start();
    }
}
