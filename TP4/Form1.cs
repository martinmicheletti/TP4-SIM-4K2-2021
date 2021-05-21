using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TP4
{
    public partial class Form1 : Form
    {
        System.Data.DataTable dt;
        System.Data.DataTable dtTest;

        public Form1()
        {
            InitializeComponent();
            rdbPoliticaA.Checked = true;
        }

        private void btnGenerar_Click_1(object sender, EventArgs e)
        {
            if (validarInputs())
            {
                if (validarParametros())
                {

                    // Cantidad de dias a simular 
                    int DiasASimular = Convert.ToInt32(txtDiasASimular.Text);

                    // Cantidad de dias a visualizar
                    int Dias = Convert.ToInt32(txtDiasAVisualizar.Text);

                    // Desde el dia: 
                    int Vista = Convert.ToInt32(txtVista.Text);

                    double diasEntrePedidos = Convert.ToDouble(txtDiasEntrePedidos.Text);

                    // Limpio las tablas
                    //dgvAux2.Rows.Clear();
                    //dgvAux2.Refresh();
                    dgvAux.Rows.Clear();
                    dgvAux.Refresh();


                    Random rand_demanda = new Random();
                    double RNDDemanda = 0;

                    Random rand_demora = new Random();
                    double demanda = 0;

                    Boolean Llega_Pedido;

                    double stock = 20;

                    double KP = 0;

                    // Costo mantenimiento
                    double KM = 3;

                    // Costo por faltante
                    double KS = 4;

                    double CantidadAPedirA = 0;

                    if (txtCantidadAPedir.Text != "")
                    {
                        CantidadAPedirA = Convert.ToDouble(txtCantidadAPedir.Text);
                    }

                    double CantidadAPedirB = 0;

                    int p = 0;
                    double costo_mantenimiento = 0;
                    double costo_faltante = 0;
                    int disponible = 0;
                    double total = 0;
                    double totalacu = 0;

                    int cantidadDiasEntrePedidosPoliticaA = 7;
                    int cantidadDiasEntrePedidosPoliticaB = 10;

                    double acumuladorDemandaPoliticaB = 0;

                    List<DataGridViewRow> listaFilas = new List<DataGridViewRow>();
                    List<DataGridViewRow> listaFilas2 = new List<DataGridViewRow>();

                    for (int i = 0; i < DiasASimular; i++)
                    {
                        Boolean hago_pedido = false;
                        double costo_pedido = 0;
                        double RNDDemora = 0;
                        int demora = 0;

                        if (i == 0)
                        {
                            stock = 20;
                            Llega_Pedido = false;
                        }
                        else
                        {
                            RNDDemanda = Math.Round(rand_demanda.NextDouble(), 2);
                            demanda = CalcularDemanda(RNDDemanda);

                            acumuladorDemandaPoliticaB += demanda;

                            if (i == 1)
                            {
                                // El primer dia hago un pedido (enunciado)
                                hago_pedido = true;
                                //total = costo_pedido + costo_faltante + costo_mantenimiento;
                                // En el caso de la Politica B, se piden los ingresados por parametro o solo los demandados ese primer dia?
                            }

                            if (rdbPoliticaA.Checked)
                            {
                                if (i % diasEntrePedidos == 0)
                                {
                                    hago_pedido = true;
                                }

                                if (hago_pedido)
                                {
                                    RNDDemora = Math.Round(rand_demora.NextDouble(), 2);
                                    demora = CalcularDemoraPedido(RNDDemora);
                                    disponible = CuandoLlega(i, demora);
                                    costo_pedido = CalcularCostoPedidoPedido(CantidadAPedirA);
                                }

                                // Si llega un pedido
                                if (disponible == i)
                                {
                                    stock += CantidadAPedirA;
                                    disponible = 0;
                                }
                            }

                            if (rdbPoliticaB.Checked)
                            {
                                if (i % diasEntrePedidos == 0)
                                {
                                    hago_pedido = true;
                                }

                                if (hago_pedido)
                                {
                                    RNDDemora = Math.Round(rand_demora.NextDouble(), 2);
                                    demora = CalcularDemoraPedido(RNDDemora);
                                    disponible = CuandoLlega(i, demora);
                                    // La cantidad a pedir debe ser la suma de la demanda 
                                    // de los ultimos 10 + el dia actual

                                    CantidadAPedirB += acumuladorDemandaPoliticaB;

                                    costo_pedido = CalcularCostoPedidoPedido(CantidadAPedirB);

                                    // Reinicio el acumulador para que cuente los 10 dias posteriores
                                    acumuladorDemandaPoliticaB = 0;
                                }

                                // Si llega un pedido
                                if (disponible == i)
                                {
                                    stock += CantidadAPedirB;
                                    disponible = 0;
                                    CantidadAPedirB = 0;
                                }
                            }

                            stock -= demanda;

                            if (stock < 0)
                            {
                                stock = 0;
                                costo_faltante = (demanda - stock) * KS;
                            }

                            costo_mantenimiento = stock * KM;
                            total = costo_mantenimiento + costo_faltante + costo_pedido;
                            totalacu += total;
                        }


                        if (i >= Vista && i < Vista + Dias)
                        {
                            dgvAux.Rows.Add();

                            dgvAux.Rows[i - Vista].Cells[0].Value = i;

                            dgvAux.Rows[i - Vista].Cells[1].Value = RNDDemanda;

                            dgvAux.Rows[i - Vista].Cells[2].Value = demanda;

                            dgvAux.Rows[i - Vista].Cells[3].Value = RNDDemora;

                            dgvAux.Rows[i - Vista].Cells[4].Value = demora;

                            dgvAux.Rows[i - Vista].Cells[5].Value = hago_pedido ? "Hago pedido" : "";

                            dgvAux.Rows[i - Vista].Cells[6].Value = disponible;

                            dgvAux.Rows[i - Vista].Cells[7].Value = stock;

                            dgvAux.Rows[i - Vista].Cells[8].Value = costo_pedido;

                            dgvAux.Rows[i - Vista].Cells[9].Value = costo_mantenimiento;

                            dgvAux.Rows[i - Vista].Cells[10].Value = costo_faltante;

                            dgvAux.Rows[i - Vista].Cells[11].Value = total;

                            dgvAux.Rows[i - Vista].Cells[12].Value = totalacu;

                        }

                        if (i == DiasASimular - 1)
                        {
                            {
                                dgvAux.Rows.Add();

                                dgvAux.Rows[Dias].Cells[0].Value = i;

                                dgvAux.Rows[Dias].Cells[1].Value = RNDDemanda;

                                dgvAux.Rows[Dias].Cells[2].Value = demanda;

                                dgvAux.Rows[Dias].Cells[3].Value = RNDDemora;

                                dgvAux.Rows[Dias].Cells[4].Value = demora;

                                dgvAux.Rows[Dias].Cells[5].Value = hago_pedido ? "Hago pedido" : "";

                                dgvAux.Rows[Dias].Cells[6].Value = disponible;

                                dgvAux.Rows[Dias].Cells[7].Value = stock;

                                dgvAux.Rows[Dias].Cells[8].Value = costo_pedido;

                                dgvAux.Rows[Dias].Cells[9].Value = costo_mantenimiento;

                                dgvAux.Rows[Dias].Cells[10].Value = costo_faltante;

                                dgvAux.Rows[Dias].Cells[11].Value = total;

                                dgvAux.Rows[Dias].Cells[12].Value = totalacu;
                            }
                        }
                    }
                }
            }
        }

        private int CalcularDemanda(double rnd)
        {
            //Se puede comparar directamente con los limites superiores de cada intervalo
            int Demanda = 0;
            if (rnd < 0.05)
            {
                Demanda = 0;
            }
            else if (rnd >= 0.05 && rnd < 0.16)
            {
                Demanda = 10;
            }
            else if (rnd >= 0.16 && rnd < 0.34)
            {
                Demanda = 20;
            }
            else if (rnd >= 0.34 && rnd < 0.59)
            {
                Demanda = 30;
            }
            else if (rnd >= 0.59 && rnd < 0.81)
            {
                Demanda = 40;
            }
            else
            {
                Demanda = 50;
            }
            return Demanda;
        }

        private int CalcularDemoraPedido(double rnd)
        {
            //Se puede comparar directamente con los limites superiores de cada intervalo
            int Demora = 0;
            if (rnd < 0.15)
            {
                Demora = 1;
            }
            else if (rnd >=0.15 && rnd < 0.34)
            {
                Demora = 2;
            }
            else if (rnd >= 0.34 && rnd < 0.74)
            {
                Demora = 3;
            }
            
            else
            {
                Demora = 4;
            }
            return Demora;
        }

        private int CuandoLlega(int i, int demora)
        {
            int Llega = 0;

            Llega = i + demora;
            return Llega;
        }

        private double CalcularCostoPedidoPedido(double nro_Pedido)
        {
            double Costo = 0;

            if (nro_Pedido < 101)
            {
                Costo = 200;
            }
            else if (nro_Pedido > 101 && nro_Pedido < 200)
            {
                Costo = 280;
            }
           
            else
            {
                Costo = 300;
            }

            return Costo;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private bool validarInputs()
        {
            if (rdbPoliticaA.Checked)
            {
                if (txtCantidadAPedir.TextLength == 0)
                {
                    MessageBox.Show("Por favor, ingrese todos los datos necesarios");
                    return false;
                }
            }
            if ((txtDiasASimular.TextLength == 0) 
                || (txtDiasAVisualizar.TextLength == 0) 
                || (txtVista.TextLength == 0) 
                || (txtDiasEntrePedidos.TextLength == 0))
            {
                MessageBox.Show("Por favor, ingrese todos los datos necesarios");
                return false;
            }
            return true;
        }

        private bool validarParametros()
        {
            //Dias a simular 
            int DiasASimular = Convert.ToInt32(txtDiasASimular.Text);
            //Dias a visualizar
            int Dias = Convert.ToInt32(txtDiasAVisualizar.Text);
            //Desde: 
            int Vista = Convert.ToInt32(txtVista.Text);
            //Cantidad
            //double Cantidad = Convert.ToDouble(txtCantidadAPedir.Text);
            //Entre pedidos
            double Entre = Convert.ToDouble(txtDiasEntrePedidos.Text);

            if (Dias >= DiasASimular || Vista > DiasASimular)
            {
                MessageBox.Show("Por favor, ingrese parámetros válids");
                //txtDiasASimular.Text = "";
                //txtDiasAVisualizar.Text = "";
                //txtVista.Text = "";
                return false;
            }
            if (Entre <= 0 || DiasASimular <= 0 || Vista < 0 || DiasASimular <= 0)
            {
                MessageBox.Show("Por favor, ingrese parámetros positivos");
                //txtCantidadAPedir.Text = "";
                //txtDiasEntrePedidos.Text = "";
                return false;
            }
            return true;
        }

        private void rdbPoliticaB_CheckedChanged(object sender, EventArgs e)
        {
            txtCantidadAPedir.Enabled = false;
        }

        private void rdbPoliticaA_CheckedChanged(object sender, EventArgs e)
        {
            txtCantidadAPedir.Enabled = true;
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            dgvAux.Rows.Clear();
            dgvAux.Refresh();
            txtCantidadAPedir.Text = "";
            txtDiasASimular.Text = "";
            txtDiasAVisualizar.Text = "";
            txtDiasEntrePedidos.Text = "";
            txtVista.Text = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
