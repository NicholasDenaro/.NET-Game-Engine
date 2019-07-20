using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MapEditor
{
    public partial class Form1 : Form
    {
        private bool anyChanges;
        private GameEngine.TileMap TileMap;
        private byte tile;
        private GameEngine.Location location;
        private GameEngine.Interfaces.Drawer drawer;
        private bool isMouseDown;
        private Bitmap buffer;

        public Form1()
        {
            InitializeComponent();
            this.anyChanges = false;
            tile = 1;
            this.drawer = new GameEngine.Interfaces.Drawer();
            this.location = new GameEngine.Location(int.Parse(this.tWidth.Text), int.Parse(this.tHeight.Text));
            this.buffer = new Bitmap(this.Width, this.Height);
        }

        private void DrawEntity(Graphics gfx, GameEngine.Entity entity)
        {

        }

        private void DrawSprite(Graphics gfx, GameEngine.Sprite sprite, int index, int x, int y)
        {

        }

        private void DrawImage(Graphics gfx, Image image, int x, int y)
        {
            gfx.DrawImage(image, x, y);
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
            Graphics gfx = Graphics.FromImage(buffer);

            if (!this.drawer.IsSetup)
            {
                this.drawer.Setup(entity => DrawEntity(gfx, entity), (sprite, index, x, y) => DrawSprite(gfx, sprite, index, x, y), (image, x, y) => DrawImage(gfx, image, x, y));
            }

            gfx.FillRectangle(Brushes.White, new Rectangle(new Point(), this.panel1.Size));
            gfx.DrawRectangle(Pens.Black, new Rectangle(new Point(), new Size(this.panel1.Size.Width - 1, this.panel1.Size.Height - 1)));
            gfx.FillRectangle(this.location.BackgroundColor, new Rectangle(0, 0, this.location.Width, this.location.Height));

            location.Draw(this.drawer);

            e.Graphics.DrawImage(buffer, 0, 0);
        }

        private void DrawTile(PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(new Point(), new Size(this.pTilePreview.Size.Width - 1, this.pTilePreview.Size.Height - 1)));
            if (TileMap != null)
            {
                e.Graphics.DrawImage(TileMap.Image(tile), new Rectangle(new Point(1, 1), new Size(this.pTilePreview.Size.Width - 1, this.pTilePreview.Size.Height - 1)));
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
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(this.location.Width);
                writer.Write(this.location.Height);
                writer.Write(this.tSprite.Text);
                writer.Write(this.location.Map.Sprite.Width);
                writer.Write(this.location.Map.Sprite.Height);
                foreach(byte b in this.location.Map.Tiles)
                {
                    writer.Write(b);
                }
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

            TileMap = new GameEngine.TileMap(
                new GameEngine.Sprite(Path.GetFileNameWithoutExtension(this.openFileDialog1.FileName),
                this.openFileDialog1.FileName,
                byte.Parse(this.tTileWidth.Text),
                byte.Parse(this.tTileHeight.Text)));
            this.location.Map = TileMap;
            TileMap.Setup(this.location);
            this.tSprite.Text = this.openFileDialog1.FileName;
            this.pTilePreview.Refresh();
        }

        private void pTilePreview_Click(object sender, EventArgs e)
        {
            SelectTile();
        }

        private void SelectTile()
        {
            if (TileMap == null)
            {
                return;
            }

            FormTileSelector tileSelector = new FormTileSelector(TileMap.Sprite.Images);
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
            if (!this.isMouseDown || e.Location.X > this.location.Width || e.Location.Y > this.location.Height)
            {
                return;
            }

            int x = e.X / this.location.Map.Sprite.Width;
            int y = e.Y / this.location.Map.Sprite.Height;
            if (this.location.Map[x, y] != tile)
            {
                this.location.Map[x, y] = tile;
                this.panel1.Refresh();
            }
        }
    }
}
