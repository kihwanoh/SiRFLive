﻿namespace SiRFLive.GUI.DlgsInputMsg
{
    using CommonClassLibrary;
    using SiRFLive.Communication;
    using SiRFLive.General;
    using SiRFLive.MessageHandling;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    public class frmMaskMode : Form
    {
        private CheckBox ABP_ckBx;
        private RadioButton AltHoldAlways_rBtn;
        private RadioButton AltHoldAuto_rBtn;
        private RadioButton AltHoldDisable_rBtn;
        private TextBox AltHoldFixedAlt_txtBx;
        private int altMode;
        private Button button_Cancel;
        private Button button_Send;
        private CommunicationManager comm;
        private IContainer components;
        private TextBox DeadReckonTimeout_txtBx;
        private int degMode;
        private int DMtime;
        private CheckBox fixAlt_ckBx;
        private GroupBox groupBox1;
        private GroupBox groupBox3;
        private Label labelDRtimeout;
        private CheckBox lastCompAlt_ckBx;
        private CheckBox TrackSmoothing_ckBx;
        private CheckBox VelocityDRmode_chkBx;

        public frmMaskMode()
        {
            this.degMode = 4;
            this.altMode = 2;
            this.InitializeComponent();
            if (this.comm.ProductFamily == CommonClass.ProductType.GSD4t)
            {
                this.ABP_ckBx.Visible = false;
            }
            else
            {
                this.ABP_ckBx.Visible = true;
            }
            this.TrackSmoothing_ckBx.Checked = false;
            this.AltHoldAuto_rBtn.Checked = true;
            this.VelocityDRmode_chkBx.Checked = true;
            this.AltHoldAuto_rBtn.Checked = true;
            this.lastCompAlt_ckBx.Checked = true;
            this.ABP_ckBx.Checked = false;
        }

        public frmMaskMode(CommunicationManager parentComm)
        {
            this.degMode = 4;
            this.altMode = 2;
            this.InitializeComponent();
            this.comm = parentComm;
            if (this.comm.ProductFamily == CommonClass.ProductType.GSD4t)
            {
                this.ABP_ckBx.Visible = false;
            }
            else
            {
                this.ABP_ckBx.Visible = true;
            }
            this.TrackSmoothing_ckBx.Checked = false;
            this.AltHoldAuto_rBtn.Checked = true;
            this.VelocityDRmode_chkBx.Checked = true;
            this.AltHoldAuto_rBtn.Checked = true;
            this.lastCompAlt_ckBx.Checked = true;
            this.ABP_ckBx.Checked = false;
        }

        private void AltHoldAlways_rBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (this.AltHoldAlways_rBtn.Checked)
            {
                this.lastCompAlt_ckBx.Checked = false;
                this.lastCompAlt_ckBx.Enabled = false;
                this.fixAlt_ckBx.Checked = true;
                this.fixAlt_ckBx.Enabled = true;
                this.AltHoldFixedAlt_txtBx.Enabled = true;
            }
        }

        private void AltHoldAuto_rBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (this.AltHoldAuto_rBtn.Checked)
            {
                this.lastCompAlt_ckBx.Enabled = true;
                this.fixAlt_ckBx.Enabled = true;
                this.AltHoldFixedAlt_txtBx.Enabled = true;
            }
        }

        private void AltHoldDisable_rBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (this.AltHoldDisable_rBtn.Checked)
            {
                this.lastCompAlt_ckBx.Enabled = false;
                this.fixAlt_ckBx.Enabled = false;
                this.AltHoldFixedAlt_txtBx.Enabled = false;
            }
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void button_Send_Click(object sender, EventArgs e)
        {
            try
            {
                Convert.ToInt16(this.AltHoldFixedAlt_txtBx.Text);
            }
            catch
            {
                MessageBox.Show("Incorrect value entered. Please try again.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if ((Convert.ToInt16(this.AltHoldFixedAlt_txtBx.Text) >= -1000) && (Convert.ToInt16(this.AltHoldFixedAlt_txtBx.Text) <= 0x2710))
            {
                try
                {
                    Convert.ToByte(this.DeadReckonTimeout_txtBx.Text);
                }
                catch
                {
                    MessageBox.Show("Incorrect value entered. Please try again.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if ((Convert.ToByte(this.DeadReckonTimeout_txtBx.Text) < 0) || (Convert.ToByte(this.DeadReckonTimeout_txtBx.Text) > 120))
                {
                    MessageBox.Show("0 = disabled, Timeout range is 0 to 120", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    int fixAlt = Convert.ToInt16(this.AltHoldFixedAlt_txtBx.Text);
                    int dRtime = Convert.ToInt16(this.DeadReckonTimeout_txtBx.Text);
                    if (!this.VelocityDRmode_chkBx.Checked)
                    {
                        dRtime = 0;
                    }
                    int trackSmooth = this.TrackSmoothing_ckBx.Checked ? 1 : 0;
                    int setABP = this.ABP_ckBx.Checked ? 1 : 0;
                    int altSource = this.fixAlt_ckBx.Checked ? 1 : 0;
                    if (this.AltHoldAuto_rBtn.Checked)
                    {
                        this.altMode = 0;
                    }
                    else if (this.AltHoldAlways_rBtn.Checked)
                    {
                        this.altMode = 1;
                    }
                    else if (this.AltHoldDisable_rBtn.Checked)
                    {
                        this.altMode = 2;
                    }
                    this.SetModeMaskControl(altSource, this.altMode, this.degMode, trackSmooth, fixAlt, this.DMtime, dRtime, setABP);
                    base.Close();
                }
            }
            else
            {
                MessageBox.Show("Range is -1000 to 10000", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void fixAlt_ckBx_CheckedChanged(object sender, EventArgs e)
        {
            if (this.AltHoldAlways_rBtn.Checked)
            {
                this.fixAlt_ckBx.Checked = true;
                this.AltHoldFixedAlt_txtBx.Enabled = true;
            }
            else if (this.AltHoldAuto_rBtn.Checked)
            {
                if (this.fixAlt_ckBx.Checked)
                {
                    this.lastCompAlt_ckBx.Checked = false;
                    this.AltHoldFixedAlt_txtBx.Enabled = true;
                }
                else
                {
                    this.lastCompAlt_ckBx.Checked = true;
                    this.AltHoldFixedAlt_txtBx.Enabled = false;
                }
            }
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmMaskMode));
            this.button_Send = new Button();
            this.button_Cancel = new Button();
            this.TrackSmoothing_ckBx = new CheckBox();
            this.groupBox1 = new GroupBox();
            this.fixAlt_ckBx = new CheckBox();
            this.lastCompAlt_ckBx = new CheckBox();
            this.AltHoldFixedAlt_txtBx = new TextBox();
            this.AltHoldDisable_rBtn = new RadioButton();
            this.AltHoldAlways_rBtn = new RadioButton();
            this.AltHoldAuto_rBtn = new RadioButton();
            this.groupBox3 = new GroupBox();
            this.DeadReckonTimeout_txtBx = new TextBox();
            this.labelDRtimeout = new Label();
            this.VelocityDRmode_chkBx = new CheckBox();
            this.ABP_ckBx = new CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            base.SuspendLayout();
            this.button_Send.Location = new Point(0xcd, 12);
            this.button_Send.Name = "button_Send";
            this.button_Send.Size = new Size(0x4b, 0x17);
            this.button_Send.TabIndex = 0;
            this.button_Send.Text = "&Send";
            this.button_Send.UseVisualStyleBackColor = true;
            this.button_Send.Click += new EventHandler(this.button_Send_Click);
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new Point(0xcd, 0x29);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new Size(0x4b, 0x17);
            this.button_Cancel.TabIndex = 1;
            this.button_Cancel.Text = "&Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new EventHandler(this.button_Cancel_Click);
            this.TrackSmoothing_ckBx.AutoSize = true;
            this.TrackSmoothing_ckBx.Location = new Point(0x29, 0x29);
            this.TrackSmoothing_ckBx.Name = "TrackSmoothing_ckBx";
            this.TrackSmoothing_ckBx.Size = new Size(0x8f, 0x11);
            this.TrackSmoothing_ckBx.TabIndex = 2;
            this.TrackSmoothing_ckBx.Text = "Enable &Track Smoothing";
            this.TrackSmoothing_ckBx.UseVisualStyleBackColor = true;
            this.groupBox1.Controls.Add(this.fixAlt_ckBx);
            this.groupBox1.Controls.Add(this.lastCompAlt_ckBx);
            this.groupBox1.Controls.Add(this.AltHoldFixedAlt_txtBx);
            this.groupBox1.Controls.Add(this.AltHoldDisable_rBtn);
            this.groupBox1.Controls.Add(this.AltHoldAlways_rBtn);
            this.groupBox1.Controls.Add(this.AltHoldAuto_rBtn);
            this.groupBox1.Location = new Point(12, 0x48);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(0x129, 0x70);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Altitude hold mode";
            this.fixAlt_ckBx.AutoSize = true;
            this.fixAlt_ckBx.Location = new Point(0x7d, 0x34);
            this.fixAlt_ckBx.Name = "fixAlt_ckBx";
            this.fixAlt_ckBx.Size = new Size(0x69, 0x11);
            this.fixAlt_ckBx.TabIndex = 7;
            this.fixAlt_ckBx.Text = "&Fixed altitude (m)";
            this.fixAlt_ckBx.UseVisualStyleBackColor = true;
            this.fixAlt_ckBx.CheckedChanged += new EventHandler(this.fixAlt_ckBx_CheckedChanged);
            this.lastCompAlt_ckBx.AutoSize = true;
            this.lastCompAlt_ckBx.Location = new Point(0x7d, 0x1a);
            this.lastCompAlt_ckBx.Name = "lastCompAlt_ckBx";
            this.lastCompAlt_ckBx.Size = new Size(0x85, 0x11);
            this.lastCompAlt_ckBx.TabIndex = 6;
            this.lastCompAlt_ckBx.Text = "&Last computed altitude";
            this.lastCompAlt_ckBx.UseVisualStyleBackColor = true;
            this.lastCompAlt_ckBx.CheckedChanged += new EventHandler(this.lastCompAlt_ckBx_CheckedChanged);
            this.AltHoldFixedAlt_txtBx.Enabled = false;
            this.AltHoldFixedAlt_txtBx.Location = new Point(0xec, 50);
            this.AltHoldFixedAlt_txtBx.Name = "AltHoldFixedAlt_txtBx";
            this.AltHoldFixedAlt_txtBx.Size = new Size(0x30, 20);
            this.AltHoldFixedAlt_txtBx.TabIndex = 5;
            this.AltHoldFixedAlt_txtBx.Text = "0";
            this.AltHoldDisable_rBtn.AutoSize = true;
            this.AltHoldDisable_rBtn.Location = new Point(11, 0x54);
            this.AltHoldDisable_rBtn.Name = "AltHoldDisable_rBtn";
            this.AltHoldDisable_rBtn.Size = new Size(120, 0x11);
            this.AltHoldDisable_rBtn.TabIndex = 4;
            this.AltHoldDisable_rBtn.TabStop = true;
            this.AltHoldDisable_rBtn.Text = "Disable altitude &hold";
            this.AltHoldDisable_rBtn.UseVisualStyleBackColor = true;
            this.AltHoldDisable_rBtn.CheckedChanged += new EventHandler(this.AltHoldDisable_rBtn_CheckedChanged);
            this.AltHoldAlways_rBtn.AutoSize = true;
            this.AltHoldAlways_rBtn.Location = new Point(11, 0x34);
            this.AltHoldAlways_rBtn.Name = "AltHoldAlways_rBtn";
            this.AltHoldAlways_rBtn.Size = new Size(0x3a, 0x11);
            this.AltHoldAlways_rBtn.TabIndex = 1;
            this.AltHoldAlways_rBtn.TabStop = true;
            this.AltHoldAlways_rBtn.Text = "Al&ways";
            this.AltHoldAlways_rBtn.UseVisualStyleBackColor = true;
            this.AltHoldAlways_rBtn.CheckedChanged += new EventHandler(this.AltHoldAlways_rBtn_CheckedChanged);
            this.AltHoldAuto_rBtn.AutoSize = true;
            this.AltHoldAuto_rBtn.Location = new Point(11, 20);
            this.AltHoldAuto_rBtn.Name = "AltHoldAuto_rBtn";
            this.AltHoldAuto_rBtn.Size = new Size(0x48, 0x11);
            this.AltHoldAuto_rBtn.TabIndex = 0;
            this.AltHoldAuto_rBtn.TabStop = true;
            this.AltHoldAuto_rBtn.Text = "Aut&omatic";
            this.AltHoldAuto_rBtn.UseVisualStyleBackColor = true;
            this.AltHoldAuto_rBtn.CheckedChanged += new EventHandler(this.AltHoldAuto_rBtn_CheckedChanged);
            this.groupBox3.Controls.Add(this.DeadReckonTimeout_txtBx);
            this.groupBox3.Controls.Add(this.labelDRtimeout);
            this.groupBox3.Controls.Add(this.VelocityDRmode_chkBx);
            this.groupBox3.Location = new Point(12, 0xc3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new Size(0x129, 0x3e);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Dead reckoning";
            this.DeadReckonTimeout_txtBx.Location = new Point(0xf9, 0x17);
            this.DeadReckonTimeout_txtBx.Name = "DeadReckonTimeout_txtBx";
            this.DeadReckonTimeout_txtBx.Size = new Size(0x23, 20);
            this.DeadReckonTimeout_txtBx.TabIndex = 2;
            this.DeadReckonTimeout_txtBx.Text = "10";
            this.labelDRtimeout.AutoSize = true;
            this.labelDRtimeout.Location = new Point(180, 0x1a);
            this.labelDRtimeout.Name = "labelDRtimeout";
            this.labelDRtimeout.Size = new Size(0x3b, 13);
            this.labelDRtimeout.TabIndex = 1;
            this.labelDRtimeout.Text = "Timeout (s)";
            this.VelocityDRmode_chkBx.AutoSize = true;
            this.VelocityDRmode_chkBx.Location = new Point(10, 0x17);
            this.VelocityDRmode_chkBx.Name = "VelocityDRmode_chkBx";
            this.VelocityDRmode_chkBx.Size = new Size(0x93, 0x11);
            this.VelocityDRmode_chkBx.TabIndex = 0;
            this.VelocityDRmode_chkBx.Text = "Enable &Velocity DR mode";
            this.VelocityDRmode_chkBx.UseVisualStyleBackColor = true;
            this.VelocityDRmode_chkBx.CheckedChanged += new EventHandler(this.VelocityDRmode_chkBx_CheckedChanged);
            this.ABP_ckBx.AutoSize = true;
            this.ABP_ckBx.Location = new Point(0x29, 0x12);
            this.ABP_ckBx.Name = "ABP_ckBx";
            this.ABP_ckBx.Size = new Size(0x2f, 0x11);
            this.ABP_ckBx.TabIndex = 6;
            this.ABP_ckBx.Text = "&ABP";
            this.ABP_ckBx.UseVisualStyleBackColor = true;
            base.AcceptButton = this.button_Send;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.button_Cancel;
            base.ClientSize = new Size(0x141, 0x10f);
            base.Controls.Add(this.ABP_ckBx);
            base.Controls.Add(this.groupBox3);
            base.Controls.Add(this.groupBox1);
            base.Controls.Add(this.TrackSmoothing_ckBx);
            base.Controls.Add(this.button_Cancel);
            base.Controls.Add(this.button_Send);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "frmMaskMode";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Mode Mask";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void lastCompAlt_ckBx_CheckedChanged(object sender, EventArgs e)
        {
            if (this.lastCompAlt_ckBx.Checked)
            {
                this.fixAlt_ckBx.Checked = false;
                this.AltHoldFixedAlt_txtBx.Enabled = false;
            }
        }

        public virtual void SetModeMaskControl(int altSource, int altMode, int degMode, int trackSmooth, int fixAlt, int DMtime, int DRtime, int setABP)
        {
            ArrayList list = new ArrayList();
            StringBuilder builder = new StringBuilder();
            list = this.comm.m_Protocols.GetInputMessageStructure(0x88, clsGlobal.noSID, "Mode Control", "OSP");
            for (int i = 0; i < list.Count; i++)
            {
                switch (((InputMsg) list[i]).fieldName)
                {
                    case "Degraded Mode":
                        builder.Append(degMode);
                        builder.Append(",");
                        break;

                    case "Altitude":
                        builder.Append(fixAlt);
                        builder.Append(",");
                        break;

                    case "Altitude Hold Mode":
                        builder.Append(altMode);
                        builder.Append(",");
                        break;

                    case "Altitude Hold Source":
                        builder.Append(altSource);
                        builder.Append(",");
                        break;

                    case "Degraded Time Out":
                        builder.Append(DMtime);
                        builder.Append(",");
                        break;

                    case "DR Time Out":
                        builder.Append(DRtime);
                        builder.Append(",");
                        break;

                    case "Track Smoothing":
                        builder.Append(trackSmooth);
                        builder.Append(",");
                        break;

                    case "Position Calc Mode":
                        builder.Append(setABP);
                        builder.Append(",");
                        break;

                    default:
                        builder.Append(((InputMsg) list[i]).defaultValue);
                        builder.Append(",");
                        break;
                }
            }
            string msg = this.comm.m_Protocols.ConvertFieldsToRaw(builder.ToString().TrimEnd(new char[] { ',' }), "Mode Control", "OSP");
            this.comm.WriteData(msg);
        }

        private void VelocityDRmode_chkBx_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.VelocityDRmode_chkBx.Checked)
            {
                this.DeadReckonTimeout_txtBx.Enabled = false;
                this.labelDRtimeout.Enabled = false;
            }
            else
            {
                this.DeadReckonTimeout_txtBx.Enabled = true;
                this.labelDRtimeout.Enabled = true;
            }
        }
    }
}

