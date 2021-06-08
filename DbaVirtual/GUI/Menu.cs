using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DTO;

namespace GUI
{
    public partial class Menu : Form
    {
        List<dtoOpcion> dtos = new List<dtoOpcion>();
        int? intId = null;
        public Menu()
        {
            InitializeComponent();
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            try
            {
                this.nuevo();
                using (srProxy.IwcfChatBotClient proxy = new srProxy.IwcfChatBotClient())
                {
                    this.dtos = proxy.opciones(new parOpcion() { intCrud = 0}).ToList();
                    proxy.Close();
                };
                foreach (var item in this.dtos)
                {
                    lvOpcion.Items.Add(new ListViewItem() { Name = item.intOpcion.ToString(), Text = item.strNombre });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void nuevo()
        {
            try
            {
                this.intId = null;
                lvOpcion.Items.Clear();                
                txtNombre.Text = string.Empty;
                txtPlantilla.Text = string.Empty;
                txtSentencia.Text = string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void lvOpcion_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.lvOpcion.SelectedItems.Count == 0)
                    return;

                this.intId = int.Parse(lvOpcion.SelectedItems[0].Name);
                var tmp = this.dtos.Where(x => x.intOpcion == this.intId).FirstOrDefault();
                txtNombre.Text = tmp.strNombre;
                txtPlantilla.Text = tmp.strPlantilla;
                try
                {
                    txtSentencia.Text = Encoding.UTF8.GetString(tmp.arbySentencia);
                }
                catch (Exception)
                {
                    txtSentencia.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void stbNuevo_Click(object sender, EventArgs e)
        {
            try
            {
                this.nuevo();
                using (srProxy.IwcfChatBotClient proxy = new srProxy.IwcfChatBotClient())
                {
                    this.dtos = proxy.opciones(new parOpcion() { intCrud = 0 }).ToList();
                    proxy.Close();
                };
                foreach (var item in this.dtos)
                {
                    lvOpcion.Items.Add(new ListViewItem() { Name = item.intOpcion.ToString(), Text = item.strNombre });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tsbGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                using (srProxy.IwcfChatBotClient proxy = new srProxy.IwcfChatBotClient())
                {
                    if (this.intId == null)
                    {
                        this.dtos = proxy.opciones(new parOpcion()
                        {
                            intCrud = 1,
                            strNombre = txtNombre.Text,
                            strPlantilla = txtPlantilla.Text,
                            arbySentencia = Encoding.UTF8.GetBytes(txtSentencia.Text),
                            strUsuario = System.Security.Principal.WindowsIdentity.GetCurrent().Name
                        }).ToList();
                    }
                    else
                    {
                        this.dtos = proxy.opciones(new parOpcion()
                        {
                            intCrud = 2,
                            intOpcion = this.intId,
                            strNombre = txtNombre.Text,
                            strPlantilla = txtPlantilla.Text,
                            arbySentencia = Encoding.UTF8.GetBytes(txtSentencia.Text),
                            strUsuario = System.Security.Principal.WindowsIdentity.GetCurrent().Name
                        }).ToList();
                    }
                    proxy.Close();
                };
                this.nuevo();
                foreach (var item in this.dtos)
                {
                    lvOpcion.Items.Add(new ListViewItem() { Name = item.intOpcion.ToString(), Text = item.strNombre });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }

        private void tsbEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                using (srProxy.IwcfChatBotClient proxy = new srProxy.IwcfChatBotClient())
                {
                    if (this.intId != null)
                    {
                        this.dtos = proxy.opciones(new parOpcion()
                        {
                            intCrud = 3,
                            intOpcion = this.intId,
                            strUsuario = System.Security.Principal.WindowsIdentity.GetCurrent().Name
                        }).ToList();
                    }
                    proxy.Close();
                };
                this.nuevo();
                foreach (var item in this.dtos)
                {
                    lvOpcion.Items.Add(new ListViewItem() { Name = item.intOpcion.ToString(), Text = item.strNombre });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }   
        }
    }
}
