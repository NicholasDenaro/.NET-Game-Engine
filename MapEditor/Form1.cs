using GameEngine._2D;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MapEditor
{
    public partial class Form1 : Form
    {
        private bool anyChanges;
        //private TileMap TileMap;
        private byte tile;
        private GameEngine.Location location;
        private GameEngine.Interfaces.IDrawer drawer;
        private bool isMouseDown;
        private Bitmap buffer;
        private Graphics gfx;

        public Form1()
        {
            InitializeComponent();
            this.anyChanges = false;
            tile = 1;
            this.buffer = new Bitmap(this.Width, this.Height);
            this.gfx = Graphics.FromImage(buffer);
            this.drawer = new Drawer2D(this.gfx);
            this.location = new GameEngine.Location(new TileMap(null, int.Parse(this.tWidth.Text), int.Parse(this.tHeight.Text)));
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            DrawLocation(e);
        }

        private void pTilePreview_Paint(object sender, PaintEventArgs e)
        {
            DrawTile(e);
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (anyChanges)
            {

            }
        }

        private void DrawLocation(PaintEventArgs e)
        {
            gfx.FillRectangle(Brushes.White, new Rectangle(new Point(), this.panel1.Size));
            gfx.DrawRectangle(Pens.Black, new Rectangle(new Point(), new Size(this.panel1.Size.Width - 1, this.panel1.Size.Height - 1)));

            location.Draw(this.drawer);

            e.Graphics.DrawImage(buffer, 0, 0);
        }

        private void DrawTile(PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(new Point(), new Size(this.pTilePreview.Size.Width - 1, this.pTilePreview.Size.Height - 1)));
            TileMap map = this.location.Description as TileMap;
            if (map.Sprite != null)
            {
                e.Graphics.DrawImage(map.Image(tile), new Rectangle(new Point(1, 1), new Size(this.pTilePreview.Size.Width - 1, this.pTilePreview.Size.Height - 1)));
            }
        }

        private void SaveFile()
        {
            if (this.saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.tSaveLocation.Text = Path.GetFileNameWithoutExtension(this.saveFileDialog1.FileName);

            using (FileStream stream = File.OpenWrite(this.saveFileDialog1.FileName))
            {
                byte[] data = GameEngine.Location.Save(this.location, this.tSprite.Text);
                stream.Write(data, 0, data.Length);
            }
        }

        private void panel1_SizeChanged(object sender, EventArgs e)
        {
            this.panel1.Refresh();
        }

        private void tSprite_Click(object sender, EventArgs e)
        {
            LoadSprite();
        }

        private void LoadSprite()
        {
            DialogResult result = this.openFileDialog1.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            this.location.Description = new TileMap(
                new Sprite(
                    Path.GetFileNameWithoutExtension(this.openFileDialog1.FileName),
                    this.openFileDialog1.FileName,
                    int.Parse(this.tTileWidth.Text),
                    int.Parse(this.tTileHeight.Text)),
                int.Parse(this.tWidth.Text),
                int.Parse(this.tHeight.Text));

            this.tSprite.Text = this.openFileDialog1.FileName;
            this.pTilePreview.Refresh();
        }

        private void pTilePreview_Click(object sender, EventArgs e)
        {
            SelectTile();
        }

        private void SelectTile()
        {
            TileMap map = this.location.Description as TileMap;
            if (map == null)
            {
                return;
            }

            FormTileSelector tileSelector = new FormTileSelector(map.Sprite.Images);
            if (tileSelector.ShowDialog() == DialogResult.OK)
            {
                tile = tileSelector.ImageIndex;
                this.pTilePreview.Refresh();
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            this.isMouseDown = true;
            FillTile(e);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            FillTile(e);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            this.isMouseDown = false;
        }

        private void FillTile(MouseEventArgs e)
        {
            if (!this.isMouseDown)
            {
                return;
            }
            TileMap description = this.location.Description as TileMap;
            if (description == null)
            {
                return;
            }

            if (e.Location.X > description.Width || e.Location.Y > description.Height)
            {
                return;
            }

            int x = e.X / description.Sprite.Width;
            int y = e.Y / description.Sprite.Height;
            if (description[x, y] != tile)
            {
                description[x, y] = tile;
                this.panel1.Refresh();
            }
        }
    }
}
