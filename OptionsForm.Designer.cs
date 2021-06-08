
namespace SplatterGenCubed {
    partial class OptionsForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            this.PresetLabel = new System.Windows.Forms.Label();
            this.PresetsComboBox = new System.Windows.Forms.ComboBox();
            this.ResetOptionsButton = new System.Windows.Forms.Button();
            this.SplatterSizeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.SizeLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.SplatterSizeNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // PresetLabel
            // 
            resources.ApplyResources(this.PresetLabel, "PresetLabel");
            this.PresetLabel.Name = "PresetLabel";
            // 
            // PresetsComboBox
            // 
            this.PresetsComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.PresetsComboBox, "PresetsComboBox");
            this.PresetsComboBox.Name = "PresetsComboBox";
            this.PresetsComboBox.SelectedIndexChanged += new System.EventHandler(this.PresetsComboBox_SelectedIndexChanged);
            // 
            // ResetOptionsButton
            // 
            resources.ApplyResources(this.ResetOptionsButton, "ResetOptionsButton");
            this.ResetOptionsButton.Name = "ResetOptionsButton";
            this.ResetOptionsButton.UseVisualStyleBackColor = true;
            // 
            // SplatterSizeNumericUpDown
            // 
            this.SplatterSizeNumericUpDown.Increment = new decimal(new int[] {
            128,
            0,
            0,
            0});
            resources.ApplyResources(this.SplatterSizeNumericUpDown, "SplatterSizeNumericUpDown");
            this.SplatterSizeNumericUpDown.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.SplatterSizeNumericUpDown.Minimum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.SplatterSizeNumericUpDown.Name = "SplatterSizeNumericUpDown";
            this.SplatterSizeNumericUpDown.ReadOnly = true;
            this.SplatterSizeNumericUpDown.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.SplatterSizeNumericUpDown.ValueChanged += new System.EventHandler(this.SplatterSizeNumericUpDown_ValueChanged);
            // 
            // SizeLabel
            // 
            resources.ApplyResources(this.SizeLabel, "SizeLabel");
            this.SizeLabel.Name = "SizeLabel";
            // 
            // OptionsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SizeLabel);
            this.Controls.Add(this.SplatterSizeNumericUpDown);
            this.Controls.Add(this.PresetLabel);
            this.Controls.Add(this.PresetsComboBox);
            this.Controls.Add(this.ResetOptionsButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "OptionsForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OptionsForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.SplatterSizeNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label PresetLabel;
        private System.Windows.Forms.ComboBox PresetsComboBox;
        private System.Windows.Forms.Button ResetOptionsButton;
        private System.Windows.Forms.NumericUpDown SplatterSizeNumericUpDown;
        private System.Windows.Forms.Label SizeLabel;
    }
}