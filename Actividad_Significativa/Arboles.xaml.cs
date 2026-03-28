using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Actividad_Significativa
{
    public partial class Arboles : Window
    {
        // BST en memoria; ultimoNodo apunta al ultimo insertado (el borrado simple solo quita ese)
        private TreeNode<dynamic> root;
        private TreeNode<dynamic> ultimoNodo;
        private const double yOffset = 60;

        public Arboles()
        {
            InitializeComponent();
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
                MessageBox.Show("Se produjo un error en arboles\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnInsertarNodo_Click(object sender, RoutedEventArgs e)
        {
            BtnAgregarNodo_Click(sender, e);
        }

        private void BtnRandomNodo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Random rng = new Random();
                if (RbCaracteres.IsChecked == true)
                {
                    char valor = (char)rng.Next('A', 'Z' + 1);
                    TxtInputArbol.Text = valor.ToString();
                }
                else
                {
                    int valor = rng.Next(1, 100);
                    TxtInputArbol.Text = valor.ToString();
                }
                BtnAgregarNodo_Click(sender, e);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error\n\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLimpiarArbol_Click(object sender, RoutedEventArgs e)
        {
            BtnReiniciar_Click(sender, e);
        }

        private void BtnInOrden_Click(object sender, RoutedEventArgs e)
        {
            BtnInorden_Click(sender, e);
        }

        private void BtnPreOrden_Click(object sender, RoutedEventArgs e)
        {
            BtnPreorden_Click(sender, e);
        }

        private void BtnPostOrden_Click(object sender, RoutedEventArgs e)
        {
            BtnPostorden_Click(sender, e);
        }

        // Insercion BST estandar: menor a la izquierda, mayor o igual a la derecha (altura promedio O(log n) si equilibrado)
        private void BtnAgregarNodo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(TxtInputArbol.Text))
                {
                    if (RbEnteros.IsChecked == true)
                    {
                        int intValue;
                        if (int.TryParse(TxtInputArbol.Text, out intValue))
                        {
                            if (root == null)
                            {
                                root = new TreeNode<dynamic>(intValue);
                                ultimoNodo = root;
                            }
                            else
                            {
                                InsertNode<dynamic>(root, intValue);
                                ultimoNodo = encontrarUltimoNodo(root, intValue);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Ingresa un entero valido");
                            return;
                        }
                    }
                    else if (RbCaracteres.IsChecked == true)
                    {
                        char charValue;
                        if (!TryReadCharacter(TxtInputArbol.Text, out charValue))
                        {
                            MessageBox.Show("Ingresa un solo caracter valido");
                            return;
                        }

                        if (root == null)
                        {
                            root = new TreeNode<dynamic>(charValue);
                            ultimoNodo = root;
                        }
                        else
                        {
                            InsertNode<dynamic>(root, charValue);
                            ultimoNodo = encontrarUltimoNodo(root, charValue);
                        }
                    }

                    TxtInputArbol.Clear();
                    TxtInputArbol.Focus();
                    ActualizarLienzo();
                }
                else
                {
                    MessageBox.Show("Ingresa un valor");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en arboles\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Baja por la rama correcta hasta encontrar null y enlaza el nuevo nodo
        private void InsertNode<T>(TreeNode<dynamic> node, dynamic value)
        {
            if (value is int intValue && node.Value is int nodeIntValue)
            {
                if (intValue < nodeIntValue)
                {
                    if (node.Left == null) node.Left = new TreeNode<dynamic>(value);
                    else InsertNode<dynamic>(node.Left, value);
                }
                else
                {
                    if (node.Right == null) node.Right = new TreeNode<dynamic>(value);
                    else InsertNode<dynamic>(node.Right, value);
                }
            }
            else if (value is char charValue && node.Value is char nodeCharValue)
            {
                int charComparison = charValue.CompareTo(nodeCharValue);
                if (charComparison < 0)
                {
                    if (node.Left == null) node.Left = new TreeNode<dynamic>(value);
                    else InsertNode<dynamic>(node.Left, value);
                }
                else
                {
                    if (node.Right == null) node.Right = new TreeNode<dynamic>(value);
                    else InsertNode<dynamic>(node.Right, value);
                }
            }
            else
            {
                MessageBox.Show("Los tipos de dato no coinciden");
            }
        }

        // Dibujo preorder implicito: primero lineas (Insert al inicio), luego nodos El offset en X crece con la profundidad del arbol
        private void DrawTree(TreeNode<dynamic> node, Canvas canvas, double x, double y, int depth, int maxDepth)
        {
            maxDepth = calcMaxDepth(root);
            double currentXOffset = 30 * Math.Pow(2, maxDepth - depth - 1);

            if (node == null) return;

            Ellipse ellipse = new Ellipse
            {
                Width = 30,
                Height = 30,
                Fill = new SolidColorBrush(Color.FromRgb(27, 38, 49)),
                Stroke = Brushes.White,
                StrokeThickness = 1
            };
            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);
            canvas.Children.Add(ellipse);

            TextBlock textBlock = new TextBlock
            {
                Text = node.Value.ToString(),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(textBlock, x + 8);
            Canvas.SetTop(textBlock, y + 8);
            canvas.Children.Add(textBlock);

            if (node.Left != null)
            {
                Line lineLeft = new Line
                {
                    X1 = x + 15,
                    Y1 = y + 30,
                    X2 = x - currentXOffset + 15,
                    Y2 = y + yOffset,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                canvas.Children.Insert(0, lineLeft);
                DrawTree(node.Left, canvas, x - currentXOffset, y + yOffset, depth + 1, maxDepth);
            }

            if (node.Right != null)
            {
                Line lineRight = new Line
                {
                    X1 = x + 15,
                    Y1 = y + 30,
                    X2 = x + currentXOffset + 15,
                    Y2 = y + yOffset,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                canvas.Children.Insert(0, lineRight);
                DrawTree(node.Right, canvas, x + currentXOffset, y + yOffset, depth + 1, maxDepth);
            }
        }

        // Altura en nodos: 1 + max(subarbol izq, subarbol der)
        private int calcMaxDepth(TreeNode<dynamic> node)
        {
            if (node == null) return 0;
            int leftDepth = calcMaxDepth(node.Left);
            int rightDepth = calcMaxDepth(node.Right);
            return Math.Max(leftDepth, rightDepth) + 1;
        }

        private void BtnEliminarNodo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ultimoNodo != null)
                {
                    elimUltimoNodo();
                    ActualizarLienzo();
                }
                else
                {
                    MessageBox.Show("No hay nodos para borrar");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en arboles\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // No es borrado BST generico: quita el nodo que marcamos como ultimo insertado
        private void elimUltimoNodo()
        {
            if (ultimoNodo == root) root = null;
            else
            {
                TreeNode<dynamic> parent = padreUltimoNodo(root, ultimoNodo.Value);
                if (parent != null)
                {
                    if (parent.Left == ultimoNodo) parent.Left = null;
                    else if (parent.Right == ultimoNodo) parent.Right = null;
                }
            }
            ultimoNodo = null;
        }

        // Recorrido preorden hasta encontrar el valor (en arbol degenerado equivale a O(n))
        private TreeNode<dynamic> encontrarUltimoNodo(TreeNode<dynamic> node, dynamic value)
        {
            if (node == null) return null;
            if (EqualityComparer<dynamic>.Default.Equals(node.Value, value)) return node;
            TreeNode<dynamic> leftResult = encontrarUltimoNodo(node.Left, value);
            if (leftResult != null) return leftResult;
            return encontrarUltimoNodo(node.Right, value);
        }

        private TreeNode<dynamic> padreUltimoNodo(TreeNode<dynamic> node, dynamic value)
        {
            if (node == null) return null;
            if ((node.Left != null && EqualityComparer<dynamic>.Default.Equals(node.Left.Value, value)) ||
                (node.Right != null && EqualityComparer<dynamic>.Default.Equals(node.Right.Value, value))) return node;

            TreeNode<dynamic> leftResult = padreUltimoNodo(node.Left, value);
            if (leftResult != null) return leftResult;
            return padreUltimoNodo(node.Right, value);
        }

        private void BtnReiniciar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CanvasArbol.Children.Clear();
                ClearTree(root);
                root = null;
                ultimoNodo = null;
                ActualizarLienzo();
                TxtResultadoRecorrido.Clear();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en arboles\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearTree(TreeNode<dynamic> node)
        {
            if (node == null) return;
            ClearTree(node.Left);
            ClearTree(node.Right);
            node.Left = null;
            node.Right = null;
            node.Value = default;
        }

        // 1) Copia el BST a nodos RN 2) Inorder > valores ordenados > reinsercion RN (LLRB) para aproximar balanceo 3) Dibuja colores
        private void BtnBalancear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (root == null) return;
                RedBlackTreeNode<dynamic> redBlackRoot = ConvertToRedBlackTree<dynamic>(root);
                BalanceTree<dynamic>(ref redBlackRoot);
                CanvasArbol.Children.Clear();

                double startX = CanvasArbol.ActualWidth / 2;
                if (startX == 0) startX = 300;
                DrawRedBlackTree<dynamic>(redBlackRoot, CanvasArbol, startX, 30, 1);
                TxtResultadoRecorrido.Text = "Arbol balanceado con estructura rojo negro";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en arboles\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Misma forma que el BST; los colores y rotaciones se aplican despues al insertar de nuevo
        private RedBlackTreeNode<dynamic> ConvertToRedBlackTree<T>(TreeNode<dynamic> binaryNode)
        {
            if (binaryNode == null) return null;
            RedBlackTreeNode<dynamic> redBlackNode = new RedBlackTreeNode<dynamic>(binaryNode.Value);
            redBlackNode.Left = ConvertToRedBlackTree<dynamic>(binaryNode.Left);
            redBlackNode.Right = ConvertToRedBlackTree<dynamic>(binaryNode.Right);
            return redBlackNode;
        }

        // Ordenar valores con inorder y volver a armar con InsertNodeRed mantiene propiedades de arbol de busqueda
        private void BalanceTree<T>(ref RedBlackTreeNode<dynamic> root)
        {
            List<dynamic> values = new List<dynamic>();
            TraverseRedBlackTree<dynamic>(root, values);
            root = null;
            foreach (dynamic value in values)
            {
                InsertNodeRed<dynamic>(ref root, value);
            }
        }

        // Izquierda, raiz, derecha: la lista queda ordenada si el arbol respeta BST
        private void TraverseRedBlackTree<T>(RedBlackTreeNode<dynamic> node, List<dynamic> values)
        {
            if (node == null) return;
            TraverseRedBlackTree<dynamic>(node.Left, values);
            values.Add(node.Value);
            TraverseRedBlackTree<dynamic>(node.Right, values);
        }

        // Tras cada insercion la raiz se pinta negra (regla RN: raiz nunca roja)
        private void InsertNodeRed<T>(ref RedBlackTreeNode<dynamic> roots, dynamic value)
        {
            roots = InsertRed<dynamic>(roots, value);
            roots.IsRed = false;
        }

        // Insercion tipo LLRB: corregimos hijo rojo a la derecha, dos rojos seguidos a la izquierda, y dos hijos rojos (split de color)
        private RedBlackTreeNode<dynamic> InsertRed<T>(RedBlackTreeNode<dynamic> node, dynamic value)
        {
            if (node == null) return new RedBlackTreeNode<dynamic>(value);

            if (CompareValues(value, node.Value) < 0)
                node.Left = InsertRed<dynamic>(node.Left, value);
            else if (CompareValues(value, node.Value) > 0)
                node.Right = InsertRed<dynamic>(node.Right, value);

            if (IsRed<dynamic>(node.Right) && !IsRed<dynamic>(node.Left)) node = RotateLeft<dynamic>(node);
            if (IsRed<dynamic>(node.Left) && IsRed<dynamic>(node.Left.Left)) node = RotateRight<dynamic>(node);
            if (IsRed<dynamic>(node.Left) && IsRed<dynamic>(node.Right)) FlipColors<dynamic>(node);

            return node;
        }

        private bool IsRed<T>(RedBlackTreeNode<dynamic> node) { return node != null && node.IsRed; }

        // Rotacion para llevar el rojo problematico del lado derecho al izquierdo (estilo LLRB)
        private RedBlackTreeNode<dynamic> RotateLeft<T>(RedBlackTreeNode<dynamic> node)
        {
            RedBlackTreeNode<dynamic> pivot = node.Right;
            node.Right = pivot.Left;
            pivot.Left = node;
            pivot.IsRed = node.IsRed;
            node.IsRed = true;
            return pivot;
        }

        // Rotacion cuando hay dos enlaces rojos seguidos a la izquierda
        private RedBlackTreeNode<dynamic> RotateRight<T>(RedBlackTreeNode<dynamic> node)
        {
            RedBlackTreeNode<dynamic> pivot = node.Left;
            node.Left = pivot.Right;
            pivot.Right = node;
            pivot.IsRed = node.IsRed;
            node.IsRed = true;
            return pivot;
        }

        // El padre sube de nivel "negro" virtual: padre rojo, hijos pasan a negros (split)
        private void FlipColors<T>(RedBlackTreeNode<dynamic> node)
        {
            node.IsRed = !node.IsRed;
            node.Left.IsRed = !node.Left.IsRed;
            node.Right.IsRed = !node.Right.IsRed;
        }

        private int CompareValues(dynamic value1, dynamic value2)
        {
            if (value1 is int && value2 is int) return value1 - value2;
            else if (value1 is char && value2 is char) return value1.CompareTo(value2);
            else throw new ArgumentException("No se pudo comparar los valores");
        }

        private int maxDepthRB<T>(RedBlackTreeNode<dynamic> blackTreeNode)
        {
            if (blackTreeNode == null) return 0;
            int leftDepth = maxDepthRB<dynamic>(blackTreeNode.Left);
            int rightDepth = maxDepthRB<dynamic>(blackTreeNode.Right);
            return Math.Max(leftDepth, rightDepth) + 1;
        }

        // Igual idea que DrawTree pero el relleno del circulo refleja IsRed (demostracion visual del balanceo)
        private void DrawRedBlackTree<T>(RedBlackTreeNode<dynamic> node, Canvas canvas, double x, double y, int depth)
        {
            int maxDepthB = maxDepthRB<dynamic>(node);
            double currentXOffset = 30 * Math.Pow(2, maxDepthB - depth);

            if (node == null) return;
            Ellipse ellipse = new Ellipse
            {
                Width = 30,
                Height = 30,
                Fill = node.IsRed ? Brushes.Red : Brushes.Black,
                Stroke = Brushes.White,
                StrokeThickness = 1
            };
            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);
            canvas.Children.Add(ellipse);

            TextBlock textBlock = new TextBlock
            {
                Text = node.Value.ToString(),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(textBlock, x + 8);
            Canvas.SetTop(textBlock, y + 8);
            canvas.Children.Add(textBlock);

            if (node.Left != null)
            {
                Line lineLeft = new Line
                {
                    X1 = x + 15,
                    Y1 = y + 30,
                    X2 = x - currentXOffset + 15,
                    Y2 = y + yOffset,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                canvas.Children.Insert(0, lineLeft);
                DrawRedBlackTree<dynamic>(node.Left, canvas, x - currentXOffset, y + yOffset, depth + 1);
            }
            if (node.Right != null)
            {
                Line lineRight = new Line
                {
                    X1 = x + 15,
                    Y1 = y + 30,
                    X2 = x + currentXOffset + 15,
                    Y2 = y + yOffset,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                canvas.Children.Insert(0, lineRight);
                DrawRedBlackTree<dynamic>(node.Right, canvas, x + currentXOffset, y + yOffset, depth + 1);
            }
        }

        // Centra la raiz en mitad del ancho actual del control (o 300 si aun no midio)
        private void ActualizarLienzo()
        {
            CanvasArbol.Children.Clear();
            if (root != null)
            {
                double startX = CanvasArbol.ActualWidth / 2;
                if (startX == 0) startX = 300;
                DrawTree(root, CanvasArbol, startX, 30, 1, calcMaxDepth(root));
            }
        }

        // Recursivo directo: 1 + nodos izquierda + nodos derecha
        private int ContarNodos(TreeNode<dynamic> node)
        {
            if (node == null) return 0;
            return 1 + ContarNodos(node.Left) + ContarNodos(node.Right);
        }

        private void BtnBuscarNodo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxtInputArbol.Text)) return;

                dynamic valorBuscado;
                if (RbEnteros.IsChecked == true)
                {
                    if (!int.TryParse(TxtInputArbol.Text, out int intVal))
                    {
                        TxtResultadoRecorrido.Text = "Ingresa un entero valido para buscar";
                        return;
                    }
                    valorBuscado = intVal;
                }
                else
                {
                    char charValue;
                    if (!TryReadCharacter(TxtInputArbol.Text, out charValue))
                    {
                        TxtResultadoRecorrido.Text = "Ingresa un solo caracter valido para buscar";
                        return;
                    }

                    valorBuscado = charValue;
                }

                TreeNode<dynamic> encontrado = encontrarUltimoNodo(root, valorBuscado);
                if (encontrado != null)
                {
                    int nivel = EncontrarNivel(root, valorBuscado, 1);
                    var minNode = EncontrarMinimo(root);
                    var maxNode = EncontrarMaximo(root);
                    TxtResultadoRecorrido.Text = $"Nodo {valorBuscado} encontrado en nivel {nivel}\n" +
                                                 $"Minimo {minNode?.Value}  Maximo {maxNode?.Value}";
                }
                else
                {
                    TxtResultadoRecorrido.Text = $"No se encontro el nodo {valorBuscado}";
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en arboles\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int EncontrarNivel(TreeNode<dynamic> node, dynamic value, int level)
        {
            if (node == null) return 0;
            if (EqualityComparer<dynamic>.Default.Equals(node.Value, value)) return level;
            int downLevel = EncontrarNivel(node.Left, value, level + 1);
            if (downLevel != 0) return downLevel;
            return EncontrarNivel(node.Right, value, level + 1);
        }

        private TreeNode<dynamic> EncontrarMinimo(TreeNode<dynamic> node)
        {
            if (node == null) return null;
            while (node.Left != null) node = node.Left;
            return node;
        }

        private TreeNode<dynamic> EncontrarMaximo(TreeNode<dynamic> node)
        {
            if (node == null) return null;
            while (node.Right != null) node = node.Right;
            return node;
        }

        private bool TryReadCharacter(string rawValue, out char value)
        {
            value = '\0';
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return false;
            }

            string trimmed = rawValue.Trim();
            if (trimmed.Length != 1)
            {
                return false;
            }

            value = char.ToUpperInvariant(trimmed[0]);
            return char.IsLetterOrDigit(value);
        }

        private void BtnPreorden_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> res = new List<string>();
                Preorden(root, res);
                TxtResultadoRecorrido.Text = "Preorden " + string.Join(" ", res);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en arboles\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Preorden: raiz, luego subarbol izquierdo, luego derecho
        private void Preorden(TreeNode<dynamic> node, List<string> lista)
        {
            if (node != null)
            {
                lista.Add(node.Value.ToString());
                Preorden(node.Left, lista);
                Preorden(node.Right, lista);
            }
        }

        private void BtnInorden_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> res = new List<string>();
                Inorden(root, res);
                TxtResultadoRecorrido.Text = "Inorden " + string.Join(" ", res);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en arboles\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Inorden en BST de enteroscaracteres devuelve valores ordenados
        private void Inorden(TreeNode<dynamic> node, List<string> lista)
        {
            if (node != null)
            {
                Inorden(node.Left, lista);
                lista.Add(node.Value.ToString());
                Inorden(node.Right, lista);
            }
        }

        private void BtnPostorden_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> res = new List<string>();
                Postorden(root, res);
                TxtResultadoRecorrido.Text = "Postorden " + string.Join(" ", res);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Se produjo un error en arboles\n\n" + ex.Message, "Error de ejecucion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Postorden: util si pensaras en liberar nodos o evaluar expresiones (aqui solo listamos)
        private void Postorden(TreeNode<dynamic> node, List<string> lista)
        {
            if (node != null)
            {
                Postorden(node.Left, lista);
                Postorden(node.Right, lista);
                lista.Add(node.Value.ToString());
            }
        }

        // Ctrl + rueda: escala el LayoutTransform del Canvas (zoom inout sin cambiar coordenadas logicas del arbol)
        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control)
            {
                double factorZoom = 0.1;

                if (e.Delta > 0)
                {
                    ZoomArbol.ScaleX += factorZoom;
                    ZoomArbol.ScaleY += factorZoom;
                }
                else
                {
                    if (ZoomArbol.ScaleX > 0.3)
                    {
                        ZoomArbol.ScaleX -= factorZoom;
                        ZoomArbol.ScaleY -= factorZoom;
                    }
                }

                e.Handled = true;
            }
        }

        public class TreeNode<T>
        {
            public dynamic Value { get; set; }
            public TreeNode<dynamic> Left { get; set; }
            public TreeNode<dynamic> Right { get; set; }

            public TreeNode(dynamic value)
            {
                Value = value;
                Left = null;
                Right = null;
            }
        }

        public class RedBlackTreeNode<T>
        {
            public dynamic Value { get; set; }
            public RedBlackTreeNode<dynamic> Parent { get; set; }
            public RedBlackTreeNode<dynamic> Left { get; set; }
            public RedBlackTreeNode<dynamic> Right { get; set; }
            public bool IsRed { get; set; }

            public RedBlackTreeNode(dynamic value)
            {
                Value = value;
                IsRed = true;
            }
        }
    }
}