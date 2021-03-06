﻿namespace SiRFLive.GUI.DlgsInputMsg
{
    using SiRFLive.Communication;
    using SiRFLive.Configuration;
    using SiRFLive.General;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class frm_TTBTimeAidingCfg : Form
    {
        private Button button_Cancel;
        private Button button_OK;
        private ComboBox comboBox_DisableEnable;
        private ComboBox comboBox_PreciseCoarse;
        private CommunicationManager comm;
        private IContainer components;
        private Label label_unit_accuracy;
        private Label label_unit_skew;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label8;
        private TextBox textBox_Accuracy;
        private TextBox textBox_Skew;

        public frm_TTBTimeAidingCfg()
        {
            this.InitializeComponent();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            uint num = 0;
            if (this.comboBox_DisableEnable.SelectedIndex == 1)
            {
                this.comm.AutoReplyCtrl.TTBTimeAidingParams.Enable = true;
                this.comm.AutoReplyCtrl.AutoReplyParams.UseTTB_ForTimeAid = true;
                this.comm.AutoReplyCtrl.AutoReplyParams.UseTTB_ForHwCfg = true;
                this.comm.AutoReplyCtrl.AutoReplyParams.AutoReplyTimeTrans = true;
                this.comm.AutoReplyCtrl.AutoReplyParams.AutoReply = true;
            }
            else
            {
                this.comm.AutoReplyCtrl.TTBTimeAidingParams.Enable = false;
                this.comm.AutoReplyCtrl.AutoReplyParams.UseTTB_ForTimeAid = false;
                this.comm.AutoReplyCtrl.AutoReplyParams.UseTTB_ForHwCfg = false;
                this.comm.AutoReplyCtrl.AutoReplyParams.UseTTB_ForFreqAid = false;
            }
            try
            {
                this.comm.AutoReplyCtrl.TTBTimeAidingParams.Accuracy = 0x3e8 * Convert.ToUInt32(this.textBox_Accuracy.Text);
                num = Convert.ToUInt32(this.textBox_Skew.Text);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            this.comm.AutoReplyCtrl.TTBTimeAidingParams.Type = (byte) this.comboBox_PreciseCoarse.SelectedIndex;
            if (this.comm.AutoReplyCtrl.TTBTimeAidingParams.Type == 1)
            {
                this.comm.AutoReplyCtrl.TTBTimeAidingParams.Skew = 0x3e8 * num;
            }
            else
            {
                this.comm.AutoReplyCtrl.TTBTimeAidingParams.Skew = num;
            }
            string tTBTimeAidingCfgMsg = this.comm.AutoReplyCtrl.GetTTBTimeAidingCfgMsg();
            if (this.comm.TTBPort.IsOpen)
            {
                this.comm.WriteData_TTB(tTBTimeAidingCfgMsg);
                this.comm.waitforMsgFromTTB(0xcc, 80);
                this.writeAutoReplyData();
                base.Close();
            }
            else
            {
                MessageBox.Show("TTB port is not connected", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void comboBox_PreciseCoarse_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.updateunitLabel();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void frm_TTBTimeAidingCfg_Load(object sender, EventArgs e)
        {
            this.setDefault();
            this.updateunitLabel();
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frm_TTBTimeAidingCfg));
            this.comboBox_DisableEnable = new ComboBox();
            this.comboBox_PreciseCoarse = new ComboBox();
            this.textBox_Accuracy = new TextBox();
            this.textBox_Skew = new TextBox();
            this.label1 = new Label();
            this.label2 = new Label();
            this.label3 = new Label();
            this.label4 = new Label();
            this.button_OK = new Button();
            this.button_Cancel = new Button();
            this.label5 = new Label();
            this.label6 = new Label();
            this.label8 = new Label();
            this.label_unit_accuracy = new Label();
            this.label_unit_skew = new Label();
            base.SuspendLayout();
            this.comboBox_DisableEnable.FormattingEnabled = true;
            this.comboBox_DisableEnable.Items.AddRange(new object[] { "Disable", "Enable" });
            this.comboBox_DisableEnable.Location = new Point(0x60, 0x22);
            this.comboBox_DisableEnable.Name = "comboBox_DisableEnable";
            this.comboBox_DisableEnable.Size = new Size(100, 0x15);
            this.comboBox_DisableEnable.TabIndex = 0;
            this.comboBox_DisableEnable.Text = "Enable";
            this.comboBox_PreciseCoarse.FormattingEnabled = true;
            this.comboBox_PreciseCoarse.Items.AddRange(new object[] { "Precise", "Coarse" });
            this.comboBox_PreciseCoarse.Location = new Point(0x60, 80);
            this.comboBox_PreciseCoarse.Name = "comboBox_PreciseCoarse";
            this.comboBox_PreciseCoarse.Size = new Size(100, 0x15);
            this.comboBox_PreciseCoarse.TabIndex = 1;
            this.comboBox_PreciseCoarse.Text = "Coarse";
            this.comboBox_PreciseCoarse.SelectedIndexChanged += new EventHandler(this.comboBox_PreciseCoarse_SelectedIndexChanged);
            this.textBox_Accuracy.Location = new Point(0x60, 0x7e);
            this.textBox_Accuracy.Name = "textBox_Accuracy";
            this.textBox_Accuracy.Size = new Size(100, 20);
            this.textBox_Accuracy.TabIndex = 2;
            this.textBox_Accuracy.Text = "2";
            this.textBox_Skew.Location = new Point(0x60, 0xab);
            this.textBox_Skew.Name = "textBox_Skew";
            this.textBox_Skew.Size = new Size(100, 20);
            this.textBox_Skew.TabIndex = 3;
            this.textBox_Skew.Text = "0";
            this.label1.AutoSize = true;
            this.label1.Location = new Point(0x26, 0x26);
            this.label1.Name = "label1";
            this.label1.Size = new Size(40, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Enable";
            this.label2.AutoSize = true;
            this.label2.Location = new Point(0x2c, 0x54);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x22, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Mode";
            this.label3.AutoSize = true;
            this.label3.Location = new Point(0x1c, 130);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x34, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Accuracy";
            this.label4.AutoSize = true;
            this.label4.Location = new Point(0x2e, 0xaf);
            this.label4.Name = "label4";
            this.label4.Size = new Size(0x22, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Skew";
            this.button_OK.Location = new Point(0xf4, 0x21);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new Size(0x4b, 0x17);
            this.button_OK.TabIndex = 4;
            this.button_OK.Text = "&OK";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new EventHandler(this.button_OK_Click);
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new Point(0xf4, 0x4e);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new Size(0x4b, 0x17);
            this.button_Cancel.TabIndex = 5;
            this.button_Cancel.Text = "&Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new EventHandler(this.button_Cancel_Click);
            this.label5.AutoSize = true;
            this.label5.ForeColor = SystemColors.ControlDarkDark;
            this.label5.Location = new Point(0x2e, 0xdd);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0xf6, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Note: Command is sent out to SLC and Comm Pipe";
            this.label6.AutoSize = true;
            this.label6.ForeColor = SystemColors.ControlDarkDark;
            this.label6.Location = new Point(0x2c, 0xf5);
            this.label6.Name = "label6";
            this.label6.Size = new Size(0xf7, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "TTB may adjust HWC msg based on these settings";
            this.label8.AutoSize = true;
            this.label8.Location = new Point(0x4c, 0x132);
            this.label8.Name = "label8";
            this.label8.Size = new Size(0, 13);
            this.label8.TabIndex = 8;
            this.label_unit_accuracy.AutoSize = true;
            this.label_unit_accuracy.Location = new Point(0xca, 130);
            this.label_unit_accuracy.Name = "label_unit_accuracy";
            this.label_unit_accuracy.Size = new Size(20, 13);
            this.label_unit_accuracy.TabIndex = 9;
            this.label_unit_accuracy.Text = "ms";
            this.label_unit_skew.AutoSize = true;
            this.label_unit_skew.Location = new Point(0xca, 0xaf);
            this.label_unit_skew.Name = "label_unit_skew";
            this.label_unit_skew.Size = new Size(20, 13);
            this.label_unit_skew.TabIndex = 10;
            this.label_unit_skew.Text = "ms";
            base.AcceptButton = this.button_OK;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.button_Cancel;
            base.ClientSize = new Size(0x15b, 290);
            base.Controls.Add(this.label_unit_skew);
            base.Controls.Add(this.label_unit_accuracy);
            base.Controls.Add(this.label8);
            base.Controls.Add(this.label6);
            base.Controls.Add(this.label5);
            base.Controls.Add(this.button_Cancel);
            base.Controls.Add(this.button_OK);
            base.Controls.Add(this.label4);
            base.Controls.Add(this.label3);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.textBox_Skew);
            base.Controls.Add(this.textBox_Accuracy);
            base.Controls.Add(this.comboBox_PreciseCoarse);
            base.Controls.Add(this.comboBox_DisableEnable);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "frm_TTBTimeAidingCfg";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Config Time Aiding";
            base.Load += new EventHandler(this.frm_TTBTimeAidingCfg_Load);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void setDefault()
        {
            if (this.comm != null)
            {
                if (this.comm.AutoReplyCtrl.TTBTimeAidingParams.Enable)
                {
                    this.comboBox_DisableEnable.SelectedIndex = 1;
                }
                else
                {
                    this.comboBox_DisableEnable.SelectedIndex = 0;
                }
                if (this.comm.AutoReplyCtrl.TTBTimeAidingParams.Type == 1)
                {
                    this.comboBox_PreciseCoarse.SelectedIndex = 1;
                    this.label_unit_accuracy.Text = "sec";
                    this.label_unit_skew.Text = "ms";
                    this.textBox_Skew.Text = (((double) this.comm.AutoReplyCtrl.TTBTimeAidingParams.Skew) / 1000.0).ToString();
                }
                else
                {
                    this.comboBox_PreciseCoarse.SelectedIndex = 0;
                    this.label_unit_accuracy.Text = "us";
                    this.label_unit_skew.Text = "ns";
                    this.textBox_Skew.Text = this.comm.AutoReplyCtrl.TTBTimeAidingParams.Skew.ToString();
                }
                this.textBox_Accuracy.Text = (this.comm.AutoReplyCtrl.TTBTimeAidingParams.Accuracy / 0x3e8).ToString();
            }
            else
            {
                this.comboBox_DisableEnable.SelectedIndex = 1;
                this.comboBox_PreciseCoarse.SelectedIndex = 1;
                this.textBox_Accuracy.Text = "2";
                this.textBox_Skew.Text = "0";
            }
        }

        private void updateunitLabel()
        {
            if (this.comboBox_PreciseCoarse.SelectedIndex == 0)
            {
                this.label_unit_accuracy.Text = "us";
                this.label_unit_skew.Text = "ns";
            }
            else
            {
                this.label_unit_accuracy.Text = "sec";
                this.label_unit_skew.Text = "ms";
            }
        }

        private void writeAutoReplyData()
        {
            this.Cursor = Cursors.WaitCursor;
            IniHelper helper = new IniHelper(clsGlobal.InstalledDirectory + @"\scripts\SiRFLiveAutomationSetupAutoReply.cfg");
            string section = string.Empty;
            string key = string.Empty;
            section = "TTB_TIME_AIDING";
            key = "ENABLE";
            if (this.comm.AutoReplyCtrl.TTBTimeAidingParams.Enable)
            {
                helper.IniWriteValue(section, key, "1");
            }
            else
            {
                helper.IniWriteValue(section, key, "0");
            }
            key = "TYPE";
            helper.IniWriteValue(section, key, this.comm.AutoReplyCtrl.TTBTimeAidingParams.Type.ToString());
            key = "TIME_ACC";
            helper.IniWriteValue(section, key, this.comm.AutoReplyCtrl.TTBTimeAidingParams.Accuracy.ToString());
            key = "TIME_SKEW";
            helper.IniWriteValue(section, key, this.comm.AutoReplyCtrl.TTBTimeAidingParams.Skew.ToString());
            this.Cursor = Cursors.Default;
        }

        public CommunicationManager CommWindow
        {
            get
            {
                return this.comm;
            }
            set
            {
                this.comm = value;
            }
        }
    }
}

