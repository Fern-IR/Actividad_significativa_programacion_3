using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Actividad_Significativa
{
    public partial class Ordenamiento : Window
    {
        // Lista de trabajo en memoria; cada algoritmo copia, ordena y muestra sin perder el original
        private List<int> _listaActual = new List<int>();
        private List<int> _ultimaListaOrdenada = new List<int>();
        private Dictionary<int, List<int>> _indiceHash = new Dictionary<int, List<int>>();

        public Ordenamiento()
        {
            InitializeComponent();
            ReindexarListaOriginal();
        }

        // BOTONES DE INTERFAZ Y CONTROLES GENERALES
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
                MessageBox.Show("Se produjo un error\n\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRandomOrdenamiento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Random rng = new Random();
                int valor = rng.Next(1, 1000);
                TxtInputOrdenamiento.Text = valor.ToString();
                BtnAgregarOrdenamiento_Click(sender, e);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error\n\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAgregarOrdenamiento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(TxtInputOrdenamiento.Text?.Trim(), out int num))
                {
                    _listaActual.Add(num);
                    ReindexarListaOriginal();
                    TxtInputOrdenamiento.Clear();
                    ActualizarDisplay();
                    MostrarAlerta($"Se agrego el valor {num} a la lista");
                }
                else
                {
                    MessageBox.Show("Ingresa un numero entero valido", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error\n\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLimpiarOrdenamiento_Click(object sender, RoutedEventArgs e)
        {
            _listaActual.Clear();
            ActualizarDisplay();
            ReindexarListaOriginal();
            _ultimaListaOrdenada.Clear();
            TxtResultadoBusqueda.Text = "Resultado de busqueda pendiente";
            MostrarAlerta("La lista quedo limpia");
        }

        private void ActualizarDisplay()
        {
            ItmsArregloOriginal.ItemsSource = null;
            ItmsArregloOriginal.ItemsSource = _listaActual;
            ItmsArregloOrdenado.ItemsSource = null;
            TxtContadorElementos.Text = _listaActual.Count.ToString();
        }

        private void MostrarAlerta(string mensaje)
        {
            TxtAlerta.Text = mensaje;
        }

        private void MostrarResultado(string algoritmo, List<int> listaOrdenada)
        {
            ItmsArregloOrdenado.ItemsSource = listaOrdenada;
            _ultimaListaOrdenada = new List<int>(listaOrdenada);
            TxtUltimoAlgoritmo.Text = algoritmo;
            TxtBadgeAlgo.Text = algoritmo;
            MostrarAlerta($"La lista se ordeno con {algoritmo}");
        }

        // Llamadas directas de los botones de la interfaz
        private void BtnOrdenarBurbuja_Click(object sender, RoutedEventArgs e) { BtnBurbuja_Click(sender, e); }
        private void BtnOrdenarSeleccion_Click(object sender, RoutedEventArgs e) { BtnSeleccion_Click(sender, e); }
        private void BtnOrdenarInsercion_Click(object sender, RoutedEventArgs e) { BtnInsercion_Click(sender, e); }
        private void BtnOrdenarMerge_Click(object sender, RoutedEventArgs e) { BtnMergeSort_Click(sender, e); }
        private void BtnOrdenarQuick_Click(object sender, RoutedEventArgs e) { BtnQuickSort_Click(sender, e); }


        // SECCION: ALGORITMOS DE ORDENAMIENTO

        // > 1 ORDENAMIENTO BURBUJA (Bubble Sort) <
        private void BtnBurbuja_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_listaActual.Count == 0) { MostrarAlerta("Agrega elementos antes de ordenar"); return; }
                List<int> lista = new List<int>(_listaActual);
                int n = lista.Count;
                int pasos = 0;

                for (int i = 0; i < n - 1; i++)
                {
                    for (int j = 0; j < n - i - 1; j++)
                    {
                        if (lista[j] > lista[j + 1])
                        {
                            int temp = lista[j];
                            lista[j] = lista[j + 1];
                            lista[j + 1] = temp;
                            pasos++;
                        }
                    }
                }
                MostrarResultado("Burbuja", lista);
                MostrarAlerta($"Burbuja completo el ordenamiento con {pasos} intercambios");
            }
            catch (System.Exception ex) { MessageBox.Show("Error\n" + ex.Message); }
        }

        // > 2 ORDENAMIENTO POR INSERCION (Insertion Sort) <
        private void BtnInsercion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_listaActual.Count == 0) { MostrarAlerta("Agrega elementos antes de ordenar"); return; }
                List<int> lista = new List<int>(_listaActual);
                int n = lista.Count;

                for (int i = 1; i < n; i++)
                {
                    int clave = lista[i];
                    int j = i - 1;
                    while (j >= 0 && lista[j] > clave)
                    {
                        lista[j + 1] = lista[j];
                        j--;
                    }
                    lista[j + 1] = clave;
                }
                MostrarResultado("Inserción", lista);
            }
            catch (System.Exception ex) { MessageBox.Show("Error\n" + ex.Message); }
        }

        // > 3 ORDENAMIENTO POR SELECCION (Selection Sort) <
        private void BtnSeleccion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_listaActual.Count == 0) { MostrarAlerta("Agrega elementos antes de ordenar"); return; }
                List<int> lista = new List<int>(_listaActual);
                int n = lista.Count;

                for (int i = 0; i < n - 1; i++)
                {
                    int minIdx = i;
                    for (int j = i + 1; j < n; j++)
                        if (lista[j] < lista[minIdx]) minIdx = j;

                    int temp = lista[minIdx];
                    lista[minIdx] = lista[i];
                    lista[i] = temp;
                }
                MostrarResultado("Selección", lista);
            }
            catch (System.Exception ex) { MessageBox.Show("Error\n" + ex.Message); }
        }

        // > 4 ORDENAMIENTO SHELL (Shell Sort) <
        private void BtnOrdenarShell_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_listaActual.Count == 0) { MostrarAlerta("Agrega elementos antes de ordenar"); return; }
                List<int> lista = new List<int>(_listaActual);
                int n = lista.Count;
                int gap = n / 2;

                while (gap > 0)
                {
                    for (int i = gap; i < n; i++)
                    {
                        int temp = lista[i];
                        int j = i;
                        while (j >= gap && lista[j - gap] > temp)
                        {
                            lista[j] = lista[j - gap];
                            j -= gap;
                        }
                        lista[j] = temp;
                    }
                    gap /= 2;
                }
                MostrarResultado("Shell Sort", lista);
            }
            catch (System.Exception ex) { MessageBox.Show("Error\n" + ex.Message); }
        }

        // > 5 ORDENAMIENTO MERGE (Merge Sort) <
        private void BtnMergeSort_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_listaActual.Count == 0) { MostrarAlerta("Agrega elementos antes de ordenar"); return; }
                List<int> lista = new List<int>(_listaActual);
                MergeSort(lista, 0, lista.Count - 1);
                MostrarResultado("Merge Sort", lista);
            }
            catch (System.Exception ex) { MessageBox.Show("Error\n" + ex.Message); }
        }

        private void MergeSort(List<int> lista, int izq, int der)
        {
            if (izq >= der) return;
            int medio = izq + (der - izq) / 2;
            MergeSort(lista, izq, medio);
            MergeSort(lista, medio + 1, der);
            Merge(lista, izq, medio, der);
        }

        private void Merge(List<int> lista, int izq, int medio, int der)
        {
            List<int> temp = new List<int>();
            int i = izq, j = medio + 1;

            while (i <= medio && j <= der)
            {
                if (lista[i] <= lista[j]) temp.Add(lista[i++]);
                else temp.Add(lista[j++]);
            }
            while (i <= medio) temp.Add(lista[i++]);
            while (j <= der) temp.Add(lista[j++]);

            for (int k = 0; k < temp.Count; k++)
                lista[izq + k] = temp[k];
        }

        // > 6 ORDENAMIENTO QUICK (Quick Sort) <
        private void BtnQuickSort_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_listaActual.Count == 0) { MostrarAlerta("Agrega elementos antes de ordenar"); return; }
                List<int> lista = new List<int>(_listaActual);
                QuickSort(lista, 0, lista.Count - 1);
                MostrarResultado("Quick Sort", lista);
            }
            catch (System.Exception ex) { MessageBox.Show("Error\n" + ex.Message); }
        }

        private void QuickSort(List<int> lista, int izq, int der)
        {
            if (izq >= der) return;
            int p = Partition(lista, izq, der);
            QuickSort(lista, izq, p - 1);
            QuickSort(lista, p + 1, der);
        }

        private int Partition(List<int> lista, int izq, int der)
        {
            int pivote = lista[der];
            int i = izq - 1;
            for (int j = izq; j < der; j++)
            {
                if (lista[j] <= pivote)
                {
                    i++;
                    int tmp = lista[i]; lista[i] = lista[j]; lista[j] = tmp;
                }
            }
            int t = lista[i + 1]; lista[i + 1] = lista[der]; lista[der] = t;
            return i + 1;
        }

        // > 7 ORDENAMIENTO RADIX (Radix Sort) <
        private void BtnOrdenarRadix_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_listaActual.Count == 0) { MostrarAlerta("Agrega elementos antes de ordenar"); return; }
                if (_listaActual.Any(x => x < 0)) { MostrarAlerta("Radix funciona solo con valores positivos"); return; }

                List<int> lista = new List<int>(_listaActual);
                int max = lista.Max();
                for (int exp = 1; max / exp > 0; exp *= 10)
                {
                    CountingSort(lista, exp);
                }
                MostrarResultado("Radix Sort", lista);
            }
            catch (System.Exception ex) { MessageBox.Show("Error\n" + ex.Message); }
        }

        private void CountingSort(List<int> lista, int exp)
        {
            int n = lista.Count;
            int[] output = new int[n];
            int[] count = new int[10];

            for (int i = 0; i < n; i++) count[(lista[i] / exp) % 10]++;
            for (int i = 1; i < 10; i++) count[i] += count[i - 1];
            for (int i = n - 1; i >= 0; i--)
            {
                output[count[(lista[i] / exp) % 10] - 1] = lista[i];
                count[(lista[i] / exp) % 10]--;
            }
            for (int i = 0; i < n; i++) lista[i] = output[i];
        }

        // > 8 ORDENAMIENTO BUCKET (Bucket Sort) <
        private void BtnOrdenarBucket_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_listaActual.Count == 0) { MostrarAlerta("Agrega elementos antes de ordenar"); return; }

                List<int> lista = new List<int>(_listaActual);
                int bucketCount = Math.Max(1, lista.Count / 5);
                int min = lista.Min();
                int max = lista.Max();
                double range = (max - min) / (double)bucketCount + 1;

                List<List<int>> buckets = new List<List<int>>();
                for (int i = 0; i < bucketCount; i++) buckets.Add(new List<int>());

                foreach (int num in lista)
                {
                    int bucketIndex = (int)((num - min) / range);
                    buckets[bucketIndex].Add(num);
                }

                lista.Clear();
                foreach (var bucket in buckets)
                {
                    bucket.Sort();
                    lista.AddRange(bucket);
                }

                MostrarResultado("Bucket Sort", lista);
            }
            catch (System.Exception ex) { MessageBox.Show("Error\n" + ex.Message); }
        }


        // SECCION: ALGORITMOS DE BUSQUEDA


        private bool TryReadSearchValue(out int valor)
        {
            valor = 0;
            if (!int.TryParse(TxtBusquedaValor.Text?.Trim(), out valor))
            {
                TxtResultadoBusqueda.Text = "Ingresa un numero entero valido para buscar";
                MostrarAlerta("No se pudo ejecutar la busqueda por valor invalido");
                return false;
            }
            return true;
        }

        // > 1 BUSQUEDA SECUENCIAL (Lineal) <
        private int BusquedaSecuencial(List<int> lista, int valor)
        {
            for (int i = 0; i < lista.Count; i++)
            {
                if (lista[i] == valor) return i; // Devuelve el índice en cuanto lo encuentra
            }
            return -1;
        }

        private void BtnBusquedaSecuencial_Click(object sender, RoutedEventArgs e)
        {
            if (_listaActual.Count == 0)
            {
                TxtResultadoBusqueda.Text = "No hay datos en el arreglo de entrada";
                MostrarAlerta("No hay datos para buscar"); return;
            }
            if (!TryReadSearchValue(out int valor)) return;

            int indice = BusquedaSecuencial(_listaActual, valor);
            if (indice >= 0)
            {
                TxtResultadoBusqueda.Text = $"Secuencial: Encontró el valor [{valor}] en el índice [{indice}] del arreglo original.";
                MostrarAlerta("Valor encontrado con busqueda secuencial");
            }
            else
            {
                TxtResultadoBusqueda.Text = $"Secuencial: No se encontró el valor [{valor}].";
                MostrarAlerta("Valor no encontrado");
            }
        }

        // > 2 BUSQUEDA BINARIA <
        private int BusquedaBinaria(List<int> lista, int valor)
        {
            int limiteInferior = 0;
            int limiteSuperior = lista.Count - 1;

            while (limiteInferior <= limiteSuperior)
            {
                int medio = limiteInferior + (limiteSuperior - limiteInferior) / 2;
                if (lista[medio] == valor) return medio;
                if (lista[medio] < valor) limiteInferior = medio + 1;
                else limiteSuperior = medio - 1;
            }
            return -1;
        }

        private void BtnBusquedaBinaria_Click(object sender, RoutedEventArgs e)
        {
            if (_ultimaListaOrdenada.Count == 0)
            {
                TxtResultadoBusqueda.Text = "¡Importante! Primero ORDENA la lista para usar la Búsqueda Binaria.";
                MostrarAlerta("Requiere lista ordenada"); return;
            }
            if (!TryReadSearchValue(out int valor)) return;

            int indice = BusquedaBinaria(_ultimaListaOrdenada, valor);
            if (indice >= 0)
            {
                TxtResultadoBusqueda.Text = $"Binaria: Encontró el valor [{valor}] en el índice [{indice}] del arreglo ORDENADO.";
                MostrarAlerta("Valor encontrado con busqueda binaria");
            }
            else
            {
                TxtResultadoBusqueda.Text = $"Binaria: No se encontró el valor [{valor}].";
                MostrarAlerta("Valor no encontrado");
            }
        }

        // > 3 BUSQUEDA HASH INDEXADA <
        private void ReindexarListaOriginal()
        {
            _indiceHash = ConstruirIndiceHash(_listaActual);
        }

        private Dictionary<int, List<int>> ConstruirIndiceHash(List<int> lista)
        {
            Dictionary<int, List<int>> indice = new Dictionary<int, List<int>>();
            for (int i = 0; i < lista.Count; i++)
            {
                int valor = lista[i];
                if (!indice.ContainsKey(valor))
                {
                    indice[valor] = new List<int>();
                }
                indice[valor].Add(i);
            }
            return indice;
        }

        private List<int> BusquedaHashIndexada(Dictionary<int, List<int>> indice, int valor)
        {
            if (indice.ContainsKey(valor)) return new List<int>(indice[valor]);
            return new List<int>();
        }

        private void BtnBusquedaHashIndexada_Click(object sender, RoutedEventArgs e)
        {
            if (_listaActual.Count == 0)
            {
                TxtResultadoBusqueda.Text = "No hay datos en el arreglo de entrada";
                MostrarAlerta("No hay datos para buscar"); return;
            }
            if (!TryReadSearchValue(out int valor)) return;

            List<int> posiciones = BusquedaHashIndexada(_indiceHash, valor);
            if (posiciones.Count > 0)
            {
                TxtResultadoBusqueda.Text = $"Hash Indexada: Encontró el valor [{valor}] en las posiciones: {string.Join(", ", posiciones)}";
                MostrarAlerta("Valor encontrado con busqueda hash indexada");
            }
            else
            {
                TxtResultadoBusqueda.Text = $"Hash Indexada: No se encontró el valor [{valor}].";
                MostrarAlerta("Valor no encontrado");
            }
        }
    }
}