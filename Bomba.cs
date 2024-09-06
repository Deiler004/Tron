using System;
using System.Drawing;
using System.Windows.Forms;

public class Bomba
{
    private PictureBox bomba;
    private Form formulario;
    private Random random = new Random();
    private List<PictureBox> bombas; // Lista de bombas en el formulario principal

    public Bomba(Form form)
    {
        this.formulario = form;
        this.bombas = new List<PictureBox>();

        bomba = new PictureBox();
        bomba.Image = Image.FromFile("Bomba.png"); // Asegúrate de que la ruta sea correcta
        bomba.SizeMode = PictureBoxSizeMode.StretchImage;
        bomba.Size = new Size(20, 20);

        // Colocar en una ubicación aleatoria
        bomba.Location = new Point(random.Next(0, this.formulario.ClientSize.Width - bomba.Width),
                                  random.Next(0, this.formulario.ClientSize.Height - bomba.Height));

        this.bombas.Add(bomba);
        this.formulario.Controls.Add(bomba);

        // Timer para eliminar la bomba después de un tiempo si no se toca
        System.Windows.Forms.Timer eliminarBombaTimer = new System.Windows.Forms.Timer();
        eliminarBombaTimer.Interval = 100000; // 5 segundos, ajusta este valor según sea necesario
        eliminarBombaTimer.Tick += (s, e) =>
        {
            this.formulario.Controls.Remove(bomba);
            this.bombas.Remove(bomba);
            eliminarBombaTimer.Stop();
            eliminarBombaTimer.Dispose();
        };
        eliminarBombaTimer.Start();
    }

    public void DetectarColision(PictureBox moto)
    {
        if (bomba.Bounds.IntersectsWith(moto.Bounds))
        {
            // Aquí defines lo que ocurre cuando la bomba es tocada
            bomba.Visible = false; // Oculta la bomba o realiza otras acciones
        }
    }
}
