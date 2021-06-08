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
    public partial class OptionsForm : Form {

        //string OptionsFormActivePreset = "MammalianBlood";

        public OptionsForm() {
            InitializeComponent();

            // Enumerate the options in the Presets combo box:
            PresetsComboBox.Items.Clear();
            foreach (string key in Form1.LiquidPresets.Keys) {
                PresetsComboBox.Items.Add(key);
            }
            Console.WriteLine(Form1.ActivePreset.Name);
            PresetsComboBox.SelectedItem = Form1.ActivePreset.Name;
        }

        private void PresetsComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            Form1.ActivePreset = Form1.LiquidPresets[PresetsComboBox.SelectedItem.ToString()];
        }
    }
}
