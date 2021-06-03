using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SplatterGenCubed {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        // Data that will be output upon exporting:
        public Image DecalOutputData { get; set; }

        // Options form stuff:
        OptionsForm OF = new OptionsForm();

        // Image settings:
        public Color bgColor = Color.FromArgb(255, 127, 127, 127);
        private int imgWidth;
        private int imgHeight;
        private int centerX;
        private int centerY;
        private int finalRes;
        private double maxDist;
        private double maxDistLinear;

        // Random number generator:
        static public Random rng = new Random();


        // Member functions used for generation:
        public void InitializeImageSettings(int size, int fr) {
            imgWidth = size;
            imgHeight = size;
            centerX = imgWidth / 2;
            centerY = imgHeight / 2;
            finalRes = fr;
            maxDist = Math.Sqrt(Math.Pow(imgWidth, 2) + Math.Pow(imgHeight, 2));
            maxDistLinear = imgWidth / 2.0d;
        }


        public static Color HslToColor(double h, double s, double l, int a = 255) {
            // Convert an HSL value into an RGB Color object.

            int r, g, b;

            double p2;
            if (l <= 0.5) p2 = l * (1 + s);
            else p2 = l + s - l * s;

            double p1 = 2 * l - p2;
            double double_r, double_g, double_b;
            if (s == 0) {
                double_r = l;
                double_g = l;
                double_b = l;
            }
            else {
                double_r = QqhToRgb(p1, p2, h + 120);
                double_g = QqhToRgb(p1, p2, h);
                double_b = QqhToRgb(p1, p2, h - 120);
            }

            // Convert RGB to the 0 to 255 range.
            r = (int)(double_r * 255.0);
            g = (int)(double_g * 255.0);
            b = (int)(double_b * 255.0);

            return Color.FromArgb(a, r, g, b);
        }


        private static double QqhToRgb(double q1, double q2, double hue) {
            if (hue > 360) hue -= 360;
            else if (hue < 0) hue += 360;

            if (hue < 60) return q1 + (q2 - q1) * hue / 60;
            if (hue < 180) return q2;
            if (hue < 240) return q1 + (q2 - q1) * (240 - hue) / 60;
            return q1;
        }

        //

        // Big important functions, probably used directly by buttons:

        private void GenerateSplatterDecal() {
            if (DecalOutputData != null) {
                DecalOutputData.Dispose();
            }

            InitializeImageSettings(2048, 256);

            SolidBrush layerBrush = new SolidBrush(HslToColor(0, 0.0, 0.25, 96));
            Bitmap imgBackground;

            using (Bitmap output = new Bitmap(imgWidth, imgHeight)) {
                using (Graphics g = Graphics.FromImage(output)) {
                    // Background color
                    g.Clear(bgColor);

                }
            }
        }

        //

        // Button member functions:

        private void GenerateButton_Click(object sender, EventArgs e) {
            GenerateSplatterDecal();
        }


        private void SaveButton_Click(object sender, EventArgs e) {
            SaveFileDialog sfd1 = new SaveFileDialog();
            sfd1.Filter = "PNG |*.png";
            sfd1.Title = "Save as .png";
            sfd1.ShowDialog();

            if (sfd1.FileName != "") {
                DecalOutputData.Save(sfd1.FileName);
            }
        }


        private void OptionsButton_Click(object sender, EventArgs e) {
            OF.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            string MessageBoxText = "SplatterGenCubed - Alpha\n\nCopyright © 2021\nLicensed under GPLV3.\nCreated by Hxdce";
            MessageBox.Show(MessageBoxText, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

}
