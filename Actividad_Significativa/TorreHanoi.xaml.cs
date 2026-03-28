using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Actividad_Significativa
{
    public partial class Torre_Hanoi : Window
    {
        private const int MinimoDiscos = 1;
        private const int MaximoDiscos = 8;

        private readonly Dictionary<string, Stack<int>> torres;
        private readonly List<MovimientoHanoi> movimientosPendientes;
        private readonly DispatcherTimer temporizadorAnimacion;
        private readonly Random generadorAleatorio;

        private int totalDiscos;
        private int indiceMovimientoActual;

        // Tres pilas como torres, lista de movimientos precalculados, timer para modo automatico

        public Torre_Hanoi()
        {
            InitializeComponent();

            torres = new Dictionary<string, Stack<int>>
            {
                { "A", new Stack<int>() },
                { "B", new Stack<int>() },
                { "C", new Stack<int>() }
            };

            movimientosPendientes = new List<MovimientoHanoi>();
            temporizadorAnimacion = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(420)
            };
            temporizadorAnimacion.Tick += TemporizadorAnimacion_Tick;
            generadorAleatorio = new Random();

            InicializarEscenario(5);
        }

        private sealed class MovimientoHanoi
        {
            public MovimientoHanoi(string origen, string destino)
            {
                Origen = origen;
                Destino = destino;
            }

            public string Origen { get; }

            public string Destino { get; }
        }

        // Si algo falla, paramos el timer para no seguir moviendo discos en estado inconsistente
        private void EjecutarAccionSegura(Action accion)
        {
            try
            {
                accion();
            }
            catch (Exception ex)
            {
                temporizadorAnimacion.Stop();
                MessageBox.Show($"Se produjo un error {ex.Message}", "Torres de Hanoi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Limitamos discos por rendimiento y tamano del Canvas (18)
        private bool TryLeerCantidadDiscos(out int cantidadDiscos)
        {
            cantidadDiscos = 0;

            if (!int.TryParse(TxtNumeroDiscos.Text?.Trim(), out int valor))
            {
                MessageBox.Show("Ingresa un numero entero valido", "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (valor < MinimoDiscos || valor > MaximoDiscos)
            {
                MessageBox.Show($"La cantidad de discos debe estar entre {MinimoDiscos} y {MaximoDiscos}", "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            cantidadDiscos = valor;
            return true;
        }

        // Torre A recibe discos del mayor al menor (arriba el 1); B y C vacias; sin movimientos pendientes
        private void InicializarEscenario(int cantidadDiscos)
        {
            totalDiscos = cantidadDiscos;
            indiceMovimientoActual = 0;
            movimientosPendientes.Clear();
            temporizadorAnimacion.Stop();

            foreach (KeyValuePair<string, Stack<int>> torre in torres)
            {
                torre.Value.Clear();
            }

            for (int disco = cantidadDiscos; disco >= 1; disco--)
            {
                torres["A"].Push(disco);
            }

            RenderizarTorres();
            ActualizarEstado();
        }

        // Recurrencia clasica: mover n1 al auxiliar, mover el disco grande al destino, mover n1 encima Genera 2^n1 movimientos
        private void GenerarMovimientos(int cantidadDiscos, string origen, string auxiliar, string destino)
        {
            if (cantidadDiscos == 1)
            {
                movimientosPendientes.Add(new MovimientoHanoi(origen, destino));
                return;
            }

            GenerarMovimientos(cantidadDiscos - 1, origen, destino, auxiliar);
            movimientosPendientes.Add(new MovimientoHanoi(origen, destino));
            GenerarMovimientos(cantidadDiscos - 1, auxiliar, origen, destino);
        }

        // Valida regla del puzzle: nunca un disco grande sobre uno mas pequeno
        private void AplicarMovimiento(MovimientoHanoi movimiento)
        {
            if (!torres.TryGetValue(movimiento.Origen, out Stack<int> torreOrigen) ||
                !torres.TryGetValue(movimiento.Destino, out Stack<int> torreDestino))
            {
                throw new InvalidOperationException("El movimiento contiene torres invalidas");
            }

            if (torreOrigen.Count == 0)
            {
                throw new InvalidOperationException("No hay discos para mover en la torre origen");
            }

            int disco = torreOrigen.Peek();
            if (torreDestino.Count > 0 && torreDestino.Peek() < disco)
            {
                throw new InvalidOperationException("Movimiento invalido no se puede colocar un disco grande sobre uno pequeno");
            }

            torreOrigen.Pop();
            torreDestino.Push(disco);
        }

        // Consume movimientosPendientesindice y redibuja el Canvas
        private bool EjecutarSiguienteMovimiento()
        {
            if (indiceMovimientoActual >= movimientosPendientes.Count)
            {
                return false;
            }

            MovimientoHanoi movimiento = movimientosPendientes[indiceMovimientoActual];
            AplicarMovimiento(movimiento);
            indiceMovimientoActual++;

            RenderizarTorres();
            ActualizarEstado();
            return true;
        }

        // Render completo: base, varillas, etiquetas y pilas desde el estado de los Stack<int>
        private void RenderizarTorres()
        {
            CanvasHanoi.Children.Clear();
            DibujarBaseYVarillas();
            DibujarEtiquetasVarillas();

            DibujarDiscosDeTorre(torres["A"], "A");
            DibujarDiscosDeTorre(torres["B"], "B");
            DibujarDiscosDeTorre(torres["C"], "C");
        }

        // Geometria fija en coordenadas del Canvas (base ancha y tres postes)
        private void DibujarBaseYVarillas()
        {
            Rectangle baseMesa = new Rectangle
            {
                Width = 1000,
                Height = 90,
                Fill = (Brush)FindResource("ColorRandom"),
                RadiusX = 4,
                RadiusY = 4
            };
            Canvas.SetLeft(baseMesa, 80);
            Canvas.SetTop(baseMesa, 290);
            CanvasHanoi.Children.Add(baseMesa);

            for (int indice = 0; indice < 3; indice++)
            {
                Rectangle varilla = new Rectangle
                {
                    Width = 20,
                    Height = 220,
                    Fill = (Brush)FindResource("ColorTextoMuted"),
                    RadiusX = 8,
                    RadiusY = 8
                };

                double centroX = ObtenerCentroXPorTorre(indice);
                Canvas.SetLeft(varilla, centroX - (varilla.Width / 2));
                Canvas.SetTop(varilla, 76);
                CanvasHanoi.Children.Add(varilla);
            }
        }

        // Letras sobre cada torre para enlazar la explicacion con la recursion (origen, auxiliar, destino)
        private void DibujarEtiquetasVarillas()
        {
            string[] etiquetas = { "A", "B", "C" };

            for (int indice = 0; indice < etiquetas.Length; indice++)
            {
                TextBlock etiqueta = new TextBlock
                {
                    Text = etiquetas[indice],
                    FontSize = 26,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = (Brush)FindResource("ColorTextoClaro")
                };

                double centroX = ObtenerCentroXPorTorre(indice);
                Canvas.SetLeft(etiqueta, centroX - 8);
                Canvas.SetTop(etiqueta, 30);
                CanvasHanoi.Children.Add(etiqueta);
            }
        }

        // Cada disco es un rectangulo mas ancho cuanto mayor su numero; se apilan desde la base hacia arriba
        private void DibujarDiscosDeTorre(Stack<int> torre, string identificadorTorre)
        {
            int indiceTorre = identificadorTorre == "A" ? 0 : identificadorTorre == "B" ? 1 : 2;
            double centroX = ObtenerCentroXPorTorre(indiceTorre);

            int[] discos = torre.ToArray();
            Array.Reverse(discos);

            double alturaDisco = 28;
            double anchoBase = 110;
            double incrementoAncho = 26;
            double topBase = 290;

            for (int indice = 0; indice < discos.Length; indice++)
            {
                int valorDisco = discos[indice];
                double anchoDisco = anchoBase + ((valorDisco - 1) * incrementoAncho);

                Rectangle disco = new Rectangle
                {
                    Width = anchoDisco,
                    Height = alturaDisco,
                    RadiusX = 14,
                    RadiusY = 14,
                    Fill = ObtenerBrochaDisco(valorDisco),
                    Stroke = (Brush)FindResource("ColorBordeOscuro"),
                    StrokeThickness = 1
                };

                double leftDisco = centroX - (anchoDisco / 2);
                double topDisco = topBase - ((indice + 1) * alturaDisco);

                Canvas.SetLeft(disco, leftDisco);
                Canvas.SetTop(disco, topDisco);
                CanvasHanoi.Children.Add(disco);

                TextBlock etiqueta = new TextBlock
                {
                    Text = valorDisco.ToString(),
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Foreground = (Brush)FindResource("ColorTextoClaro")
                };

                Canvas.SetLeft(etiqueta, centroX - 4);
                Canvas.SetTop(etiqueta, topDisco + 4);
                CanvasHanoi.Children.Add(etiqueta);
            }
        }

        // Tres centros horizontales fijos para alinear postes y discos
        private static double ObtenerCentroXPorTorre(int indiceTorre)
        {
            double[] centros = { 230, 580, 930 };
            return centros[indiceTorre];
        }

        // Paleta rotativa para distinguir discos en la explicacion oral
        private Brush ObtenerBrochaDisco(int valorDisco)
        {
            string[] paleta =
            {
                "ColorPeligro",
                "ColorRandom",
                "ColorAcento",
                "ColorTeal",
                "ColorExito",
                "ColorResaltado",
                "ColorAzulClaro",
                "ColorTextoMuted"
            };

            int indice = (valorDisco - 1) % paleta.Length;
            return (Brush)FindResource(paleta[indice]);
        }

        // Muestra cuantos movimientos lleva la simulacion respecto al total generado
        private void ActualizarEstado()
        {
            int totalMovimientos = movimientosPendientes.Count;
            TxtEstado.Text = $"Discos {totalDiscos}  Movimiento {indiceMovimientoActual} de {totalMovimientos}";
        }

        // Tick del modo automatico: un movimiento por intervalo hasta agotar la lista
        private void TemporizadorAnimacion_Tick(object sender, EventArgs e)
        {
            if (!EjecutarSiguienteMovimiento())
            {
                temporizadorAnimacion.Stop();
            }
        }

        private void BtnResolver_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                if (!TryLeerCantidadDiscos(out int cantidadDiscos))
                {
                    return;
                }

                InicializarEscenario(cantidadDiscos);
                GenerarMovimientos(cantidadDiscos, "A", "B", "C");

                if (movimientosPendientes.Count == 0)
                {
                    return;
                }

                temporizadorAnimacion.Start();
            });
        }

        private void BtnPaso_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                if (movimientosPendientes.Count == 0)
                {
                    if (!TryLeerCantidadDiscos(out int cantidadDiscos))
                    {
                        return;
                    }

                    InicializarEscenario(cantidadDiscos);
                    GenerarMovimientos(cantidadDiscos, "A", "B", "C");
                }

                temporizadorAnimacion.Stop();
                EjecutarSiguienteMovimiento();
            });
        }

        private void BtnReiniciar_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                if (!TryLeerCantidadDiscos(out int cantidadDiscos))
                {
                    return;
                }

                InicializarEscenario(cantidadDiscos);
            });
        }

        // Solo afecta el modo automatico; la lista de movimientos sigue intacta para continuar paso a paso
        private void BtnDetener_Click(object sender, RoutedEventArgs e)
        {
            temporizadorAnimacion.Stop();
            ActualizarEstado();
        }

        // Elige n al azar, actualiza el TextBox y reinicia solo el escenario (sin generar movimientos hasta que el usuario ejecute)
        private void BtnAleatorio_Click(object sender, RoutedEventArgs e)
        {
            EjecutarAccionSegura(() =>
            {
                int cantidadDiscos = generadorAleatorio.Next(MinimoDiscos, MaximoDiscos + 1);
                TxtNumeroDiscos.Text = cantidadDiscos.ToString();
                InicializarEscenario(cantidadDiscos);
            });
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            MainWindow menuPrincipal = new MainWindow();
            menuPrincipal.Show();
            Close();
        }
    }
}

