
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
            this.ApplyOptionsButton = new System.Windows.Forms.Button();
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
            // 
            // ResetOptionsButton
            // 
            resources.ApplyResources(this.ResetOptionsButton, "ResetOptionsButton");
            this.ResetOptionsButton.Name = "ResetOptionsButton";
            this.ResetOptionsButton.UseVisualStyleBackColor = true;
            // 
            // ApplyOptionsButton
            // 
            resources.ApplyResources(this.ApplyOptionsButton, "ApplyOptionsButton");
            this.ApplyOptionsButton.Name = "ApplyOptionsButton";
            this.ApplyOptionsButton.UseVisualStyleBackColor = true;
            // 
            // OptionsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.PresetLabel);
            this.Controls.Add(this.PresetsComboBox);
            this.Controls.Add(this.ResetOptionsButton);
            this.Controls.Add(this.ApplyOptionsButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "OptionsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label PresetLabel;
        private System.Windows.Forms.ComboBox PresetsComboBox;
        private System.Windows.Forms.Button ResetOptionsButton;
        private System.Windows.Forms.Button ApplyOptionsButton;
    }
}