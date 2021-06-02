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

        // Options form stuff:
        OptionsForm OF = new OptionsForm();

        // Image settings:
        public Color bgColor = Color.FromArgb(255, 127, 127, 127);
        private int imgWidth;
        private int imgHeight;
        private int centerX;
        private int centerY;

        // Random number generator:
        static public Random rng = new Random();
        // Data that will be output upon exporting:
        public Image DecalOutputData { get; set; }



        private void GenerateButton_Click(object sender, EventArgs e) {

        }


        private void SaveButton_Click(object sender, EventArgs e) {

        }


        private void OptionsButton_Click(object sender, EventArgs e) {
            OF.ShowDialog();
        }
    }

}
