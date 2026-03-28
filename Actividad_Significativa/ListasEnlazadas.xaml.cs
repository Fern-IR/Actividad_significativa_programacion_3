using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Actividad_Significativa
{
    public partial class ListasEnlazadas : Window
    {
        // Dos implementaciones en memoria: simple (un puntero) y doble (anterior + siguiente)
        private readonly ListaSimple _listaSimple = new ListaSimple();
        private readonly ListaDoble _listaDoble = new ListaDoble();
        private readonly Random _generador = new Random();

        public ListasEnlazadas()
        {
            InitializeComponent();
            RefrescarVistaCompleta();
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                MainWindow menuPrincipal = new MainWindow();
                menuPrincipal.Show();
                Close();
            });
        }

        private void BtnLimpiarDisplay_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                ListaDisplay.Items.Clear();
                ListaDisplayDoble.Items.Clear();
                MostrarAlerta("Las vistas quedaron limpias");
            });
        }

        private void BtnRandomSimple_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                int valor = _generador.Next(1, 1000);
                _listaSimple.InsertarInicio(valor);
                RefrescarSimple();
                MostrarAlerta("Se agrego el dato " + valor + " a la lista simple");
            });
        }

        private void BtnAgregarInicioSimple_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                int valor;
                if (!TryLeerEntero(TxtInputSimple, "lista simple", out valor))
                {
                    return;
                }

                _listaSimple.InsertarInicio(valor);
                TxtInputSimple.Clear();
                RefrescarSimple();
                MostrarAlerta("Se agrego el valor al inicio de la lista simple");
            });
        }

        private void BtnAgregarFinalSimple_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                int valor;
                if (!TryLeerEntero(TxtInputSimple, "lista simple", out valor))
                {
                    return;
                }

                _listaSimple.InsertarFinal(valor);
                TxtInputSimple.Clear();
                RefrescarSimple();
                MostrarAlerta("Se agrego el valor al final de la lista simple");
            });
        }

        private void BtnEliminarInicioSimple_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                if (!_listaSimple.EliminarInicio())
                {
                    MostrarAlerta("No se puede eliminar porque la lista simple esta vacia");
                    return;
                }

                RefrescarSimple();
                MostrarAlerta("Se elimino el primer nodo de la lista simple");
            });
        }

        private void BtnEliminarFinalSimple_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                if (!_listaSimple.EliminarFinal())
                {
                    MostrarAlerta("No se puede eliminar porque la lista simple esta vacia");
                    return;
                }

                RefrescarSimple();
                MostrarAlerta("Se elimino el ultimo nodo de la lista simple");
            });
        }

        private void BtnBuscarSimple_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                int valor;
                if (!TryLeerEntero(TxtInputSimple, "lista simple", out valor))
                {
                    return;
                }

                TxtResultadoBuscarSimple.Text = _listaSimple.Buscar(valor) ? "SI" : "NO";
                MostrarAlerta("La busqueda en lista simple finalizo");
            });
        }

        private void BtnSumaSimple_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                TxtResultadoSumaSimple.Text = _listaSimple.Sumar().ToString();
                MostrarAlerta("La suma de la lista simple esta lista");
            });
        }

        private void BtnModaSimple_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                int moda;
                TxtResultadoModaSimple.Text = _listaSimple.TryObtenerModa(out moda) ? moda.ToString() : "Vacia";
                MostrarAlerta("La moda de la lista simple esta lista");
            });
        }

        private void BtnMayorSimple_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                int maximo;
                TxtResultadoMayorSimple.Text = _listaSimple.TryObtenerMaximo(out maximo) ? maximo.ToString() : "Vacia";
                MostrarAlerta("El maximo de la lista simple esta listo");
            });
        }

        private void BtnEstaVaciaSimple_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                TxtResultadoVaciaSimple.Text = _listaSimple.EstaVacia ? "SI" : "NO";
                MostrarAlerta("La verificacion de lista simple termino");
            });
        }

        private void BtnOrdenarDescendente_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                if (_listaSimple.EstaVacia)
                {
                    MostrarAlerta("No se puede ordenar porque la lista simple esta vacia");
                    return;
                }

                _listaSimple.OrdenarDescendente();
                RefrescarSimple();
                MostrarAlerta("La lista simple se ordeno de forma descendente");
            });
        }

        private void BtnRandomDoble_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                int valor = _generador.Next(1, 1000);
                _listaDoble.InsertarInicio(valor);
                RefrescarDoble();
                MostrarAlerta("Se agrego el dato " + valor + " a la lista doble");
            });
        }

        private void BtnAgregarInicioDoble_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                int valor;
                if (!TryLeerEntero(TxtInputDoble, "lista doble", out valor))
                {
                    return;
                }

                _listaDoble.InsertarInicio(valor);
                TxtInputDoble.Clear();
                RefrescarDoble();
                MostrarAlerta("Se agrego el valor al inicio de la lista doble");
            });
        }

        private void BtnAgregarFinalDoble_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                int valor;
                if (!TryLeerEntero(TxtInputDoble, "lista doble", out valor))
                {
                    return;
                }

                _listaDoble.InsertarFinal(valor);
                TxtInputDoble.Clear();
                RefrescarDoble();
                MostrarAlerta("Se agrego el valor al final de la lista doble");
            });
        }

        private void BtnEliminarInicioDoble_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                if (!_listaDoble.EliminarInicio())
                {
                    MostrarAlerta("No se puede eliminar porque la lista doble esta vacia");
                    return;
                }

                RefrescarDoble();
                MostrarAlerta("Se elimino el primer nodo de la lista doble");
            });
        }

        private void BtnEliminarFinalDoble_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                if (!_listaDoble.EliminarFinal())
                {
                    MostrarAlerta("No se puede eliminar porque la lista doble esta vacia");
                    return;
                }

                RefrescarDoble();
                MostrarAlerta("Se elimino el ultimo nodo de la lista doble");
            });
        }

        private void BtnBuscarDoble_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                int valor;
                if (!TryLeerEntero(TxtInputDoble, "lista doble", out valor))
                {
                    return;
                }

                TxtResultadoBuscarDoble.Text = _listaDoble.Buscar(valor) ? "SI" : "NO";
                MostrarAlerta("La busqueda en lista doble finalizo");
            });
        }

        private void BtnEstaVaciaDoble_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                TxtResultadoVaciaDoble.Text = _listaDoble.EstaVacia ? "SI" : "NO";
                MostrarAlerta("La verificacion de lista doble termino");
            });
        }

        private void BtnOrdenarAscendente_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                if (_listaDoble.EstaVacia)
                {
                    MostrarAlerta("No se puede ordenar porque la lista doble esta vacia");
                    return;
                }

                _listaDoble.OrdenarAscendente();
                RefrescarDoble();
                MostrarAlerta("La lista doble se ordeno de forma ascendente");
            });
        }

        // Sincroniza ListBox y contadores con el estado interno de las listas

        private void RefrescarVistaCompleta()
        {
            RefrescarSimple();
            RefrescarDoble();
        }

        private void RefrescarSimple()
        {
            CargarElementos(ListaDisplay, _listaSimple.ObtenerRepresentacion());
            TxtContadorSimple.Text = "Nodos " + _listaSimple.Contar();
        }

        private void RefrescarDoble()
        {
            CargarElementos(ListaDisplayDoble, _listaDoble.ObtenerRepresentacion());
            TxtContadorDoble.Text = "Nodos " + _listaDoble.Contar();
        }

        private static void CargarElementos(ListBox listaVisual, IEnumerable<string> elementos)
        {
            listaVisual.Items.Clear();
            foreach (string elemento in elementos)
            {
                listaVisual.Items.Add(elemento);
            }
        }

        private bool TryLeerEntero(TextBox cajaTexto, string nombreSeccion, out int valor)
        {
            string texto = cajaTexto.Text != null ? cajaTexto.Text.Trim() : string.Empty;
            if (int.TryParse(texto, out valor))
            {
                return true;
            }

            MostrarAlerta("Entrada invalida en " + nombreSeccion + " ingresa un numero entero");
            cajaTexto.Focus();
            return false;
        }

        private void MostrarAlerta(string mensaje)
        {
            TxtAlerta.Text = mensaje;
        }

        // Evita que una excepcion en un handler deje la UI sin explicacion; el usuario ve un cuadro de error
        private static void EjecutarAccionSegura(Action accion)
        {
            try
            {
                accion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Se produjo un error durante la operacion\n\n" + ex.Message,
                    "Error de ejecucion",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private sealed class NodoSimple
        {
            public int Valor;
            public NodoSimple Siguiente;

            public NodoSimple(int valor)
            {
                Valor = valor;
            }
        }

        private sealed class ListaSimple
        {
            private NodoSimple _inicio;

            public bool EstaVacia
            {
                get { return _inicio == null; }
            }

            // Nuevo nodo apunta al antiguo inicio; actualiza cabeza en tiempo constante
            public void InsertarInicio(int valor)
            {
                NodoSimple nuevoNodo = new NodoSimple(valor);
                nuevoNodo.Siguiente = _inicio;
                _inicio = nuevoNodo;
            }

            // Recorre hasta el ultimo nodo simple para enlazar el nuevo al final
            public void InsertarFinal(int valor)
            {
                NodoSimple nuevoNodo = new NodoSimple(valor);
                if (_inicio == null)
                {
                    _inicio = nuevoNodo;
                    return;
                }

                NodoSimple nodoActual = _inicio;
                while (nodoActual.Siguiente != null)
                {
                    nodoActual = nodoActual.Siguiente;
                }

                nodoActual.Siguiente = nuevoNodo;
            }

            // Avanza _inicio al siguiente; O(1)
            public bool EliminarInicio()
            {
                if (_inicio == null)
                {
                    return false;
                }

                _inicio = _inicio.Siguiente;
                return true;
            }

            // Recorre hasta el penultimo para cortar la referencia Siguiente del ultimo
            public bool EliminarFinal()
            {
                if (_inicio == null)
                {
                    return false;
                }

                if (_inicio.Siguiente == null)
                {
                    _inicio = null;
                    return true;
                }

                NodoSimple nodoActual = _inicio;
                while (nodoActual.Siguiente.Siguiente != null)
                {
                    nodoActual = nodoActual.Siguiente;
                }

                nodoActual.Siguiente = null;
                return true;
            }

            // Recorrido secuencial hasta encontrar el entero (o fallar)
            public bool Buscar(int valorBuscado)
            {
                NodoSimple nodoActual = _inicio;
                while (nodoActual != null)
                {
                    if (nodoActual.Valor == valorBuscado)
                    {
                        return true;
                    }

                    nodoActual = nodoActual.Siguiente;
                }

                return false;
            }

            // Un solo pase acumulando valores
            public int Sumar()
            {
                int suma = 0;
                NodoSimple nodoActual = _inicio;
                while (nodoActual != null)
                {
                    suma += nodoActual.Valor;
                    nodoActual = nodoActual.Siguiente;
                }

                return suma;
            }

            // Compara cada nodo con el maximo provisional
            public bool TryObtenerMaximo(out int maximo)
            {
                maximo = 0;
                if (_inicio == null)
                {
                    return false;
                }

                maximo = _inicio.Valor;
                NodoSimple nodoActual = _inicio.Siguiente;
                while (nodoActual != null)
                {
                    if (nodoActual.Valor > maximo)
                    {
                        maximo = nodoActual.Valor;
                    }

                    nodoActual = nodoActual.Siguiente;
                }

                return true;
            }

            // Primera pasada cuenta frecuencias; segunda elige el valor mas repetido
            public bool TryObtenerModa(out int moda)
            {
                moda = 0;
                if (_inicio == null)
                {
                    return false;
                }

                Dictionary<int, int> frecuencias = new Dictionary<int, int>();
                NodoSimple nodoActual = _inicio;
                while (nodoActual != null)
                {
                    if (!frecuencias.ContainsKey(nodoActual.Valor))
                    {
                        frecuencias[nodoActual.Valor] = 0;
                    }

                    frecuencias[nodoActual.Valor]++;
                    nodoActual = nodoActual.Siguiente;
                }

                int mejorFrecuencia = int.MinValue;
                foreach (KeyValuePair<int, int> entrada in frecuencias)
                {
                    if (entrada.Value > mejorFrecuencia)
                    {
                        mejorFrecuencia = entrada.Value;
                        moda = entrada.Key;
                    }
                }

                return true;
            }

            // Extrae a List<int>, ordena con el comparador del framework y reconstruye por inserciones al inicio
            public void OrdenarDescendente()
            {
                List<int> valores = ExtraerValores();
                valores.Sort((a, b) => b.CompareTo(a));
                ReconstruirDesdeValores(valores);
            }

            // Cuenta saltando con Siguiente hasta null
            public int Contar()
            {
                int cantidad = 0;
                NodoSimple nodoActual = _inicio;
                while (nodoActual != null)
                {
                    cantidad++;
                    nodoActual = nodoActual.Siguiente;
                }

                return cantidad;
            }

            // Cada item del ListBox es el dato de un nodo en orden de enlace
            public IEnumerable<string> ObtenerRepresentacion()
            {
                List<string> salida = new List<string>();
                NodoSimple nodoActual = _inicio;
                if (nodoActual == null)
                {
                    salida.Add("lista vacia");
                    return salida;
                }

                while (nodoActual != null)
                {
                    salida.Add(nodoActual.Valor.ToString());
                    nodoActual = nodoActual.Siguiente;
                }

                return salida;
            }

            private List<int> ExtraerValores()
            {
                List<int> valores = new List<int>();
                NodoSimple nodoActual = _inicio;
                while (nodoActual != null)
                {
                    valores.Add(nodoActual.Valor);
                    nodoActual = nodoActual.Siguiente;
                }

                return valores;
            }

            private void ReconstruirDesdeValores(List<int> valores)
            {
                _inicio = null;
                for (int indice = valores.Count - 1; indice >= 0; indice--)
                {
                    InsertarInicio(valores[indice]);
                }
            }
        }

        private sealed class NodoDoble
        {
            public int Valor;
            public NodoDoble Siguiente;
            public NodoDoble Anterior;

            public NodoDoble(int valor)
            {
                Valor = valor;
            }
        }

        private sealed class ListaDoble
        {
            private NodoDoble _inicio;

            public bool EstaVacia
            {
                get { return _inicio == null; }
            }

            // Enlaza el nuevo delante del actual y rearma punteros AnteriorSiguiente del antiguo inicio
            public void InsertarInicio(int valor)
            {
                NodoDoble nuevoNodo = new NodoDoble(valor);
                if (_inicio != null)
                {
                    _inicio.Anterior = nuevoNodo;
                    nuevoNodo.Siguiente = _inicio;
                }

                _inicio = nuevoNodo;
            }

            // Recorre hasta el ultimo; el nuevo nodo queda como final y enlaza Anterior al penultimo
            public void InsertarFinal(int valor)
            {
                NodoDoble nuevoNodo = new NodoDoble(valor);
                if (_inicio == null)
                {
                    _inicio = nuevoNodo;
                    return;
                }

                NodoDoble nodoActual = _inicio;
                while (nodoActual.Siguiente != null)
                {
                    nodoActual = nodoActual.Siguiente;
                }

                nodoActual.Siguiente = nuevoNodo;
                nuevoNodo.Anterior = nodoActual;
            }

            // Avanza cabeza y limpia Anterior del nuevo primer nodo
            public bool EliminarInicio()
            {
                if (_inicio == null)
                {
                    return false;
                }

                _inicio = _inicio.Siguiente;
                if (_inicio != null)
                {
                    _inicio.Anterior = null;
                }

                return true;
            }

            // Va al ultimo nodo y corta la referencia desde el penultimo
            public bool EliminarFinal()
            {
                if (_inicio == null)
                {
                    return false;
                }

                if (_inicio.Siguiente == null)
                {
                    _inicio = null;
                    return true;
                }

                NodoDoble nodoActual = _inicio;
                while (nodoActual.Siguiente != null)
                {
                    nodoActual = nodoActual.Siguiente;
                }

                nodoActual.Anterior.Siguiente = null;
                return true;
            }

            // Igual que en la lista simple: recorrido hacia adelante con Siguiente
            public bool Buscar(int valorBuscado)
            {
                NodoDoble nodoActual = _inicio;
                while (nodoActual != null)
                {
                    if (nodoActual.Valor == valorBuscado)
                    {
                        return true;
                    }

                    nodoActual = nodoActual.Siguiente;
                }

                return false;
            }

            // Extrae valores, ordena ascendente y reconstruye enlazando de nuevo por el inicio
            public void OrdenarAscendente()
            {
                List<int> valores = ExtraerValores();
                valores.Sort();
                ReconstruirDesdeValores(valores);
            }

            // Igual que Contar en lista simple: solo seguimos Siguiente
            public int Contar()
            {
                int cantidad = 0;
                NodoDoble nodoActual = _inicio;
                while (nodoActual != null)
                {
                    cantidad++;
                    nodoActual = nodoActual.Siguiente;
                }

                return cantidad;
            }

            // Representacion lineal leyendo solo hacia adelante (Anterior no se muestra en la UI)
            public IEnumerable<string> ObtenerRepresentacion()
            {
                List<string> salida = new List<string>();
                NodoDoble nodoActual = _inicio;
                if (nodoActual == null)
                {
                    salida.Add("lista vacia");
                    return salida;
                }

                while (nodoActual != null)
                {
                    salida.Add(nodoActual.Valor.ToString());
                    nodoActual = nodoActual.Siguiente;
                }

                return salida;
            }

            private List<int> ExtraerValores()
            {
                List<int> valores = new List<int>();
                NodoDoble nodoActual = _inicio;
                while (nodoActual != null)
                {
                    valores.Add(nodoActual.Valor);
                    nodoActual = nodoActual.Siguiente;
                }

                return valores;
            }

            private void ReconstruirDesdeValores(List<int> valores)
            {
                _inicio = null;
                for (int indice = valores.Count - 1; indice >= 0; indice--)
                {
                    InsertarInicio(valores[indice]);
                }
            }
        }
    }
}
