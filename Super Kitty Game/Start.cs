﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Super_Kitty_Game
{
    public partial class Start : Form
    {
        public MyUDPClient client;
        public Start()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (MasterCheckBox.Checked)
                client = new MasterClient(MyUDPClient.MasterPort);
            else
                client = new MyUDPClient(MyUDPClient.NormalPort, tbMasterIP.Text);
            this.DialogResult = DialogResult.OK;
        }

        private void MasterCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            tbMasterIP.Text = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
            tbMasterIP.Enabled = !tbMasterIP.Enabled;
        }

        private void Start_Load(object sender, EventArgs e)
        {

        }
    }
}
