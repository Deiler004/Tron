public class Velocidad
{
    private PictureBox velocidadItem;
    private Random random;
    private Control parentControl;
    private List<PictureBox> itemsVelocidad;
    private int duracion; // Duración en milisegundos que el item estará en el mapa

    public Velocidad(Control parentControl, List<PictureBox> itemsVelocidad, int duracion = 10000)
    {
        this.parentControl = parentControl;
        this.itemsVelocidad = itemsVelocidad;
        this.random = new Random();
        this.duracion = duracion;
        ColocarVelocidad();
    }

    private void ColocarVelocidad()
    {
        velocidadItem = new PictureBox();
        velocidadItem.Image = Image.FromFile("Velocidad.png"); // Ruta de la imagen del item de velocidad
        velocidadItem.SizeMode = PictureBoxSizeMode.StretchImage;
        velocidadItem.Size = new Size(20, 20);

        // Colocar en una ubicación aleatoria
        velocidadItem.Location = new Point(
            random.Next(0, parentControl.ClientSize.Width - velocidadItem.Width),
            random.Next(0, parentControl.ClientSize.Height - velocidadItem.Height)
        );

        itemsVelocidad.Add(velocidadItem);
        parentControl.Controls.Add(velocidadItem);

        // Timer para eliminar el item después de un tiempo si no se toca
        var eliminarVelocidadTimer = new System.Windows.Forms.Timer();
        eliminarVelocidadTimer.Interval = duracion; // Duración en milisegundos
        eliminarVelocidadTimer.Tick += (s, e) =>
        {
            parentControl.Controls.Remove(velocidadItem);
            itemsVelocidad.Remove(velocidadItem);
            eliminarVelocidadTimer.Stop();
            eliminarVelocidadTimer.Dispose();
        };
        eliminarVelocidadTimer.Start();
    }
}

