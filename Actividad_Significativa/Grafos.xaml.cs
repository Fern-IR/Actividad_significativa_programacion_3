using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Actividad_Significativa
{
    public partial class Grafos : Window
    {
        private const double NodeRadius = 24.0;
        private readonly Dictionary<string, Dictionary<string, int>> adjacencyList;
        private readonly Dictionary<string, Point> vertexPositions;

        // Grafo en lista de adyacencia (diccionario de vecinos con peso) Las posiciones son solo para dibujar

        public Grafos()
        {
            InitializeComponent();
            adjacencyList = new Dictionary<string, Dictionary<string, int>>(StringComparer.OrdinalIgnoreCase);
            vertexPositions = new Dictionary<string, Point>(StringComparer.OrdinalIgnoreCase);
            CanvasGrafo.SizeChanged += CanvasGrafo_SizeChanged;
            UpdateCounters();
            SetStatus("Listo para crear el grafo", false);
        }

        // Al redimensionar, recalculamos el circulo de layout y redibujamos (sin cambiar la logica del grafo)

        private void CanvasGrafo_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RecalculateLayout();
            RedrawGraph();
        }

        private void BtnNuevoVertice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string vertex = TxtNuevoVertice.Text.Trim();
                AddVertex(vertex);
                TxtNuevoVertice.Clear();
                LogMessage($"Se agrego el vertice {vertex}");
                SetStatus("Vertice agregado correctamente", false);
            }
            catch (Exception ex)
            {
                SetStatus($"No se pudo agregar el vertice {ex.Message}", true);
            }
        }

        private void BtnAgregarLazo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string origin = TxtNodoOrigen.Text.Trim();
                string destination = TxtNodoDestino.Text.Trim();
                int weight = ParseWeight(TxtPesoArista.Text.Trim());

                AddEdge(origin, destination, weight);
                LogMessage($"Se agrego la arista de {origin} a {destination} con peso {weight}");
                SetStatus("Arista agregada correctamente", false);
            }
            catch (Exception ex)
            {
                SetStatus($"No se pudo agregar la arista {ex.Message}", true);
            }
        }

        private void BtnDFS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnsureGraphHasVertices();
                string startVertex = RequireValidStartVertex();
                List<string> order = DepthFirstSearch(startVertex);
                LogMessage($"Recorrido DFS desde {startVertex} {string.Join(" ", order)}");
                SetStatus("DFS ejecutado", false);
            }
            catch (Exception ex)
            {
                SetStatus($"No se pudo ejecutar DFS {ex.Message}", true);
            }
        }

        private void BtnBFS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnsureGraphHasVertices();
                string startVertex = RequireValidStartVertex();
                List<string> order = BreadthFirstSearch(startVertex);
                LogMessage($"Recorrido BFS desde {startVertex} {string.Join(" ", order)}");
                SetStatus("BFS ejecutado", false);
            }
            catch (Exception ex)
            {
                SetStatus($"No se pudo ejecutar BFS {ex.Message}", true);
            }
        }

        private void BtnDijkstra_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnsureGraphHasVertices();
                string startVertex = RequireValidStartVertex();
                Dictionary<string, int> distances = Dijkstra(startVertex);
                string report = string.Join(
                    ", ",
                    distances
                        .OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase)
                        .Select(item => $"{item.Key}: {(item.Value == int.MaxValue ? "INF" : item.Value.ToString(CultureInfo.InvariantCulture))}"));

                LogMessage($"Dijkstra desde {startVertex} {report}");
                SetStatus("Dijkstra ejecutado", false);
            }
            catch (Exception ex)
            {
                SetStatus($"No se pudo ejecutar Dijkstra {ex.Message}", true);
            }
        }

        private void BtnMatrizAdyacencia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnsureGraphHasVertices();
                DataTable matrix = BuildAdjacencyMatrix();
                GridMatriz.ItemsSource = matrix.DefaultView;
                GridMatriz.Visibility = Visibility.Visible;
                ListaAdyacencia.Visibility = Visibility.Collapsed;
                TxtTituloPanel.Text = "Matriz de Adyacencia";
                PanelRepresentacion.Visibility = Visibility.Visible;
                LogMessage("Matriz de adyacencia generada correctamente");
                SetStatus("Matriz de adyacencia visible", false);
            }
            catch (Exception ex)
            {
                SetStatus($"No se pudo generar la matriz de adyacencia {ex.Message}", true);
            }
        }

        private void BtnListaAdyacencia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnsureGraphHasVertices();
                List<string> lines = BuildAdjacencyListLines();
                ListaAdyacencia.ItemsSource = lines;
                ListaAdyacencia.Visibility = Visibility.Visible;
                GridMatriz.Visibility = Visibility.Collapsed;
                TxtTituloPanel.Text = "Lista de Adyacencia";
                PanelRepresentacion.Visibility = Visibility.Visible;
                LogMessage("Lista de adyacencia");
                foreach (string line in lines)
                {
                    LogMessage(line);
                }
                SetStatus("Lista de adyacencia visible", false);
            }
            catch (Exception ex)
            {
                SetStatus($"No se pudo generar la lista de adyacencia {ex.Message}", true);
            }
        }

        private void BtnCerrarPanel_Click(object sender, RoutedEventArgs e)
        {
            PanelRepresentacion.Visibility = Visibility.Collapsed;
            SetStatus("Panel de representacion cerrado", false);
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                SetStatus($"No se pudo volver al menu principal {ex.Message}", true);
            }
        }

        // Nuevo vertice: entrada vacia en la lista de adyacencia y punto en el mapa de posiciones
        private void AddVertex(string vertex)
        {
            if (string.IsNullOrWhiteSpace(vertex))
            {
                throw new InvalidOperationException("Ingresa un nombre de vertice");
            }

            if (adjacencyList.ContainsKey(vertex))
            {
                throw new InvalidOperationException("Ese vertice ya existe");
            }

            adjacencyList[vertex] = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            vertexPositions[vertex] = new Point();
            RecalculateLayout();
            UpdateCounters();
            RedrawGraph();
        }

        // Arista dirigida origen > destino; si ya existia, se reemplaza el peso
        private void AddEdge(string origin, string destination, int weight)
        {
            if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(destination))
            {
                throw new InvalidOperationException("Debes indicar nodo origen y nodo destino");
            }

            if (!adjacencyList.ContainsKey(origin) || !adjacencyList.ContainsKey(destination))
            {
                throw new InvalidOperationException("El origen y el destino deben existir como vertices");
            }

            adjacencyList[origin][destination] = weight;
            UpdateCounters();
            RedrawGraph();
        }

        // DFS iterativo: profundidad primero; el orden de vecinos es alfabetico descendente al apilar
        private List<string> DepthFirstSearch(string startVertex)
        {
            var result = new List<string>();
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var stack = new Stack<string>();
            stack.Push(startVertex);

            while (stack.Count > 0)
            {
                string current = stack.Pop();
                if (!visited.Add(current))
                {
                    continue;
                }

                result.Add(current);
                List<string> neighbors = adjacencyList[current]
                    .Keys
                    .OrderByDescending(name => name, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                foreach (string neighbor in neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        stack.Push(neighbor);
                    }
                }
            }

            return result;
        }

        // BFS por niveles: cola FIFO; vecinos en orden alfabetico ascendente
        private List<string> BreadthFirstSearch(string startVertex)
        {
            var result = new List<string>();
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var queue = new Queue<string>();
            queue.Enqueue(startVertex);
            visited.Add(startVertex);

            while (queue.Count > 0)
            {
                string current = queue.Dequeue();
                result.Add(current);

                foreach (string neighbor in adjacencyList[current].Keys.OrderBy(name => name, StringComparer.OrdinalIgnoreCase))
                {
                    if (visited.Add(neighbor))
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return result;
        }

        // Dijkstra sin cola de prioridad: en cada paso se escanea todo V (sencillo de explicar; O(V^2) en densos)
        private Dictionary<string, int> Dijkstra(string startVertex)
        {
            var distances = adjacencyList.Keys.ToDictionary(vertex => vertex, _ => int.MaxValue, StringComparer.OrdinalIgnoreCase);
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            distances[startVertex] = 0;

            while (visited.Count < adjacencyList.Count)
            {
                string current = GetClosestUnvisitedVertex(distances, visited);
                if (current == string.Empty)
                {
                    break;
                }

                visited.Add(current);
                int currentDistance = distances[current];
                if (currentDistance == int.MaxValue)
                {
                    continue;
                }

                foreach (KeyValuePair<string, int> edge in adjacencyList[current])
                {
                    if (visited.Contains(edge.Key))
                    {
                        continue;
                    }

                    int candidateDistance = currentDistance + edge.Value;
                    if (candidateDistance < distances[edge.Key])
                    {
                        distances[edge.Key] = candidateDistance;
                    }
                }
            }

            return distances;
        }

        // Matriz |V|x|V|: fila origen, columna destino, celda peso o 0 si no hay arista
        private DataTable BuildAdjacencyMatrix()
        {
            List<string> vertices = adjacencyList.Keys.OrderBy(vertex => vertex, StringComparer.OrdinalIgnoreCase).ToList();
            var table = new DataTable();
            table.Columns.Add("Vertice");

            foreach (string columnVertex in vertices)
            {
                table.Columns.Add(columnVertex);
            }

            foreach (string rowVertex in vertices)
            {
                DataRow row = table.NewRow();
                row["Vertice"] = rowVertex;
                foreach (string columnVertex in vertices)
                {
                    row[columnVertex] = adjacencyList[rowVertex].TryGetValue(columnVertex, out int weight) ? weight : 0;
                }
                table.Rows.Add(row);
            }

            return table;
        }

        // Una linea por vertice listando vecinos como destino(peso)
        private List<string> BuildAdjacencyListLines()
        {
            var lines = new List<string>();
            foreach (string vertex in adjacencyList.Keys.OrderBy(vertex => vertex, StringComparer.OrdinalIgnoreCase))
            {
                if (adjacencyList[vertex].Count == 0)
                {
                    lines.Add($"{vertex} sin conexiones");
                    continue;
                }

                IEnumerable<string> edges = adjacencyList[vertex]
                    .OrderBy(edge => edge.Key, StringComparer.OrdinalIgnoreCase)
                    .Select(edge => $"{edge.Key}({edge.Value})");

                lines.Add($"{vertex} conecta con {string.Join(", ", edges)}");
            }

            return lines;
        }

        private string RequireValidStartVertex()
        {
            string startVertex = TxtNodoOrigen.Text.Trim();
            if (string.IsNullOrWhiteSpace(startVertex))
            {
                throw new InvalidOperationException("Debes escribir un valor en el campo Nodo origen por ejemplo Nodo A");
            }

            if (!adjacencyList.ContainsKey(startVertex))
            {
                throw new InvalidOperationException("El nodo de origen no existe en el grafo");
            }

            return startVertex;
        }

        private void EnsureGraphHasVertices()
        {
            if (adjacencyList.Count == 0)
            {
                throw new InvalidOperationException("El grafo esta vacio agrega vertices primero");
            }
        }

        private int ParseWeight(string weightText)
        {
            if (string.IsNullOrWhiteSpace(weightText))
            {
                return 1;
            }

            if (!int.TryParse(weightText, out int weight) || weight < 0)
            {
                throw new InvalidOperationException("El campo peso de arista debe contener un numero entero mayor o igual a 0 ejemplo 5");
            }

            return weight;
        }

        private void UpdateCounters()
        {
            TxtCantVertices.Text = adjacencyList.Count.ToString(CultureInfo.InvariantCulture);
            int edgeCount = adjacencyList.Values.Sum(neighbors => neighbors.Count);
            TxtCantAristas.Text = edgeCount.ToString(CultureInfo.InvariantCulture);
        }

        private void LogMessage(string message)
        {
            TxtConsola.AppendText($"{DateTime.Now:HH:mm:ss}  {message}{Environment.NewLine}");
            TxtConsola.ScrollToEnd();
        }

        private void SetStatus(string message, bool isError)
        {
            TxtEstado.Text = message;
            TxtEstado.Foreground = isError ? GetBrush("ColorPeligro") : GetBrush("ColorExito");
        }

        private Brush GetBrush(string key)
        {
            return (Brush)FindResource(key);
        }

        // Layout circular: reparte angulos 2piV para que el dibujo no se amontone al crecer el grafo
        private void RecalculateLayout()
        {
            if (adjacencyList.Count == 0)
            {
                return;
            }

            double canvasWidth = CanvasGrafo.ActualWidth > 0 ? CanvasGrafo.ActualWidth : 1000;
            double canvasHeight = CanvasGrafo.ActualHeight > 0 ? CanvasGrafo.ActualHeight : 620;
            Point center = new Point(canvasWidth / 2.0, canvasHeight / 2.0);
            double radius = Math.Max(120, Math.Min(canvasWidth, canvasHeight) * 0.34);
            List<string> orderedVertices = adjacencyList.Keys.OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToList();

            for (int i = 0; i < orderedVertices.Count; i++)
            {
                double angle = (2.0 * Math.PI * i) / orderedVertices.Count;
                double x = center.X + radius * Math.Cos(angle);
                double y = center.Y + radius * Math.Sin(angle);
                vertexPositions[orderedVertices[i]] = new Point(x, y);
            }
        }

        // Primero aristas (Insert al canvas en orden), luego circulos y etiquetas encima
        private void RedrawGraph()
        {
            CanvasGrafo.Children.Clear();

            foreach (KeyValuePair<string, Dictionary<string, int>> fromVertex in adjacencyList.OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase))
            {
                foreach (KeyValuePair<string, int> edge in fromVertex.Value.OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase))
                {
                    DrawEdge(fromVertex.Key, edge.Key, edge.Value);
                }
            }

            foreach (string vertex in adjacencyList.Keys.OrderBy(name => name, StringComparer.OrdinalIgnoreCase))
            {
                DrawVertex(vertex);
            }
        }

        private void DrawEdge(string origin, string destination, int weight)
        {
            if (!vertexPositions.ContainsKey(origin) || !vertexPositions.ContainsKey(destination))
            {
                return;
            }

            // La linea va del borde de un circulo al borde del otro (radio fijo), no del centro al centro
            Point from = vertexPositions[origin];
            Point to = vertexPositions[destination];

            if (origin.Equals(destination, StringComparison.OrdinalIgnoreCase))
            {
                DrawSelfLoop(from, weight);
                return;
            }

            Vector direction = to - from;
            if (direction.Length < 1)
            {
                return;
            }

            direction.Normalize();
            Point start = from + (direction * NodeRadius);
            Point end = to - (direction * NodeRadius);

            var line = new Line
            {
                X1 = start.X,
                Y1 = start.Y,
                X2 = end.X,
                Y2 = end.Y,
                Stroke = GetBrush("ColorResaltado"),
                StrokeThickness = 1.8
            };

            CanvasGrafo.Children.Add(line);

            var weightLabel = new TextBlock
            {
                Text = weight.ToString(CultureInfo.InvariantCulture),
                Foreground = GetBrush("ColorTextoClaro"),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Background = GetBrush("ColorSuperficieOscuraAlt"),
                Padding = new Thickness(4, 2, 4, 2)
            };

            double midX = (start.X + end.X) / 2;
            double midY = (start.Y + end.Y) / 2;
            Canvas.SetLeft(weightLabel, midX - 8);
            Canvas.SetTop(weightLabel, midY - 18);
            CanvasGrafo.Children.Add(weightLabel);
        }

        private void DrawSelfLoop(Point center, int weight)
        {
            double loopSize = NodeRadius + 10;
            var loop = new Path
            {
                Stroke = GetBrush("ColorResaltado"),
                StrokeThickness = 1.8,
                Data = new EllipseGeometry(
                    new Point(center.X, center.Y - NodeRadius - 8),
                    loopSize / 2,
                    loopSize / 2)
            };

            CanvasGrafo.Children.Add(loop);

            var weightLabel = new TextBlock
            {
                Text = weight.ToString(CultureInfo.InvariantCulture),
                Foreground = GetBrush("ColorTextoClaro"),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Background = GetBrush("ColorSuperficieOscuraAlt"),
                Padding = new Thickness(4, 2, 4, 2)
            };

            Canvas.SetLeft(weightLabel, center.X - 10);
            Canvas.SetTop(weightLabel, center.Y - NodeRadius - loopSize - 4);
            CanvasGrafo.Children.Add(weightLabel);
        }

        private void DrawVertex(string vertex)
        {
            if (!vertexPositions.ContainsKey(vertex))
            {
                return;
            }

            Point position = vertexPositions[vertex];
            var ellipse = new Ellipse
            {
                Width = NodeRadius * 2,
                Height = NodeRadius * 2,
                Fill = GetBrush("ColorAcento"),
                Stroke = GetBrush("ColorTextoClaro"),
                StrokeThickness = 1.6
            };

            Canvas.SetLeft(ellipse, position.X - NodeRadius);
            Canvas.SetTop(ellipse, position.Y - NodeRadius);
            CanvasGrafo.Children.Add(ellipse);

            var label = new TextBlock
            {
                Text = vertex,
                Foreground = GetBrush("ColorTextoClaro"),
                FontWeight = FontWeights.SemiBold,
                FontSize = 12
            };

            label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Size textSize = label.DesiredSize;
            Canvas.SetLeft(label, position.X - textSize.Width / 2);
            Canvas.SetTop(label, position.Y - textSize.Height / 2);
            CanvasGrafo.Children.Add(label);
        }

        // Paso greedy de Dijkstra: minimo distancia entre los que faltan por relajar
        private string GetClosestUnvisitedVertex(Dictionary<string, int> distances, HashSet<string> visited)
        {
            string bestVertex = string.Empty;
            int bestDistance = int.MaxValue;

            foreach (KeyValuePair<string, int> item in distances)
            {
                if (visited.Contains(item.Key))
                {
                    continue;
                }

                if (item.Value < bestDistance)
                {
                    bestDistance = item.Value;
                    bestVertex = item.Key;
                }
            }

            return bestVertex;
        }
    }
}