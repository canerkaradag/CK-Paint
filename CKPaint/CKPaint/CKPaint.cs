using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CKPaint
{    
    /*
        Created by Caner Karadag on 19.05.15.
    */    
    public partial class CKPaint : Form
    {
        
        #region Variables
        static public float penSize = 3;
        private Bitmap iniImg, changedImg = null, SelectedImage = null;
        private Graphics graphics = null;
        private Pen p = new Pen(Color.Black, penSize);
        private Point start = new Point(0, 0);
        private Point end = new Point(0, 0);
        private Rectangle rect;
        private Color colorFront, colorBack, colorActive;
        public string title = "";
        private int X0, Y0, X1, Y1;
        private bool SelectingArea = false, MadeSelection = false, drawing = false, drawSquare = false, drawRectangle = false, drawCircle = false;
        #endregion

        private static CKPaint instance;
        public CKPaint()
        {
            InitializeComponent();
            colorFront = Color.Black;
            colorBack = Color.White;
            colorActive = Color.Black;
            picBoxPen.BackColor = (Color)ColorTranslator.FromHtml("#cee5fc");
            btnColor1.BackColor = (Color)ColorTranslator.FromHtml("#c9e0f7");
            picLine2.BackColor = (Color)ColorTranslator.FromHtml("#cee5fc");
            toolTip();
        }
        public static CKPaint Instance()
        {
            if (instance == null)
            {
                instance = new CKPaint();
            }
            return instance;
        }

        #region FileOptions
        public void SaveFile()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Farklı Kaydet";
            sfd.FileName = title;
            sfd.Filter = "PNG (*.png)|*.png|BMP (*.bmp)|*.bmp|JPEG (*.jpg;*.jif;*.jpeg)|*.jpg;*.jif;*.jpeg";
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                switch (sfd.FilterIndex)
                {
                    case 1:
                        changedImg.Save(sfd.FileName, ImageFormat.Png);
                        break;

                    case 2:
                        changedImg.Save(sfd.FileName, ImageFormat.Jpeg);
                        break;

                    case 3:
                        changedImg.Save(sfd.FileName, ImageFormat.Bmp);
                        break;
                }
            }
        }
        public void NewFile(int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);
            graphics = Graphics.FromImage(bmp);
            graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, w, h);
            drawingPanel.Size = new System.Drawing.Size(w, h);
            NewImage(bmp);
        }
        private void OpenFile()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.RestoreDirectory = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Bitmap img = new Bitmap(ofd.FileName);
                    NewImage((Bitmap)Bitmap.FromFile(ofd.FileName));
                    drawingPanel.Height = img.Height;
                    drawingPanel.Width = img.Width;
                    changedImg = new Bitmap(drawingPanel.Image);
                    this.KeyPreview = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Görüntüyü yüklerken bir hata oluştu. Bunu göremiyorsanız 24bpp görüntüleri ile deneyin.", "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void NewImage(Bitmap bmp)
        {
            iniImg = bmp;
            changedImg = (Bitmap)iniImg.Clone();
            drawingPanel.Image = changedImg;
            graphics = Graphics.FromImage(changedImg);
            changedImg = new Bitmap(drawingPanel.Image);
            drawingPanel.Refresh();
            this.KeyPreview = true;
        }
        #endregion

        #region Menu
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewPage np = new NewPage();
            np.ShowDialog();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (graphics != null)
            {
                if (MessageBox.Show(title + " dosyasında yapılan değişiklikleri  kaydetmek istitor musunuz?", "CKPaint", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SaveFile();
                }
                else
                {
                    Application.Exit();
                }
            }
            else
            {
                Application.Exit();
            }
        }
        #endregion

        #region DrawingPanel
        private void drawingPanel_MouseDown(object sender, MouseEventArgs e)
        {
            start = e.Location;
            if (picBoxChoice.BackColor == (Color)ColorTranslator.FromHtml("#cee5fc"))
            {
                if (e.Button == MouseButtons.Left)
                {
                    SelectingArea = true;
                    X0 = e.X;
                    Y0 = e.Y;
                    SelectedImage = new Bitmap(drawingPanel.Image);
                    graphics = Graphics.FromImage(SelectedImage);
                    drawingPanel.Image = SelectedImage;
                }
            }
            else if (picBoxCircle.BackColor == (Color)ColorTranslator.FromHtml("#edf4fc") && drawCircle)
            {
                SolidBrush sb = new SolidBrush(colorActive);
                if (penSize > 15)
                {
                    graphics.FillEllipse(sb, e.X, e.Y, penSize, penSize);
                }
                else
                {
                    graphics.FillEllipse(sb, e.X, e.Y, 15, 15);
                }
                drawCircle = false;
                drawing = false;
            }

            else if (picBoxRectangle.BackColor == (Color)ColorTranslator.FromHtml("#edf4fc") && drawRectangle)
            {
                SolidBrush sb = new SolidBrush(colorActive);
                if (penSize > 15)
                {
                    graphics.FillRectangle(sb, e.X, e.Y, penSize, penSize);
                }
                else
                {
                    graphics.FillRectangle(sb, e.X, e.Y, 2 * 15, 15);
                }
                drawSquare = false;
                drawing = false;
            }
            else if (picBoxSquare.BackColor == (Color)ColorTranslator.FromHtml("#edf4fc") && drawSquare)
            {
                SolidBrush sb = new SolidBrush(colorActive);
                if (penSize > 15)
                {
                    graphics.FillRectangle(sb, e.X, e.Y, penSize, penSize);
                }
                else
                {
                    graphics.FillRectangle(sb, e.X, e.Y, 15, 15);
                }
                drawSquare = false;
                drawing = false;
            }
            else if (picBoxTriangle.BackColor == (Color)ColorTranslator.FromHtml("#edf4fc"))
            {
                SolidBrush sb = new SolidBrush(colorActive);
                if (penSize > 15)
                {
                    graphics.FillRectangle(sb, e.X, e.Y, penSize, penSize);
                }
                else
                {
                    graphics.FillRectangle(sb, e.X, e.Y, 15, 15);
                }
                drawSquare = false;
                drawing = false;
            }
            else
            {
                if (e.Button == MouseButtons.Left)
                {
                    drawing = true;
                    if (picBoxRubber.BackColor == (Color)ColorTranslator.FromHtml("#cee5fc"))
                    {
                        p = new Pen(Color.White, penSize);
                    }
                    else
                    {
                        p = new Pen(picBoxColor1.BackColor, penSize);
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    drawing = true;
                    if (picBoxRubber.BackColor == (Color)ColorTranslator.FromHtml("#cee5fc"))
                    {
                        p = new Pen(Color.White, penSize);
                    }
                    else
                    {
                        p = new Pen(picBoxColor2.BackColor, penSize);
                    }
                }
                if (penSize > 8)
                {
                    picLine1.BackColor = Color.Transparent;
                    picLine2.BackColor = Color.Transparent;
                    picLine3.BackColor = Color.Transparent;
                    picLine4.BackColor = Color.Transparent;
                }
                panelLine.Visible = false;
            }
        }
        private void drawingPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (picBoxChoice.BackColor == (Color)ColorTranslator.FromHtml("#cee5fc"))
            {
                if (!SelectingArea) return;
                X1 = e.X;
                Y1 = e.Y;
                graphics.DrawImage(changedImg, 0, 0);
                using (Pen select_pen = new Pen(Color.Gray))
                {
                    select_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    rect = MakeRectangle(X0, Y0, X1, Y1);
                    graphics.DrawRectangle(select_pen, rect);
                }
            }
            else
            {
                if (drawing)
                {
                    end = e.Location;
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.DrawEllipse(p, rect);
                    graphics.DrawLine(p, start, end);
                    changedImg = new Bitmap(drawingPanel.Image);
                    drawingPanel.Refresh();
                }
                start = end;
            }
        }
        private void drawingPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (picBoxChoice.BackColor == (Color)ColorTranslator.FromHtml("#cee5fc"))
            {
                if (!SelectingArea)
                    return;
                SelectingArea = false;
                rect = MakeRectangle(X0, Y0, X1, Y1);
                MadeSelection = (
                    (rect.Width > 0) &&
                    (rect.Height > 0));
            }
            drawing = false;
        }
        #endregion

        #region ChangeColor
        private void picBoxBlack_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.Black, e);
        }
        private void picBoxDarkGray_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.Gray, e);
        }
        private void picBoxMaroon_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.Maroon, e);
        }
        private void picBoxOlive_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.Olive, e);
        }
        private void picBoxGreen_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.Green, e);
        }
        private void picBoxTeal_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.Teal, e);
        }
        private void picBoxNavy_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.Navy, e);
        }
        private void picBoxPurple_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.Purple, e);
        }
        private void picBox192_192_0_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.FromArgb(192, 192, 0), e);
        }
        private void picBox64_64_64_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.FromArgb(64, 64, 64), e);
        }
        private void picBox128_128_255_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.FromArgb(128, 128, 255), e);
        }
        private void picBox255_128_0_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.FromArgb(255, 128, 0), e);
        }
        private void picBoxWhite_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.White, e);
        }
        private void picBoxLightGray_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.Gray, e);
        }
        private void picBoxRed_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.Red, e);
        }
        private void picBoxYellow_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.Yellow, e);
        }
        private void picBoxLime_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.Lime, e);
        }
        private void picBoxCyan_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.Cyan, e);
        }
        private void picBoxBlue_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.Blue, e);
        }
        private void picBoxFuchsia_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.Fuchsia, e);
        }
        private void picBox255_255_192_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.FromArgb(255, 255, 192), e);
        }
        private void picBox192_255_192_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.FromArgb(192, 255, 192), e);
        }
        private void picBox192_255_255_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.FromArgb(192, 255, 255), e);
        }
        private void picBox255_192_128_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeColor(Color.FromArgb(255, 192, 128), e);
        }
        private void picBoxTrans1_MouseDown(object sender, MouseEventArgs e)
        {
            RgbColor(picBoxTrans1, sender, e);
        }
        private void picBoxTrans2_MouseDown(object sender, MouseEventArgs e)
        {
            RgbColor(picBoxTrans2, sender, e);
        }
        private void picBoxTrans3_MouseDown(object sender, MouseEventArgs e)
        {
            RgbColor(picBoxTrans3, sender, e);
        }
        private void picBoxTrans4_MouseDown(object sender, MouseEventArgs e)
        {
            RgbColor(picBoxTrans4, sender, e);
        }
        private void picBoxTrans5_MouseDown(object sender, MouseEventArgs e)
        {
            RgbColor(picBoxTrans5, sender, e);
        }
        private void picBoxTrans6_MouseDown(object sender, MouseEventArgs e)
        {
            RgbColor(picBoxTrans6, sender, e);
        }
        private void picBoxTrans7_MouseDown(object sender, MouseEventArgs e)
        {
            RgbColor(picBoxTrans7, sender, e);
        }
        private void picBoxTrans8_MouseDown(object sender, MouseEventArgs e)
        {
            RgbColor(picBoxTrans8, sender, e);
        }
        private void picBoxTrans9_MouseDown(object sender, MouseEventArgs e)
        {
            RgbColor(picBoxTrans9, sender, e);
        }
        private void picBoxTrans10_MouseDown(object sender, MouseEventArgs e)
        {
            RgbColor(picBoxTrans10, sender, e);
        }
        private void picBoxTrans11_MouseDown(object sender, MouseEventArgs e)
        {
            RgbColor(picBoxTrans11, sender, e);
        }
        private void picBoxTrans12_MouseDown(object sender, MouseEventArgs e)
        {
            RgbColor(picBoxTrans12, sender, e);
        }    
        #endregion

        #region Buttons
        int value = 0;
        private void picBoxColor1_Click(object sender, EventArgs e)
        {
            btnChangeColor1();
        }
        private void picBoxColor2_Click(object sender, EventArgs e)
        {
            btnChangeColor2();
        }
        private void btnColor1_Click(object sender, EventArgs e)
        {
            btnChangeColor1();
        }
        private void btnColor2_Click(object sender, EventArgs e)
        {
            btnChangeColor2();
        }
        private void picBoxChoice_Click(object sender, EventArgs e)
        {
            if (picBoxChoice.BackColor != (Color)ColorTranslator.FromHtml("#cee5fc"))
            {
                picBoxChoice.BackColor = (Color)ColorTranslator.FromHtml("#cee5fc");
                picBoxPen.BackColor = Color.Transparent;
                picBoxRubber.BackColor = Color.Transparent;
            }
            else
            {
                picBoxChoice.BackColor = Color.Transparent;
            }
        }
        private void kopyalaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyToClipboard(rect);
            System.Media.SystemSounds.Beep.Play();
        }
        private void yapistirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsImage()) return;
            Image clipboard_image = Clipboard.GetImage();
            int cx = rect.X + (rect.Width - clipboard_image.Width) / 2;
            int cy = rect.Y + (rect.Height - clipboard_image.Height) / 2;
            Rectangle dest_rect = new Rectangle(
                cx, cy,
                clipboard_image.Width,
                clipboard_image.Height);

            using (Graphics gr = Graphics.FromImage(changedImg))
            {
                gr.DrawImage(clipboard_image, dest_rect);
            }
            drawingPanel.Image = changedImg;
            drawingPanel.Refresh();
            SelectedImage = null;
            //graphics = null;
            MadeSelection = false;
        }
        private void kesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CopyToClipboard(rect);
            using (Graphics gr = Graphics.FromImage(changedImg))
            {
                using (SolidBrush br = new SolidBrush(drawingPanel.BackColor))
                {
                    gr.FillRectangle(br, rect);
                }
            }
            SelectedImage = new Bitmap(changedImg);
            drawingPanel.Image = SelectedImage;
            EnableMenuItems();
            SelectedImage = null;
            MadeSelection = false;
            System.Media.SystemSounds.Beep.Play();
        }
        private void colorDialog()
        {
            panelLine.Visible = false;
            bool checkColor = false;
            Color[] arrayColor = new Color[12];
            ColorDialog colorDialog1 = new ColorDialog();
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (value > 11)
                {
                    picBoxTrans1.BackColor = picBoxTrans2.BackColor;
                    picBoxTrans2.BackColor = picBoxTrans3.BackColor;
                    picBoxTrans3.BackColor = picBoxTrans4.BackColor;
                    picBoxTrans4.BackColor = picBoxTrans5.BackColor;
                    picBoxTrans5.BackColor = picBoxTrans6.BackColor;
                    picBoxTrans6.BackColor = picBoxTrans7.BackColor;
                    picBoxTrans7.BackColor = picBoxTrans8.BackColor;
                    picBoxTrans8.BackColor = picBoxTrans9.BackColor;
                    picBoxTrans9.BackColor = picBoxTrans10.BackColor;
                    picBoxTrans10.BackColor = picBoxTrans11.BackColor;
                    picBoxTrans11.BackColor = picBoxTrans12.BackColor;
                    picBoxTrans12.BackColor = colorDialog1.Color;
                    if (btnColor2.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
                    {
                        picBoxColor2.BackColor = colorDialog1.Color;
                    }
                    else
                    {
                        picBoxColor1.BackColor = colorDialog1.Color;
                    }
                }
                foreach (Control c in this.Controls)
                {
                    if (c is Panel)
                    {
                        foreach (Control k in c.Controls)
                        {
                            if (k.Name == "panelColors")
                            {
                                foreach (Control r in k.Controls)
                                {
                                    if (r is Panel)
                                    {
                                        int g = 0;
                                        foreach (Control y in r.Controls)
                                        {
                                            arrayColor[g] = y.BackColor;
                                            g++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                for (int k = 0; k < arrayColor.Length; k++)
                {
                    if (arrayColor[k] != Color.Transparent)
                    {
                        if (arrayColor[k] == colorDialog1.Color)
                        {
                            checkColor = true;
                            break;
                        }
                    }
                }
                if (checkColor == false)
                {
                    if (picBoxTrans1.BackColor == Color.Transparent && value == 0)
                    {
                        picBoxTrans1.BackColor = colorDialog1.Color;
                        if (btnColor2.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
                        {
                            picBoxColor2.BackColor = colorDialog1.Color;
                        }
                        else
                        {
                            picBoxColor1.BackColor = colorDialog1.Color;
                        }
                    }
                    if (picBoxTrans2.BackColor == Color.Transparent && value == 1)
                    {
                        picBoxTrans2.BackColor = colorDialog1.Color;
                        if (btnColor2.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
                        {
                            picBoxColor2.BackColor = colorDialog1.Color;
                        }
                        else
                        {
                            picBoxColor1.BackColor = colorDialog1.Color;
                        }
                    }
                    if (picBoxTrans3.BackColor == Color.Transparent && value == 2)
                    {
                        picBoxTrans3.BackColor = colorDialog1.Color;
                        if (btnColor2.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
                        {
                            picBoxColor2.BackColor = colorDialog1.Color;
                        }
                        else
                        {
                            picBoxColor1.BackColor = colorDialog1.Color;
                        }
                    }
                    if (picBoxTrans4.BackColor == Color.Transparent && value == 3)
                    {
                        picBoxTrans4.BackColor = colorDialog1.Color;
                        if (btnColor2.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
                        {
                            picBoxColor2.BackColor = colorDialog1.Color;
                        }
                        else
                        {
                            picBoxColor1.BackColor = colorDialog1.Color;
                        }
                    }
                    if (picBoxTrans5.BackColor == Color.Transparent && value == 4)
                    {
                        picBoxTrans5.BackColor = colorDialog1.Color;
                        if (btnColor2.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
                        {
                            picBoxColor2.BackColor = colorDialog1.Color;
                        }
                        else
                        {
                            picBoxColor1.BackColor = colorDialog1.Color;
                        }
                    }
                    if (picBoxTrans6.BackColor == Color.Transparent && value == 5)
                    {
                        picBoxTrans6.BackColor = colorDialog1.Color;
                        if (btnColor2.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
                        {
                            picBoxColor2.BackColor = colorDialog1.Color;
                        }
                        else
                        {
                            picBoxColor1.BackColor = colorDialog1.Color;
                        }
                    }
                    if (picBoxTrans7.BackColor == Color.Transparent && value == 6)
                    {
                        picBoxTrans7.BackColor = colorDialog1.Color;
                        if (btnColor2.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
                        {
                            picBoxColor2.BackColor = colorDialog1.Color;
                        }
                        else
                        {
                            picBoxColor1.BackColor = colorDialog1.Color;
                        }
                    }
                    if (picBoxTrans8.BackColor == Color.Transparent && value == 7)
                    {
                        picBoxTrans8.BackColor = colorDialog1.Color;
                        if (btnColor2.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
                        {
                            picBoxColor2.BackColor = colorDialog1.Color;
                        }
                        else
                        {
                            picBoxColor1.BackColor = colorDialog1.Color;
                        }
                    }
                    if (picBoxTrans9.BackColor == Color.Transparent && value == 8)
                    {
                        picBoxTrans9.BackColor = colorDialog1.Color;
                        if (btnColor2.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
                        {
                            picBoxColor2.BackColor = colorDialog1.Color;
                        }
                        else
                        {
                            picBoxColor1.BackColor = colorDialog1.Color;
                        }
                    }
                    if (picBoxTrans10.BackColor == Color.Transparent && value == 9)
                    {
                        picBoxTrans10.BackColor = colorDialog1.Color;
                        if (btnColor2.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
                        {
                            picBoxColor2.BackColor = colorDialog1.Color;
                        }
                        else
                        {
                            picBoxColor1.BackColor = colorDialog1.Color;
                        }
                    }
                    if (picBoxTrans11.BackColor == Color.Transparent && value == 10)
                    {
                        picBoxTrans11.BackColor = colorDialog1.Color;
                        if (btnColor2.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
                        {
                            picBoxColor2.BackColor = colorDialog1.Color;
                        }
                        else
                        {
                            picBoxColor1.BackColor = colorDialog1.Color;
                        }
                    }
                    if (picBoxTrans12.BackColor == Color.Transparent && value == 11)
                    {
                        picBoxTrans12.BackColor = colorDialog1.Color;
                        if (btnColor2.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
                        {
                            picBoxColor2.BackColor = colorDialog1.Color;
                        }
                        else
                        {
                            picBoxColor1.BackColor = colorDialog1.Color;
                        }
                    }
                    value++;
                }
            }
        }
        private void btnEditColors_Click(object sender, EventArgs e)
        {
            colorDialog();
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            colorDialog();
        }
        private void picBoxReSize_MouseDown(object sender, MouseEventArgs e)
        {
            if (panelLine.Visible == false)
            {
                panelLine.Visible = true;
            }
            else
            {
                panelLine.Visible = false;
            }
        }
        private void leftPanel_MouseDown(object sender, MouseEventArgs e)
        {
            panelLine.Visible = false;
        }
        private void panelColors_MouseDown(object sender, MouseEventArgs e)
        {
            panelLine.Visible = false;
        }
        private void picBox_MouseHover(PictureBox pB)
        {
            if (pB.BackColor != (Color)ColorTranslator.FromHtml("#cee5fc"))
            {
                pB.BackColor = (Color)ColorTranslator.FromHtml("#edf4fc");
                picBoxChoice.BackColor = Color.Transparent;
            }
        }
        private void picBox_MouseLeave(PictureBox pB1, PictureBox pB2, PictureBox pB3)
        {
            if (pB1.BackColor != (Color)ColorTranslator.FromHtml("#cee5fc"))
            {
                pB1.BackColor = Color.Transparent;
            }
            if (pB2.BackColor != (Color)ColorTranslator.FromHtml("#cee5fc"))
            {
                pB2.BackColor = Color.Transparent;
            }
            if (pB3.BackColor != (Color)ColorTranslator.FromHtml("#cee5fc"))
            {
                pB3.BackColor = Color.Transparent;
            }
            picBoxChoice.BackColor = Color.Transparent;
        }
        private void picBox_MouseLeave(PictureBox pB1, PictureBox pB2, PictureBox pB3, PictureBox pB4)
        {
            if (pB1.BackColor != (Color)ColorTranslator.FromHtml("#cee5fc"))
            {
                pB1.BackColor = Color.Transparent;
            }
            if (pB2.BackColor != (Color)ColorTranslator.FromHtml("#cee5fc"))
            {
                pB2.BackColor = Color.Transparent;
            }
            if (pB3.BackColor != (Color)ColorTranslator.FromHtml("#cee5fc"))
            {
                pB3.BackColor = Color.Transparent;
            }
            if (pB4.BackColor != (Color)ColorTranslator.FromHtml("#cee5fc"))
            {
                pB4.BackColor = Color.Transparent;
            }
            picBoxChoice.BackColor = Color.Transparent;
        }
        private void picLine1_MouseHover(object sender, EventArgs e)
        {
            picBox_MouseHover(picLine1);
        }
        private void picLine1_MouseLeave(object sender, EventArgs e)
        {
            picBox_MouseLeave(picLine2, picLine3, picLine4);
        }
        private void picLine1_Click(object sender, EventArgs e)
        {
            penSize = 1;
            picLine1.BackColor = (Color)ColorTranslator.FromHtml("#cee5fc");
            picLine2.BackColor = Color.Transparent;
            picLine3.BackColor = Color.Transparent;
            picLine4.BackColor = Color.Transparent;
            if (btnColor1.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
            {
                p = new Pen(picBoxColor1.BackColor, penSize);
            }
            else if (btnColor2.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
            {
                p = new Pen(picBoxColor2.BackColor, penSize);
            }
        }
        private void picLine2_MouseHover(object sender, EventArgs e)
        {
            picBox_MouseHover(picLine2);
        }
        private void picLine2_MouseLeave(object sender, EventArgs e)
        {
            picBox_MouseLeave(picLine1, picLine3, picLine4);
        }
        private void picLine2_Click(object sender, EventArgs e)
        {
            penSize = 3;
            picLine1.BackColor = Color.Transparent;
            picLine2.BackColor = (Color)ColorTranslator.FromHtml("#cee5fc");
            picLine3.BackColor = Color.Transparent;
            picLine4.BackColor = Color.Transparent;
            if (btnColor1.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
            {
                p = new Pen(picBoxColor1.BackColor, penSize);
            }
            else if (btnColor2.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
            {
                p = new Pen(picBoxColor2.BackColor, penSize);
            }
        }
        private void picLine3_MouseHover(object sender, EventArgs e)
        {
            picBox_MouseHover(picLine3);
        }
        private void picLine3_MouseLeave(object sender, EventArgs e)
        {
            picBox_MouseLeave(picLine2, picLine1, picLine4);
        }
        private void picLine3_Click(object sender, EventArgs e)
        {
            penSize = 5;
            picLine1.BackColor = Color.Transparent;
            picLine2.BackColor = Color.Transparent;
            picLine3.BackColor = (Color)ColorTranslator.FromHtml("#cee5fc");
            picLine4.BackColor = Color.Transparent;
            if (btnColor1.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
            {
                p = new Pen(picBoxColor1.BackColor, penSize);
            }
            else if (btnColor2.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
            {
                p = new Pen(picBoxColor2.BackColor, penSize);
            }
        }
        private void picLine4_MouseHover(object sender, EventArgs e)
        {
            picBox_MouseHover(picLine4);
        }
        private void picLine4_MouseLeave(object sender, EventArgs e)
        {
            picBox_MouseLeave(picLine2, picLine3, picLine1);
        }
        private void picLine4_Click(object sender, EventArgs e)
        {
            penSize = 8;
            picLine1.BackColor = Color.Transparent;
            picLine2.BackColor = Color.Transparent;
            picLine3.BackColor = Color.Transparent;
            picLine4.BackColor = (Color)ColorTranslator.FromHtml("#cee5fc");
            if (btnColor1.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
            {
                p = new Pen(picBoxColor1.BackColor, penSize);
            }
            else if (btnColor2.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
            {
                p = new Pen(picBoxColor1.BackColor, penSize);
            }
        }
        private void panelLineVisible_MouseDown(object sender, MouseEventArgs e)
        {
            panelLine.Visible = false;
        }
        private void picBoxChoice_MouseHover(object sender, EventArgs e)
        {
            picBox_MouseHover(picBoxChoice);
        }
        private void picBoxChoice_MouseLeave(object sender, EventArgs e)
        {
            if (picBoxChoice.BackColor != (Color)ColorTranslator.FromHtml("#cee5fc"))
            {
                picBoxChoice.BackColor = Color.Transparent;
            }
        }
        private void picBoxPen_MouseHover(object sender, EventArgs e)
        {
            picBox_MouseHover(picBoxPen);
        }
        private void picBoxPen_MouseLeave(object sender, EventArgs e)
        {
            if (picBoxPen.BackColor != (Color)ColorTranslator.FromHtml("#cee5fc"))
            {
                picBoxPen.BackColor = Color.Transparent;
            }
        }
        private void picBoxPen_Click(object sender, EventArgs e)
        {
            if (picBoxPen.BackColor != (Color)ColorTranslator.FromHtml("#cee5fc"))
            {
                picBoxReSize.Enabled = true;
                picBoxPen.BackColor = (Color)ColorTranslator.FromHtml("#cee5fc");
                picBoxRubber.BackColor = Color.Transparent;
            }
            else
            {
                picBoxPen.BackColor = Color.Transparent;
                picBoxChoice.BackColor = Color.Transparent;
            }
        }
        private void picBoxRubber_MouseHover(object sender, EventArgs e)
        {
            picBox_MouseHover(picBoxRubber);
        }
        private void picBoxRubber_MouseLeave(object sender, EventArgs e)
        {
            if (picBoxRubber.BackColor != (Color)ColorTranslator.FromHtml("#cee5fc"))
            {
                picBoxRubber.BackColor = Color.Transparent;
            }
        }
        private void picBoxRubber_Click(object sender, EventArgs e)
        {
            if (picBoxRubber.BackColor != (Color)ColorTranslator.FromHtml("#cee5fc"))
            {
                picBoxReSize.Enabled = true;
                picBoxPen.BackColor = Color.Transparent;
                picBoxRubber.BackColor = (Color)ColorTranslator.FromHtml("#cee5fc");
                p = new Pen(Color.White, penSize);
                //graphics.SmoothingMode = SmoothingMode.AntiAlias;

            }
            else
            {
                picBoxRubber.BackColor = Color.Transparent;
                picBoxChoice.BackColor = Color.Transparent;
            }
        }
        private void picBoxCircle_MouseLeave(object sender, EventArgs e)
        {
            picBox_MouseLeave(picBoxSquare, picBoxTriangle, picBoxRectangle);
        }
        private void picBoxCircle_MouseHover(object sender, EventArgs e)
        {
            picBox_MouseHover(picBoxCircle);
        }
        private void picBoxSquare_MouseLeave(object sender, EventArgs e)
        {
            picBox_MouseLeave(picBoxCircle, picBoxTriangle, picBoxRectangle);
        }
        private void picBoxSquare_MouseHover(object sender, EventArgs e)
        {
            picBox_MouseHover(picBoxSquare);
        }
        private void picBoxTriangle_MouseLeave(object sender, EventArgs e)
        {
            picBox_MouseLeave(picBoxCircle, picBoxSquare, picBoxRectangle);
        }
        private void picBoxTriangle_MouseHover(object sender, EventArgs e)
        {
            picBox_MouseHover(picBoxTriangle);
        }
        private void picBoxRectangle_MouseLeave(object sender, EventArgs e)
        {
            picBox_MouseLeave(picBoxCircle, picBoxSquare, picBoxTriangle);
        }
        private void picBoxRectangle_MouseHover(object sender, EventArgs e)
        {
            picBox_MouseHover(picBoxRectangle);
        }
        private void hakkindaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.ShowDialog();
        }
        private void picBoxCircle_Click(object sender, EventArgs e)
        {
            drawCircle = true;
            picBoxRubber.BackColor = Color.Transparent;
            picBoxPen.BackColor = Color.Transparent;
        }
        private void picBoxSquare_Click(object sender, EventArgs e)
        {
            drawSquare = true;
            picBoxRubber.BackColor = Color.Transparent;
            picBoxPen.BackColor = Color.Transparent;
        }
        private void picBoxRectangle_Click(object sender, EventArgs e)
        {
            drawRectangle = true;
            picBoxRubber.BackColor = Color.Transparent;
            picBoxPen.BackColor = Color.Transparent;
        }
        private void picBoxTriangle_Click(object sender, EventArgs e)
        {
            picBoxRubber.BackColor = Color.Transparent;
            picBoxPen.BackColor = Color.Transparent;
            DrawTriangle(graphics, rect, Direction.Left);
        }
        #endregion

        #region FormEvent
        private void CKPaint_Load(object sender, EventArgs e)
        {
            NewFile(drawingPanel.Width, drawingPanel.Height);
            if (title == "")
            {
                title = "Adsız";
            }
            this.Text = title + " - CK Paint";
        }
        private void CKPaint_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '+')
            {
                if (penSize < 48)
                {
                    penSize++;
                }
            }
            if (e.KeyChar == '-')
            {
                if (penSize >= 1)
                {
                    penSize--;
                }
            }
            if (e.KeyChar == 27)
            {
                if (!SelectingArea) return;
                SelectingArea = false;
                SelectedImage = null;
                graphics = null;
                drawingPanel.Image = changedImg;
                drawingPanel.Refresh();
                MadeSelection = false;
                EnableMenuItems();
            }
        }
        private void CKPaint_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (instance != null)
            {
                instance = null;
            }
        }
        private void CKPaint_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (graphics != null)
            {
                if (MessageBox.Show(title + " dosyasında yapılan değişiklikleri  kaydetmek istitor musunuz?", "CKPaint", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SaveFile();
                }
            }
            else
            {
                Application.Exit();
            }
        }
        #endregion

        #region OtherFunctions
        private void toolTip()
        {
            new ToolTip().SetToolTip(picLine1, "1px");
            new ToolTip().SetToolTip(picLine2, "3px");
            new ToolTip().SetToolTip(picLine3, "5px");
            new ToolTip().SetToolTip(picLine4, "8px");
            new ToolTip().SetToolTip(picBoxPen, "Kalem");
            new ToolTip().SetToolTip(picBoxRubber, "Silgi");
            new ToolTip().SetToolTip(picBoxReSize, "Boyut (Ctrl++, Ctrl+-)");
            new ToolTip().SetToolTip(btnColor1, "1 Renk (ön plan rengi)");
            new ToolTip().SetToolTip(picBoxColor1, "1 Renk (ön plan rengi)");
            new ToolTip().SetToolTip(btnColor2, "2 Renk (arka plan rengi)");
            new ToolTip().SetToolTip(picBoxColor2, "2 Renk (arka plan rengi)");
            new ToolTip().SetToolTip(btnEditColors, "Renkleri düzenle");
            new ToolTip().SetToolTip(pictureBox1, "Renkleri düzenle");
            new ToolTip().SetToolTip(picBoxChoice, "Seçim");
            new ToolTip().SetToolTip(picBoxCircle, "Oval");
            new ToolTip().SetToolTip(picBoxSquare, "Kare");
            new ToolTip().SetToolTip(picBoxTriangle, "Üçgen");
            new ToolTip().SetToolTip(picBoxRectangle, "Dikdörtgen");
        }
        private void btnChangeColor1()
        {
            panelLine.Visible = false;
            if (btnColor1.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
            {
                btnColor2.BackColor = Color.Transparent;
            }
            else
            {
                btnColor1.BackColor = (Color)ColorTranslator.FromHtml("#c9e0f7");
                btnColor2.BackColor = Color.Transparent;
                colorFront = btnColor1.BackColor;
            }
            p = new Pen(picBoxColor1.BackColor, penSize);
        }
        private void btnChangeColor2()
        {
            panelLine.Visible = false;
            if (btnColor2.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
            {
                btnColor1.BackColor = Color.Transparent;
            }
            else
            {
                btnColor2.BackColor = (Color)ColorTranslator.FromHtml("#c9e0f7");
                btnColor1.BackColor = Color.Transparent;
                colorBack = btnColor2.BackColor;
            }
            p = new Pen(picBoxColor2.BackColor, penSize);
        }
        private void RgbColor(PictureBox pB, object sender, MouseEventArgs e)
        {
            int R = Convert.ToInt32(pB.BackColor.R.ToString());
            int G = Convert.ToInt32(pB.BackColor.G.ToString());
            int B = Convert.ToInt32(pB.BackColor.B.ToString());
            if (pB.BackColor != Color.Transparent)
            {
                ChangeColor(Color.FromArgb(R, G, B), e);
            }
            panelLine.Visible = false;
        }
        private void ChangeColor(Color color, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (btnColor1.BackColor == (Color)ColorTranslator.FromHtml("#c9e0f7"))
                {
                    colorFront = color;
                    picBoxColor1.BackColor = color;
                    btnChangeColor1();
                    colorActive = picBoxColor1.BackColor;
                }
                else
                {
                    colorFront = color;
                    picBoxColor2.BackColor = color;
                    btnChangeColor2();
                    colorActive = picBoxColor2.BackColor;
                }
            }
            panelLine.Visible = false;
        }
        private Rectangle MakeRectangle(int x0, int y0, int x1, int y1)
        {
            return new Rectangle(
                Math.Min(x0, x1),
                Math.Min(y0, y1),
                Math.Abs(x0 - x1),
                Math.Abs(y0 - y1));
        }
        private void EnableMenuItems()
        {
            kopyalaToolStripMenuItem.Enabled = MadeSelection;
            kesToolStripMenuItem.Enabled = MadeSelection;
            yapistirToolStripMenuItem.Enabled = MadeSelection;
        }
        private void CopyToClipboard(Rectangle src_rect)
        {
            Bitmap bm = new Bitmap(src_rect.Width, src_rect.Height);
            using (Graphics gr = Graphics.FromImage(bm))
            {
                Rectangle dest_rect = new Rectangle(0, 0, src_rect.Width, src_rect.Height);
                gr.DrawImage(changedImg, dest_rect, src_rect, GraphicsUnit.Pixel);
            }
            Clipboard.SetImage(bm);
        }
        public enum Direction
        {
            Up,
            Right,
            Down,
            Left
        }
        private void DrawTriangle(Graphics g, Rectangle rect, Direction direction)
        {
            int halfWidth = rect.Width / 2;
            int halfHeight = rect.Height / 2;
            Point p0 = Point.Empty;
            Point p1 = Point.Empty;
            Point p2 = Point.Empty;

            switch (direction)
            {
                case Direction.Up:
                    p0 = new Point(rect.Left + halfWidth, rect.Top);
                    p1 = new Point(rect.Left, rect.Bottom);
                    p2 = new Point(rect.Right, rect.Bottom);
                    break;
                case Direction.Down:
                    p0 = new Point(rect.Left + halfWidth, rect.Bottom);
                    p1 = new Point(rect.Left, rect.Top);
                    p2 = new Point(rect.Right, rect.Top);
                    break;
                case Direction.Left:
                    p0 = new Point(rect.Left, rect.Top + halfHeight);
                    p1 = new Point(rect.Right, rect.Top);
                    p2 = new Point(rect.Right, rect.Bottom);
                    break;
                case Direction.Right:
                    p0 = new Point(rect.Right, rect.Top + halfHeight);
                    p1 = new Point(rect.Left, rect.Bottom);
                    p2 = new Point(rect.Left, rect.Top);
                    break;
            }
            SolidBrush brushColor = new SolidBrush(colorActive);
            g.FillPolygon(brushColor, new Point[] { p0, p1, p2 });
        }
        #endregion
    }
}