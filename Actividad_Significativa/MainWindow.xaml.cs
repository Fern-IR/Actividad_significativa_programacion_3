using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Actividad_Significativa
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOrdenamiento_Click(object sender, RoutedEventArgs e)
        {
            Ordenamiento ventanaOrdenamiento = new Ordenamiento();
            ventanaOrdenamiento.Show();
            this.Close();
        }

        private void btnListas_Click(object sender, RoutedEventArgs e)
        {
            ListasEnlazadas ventanaListas = new ListasEnlazadas();
            ventanaListas.Show();
            this.Close();
        }

        private void btnPilasColas_Click(object sender, RoutedEventArgs e)
        {
            PilasColas ventanaPilasColas = new PilasColas();
            ventanaPilasColas.Show();
            this.Close();
        }

        private void btnCalculadora_Click(object sender, RoutedEventArgs e)
        {
            Calculadora ventanaCalculadora = new Calculadora();
            ventanaCalculadora.Show();
            this.Close();
        }

        private void btnArboles_Click(object sender, RoutedEventArgs e)
        {
            Arboles ventanaArboles = new Arboles();
            ventanaArboles.Show();
            this.Close();
        }

        private void btnGrafos_Click(object sender, RoutedEventArgs e)
        {
            Grafos ventanaGrafos = new Grafos();
            ventanaGrafos.Show();
            this.Close();
        }

        private void btnRecursividad_Click(object sender, RoutedEventArgs e)
        {
            Recursividad ventanaRecursividad = new Recursividad();
            ventanaRecursividad.Show();
            this.Close();
        }

        private void btnTorreHanoi_Click(object sender, RoutedEventArgs e)
        {
            Torre_Hanoi ventanaTorreHanoi = new Torre_Hanoi();
            ventanaTorreHanoi.Show();
            this.Close();
        }
    }
}
