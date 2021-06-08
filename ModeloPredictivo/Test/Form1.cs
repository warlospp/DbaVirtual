using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                bllDeploy bll = new bllDeploy("Server=192.168.100.219;Database=deploy;User Id=sa;Password=Admin.123;");
                bll.ejecutar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);;
            }
        }
    }
}
