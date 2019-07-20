namespace MapEditor
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.tWidth = new System.Windows.Forms.TextBox();
            this.tHeight = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tSprite = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tTileWidth = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tTileHeight = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.bSave = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.tSaveLocation = new System.Windows.Forms.TextBox();
            this.pTilePreview = new System.Windows.Forms.Panel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(812, 584);
            this.panel1.TabIndex = 0;
            this.panel1.SizeChanged += new System.EventHandler(this.panel1_SizeChanged);
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(830, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Width";
            // 
            // tWidth
            // 
            this.tWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tWidth.Location = new System.Drawing.Point(830, 32);
            this.tWidth.Name = "tWidth";
            this.tWidth.Size = new System.Drawing.Size(132, 22);
            this.tWidth.TabIndex = 2;
            this.tWidth.Text = "320";
            // 
            // tHeight
            // 
            this.tHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tHeight.Location = new System.Drawing.Point(830, 77);
            this.tHeight.Name = "tHeight";
            this.tHeight.Size = new System.Drawing.Size(132, 22);
            this.tHeight.TabIndex = 4;
            this.tHeight.Text = "240";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(830, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Height";
            // 
            // tSprite
            // 
            this.tSprite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tSprite.Location = new System.Drawing.Point(830, 122);
            this.tSprite.Name = "tSprite";
            this.tSprite.Size = new System.Drawing.Size(132, 22);
            this.tSprite.TabIndex = 6;
            this.tSprite.Click += new System.EventHandler(this.tSprite_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(830, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Sprite Location";
            // 
            // tTileWidth
            // 
            this.tTileWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tTileWidth.Location = new System.Drawing.Point(830, 205);
            this.tTileWidth.Name = "tTileWidth";
            this.tTileWidth.Size = new System.Drawing.Size(132, 22);
            this.tTileWidth.TabIndex = 8;
            this.tTileWidth.Text = "16";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(830, 185);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "Tile Width";
            // 
            // tTileHeight
            // 
            this.tTileHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tTileHeight.Location = new System.Drawing.Point(830, 250);
            this.tTileHeight.Name = "tTileHeight";
            this.tTileHeight.Size = new System.Drawing.Size(132, 22);
            this.tTileHeight.TabIndex = 10;
            this.tTileHeight.Text = "16";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(827, 230);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 17);
            this.label5.TabIndex = 9;
            this.label5.Text = "Tile Height";
            // 
            // bSave
            // 
            this.bSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bSave.Location = new System.Drawing.Point(887, 573);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 23);
            this.bSave.TabIndex = 11;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // tSaveLocation
            // 
            this.tSaveLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tSaveLocation.Location = new System.Drawing.Point(830, 545);
            this.tSaveLocation.Name = "tSaveLocation";
            this.tSaveLocation.Size = new System.Drawing.Size(132, 22);
            this.tSaveLocation.TabIndex = 12;
            // 
            // pTilePreview
            // 
            this.pTilePreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pTilePreview.Location = new System.Drawing.Point(830, 150);
            this.pTilePreview.Margin = new System.Windows.Forms.Padding(0);
            this.pTilePreview.Name = "pTilePreview";
            this.pTilePreview.Size = new System.Drawing.Size(34, 34);
            this.pTilePreview.TabIndex = 13;
            this.pTilePreview.Click += new System.EventHandler(this.pTilePreview_Click);
            this.pTilePreview.Paint += new System.Windows.Forms.PaintEventHandler(this.pTilePreview_Paint);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(974, 608);
            this.Controls.Add(this.pTilePreview);
            this.Controls.Add(this.tSaveLocation);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.tTileHeight);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tTileWidth);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tSprite);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tHeight);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tWidth);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tWidth;
        private System.Windows.Forms.TextBox tHeight;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tSprite;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tTileWidth;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tTileHeight;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TextBox tSaveLocation;
        private System.Windows.Forms.Panel pTilePreview;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}