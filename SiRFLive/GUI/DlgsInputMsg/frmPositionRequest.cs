﻿namespace SiRFLive.GUI.DlgsInputMsg
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class frmPositionRequest : Form
    {
        private Button button_Cancel;
        private Button button_Send;
        private ComboBox comboBox_PositionMethod;
        private ComboBox comboBox_Priority;
        private IContainer components;
        private GroupBox groupBox1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private NumericUpDown numericUpDown_HorrErrMax;
        private NumericUpDown numericUpDown_NumOfFixes;
        private NumericUpDown numericUpDown_ResponseTimeMax;
        private NumericUpDown numericUpDown_TimeBetweenFixes;
        private NumericUpDown numericUpDown_VertErrMax;
        private NumericUpDown numericUpDown5;

        public frmPositionRequest()
        {
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmPositionRequest));
            this.groupBox1 = new GroupBox();
            this.numericUpDown5 = new NumericUpDown();
            this.numericUpDown_ResponseTimeMax = new NumericUpDown();
            this.numericUpDown_VertErrMax = new NumericUpDown();
            this.numericUpDown_HorrErrMax = new NumericUpDown();
            this.numericUpDown_TimeBetweenFixes = new NumericUpDown();
            this.numericUpDown_NumOfFixes = new NumericUpDown();
            this.label7 = new Label();
            this.label6 = new Label();
            this.label5 = new Label();
            this.label4 = new Label();
            this.label3 = new Label();
            this.label2 = new Label();
            this.comboBox_Priority = new ComboBox();
            this.label1 = new Label();
            this.comboBox_PositionMethod = new ComboBox();
            this.button_Send = new Button();
            this.button_Cancel = new Button();
            this.groupBox1.SuspendLayout();
            this.numericUpDown5.BeginInit();
            this.numericUpDown_ResponseTimeMax.BeginInit();
            this.numericUpDown_VertErrMax.BeginInit();
            this.numericUpDown_HorrErrMax.BeginInit();
            this.numericUpDown_TimeBetweenFixes.BeginInit();
            this.numericUpDown_NumOfFixes.BeginInit();
            base.SuspendLayout();
            this.groupBox1.Controls.Add(this.numericUpDown5);
            this.groupBox1.Controls.Add(this.numericUpDown_ResponseTimeMax);
            this.groupBox1.Controls.Add(this.numericUpDown_VertErrMax);
            this.groupBox1.Controls.Add(this.numericUpDown_HorrErrMax);
            this.groupBox1.Controls.Add(this.numericUpDown_TimeBetweenFixes);
            this.groupBox1.Controls.Add(this.numericUpDown_NumOfFixes);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.comboBox_Priority);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBox_PositionMethod);
            this.groupBox1.Location = new Point(0x16, 30);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(0x148, 0xfd);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Position Request";
            this.numericUpDown5.Location = new Point(0xcd, 0x17f);
            this.numericUpDown5.Name = "numericUpDown5";
            this.numericUpDown5.Size = new Size(120, 20);
            this.numericUpDown5.TabIndex = 6;
            this.numericUpDown_ResponseTimeMax.Location = new Point(180, 0x93);
            this.numericUpDown_ResponseTimeMax.Name = "numericUpDown_ResponseTimeMax";
            this.numericUpDown_ResponseTimeMax.Size = new Size(120, 20);
            this.numericUpDown_ResponseTimeMax.TabIndex = 4;
            this.numericUpDown_VertErrMax.Location = new Point(180, 0x74);
            this.numericUpDown_VertErrMax.Name = "numericUpDown_VertErrMax";
            this.numericUpDown_VertErrMax.Size = new Size(120, 20);
            this.numericUpDown_VertErrMax.TabIndex = 3;
            this.numericUpDown_HorrErrMax.Location = new Point(180, 0x55);
            this.numericUpDown_HorrErrMax.Name = "numericUpDown_HorrErrMax";
            this.numericUpDown_HorrErrMax.Size = new Size(120, 20);
            this.numericUpDown_HorrErrMax.TabIndex = 2;
            this.numericUpDown_TimeBetweenFixes.Location = new Point(180, 0x36);
            this.numericUpDown_TimeBetweenFixes.Name = "numericUpDown_TimeBetweenFixes";
            this.numericUpDown_TimeBetweenFixes.Size = new Size(120, 20);
            this.numericUpDown_TimeBetweenFixes.TabIndex = 1;
            this.numericUpDown_NumOfFixes.Location = new Point(180, 0x17);
            this.numericUpDown_NumOfFixes.Name = "numericUpDown_NumOfFixes";
            this.numericUpDown_NumOfFixes.Size = new Size(120, 20);
            this.numericUpDown_NumOfFixes.TabIndex = 0;
            this.label7.AutoSize = true;
            this.label7.Location = new Point(0x27, 0xb6);
            this.label7.Name = "label7";
            this.label7.Size = new Size(0x26, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Priority";
            this.label6.AutoSize = true;
            this.label6.Location = new Point(0x27, 0x97);
            this.label6.Name = "label6";
            this.label6.Size = new Size(0x76, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Response Time Max (s)";
            this.label5.AutoSize = true;
            this.label5.Location = new Point(0x27, 120);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0x68, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Vertical Error Max (s)";
            this.label4.AutoSize = true;
            this.label4.Location = new Point(0x27, 0x3a);
            this.label4.Name = "label4";
            this.label4.Size = new Size(0x74, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Time Between Fixes (s)";
            this.label3.AutoSize = true;
            this.label3.Location = new Point(0x24, 0x59);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x77, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Horizontal Error Max (m)";
            this.label2.AutoSize = true;
            this.label2.Location = new Point(0x27, 0x1b);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x44, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Num of Fixes";
            this.comboBox_Priority.FormattingEnabled = true;
            this.comboBox_Priority.Items.AddRange(new object[] { "No Priority", "Response Time", "Position Error" });
            this.comboBox_Priority.Location = new Point(180, 0xb2);
            this.comboBox_Priority.Name = "comboBox_Priority";
            this.comboBox_Priority.Size = new Size(0x79, 0x15);
            this.comboBox_Priority.TabIndex = 5;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(0x27, 0xd5);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x56, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Location method";
            this.comboBox_PositionMethod.FormattingEnabled = true;
            this.comboBox_PositionMethod.Items.AddRange(new object[] { "MS Assisted", "MS Based", "MS Based Preferred", "MS Assist Preferred", "Simul. MSB & MSA", "Coarse Location" });
            this.comboBox_PositionMethod.Location = new Point(0xb3, 0xd1);
            this.comboBox_PositionMethod.Name = "comboBox_PositionMethod";
            this.comboBox_PositionMethod.Size = new Size(0x79, 0x15);
            this.comboBox_PositionMethod.TabIndex = 6;
            this.button_Send.Location = new Point(0x17f, 30);
            this.button_Send.Name = "button_Send";
            this.button_Send.Size = new Size(0x4b, 0x17);
            this.button_Send.TabIndex = 7;
            this.button_Send.Text = "&Send";
            this.button_Send.UseVisualStyleBackColor = true;
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new Point(0x17f, 0x4e);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new Size(0x4b, 0x17);
            this.button_Cancel.TabIndex = 8;
            this.button_Cancel.Text = "&Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            base.AcceptButton = this.button_Send;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.button_Cancel;
            base.ClientSize = new Size(480, 0x138);
            base.Controls.Add(this.button_Cancel);
            base.Controls.Add(this.button_Send);
            base.Controls.Add(this.groupBox1);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "frmPositionRequest";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Position Request";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.numericUpDown5.EndInit();
            this.numericUpDown_ResponseTimeMax.EndInit();
            this.numericUpDown_VertErrMax.EndInit();
            this.numericUpDown_HorrErrMax.EndInit();
            this.numericUpDown_TimeBetweenFixes.EndInit();
            this.numericUpDown_NumOfFixes.EndInit();
            base.ResumeLayout(false);
        }
    }
}

