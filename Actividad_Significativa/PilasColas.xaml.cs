using System;
using System.Windows;
using System.Windows.Controls;

namespace Actividad_Significativa
{
    public partial class PilasColas : Window
    {
        // Implementacion enlazada (no arreglo): pushpop en O(1), la UI lista desde el tope
        private PilaEnlazada pila = new PilaEnlazada();
        private ColaEnlazada cola = new ColaEnlazada();

        public PilasColas()
        {
            InitializeComponent();
            ActualizarUIsPila();
            ActualizarUIsCola();
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow menuPrincipal = new MainWindow();
                menuPrincipal.Show();
                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en pilas y colas\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Push: respeta tope maximo si el usuario escribio un limite en la caja correspondiente
        private void BtnInsertarPila_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(TxtResMaxPila.Text) && int.TryParse(TxtResMaxPila.Text, out int maximo))
                {
                    if (LstVisPila.Items.Count >= maximo)
                    {
                        MostrarMensaje("La pila llego al tamano maximo");
                        return;
                    }
                }

                if (string.IsNullOrWhiteSpace(TxtInputPila.Text))
                {
                    MostrarMensaje("Ingresa un valor para apilar");
                    return;
                }

                pila.Push(TxtInputPila.Text.Trim());
                TxtInputPila.Clear();
                ActualizarUIsPila();
                LimpiarEstadosPila();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en pilas y colas\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Pop: saca el ultimo apilado y refresca la lista visual
        private void BtnQuitarPila_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (pila.EstaVacia())
                {
                    MostrarMensaje("La pila esta vacia");
                    return;
                }
                string extraido = pila.Pop();
                MostrarMensaje($"Se retiro {extraido} de la pila");
                ActualizarUIsPila();
                LimpiarEstadosPila();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en pilas y colas\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnVaciaPila_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TxtResVaciaPila.Text = pila.EstaVacia() ? "Sí" : "No";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en pilas y colas\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLlenaPila_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(TxtResMaxPila.Text) && int.TryParse(TxtResMaxPila.Text, out int maximo))
                {
                    TxtResLlenaPila.Text = LstVisPila.Items.Count >= maximo ? "Sí" : "No";
                }
                else
                {
                    TxtResLlenaPila.Text = "Sin limite";
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en pilas y colas\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCimaPila_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (pila.EstaVacia())
                {
                    TxtResCimaPila.Text = "Vacia";
                    return;
                }
                TxtResCimaPila.Text = pila.Tope();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en pilas y colas\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLimpiarPila_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pila = new PilaEnlazada();
                ActualizarUIsPila();
                LimpiarEstadosPila();
                MostrarMensaje("La pila se reinicio");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en pilas y colas\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRandomPila_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Random rng = new Random();
                TxtInputPila.Text = rng.Next(1, 100).ToString();
                BtnInsertarPila_Click(sender, e);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error\n\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnMaxPila_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtResMaxPila.Text) && int.TryParse(TxtResMaxPila.Text, out int maximo))
            {
                MostrarMensaje($"Tamano maximo establecido en {maximo}");
            }
            else
            {
                MostrarMensaje("Ingresa un numero valido");
            }
        }

        // De tope hacia abajo: el primer item del ListBox es la cima de la pila
        private void ActualizarUIsPila()
        {
            LstVisPila.Items.Clear();
            Nodo actual = pila.tope;
            while (actual != null)
            {
                LstVisPila.Items.Add(actual.key);
                actual = actual.next;
            }
        }

        private void LimpiarEstadosPila()
        {
            TxtResVaciaPila.Clear();
            TxtResCimaPila.Clear();
            if (TxtResLlenaPila != null) TxtResLlenaPila.Clear();
        }

        private void BtnInsertarCola_Click(object sender, RoutedEventArgs e)
        {
            BtnEncolar_Click(sender, e);
        }

        private void BtnQuitarCola_Click(object sender, RoutedEventArgs e)
        {
            BtnDesencolar_Click(sender, e);
        }

        private void BtnRandomCola_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Random rng = new Random();
                TxtInputCola.Text = rng.Next(1, 100).ToString();
                BtnEncolar_Click(sender, e);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error\n\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLlenaCola_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(TxtResMaxCola.Text) && int.TryParse(TxtResMaxCola.Text, out int maximo))
                {
                    TxtResLlenaCola.Text = LstVisCola.Items.Count >= maximo ? "Sí" : "No";
                }
                else
                {
                    TxtResLlenaCola.Text = "Sin limite";
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en pilas y colas\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnMaxCola_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtResMaxCola.Text) && int.TryParse(TxtResMaxCola.Text, out int maximo))
            {
                MostrarMensaje($"Tamano maximo de cola establecido en {maximo}");
            }
            else
            {
                MostrarMensaje("Ingresa un numero valido para la cola");
            }
        }

        // Encolar al final de la lista; puntero final evita recorrer toda la cola
        private void BtnEncolar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(TxtResMaxCola.Text) && int.TryParse(TxtResMaxCola.Text, out int maximo))
                {
                    if (LstVisCola.Items.Count >= maximo)
                    {
                        MostrarMensaje("La cola llego al tamano maximo");
                        return;
                    }
                }

                if (string.IsNullOrWhiteSpace(TxtInputCola.Text))
                {
                    MostrarMensaje("Ingresa un valor para encolar");
                    return;
                }
                cola.Enqueue(TxtInputCola.Text.Trim());
                TxtInputCola.Clear();
                ActualizarUIsCola();
                LimpiarEstadosCola();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en pilas y colas\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Desencolar: siempre desde frente (FIFO)
        private void BtnDesencolar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cola.EstaVacia())
                {
                    MostrarMensaje("La cola esta vacia");
                    return;
                }
                string extraido = cola.Dequeue();
                MostrarMensaje($"Se retiro {extraido} de la cola");
                ActualizarUIsCola();
                LimpiarEstadosCola();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en pilas y colas\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnVaciaCola_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TxtResVaciaCola.Text = cola.EstaVacia() ? "Sí" : "No";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en pilas y colas\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnFrenteCola_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cola.EstaVacia())
                {
                    TxtResFrenteCola.Text = "Vacia";
                    return;
                }
                TxtResFrenteCola.Text = cola.Frente();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en pilas y colas\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLimpiarCola_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cola = new ColaEnlazada();
                ActualizarUIsCola();
                LimpiarEstadosCola();
                MostrarMensaje("La cola se reinicio");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en pilas y colas\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Lista la cola de frente a final (orden de llegada)
        private void ActualizarUIsCola()
        {
            LstVisCola.Items.Clear();
            Nodo actual = cola.frente;
            while (actual != null)
            {
                LstVisCola.Items.Add(actual.key);
                actual = actual.next;
            }
        }

        private void LimpiarEstadosCola()
        {
            TxtResVaciaCola.Clear();
            if (TxtResLlenaCola != null) TxtResLlenaCola.Clear();
            TxtResFrenteCola.Clear();
        }

        private void MostrarMensaje(string msg)
        {
            TxtStatusBar.Text = msg;
        }
    }

    public class Nodo
    {
        public string key;
        public Nodo next;

        public Nodo(string key)
        {
            this.key = key;
            this.next = null;
        }
    }

    public class PilaEnlazada
    {
        public Nodo tope = null;

        // LIFO: el nuevo nodo pasa a ser el tope
        public void Push(string key)
        {
            Nodo nuevo = new Nodo(key);
            nuevo.next = tope;
            tope = nuevo;
        }

        // Descarta el nodo del tope y devuelve su valor
        public string Pop()
        {
            if (EstaVacia()) throw new InvalidOperationException("La pila esta vacia");
            string valor = tope.key;
            tope = tope.next;
            return valor;
        }

        // Peek: mira el tope sin modificar la lista
        public string Tope()
        {
            if (EstaVacia()) throw new InvalidOperationException("La pila esta vacia");
            return tope.key;
        }

        public bool EstaVacia()
        {
            return tope == null;
        }
    }

    public class ColaEnlazada
    {
        public Nodo frente = null;
        public Nodo final = null;

        // FIFO: nuevo elemento al final; si estaba vacia, frente y final apuntan al mismo nodo
        public void Enqueue(string key)
        {
            Nodo nuevo = new Nodo(key);
            if (EstaVacia())
            {
                frente = nuevo;
                final = nuevo;
            }
            else
            {
                final.next = nuevo;
                final = nuevo;
            }
        }

        // Avanza frente; si queda vacia, tambien anula final
        public string Dequeue()
        {
            if (EstaVacia()) throw new InvalidOperationException("La cola esta vacia");
            string valor = frente.key;
            frente = frente.next;
            if (frente == null) final = null;
            return valor;
        }

        // Consulta el frente sin desencolar
        public string Frente()
        {
            if (EstaVacia()) throw new InvalidOperationException("La cola esta vacia");
            return frente.key;
        }

        public bool EstaVacia()
        {
            return frente == null;
        }
    }
}
