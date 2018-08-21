namespace FieldExporter.Components
{
    partial class JointPropertiesForm
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gamepieceGroupbox = new System.Windows.Forms.GroupBox();
            this.gamepieceLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.centerLabel = new System.Windows.Forms.Label();
            this.selectCenterButton = new System.Windows.Forms.Button();
            this.jointCheckBox = new System.Windows.Forms.CheckBox();
            this.axisLabel = new System.Windows.Forms.Label();
            this.selectAxisButton = new System.Windows.Forms.Button();
            this.gamepieceGroupbox.SuspendLayout();
            this.gamepieceLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // gamepieceGroupbox
            // 
            this.gamepieceGroupbox.AutoSize = true;
            this.gamepieceGroupbox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gamepieceGroupbox.Controls.Add(this.gamepieceLayoutPanel);
            this.gamepieceGroupbox.Controls.Add(this.jointCheckBox);
            this.gamepieceGroupbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.gamepieceGroupbox.Location = new System.Drawing.Point(0, 0);
            this.gamepieceGroupbox.Name = "gamepieceGroupbox";
            this.gamepieceGroupbox.Size = new System.Drawing.Size(250, 77);
            this.gamepieceGroupbox.TabIndex = 1;
            this.gamepieceGroupbox.TabStop = false;
            // 
            // gamepieceLayoutPanel
            // 
            this.gamepieceLayoutPanel.AutoSize = true;
            this.gamepieceLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gamepieceLayoutPanel.ColumnCount = 2;
            this.gamepieceLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.gamepieceLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.gamepieceLayoutPanel.Controls.Add(this.selectAxisButton, 1, 1);
            this.gamepieceLayoutPanel.Controls.Add(this.axisLabel, 0, 1);
            this.gamepieceLayoutPanel.Controls.Add(this.centerLabel, 0, 0);
            this.gamepieceLayoutPanel.Controls.Add(this.selectCenterButton, 1, 0);
            this.gamepieceLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.gamepieceLayoutPanel.Location = new System.Drawing.Point(3, 16);
            this.gamepieceLayoutPanel.Name = "gamepieceLayoutPanel";
            this.gamepieceLayoutPanel.RowCount = 2;
            this.gamepieceLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.gamepieceLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.gamepieceLayoutPanel.Size = new System.Drawing.Size(244, 58);
            this.gamepieceLayoutPanel.TabIndex = 1;
            // 
            // centerLabel
            // 
            this.centerLabel.AutoSize = true;
            this.centerLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.centerLabel.Location = new System.Drawing.Point(3, 3);
            this.centerLabel.Margin = new System.Windows.Forms.Padding(3);
            this.centerLabel.Name = "centerLabel";
            this.centerLabel.Size = new System.Drawing.Size(107, 23);
            this.centerLabel.TabIndex = 1;
            this.centerLabel.Text = "Center: [0.0, 0.0, 0.0]";
            this.centerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // selectCenterButton
            // 
            this.selectCenterButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.selectCenterButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.selectCenterButton.Location = new System.Drawing.Point(179, 3);
            this.selectCenterButton.Name = "selectCenterButton";
            this.selectCenterButton.Size = new System.Drawing.Size(62, 23);
            this.selectCenterButton.TabIndex = 0;
            this.selectCenterButton.Text = "Select";
            this.selectCenterButton.UseVisualStyleBackColor = true;
            // 
            // jointCheckBox
            // 
            this.jointCheckBox.AutoSize = true;
            this.jointCheckBox.Checked = true;
            this.jointCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.jointCheckBox.Location = new System.Drawing.Point(6, 0);
            this.jointCheckBox.Name = "jointCheckBox";
            this.jointCheckBox.Size = new System.Drawing.Size(60, 17);
            this.jointCheckBox.TabIndex = 0;
            this.jointCheckBox.Text = "Jointed";
            this.jointCheckBox.UseVisualStyleBackColor = true;
            // 
            // axisLabel
            // 
            this.axisLabel.AutoSize = true;
            this.axisLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.axisLabel.Location = new System.Drawing.Point(3, 32);
            this.axisLabel.Margin = new System.Windows.Forms.Padding(3);
            this.axisLabel.Name = "axisLabel";
            this.axisLabel.Size = new System.Drawing.Size(95, 23);
            this.axisLabel.TabIndex = 4;
            this.axisLabel.Text = "Axis: [0.0, 0.0, 0.0]";
            this.axisLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // selectAxisButton
            // 
            this.selectAxisButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.selectAxisButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.selectAxisButton.Location = new System.Drawing.Point(179, 32);
            this.selectAxisButton.Name = "selectAxisButton";
            this.selectAxisButton.Size = new System.Drawing.Size(62, 23);
            this.selectAxisButton.TabIndex = 5;
            this.selectAxisButton.Text = "Select";
            this.selectAxisButton.UseVisualStyleBackColor = true;
            // 
            // JointPropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.gamepieceGroupbox);
            this.MinimumSize = new System.Drawing.Size(250, 0);
            this.Name = "JointPropertiesForm";
            this.Size = new System.Drawing.Size(250, 77);
            this.gamepieceGroupbox.ResumeLayout(false);
            this.gamepieceGroupbox.PerformLayout();
            this.gamepieceLayoutPanel.ResumeLayout(false);
            this.gamepieceLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gamepieceGroupbox;
        private System.Windows.Forms.TableLayoutPanel gamepieceLayoutPanel;
        private System.Windows.Forms.Label centerLabel;
        private System.Windows.Forms.Button selectCenterButton;
        private System.Windows.Forms.CheckBox jointCheckBox;
        private System.Windows.Forms.Button selectAxisButton;
        private System.Windows.Forms.Label axisLabel;
    }
}
