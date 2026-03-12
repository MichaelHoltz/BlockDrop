namespace BlockDrop.Forms
{
    partial class FormSelection
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
            this.btnFormBlockDrop = new System.Windows.Forms.Button();
            this.btnGearConfig = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnFormBlockDrop
            // 
            this.btnFormBlockDrop.Location = new System.Drawing.Point(57, 48);
            this.btnFormBlockDrop.Name = "btnFormBlockDrop";
            this.btnFormBlockDrop.Size = new System.Drawing.Size(141, 40);
            this.btnFormBlockDrop.TabIndex = 0;
            this.btnFormBlockDrop.Text = "BlockDrop";
            this.btnFormBlockDrop.UseVisualStyleBackColor = true;
            this.btnFormBlockDrop.Click += new System.EventHandler(this.btnFormBlockDrop_Click);
            // 
            // btnGearConfig
            // 
            this.btnGearConfig.Location = new System.Drawing.Point(57, 116);
            this.btnGearConfig.Name = "btnGearConfig";
            this.btnGearConfig.Size = new System.Drawing.Size(141, 40);
            this.btnGearConfig.TabIndex = 1;
            this.btnGearConfig.Text = "Gear Config";
            this.btnGearConfig.UseVisualStyleBackColor = true;
            this.btnGearConfig.Click += new System.EventHandler(this.btnGearConfig_Click);
            // 
            // FormSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(249, 304);
            this.Controls.Add(this.btnGearConfig);
            this.Controls.Add(this.btnFormBlockDrop);
            this.Name = "FormSelection";
            this.Text = "Selection Form";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnFormBlockDrop;
        private System.Windows.Forms.Button btnGearConfig;
    }
}