using AppSiacpiMovil.Host;
using GsmComm.GsmCommunication;
using GsmComm.PduConverter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppSiacpiMovil.App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {

            string dateTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            MessageBox.Show( dateTime,"app");
        }
  
    }
}
