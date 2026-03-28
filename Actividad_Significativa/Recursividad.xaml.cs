using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Actividad_Significativa
{
    public partial class Recursividad : Window
    {
        // Ejemplos clasicos de recursion + memoizacion en Fibonacci; trazas en StringBuilder para la exposicion
        private readonly Random _random = new Random();

        public Recursividad()
        {
            InitializeComponent();
            SetStatus("Listo para calcular", false);
        }

        private void BtnFactorial_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!TryParseInRange(TxtFactorial.Text, 0, 20, "Factorial", out int n))
                {
                    return;
                }

                var trace = new StringBuilder();
                trace.AppendLine("Casos base 0! = 1 y 1! = 1");

                long result = FactorialRecursive(n, trace);

                TxtResFactorial.Text = result.ToString();
                TxtTraceFactorial.Text = trace.ToString();
                TxtCasoBase.Text = "n = 0 da 1";
                TxtComplejidad.Text = "O(n)";
                SetStatus($"El factorial para n = {n} se calculo correctamente", false);
            }
            catch (Exception ex)
            {
                SetStatus($"No se pudo calcular el factorial {ex.Message}", true);
            }
        }

        private void BtnFibonacci_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!TryParseInRange(TxtFibonacci.Text, 0, 45, "Fibonacci", out int n))
                {
                    return;
                }

                var trace = new StringBuilder();
                trace.AppendLine("Casos base F(0) = 0 y F(1) = 1");

                long[] memo = CreateMemoArray(n);
                long result = FibonacciMemo(n, memo, trace);

                TxtResFibonacci.Text = result.ToString();
                TxtTraceFibonacci.Text = trace.ToString();
                TxtCasoBase.Text = "F(0)=0, F(1)=1";
                TxtComplejidad.Text = "O(n)";
                SetStatus($"Fibonacci para n = {n} se calculo correctamente", false);
            }
            catch (Exception ex)
            {
                SetStatus($"No se pudo calcular fibonacci {ex.Message}", true);
            }
        }

        private void BtnSumarVector_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!TryParseVector(TxtVector.Text, out int[] values))
                {
                    return;
                }

                var trace = new StringBuilder();
                trace.AppendLine("Desarrollo de la suma recursiva");

                int result = SumVectorRecursive(values, 0, trace);

                TxtResVector.Text = result.ToString();
                TxtTraceVector.Text = trace.ToString();
                TxtCasoBase.Text = "i == n";
                TxtComplejidad.Text = "O(n)";
                SetStatus("La suma recursiva del vector se completo", false);
            }
            catch (Exception ex)
            {
                SetStatus($"No se pudo sumar el vector {ex.Message}", true);
            }
        }

        private void BtnInvertirNumero_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!long.TryParse((TxtInvertir.Text ?? string.Empty).Trim(), out long number) || number < 0)
                {
                    SetStatus("Para invertir ingresa un entero no negativo valido", true);
                    return;
                }

                var trace = new StringBuilder();
                trace.AppendLine("Proceso de inversion por digitos");

                long result = ReverseNumberRecursive(number, 0, trace);

                TxtResInvertir.Text = result.ToString();
                TxtTraceInvertir.Text = trace.ToString();
                TxtCasoBase.Text = "n == 0";
                TxtComplejidad.Text = "O(d)";
                SetStatus("El numero se invirtio correctamente", false);
            }
            catch (Exception ex)
            {
                SetStatus($"No se pudo invertir el numero {ex.Message}", true);
            }
        }

        private void BtnRandomFactorial_Click(object sender, RoutedEventArgs e)
        {
            int valor = _random.Next(1, 16);
            TxtFactorial.Text = valor.ToString();
            SetStatus($"Se genero un valor aleatorio para factorial {valor}", false);
        }

        private void BtnRandomFibonacci_Click(object sender, RoutedEventArgs e)
        {
            int valor = _random.Next(5, 31);
            TxtFibonacci.Text = valor.ToString();
            SetStatus($"Se genero un valor aleatorio para fibonacci {valor}", false);
        }

        private void BtnRandomVector_Click(object sender, RoutedEventArgs e)
        {
            int cantidad = _random.Next(4, 8);
            int[] valores = new int[cantidad];
            for (int i = 0; i < cantidad; i++)
            {
                valores[i] = _random.Next(1, 100);
            }

            TxtVector.Text = string.Join(",", valores);
            SetStatus("Se genero un vector aleatorio valido", false);
        }

        private void BtnRandomInvertir_Click(object sender, RoutedEventArgs e)
        {
            int cantidadDigitos = _random.Next(4, 7);
            int minimo = (int)Math.Pow(10, cantidadDigitos - 1);
            int maximoExclusivo = (int)Math.Pow(10, cantidadDigitos);
            int valor = _random.Next(minimo, maximoExclusivo);
            TxtInvertir.Text = valor.ToString();
            SetStatus($"Se genero un numero aleatorio de {cantidadDigitos} digitos", false);
        }

        // Cierra esta ventana cuando el menu ya termino de cargar (evita parpadeo si se cierra antes)
        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow menu = new MainWindow();
                menu.ContentRendered += (s, ev) => this.Close();
                menu.Show();
            }
            catch (Exception ex)
            {
                SetStatus($"No se pudo volver al menu principal {ex.Message}", true);
            }
        }

        // n! n * (n1)!; caso base 0 y 1; cada retorno deja una linea en la traza
        private long FactorialRecursive(int n, StringBuilder trace)
        {
            if (n == 0 || n == 1)
            {
                return 1;
            }

            long subResult = FactorialRecursive(n - 1, trace);
            long result = n * subResult;

            trace.AppendLine($"{n}! = {n} x {n - 1}! = {result}");
            return result;
        }

        // Tabla memon: si ya se calculo F(n) se devuelve sin reexpandir el arbol exponencial naive
        private long FibonacciMemo(int n, long[] memo, StringBuilder trace)
        {
            if (n == 0) return 0;
            if (n == 1) return 1;

            if (memo[n] != -1) return memo[n];

            long left = FibonacciMemo(n - 1, memo, trace);
            long right = FibonacciMemo(n - 2, memo, trace);
            memo[n] = left + right;

            trace.AppendLine($"F({n}) = F({n - 1}) + F({n - 2}) = {memo[n]}");
            return memo[n];
        }

        // Suma recursiva: primero el resto del arreglo (index+1), luego se suma valuesindex al acumulado que vuelve
        private int SumVectorRecursive(int[] values, int index, StringBuilder trace)
        {
            if (index >= values.Length)
            {
                return 0;
            }

            int partial = SumVectorRecursive(values, index + 1, trace);
            int current = values[index] + partial;

            if (index == values.Length - 1)
            {
                trace.AppendLine($"Posicion {index} valor {values[index]} ultimo elemento");
            }
            else
            {
                trace.AppendLine($"Posicion {index} valor {values[index]} mas acumulado {partial} igual a {current}");
            }

            return current;
        }

        // reversed acumula por la izquierda (x10 + digito); number se reduce con division entera hasta 0
        private long ReverseNumberRecursive(long number, long reversed, StringBuilder trace)
        {
            if (number == 0)
            {
                trace.AppendLine($"Resultado final {reversed}");
                return reversed;
            }

            long digit = number % 10;
            long newReversed = (reversed * 10) + digit;

            trace.AppendLine($"Queda {number} extraemos {digit} y el inverso actual es {newReversed}");

            return ReverseNumberRecursive(number / 10, newReversed, trace);
        }

        private bool TryParseInRange(string rawText, int minValue, int maxValue, string label, out int parsedValue)
        {
            parsedValue = 0;
            string value = rawText?.Trim() ?? string.Empty;

            if (!int.TryParse(value, out int result))
            {
                SetStatus($"Ingresa un entero valido para {label}", true);
                return false;
            }

            if (result < minValue || result > maxValue)
            {
                SetStatus($"El valor para {label} debe estar entre {minValue} y {maxValue}", true);
                return false;
            }

            parsedValue = result;
            return true;
        }

        private static long[] CreateMemoArray(int n)
        {
            long[] memo = new long[n + 1];
            for (int i = 0; i < memo.Length; i++)
            {
                memo[i] = -1;
            }
            return memo;
        }

        private bool TryParseVector(string rawText, out int[] values)
        {
            values = Array.Empty<int>();
            string source = (rawText ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(source))
            {
                SetStatus("Para sumar el vector ingresa valores separados por comas", true);
                return false;
            }

            string[] parts = source.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                SetStatus("El formato del vector no es valido", true);
                return false;
            }

            int[] parsed = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                if (!int.TryParse(parts[i].Trim(), out parsed[i]))
                {
                    SetStatus("Todos los elementos del vector deben ser enteros", true);
                    return false;
                }
            }

            values = parsed;
            return true;
        }

        private void SetStatus(string message, bool isError)
        {
            TxtEstado.Text = message;
            TxtEstado.Foreground = isError
                ? (Brush)FindResource("ColorPeligro")
                : (Brush)FindResource("ColorExito");
        }
    }
}
