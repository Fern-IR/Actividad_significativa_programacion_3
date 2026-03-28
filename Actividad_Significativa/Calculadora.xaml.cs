using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Actividad_Significativa
{
    public partial class Calculadora : Window
    {
        private bool _mostrandoResultado = false;

        public Calculadora()
        {
            InitializeComponent();
            TxtDisplay.Text = "0";
        }

        // Vuelve al MainWindow y cierra esta ventana
        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow menuPrincipal = new MainWindow();
                menuPrincipal.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible volver al menu\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Concatena digitos o punto; si veniamos de un resultado, el siguiente tecleo arranca expresion nueva
        private void BotonNumerico_Click(object sender, RoutedEventArgs e)
        {
            Button boton = (Button)sender;
            string valor = boton.Content.ToString();

            if (valor == "." && TxtDisplay.Text.Contains(".") && !_mostrandoResultado)
            {
                return;
            }

            if (TxtDisplay.Text == "0" || TxtDisplay.Text == "Error" || _mostrandoResultado)
            {
                TxtDisplay.Text = "";
                _mostrandoResultado = false;
            }
            TxtDisplay.Text += valor;
        }

        // Anexa operador o parentesis a la misma cadena que muestra TxtDisplay
        private void BotonOperador_Click(object sender, RoutedEventArgs e)
        {
            Button boton = (Button)sender;
            string operador = boton.Content.ToString();

            _mostrandoResultado = false;
            if (TxtDisplay.Text == "Error")
            {
                TxtDisplay.Text = "0";
            }

            if (TxtDisplay.Text == "0" && operador == "(")
            {
                TxtDisplay.Text = operador;
                return;
            }
            TxtDisplay.Text += operador;
        }

        // Retroceso caracter a caracter; si habia resultado mostrado, vuelve a modo edicion limpia
        private void BtnBorrarUno_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_mostrandoResultado || TxtDisplay.Text == "Error")
                {
                    TxtDisplay.Text = "0";
                    _mostrandoResultado = false;
                    return;
                }
                if (TxtDisplay.Text.Length > 0 && TxtDisplay.Text != "0")
                {
                    TxtDisplay.Text = TxtDisplay.Text.Substring(0, TxtDisplay.Text.Length - 1);
                    if (TxtDisplay.Text == "")
                    {
                        TxtDisplay.Text = "0";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible borrar el ultimo valor\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Limpia toda la UI de la calculadora para empezar de cero
        private void BtnLimpiarTodo_Click(object sender, RoutedEventArgs e)
        {
            TxtDisplay.Text = "0";
            TxtExpresionMostrada.Text = string.Empty;
            TxtPostfija.Text = string.Empty;
            TxtResultadoExpr.Text = string.Empty;
            _mostrandoResultado = false;
        }

        // Pipeline: infija > postfija (pila de operadores) > evaluacion (pila de operandos) Coste lineal en longitud de la expresion
        private void BtnIgual_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string expresion = TxtDisplay.Text;
                if (string.IsNullOrEmpty(expresion) || expresion == "Error")
                {
                    return;
                }

                TxtExpresionMostrada.Text = expresion;

                Postfija calculadoraLogica = new Postfija();
                string postfija;

                int resultado = calculadoraLogica.EvaluarExpresionPostFija(expresion, out postfija);

                TxtPostfija.Text = postfija;
                TxtDisplay.Text = resultado.ToString();
                TxtResultadoExpr.Text = resultado.ToString();
            }
            catch (Exception)
            {
                TxtDisplay.Text = "Error";
                TxtResultadoExpr.Text = "La expresion no es valida";
                TxtPostfija.Text = "No disponible";
            }
            finally
            {
                _mostrandoResultado = true;
            }
        }
    }

    // Alias de Stack<string> para hablar de "pila" igual que en clase
    public class Pila : Stack<string> { }

    // Shuntingyard simplificado: precedencia, parentesis y luego evaluacion postfija
    public class Postfija
    {
        public Pila pila = new Pila();
        public List<string> operadores = new List<string> { "*", "/", "+", "-" };

        // Operacion binaria; el orden de operandos lo controla quien llama al evaluar la postfija
        public int CalcularOperacion(int x, int y, string operador)
        {
            int resultado = 0;
            switch (operador)
            {
                case "+":
                    resultado = x + y;
                    break;
                case "-":
                    resultado = x - y;
                    break;
                case "*":
                    resultado = x * y;
                    break;
                case "/":
                    resultado = x / y;
                    break;
                default:
                    break;
            }
            return resultado;
        }

        // Parte la cadena en tokens: agrupa digitos consecutivos en un solo numero
        public List<string> StringToList(string expresion)
        {
            var listaExpresion = new List<string>();
            int indiceListaExpresion = 0;

            listaExpresion.Add(expresion[0].ToString());

            for (int i = 1; i < expresion.Length; i++)
            {
                var valor = expresion[i];

                List<string> opsYPar = new List<string>(operadores);
                opsYPar.Add("(");
                opsYPar.Add(")");

                if (char.IsDigit(valor) && opsYPar.Contains(listaExpresion[indiceListaExpresion]))
                {
                    listaExpresion.Add(valor.ToString());
                    indiceListaExpresion++;
                }
                else if (opsYPar.Contains(valor.ToString()))
                {
                    listaExpresion.Add(valor.ToString());
                    indiceListaExpresion++;
                }
                else
                {
                    string numero = listaExpresion[indiceListaExpresion] + valor;
                    listaExpresion.RemoveAt(indiceListaExpresion);
                    listaExpresion.Add(numero);
                }
            }
            return listaExpresion;
        }

        // Numero mas bajo mayor prioridad (* y antes que + y ) Asi se compara con el tope de la pila
        public int PrecedenciaOperador(string operador)
        {
            int precedencia = 0;
            if (operador == "*" || operador == "/")
            {
                precedencia = 1;
            }
            else if (operador == "+" || operador == "-")
            {
                precedencia = 2;
            }
            return precedencia;
        }

        // Infija a postfija: ( en pila; operadores salen mientras tengan prioridad >; ) vacia hasta (
        public string InFijaToPostFija(string expresionInfija)
        {
            string expresionPostFija = "";
            int numero = 0;

            expresionInfija = expresionInfija.Replace(" ", "");
            var listaExpresionInfija = StringToList(expresionInfija);

            pila.Push("(");
            listaExpresionInfija.Add(")"); // Sentinel: fuerza vaciar operadores al final como un parentesis de cierre.

            foreach (var x in listaExpresionInfija)
            {
                if (Int32.TryParse(x, out numero))
                {
                    expresionPostFija += x + " ";
                }
                else if (x == "(")
                {
                    pila.Push(x.ToString());
                }
                else if (operadores.Contains(x.ToString()))
                {
                    var operadorPila = pila.Peek();
                    int precedenciaOperadorPila = PrecedenciaOperador(operadorPila);
                    int precedenciaOperadorExpresion = PrecedenciaOperador(x.ToString());

                    while (precedenciaOperadorPila <= precedenciaOperadorExpresion && operadores.Contains(operadorPila))
                    {
                        expresionPostFija += pila.Pop() + " ";
                        operadorPila = pila.Peek();
                        precedenciaOperadorPila = PrecedenciaOperador(operadorPila);
                    }
                    pila.Push(x.ToString());
                }
                else if (x == ")")
                {
                    while (pila.Peek() != "(")
                    {
                        expresionPostFija += pila.Pop() + " ";
                    }
                    pila.Pop();
                }
            }
            return expresionPostFija;
        }

        // Postfija: numero > push; operador > pop dos operandos (segundo operando se pop primero), push resultado
        public int EvaluarExpresionPostFija(string expresionInfija, out string expresionPostfija)
        {
            expresionPostfija = InFijaToPostFija(expresionInfija);
            int op1 = 0;
            int op2 = 0;
            int numero = 0;

            // Tokens separados por espacio (la postfija se construyo con " " entre elementos)
            string[] arrayPostFija = expresionPostfija.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> listaPostFija = new List<string>(arrayPostFija);

            foreach (var x in listaPostFija)
            {
                if (Int32.TryParse(x, out numero))
                {
                    pila.Push(x);
                }
                else if (operadores.Contains(x))
                {
                    op1 = Convert.ToInt32(pila.Pop());
                    op2 = Convert.ToInt32(pila.Pop());
                    pila.Push(CalcularOperacion(op2, op1, x).ToString());
                }
            }
            return Convert.ToInt32(pila.Pop());
        }
    }
}