﻿namespace SiRFLive.GUI.Reporting
{
    using SiRFLive.Reporting;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class frmSimpleReportGen : Form
    {
        private string _reportTypeString = "Unknown";
        private Label autoTestFilePathLabel;
        private Button cancelBtn;
        private IContainer components;
        private Label dirPathLabel;
        private Button generateBtn;
        private Button TestResultDirBrowserBtn;
        private TextBox testResultDirTextBox;

        public frmSimpleReportGen(string reportType)
        {
            this.InitializeComponent();
            this._reportTypeString = reportType;
            this.Text = reportType;
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void frmSimpleReportGen_Load(object sender, EventArgs e)
        {
        }

        private void generateBtn_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            if (this.testResultDirTextBox.Text != string.Empty)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    if (this._reportTypeString.Contains("SDO"))
                    {
                        report.ConvertCSVToSDOFormat(this.testResultDirTextBox.Text);
                    }
                    else if (this._reportTypeString.Contains("MPM"))
                    {
                        report.Summary_MPM(this.testResultDirTextBox.Text);
                    }
                    this.Cursor = Cursors.Default;
                }
                catch (Exception exception)
                {
                    string str = "Error";
                    if (this._reportTypeString.Contains("SDO"))
                    {
                        str = "Error encounters while SDO files are being generated\r\n";
                    }
                    else if (this._reportTypeString.Contains("MPM"))
                    {
                        str = "Error encounters while processing MPM files\r\n";
                    }
                    MessageBox.Show(str + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    this.Cursor = Cursors.Default;
                }
                report.Dispose();
                report = null;
            }
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmSimpleReportGen));
            this.autoTestFilePathLabel = new Label();
            this.TestResultDirBrowserBtn = new Button();
            this.dirPathLabel = new Label();
            this.testResultDirTextBox = new TextBox();
            this.generateBtn = new Button();
            this.cancelBtn = new Button();
            base.SuspendLayout();
            this.autoTestFilePathLabel.AutoSize = true;
            this.autoTestFilePathLabel.Location = new Point(-150, 0x38);
            this.autoTestFilePathLabel.Name = "autoTestFilePathLabel";
            this.autoTestFilePathLabel.Size = new Size(0x6c, 13);
            this.autoTestFilePathLabel.TabIndex = 3;
            this.autoTestFilePathLabel.Text = "Test Scripts Directory";
            this.TestResultDirBrowserBtn.Location = new Point(0x173, 0x2c);
            this.TestResultDirBrowserBtn.Name = "TestResultDirBrowserBtn";
            this.TestResultDirBrowserBtn.Size = new Size(0x1a, 0x17);
            this.TestResultDirBrowserBtn.TabIndex = 6;
            this.TestResultDirBrowserBtn.Text = "...";
            this.TestResultDirBrowserBtn.UseVisualStyleBackColor = true;
            this.TestResultDirBrowserBtn.Click += new EventHandler(this.TestResultDirBrowserBtn_Click);
            this.dirPathLabel.AutoSize = true;
            this.dirPathLabel.Location = new Point(14, 0x1a);
            this.dirPathLabel.Name = "dirPathLabel";
            this.dirPathLabel.Size = new Size(0x6f, 13);
            this.dirPathLabel.TabIndex = 5;
            this.dirPathLabel.Text = "Test Results Directory";
            this.testResultDirTextBox.Location = new Point(0x11, 0x2f);
            this.testResultDirTextBox.Name = "testResultDirTextBox";
            this.testResultDirTextBox.Size = new Size(0x153, 20);
            this.testResultDirTextBox.TabIndex = 4;
            this.generateBtn.Location = new Point(0x79, 0x5c);
            this.generateBtn.Name = "generateBtn";
            this.generateBtn.Size = new Size(0x4b, 0x17);
            this.generateBtn.TabIndex = 7;
            this.generateBtn.Text = "Generate";
            this.generateBtn.UseVisualStyleBackColor = true;
            this.generateBtn.Click += new EventHandler(this.generateBtn_Click);
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new Point(0xde, 0x5c);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new Size(0x4b, 0x17);
            this.cancelBtn.TabIndex = 8;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new EventHandler(this.cancelBtn_Click);
            base.AcceptButton = this.generateBtn;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.cancelBtn;
            base.ClientSize = new Size(0x19e, 0x8b);
            base.Controls.Add(this.cancelBtn);
            base.Controls.Add(this.generateBtn);
            base.Controls.Add(this.TestResultDirBrowserBtn);
            base.Controls.Add(this.dirPathLabel);
            base.Controls.Add(this.testResultDirTextBox);
            base.Controls.Add(this.autoTestFilePathLabel);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "frmSimpleReportGen";
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "SDO Format Generator";
            base.Load += new EventHandler(this.frmSimpleReportGen_Load);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void TestResultDirBrowserBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.testResultDirTextBox.Text = dialog.SelectedPath;
            }
        }
    }
}

