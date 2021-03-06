﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArdupilotMega.Controls.BackstageView;

namespace ArdupilotMega.GCSViews.ConfigurationView
{
    public partial class ConfigHardwareOptions : BackStageViewContentPanel
    {
        bool startup = false;

        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        public ConfigHardwareOptions()
        {
            InitializeComponent();
        }

        private void BUT_MagCalibration_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == CustomMessageBox.Show("Use live data, or a log\n\nYes for Live data", "Mag Calibration", MessageBoxButtons.YesNo))
            {
                List<Tuple<float, float, float>> data = new List<Tuple<float, float, float>>();

                byte backupratesens = MainV2.cs.ratesensors;

                MainV2.cs.ratesensors = 10;

                MainV2.comPort.requestDatastream((byte)MAVLink.MAV_DATA_STREAM.RAW_SENSORS, MainV2.cs.ratesensors); // mag captures at 10 hz

                CustomMessageBox.Show("Data will be collected for 30 seconds, Please click ok and move the apm around all axises");

                DateTime deadline = DateTime.Now.AddSeconds(30);

                float oldmx = 0;
                float oldmy = 0;
                float oldmz = 0;

                while (deadline > DateTime.Now)
                {
                    Application.DoEvents();

                    if (oldmx != MainV2.cs.mx &&
                        oldmy != MainV2.cs.my &&
                        oldmz != MainV2.cs.mz)
                    {
                        data.Add(new Tuple<float, float, float>(
                            MainV2.cs.mx - (float)MainV2.cs.mag_ofs_x,
                            MainV2.cs.my - (float)MainV2.cs.mag_ofs_y,
                            MainV2.cs.mz - (float)MainV2.cs.mag_ofs_z));

                        oldmx = MainV2.cs.mx;
                        oldmy = MainV2.cs.my;
                        oldmz = MainV2.cs.mz;
                    }
                }

                MainV2.cs.ratesensors = backupratesens;

                if (data.Count < 10)
                {
                    CustomMessageBox.Show("Log does not contain enough data");
                    return;
                }

                double[] ans = MagCalib.LeastSq(data);

                MagCalib.SaveOffsets(ans);

            }
            else
            {
                string minthro = "30";
                Common.InputBox("Min Throttle", "Use only data above this throttle percent.", ref minthro);

                int ans = 0;
                int.TryParse(minthro, out ans);

                MagCalib.ProcessLog(ans);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                //System.Diagnostics.Process.Start("http://www.ngdc.noaa.gov/geomagmodels/Declination.jsp");
                System.Diagnostics.Process.Start("http://www.magnetic-declination.com/");
            }
            catch { CustomMessageBox.Show("Webpage open failed... do you have a virus?\nhttp://www.magnetic-declination.com/"); }
        }

        private void TXT_declination_Validating(object sender, CancelEventArgs e)
        {
            float ans = 0;
            e.Cancel = !float.TryParse(TXT_declination.Text, out ans);
        }

        private void TXT_declination_Validated(object sender, EventArgs e)
        {
            if (startup)
                return;
            try
            {
                if (MainV2.comPort.param["COMPASS_DEC"] == null)
                {
                    CustomMessageBox.Show("Not Available");
                }
                else
                {
                    float dec = 0.0f;
                    try
                    {
                        string declination = TXT_declination.Text;
                        float.TryParse(declination, out dec);
                        float deg = (float)((int)dec);
                        float mins = (dec - deg);
                        if (dec > 0)
                        {
                            dec += ((mins) / 60.0f);
                        }
                        else
                        {
                            dec -= ((mins) / 60.0f);
                        }
                    }
                    catch { CustomMessageBox.Show("Invalid input!"); return; }

                    TXT_declination.Text = dec.ToString();

                    MainV2.comPort.setParam("COMPASS_DEC", dec * deg2rad);
                }
            }
            catch { CustomMessageBox.Show("Set COMPASS_DEC Failed"); }
        }


        private void CHK_enablecompass_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            try
            {
                if (MainV2.comPort.param["MAG_ENABLE"] == null)
                {
                    CustomMessageBox.Show("Not Available");
                }
                else
                {
                    MainV2.comPort.setParam("MAG_ENABLE", ((CheckBox)sender).Checked == true ? 1 : 0);
                }
            }
            catch { CustomMessageBox.Show("Set MAG_ENABLE Failed"); }
        }

        private void CHK_enablesonar_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            try
            {
                if (MainV2.comPort.param["SONAR_ENABLE"] == null)
                {
                    CustomMessageBox.Show("Not Available");
                }
                else
                {
                    MainV2.comPort.setParam("SONAR_ENABLE", ((CheckBox)sender).Checked == true ? 1 : 0);
                }
            }
            catch { CustomMessageBox.Show("Set SONAR_ENABLE Failed"); }
        }

        private void CHK_enableairspeed_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            try
            {
                if (MainV2.comPort.param["ARSPD_ENABLE"] == null)
                {
                    CustomMessageBox.Show("Not Available on " + MainV2.cs.firmware.ToString());
                }
                else
                {
                    MainV2.comPort.setParam("ARSPD_ENABLE", ((CheckBox)sender).Checked == true ? 1 : 0);
                }
            }
            catch { CustomMessageBox.Show("Set ARSPD_ENABLE Failed"); }
        }

        private void CHK_enableoptflow_CheckedChanged(object sender, EventArgs e)
        {

            if (startup)
                return;
            try
            {
                if (MainV2.comPort.param["FLOW_ENABLE"] == null)
                {
                    CustomMessageBox.Show("Not Available on " + MainV2.cs.firmware.ToString());
                }
                else
                {
                    MainV2.comPort.setParam("FLOW_ENABLE", ((CheckBox)sender).Checked == true ? 1 : 0);
                }
            }
            catch { CustomMessageBox.Show("Set FLOW_ENABLE Failed"); }
        }

        private void CMB_sonartype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            try
            {
                if (MainV2.comPort.param["SONAR_TYPE"] == null)
                {
                    CustomMessageBox.Show("Not Available on " + MainV2.cs.firmware.ToString());
                }
                else
                {
                    MainV2.comPort.setParam("SONAR_TYPE", ((ComboBox)sender).SelectedIndex);
                }
            }
            catch { CustomMessageBox.Show("Set SONAR_TYPE Failed"); }
        }

        private void ConfigHardwareOptions_Load(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                this.Enabled = false;
                return;
            }
            else
            {
                this.Enabled = true;
            }

            startup = true;

            if (MainV2.comPort.param["ARSPD_ENABLE"] != null)
                CHK_enableairspeed.Checked = MainV2.comPort.param["ARSPD_ENABLE"].ToString() == "1" ? true : false;

            if (MainV2.comPort.param["SONAR_ENABLE"] != null)
                CHK_enablesonar.Checked = MainV2.comPort.param["SONAR_ENABLE"].ToString() == "1" ? true : false;

            if (MainV2.comPort.param["MAG_ENABLE"] != null)
                CHK_enablecompass.Checked = MainV2.comPort.param["MAG_ENABLE"].ToString() == "1" ? true : false;

            if (MainV2.comPort.param["COMPASS_DEC"] != null)
                TXT_declination.Text = (float.Parse(MainV2.comPort.param["COMPASS_DEC"].ToString()) * rad2deg).ToString();

            if (MainV2.comPort.param["SONAR_TYPE"] != null)
                CMB_sonartype.SelectedIndex = int.Parse(MainV2.comPort.param["SONAR_TYPE"].ToString());

            if (MainV2.comPort.param["FLOW_ENABLE"] != null)
                CHK_enableoptflow.Checked = MainV2.comPort.param["FLOW_ENABLE"].ToString() == "1" ? true : false;


            startup = false;
        }
    }
}
