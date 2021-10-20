using Simulador_CAG;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Simulador_CAG
{
    public partial class SimuladorForm : Form
    {
        Label[] valores = new Label[16];

        public SimuladorForm()
        {
            InitializeComponent();

            btnInfo.Image = (Image)(new Bitmap(Simulador_CAG.Properties.Resources.Info_Image, btnInfo.Size));
            Simulador.instance().ConnectionsChanged += new Simulador.ConnectionsChangedHandler(connectionsChanged);
            Simulador.instance().ProcessChanged += new Simulador.ProcessChangedHandler(processChanged);

            tableLayoutPanel1.Controls.Add(new Label() { Text = "Registro" }, 0, 0);
            tableLayoutPanel1.Controls.Add(new Label() { Text = "Valor" }, 0, 0);
            for (int line=1; line<=16; line++)
            {
                tableLayoutPanel1.Controls.Add(new Label() { Text = (40000 + line).ToString() }, 0, line);
                valores[line - 1] = new Label() { Text = "0" };
                tableLayoutPanel1.Controls.Add(valores[line-1], 1, line);
            }
        }

        bool LockProcessChanged = false;
        delegate void processChangedCallback(int[] registros);

        private void processChanged(int[] registros)
        {
            if (this.tableLayoutPanel1.InvokeRequired && !LockProcessChanged)
            {
                {
                    lock (this)
                    {
                        LockProcessChanged = true;
                        processChangedCallback d = new processChangedCallback(processChanged);
                        try
                        {
                            this.Invoke(d, registros);
                        }
                        catch (Exception) { }
                        finally
                        {
                            LockProcessChanged = false;
                        }
                    }
                }
            }
            else
            {
                try
                {
                    for (int x = 0; x < 16; x++)
                    {
                        valores[x].Text = registros[x].ToString();
                    }
                }
                catch (Exception)
                { }
            }
  
            
        }

        bool LockConnectionsChanged = false;
        delegate void connectionsChangedCallback(int numberConnections);

        private void connectionsChanged(int numberConnections)
        {
            if (this.lblConnections.InvokeRequired && !LockConnectionsChanged)
            {
                {
                    lock (this)
                    {
                        LockConnectionsChanged = true;
                        connectionsChangedCallback d = new connectionsChangedCallback(connectionsChanged);
                        try
                        {
                            this.Invoke(d, numberConnections);
                        }
                        catch (Exception) { }
                        finally
                        {
                            LockConnectionsChanged = false;
                        }
                    }
                }
            }
            else
            {
                try
                {
                    lblConnections.Text = numberConnections.ToString();
                }
                catch (Exception)
                { }
            }
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
        }

    }

}