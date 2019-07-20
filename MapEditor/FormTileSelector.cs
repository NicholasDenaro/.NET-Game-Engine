using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapEditor
{
    public partial class FormTileSelector : Form
    {
        private Image[] images;
        private const int displaySize = 32;
        public Image Image { get; private set; }
        public byte ImageIndex { get; private set; }

        public FormTileSelector(Image[] images)
        {
            InitializeComponent();
            this.images = images;
            this.DialogResult = DialogResult.Abort;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            int x = 0;
            int y = 0;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            foreach(Image image in images)
            {
                e.Graphics.DrawImage(image, new Rectangle(x, y, displaySize, displaySize));
                x += displaySize;
                if (x + displaySize > this.panel1.Width)
                {
                    x = 0;
                    y += displaySize;
                }
            }
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            this.panel1.Refresh();
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            ImageIndex = (byte)(e.X / displaySize + e.Y / displaySize * (this.panel1.Width / displaySize));
            Image = images[ImageIndex];

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
