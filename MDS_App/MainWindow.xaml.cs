using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using MDS_;

namespace MDS_App
{
    public partial class MainWindow : Window
    {
        const int maxSimplexDim = 500;
        const int maxHypercubeDim = 13;

        Visualization v;
        DispatcherTimer timer;
        IMinimalizationMethod minimalization;
        MDS mds;
        ISolution s;
        double zoom = 250.0;
        bool simulationInProgress = false;
        bool drawPts = false;

        Polytope polytope = Polytope.Simplex;
        double alpha = 0.2;
        int interval = 16;
        static double[,] d0;
        int dimX = 3;
        const int dimY = 2;
        double epsilon = 0.000001;

        public MainWindow()
        {
            InitializeComponent();

            // Controls
            startButton.Click += ToggleSimulation;
            createButton.Click += ResetSimulation;
            clearButton.Click += ClearCanvas;
            zoomSlider.ValueChanged += ZoomChanged;
            dimensionControl.ValueChanged += DimensionValueChanged;
            this.SizeChanged += MainWindowSizeChanged;
            polytopeTypeBox.SelectionChanged += PolytopeTypeChanged;
            stepSizeSlider.ValueChanged += StepSizeChanged;
            intervalSlider.ValueChanged += IntervalChanged;
            drawPointsControl.Click += TogglePoints;
            epsilonControl.ValueChanged += EpsilonChanged;

            ToggleControlsClear(true);

            // Refreshing visualization
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(TimerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, interval);
        }

        private void EpsilonChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (epsilonControl.Value.HasValue)
                epsilon = epsilonControl.Value.Value;
        }

        private void TogglePoints(object sender, RoutedEventArgs e)
        {
            if (v != null)
            {
                drawPts = drawPointsControl.IsChecked.Value;
                v.DrawPoints = drawPts;
                DrawSolution(s, zoom);
            } 
        }

        private void IntervalChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            interval = (int)intervalSlider.Value;
            bool resume = timer.IsEnabled;

            timer.Stop();

            timer.Interval = new TimeSpan(0, 0, 0, 0, interval);
            intervalDisplay.Content = interval.ToString() + "ms";

            if (resume)
                timer.Start();
        }

        private void StepSizeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            alpha = stepSizeSlider.Value;

            ((GradientDescent)minimalization).Alpha = alpha;
            stepSizeDisplay.Content = alpha.ToString();
        }

        private void PolytopeTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            polytope = (Polytope)polytopeTypeBox.SelectedIndex;

            switch (polytope)
            {
                case Polytope.Simplex:
                    dimensionControl.Maximum = maxSimplexDim;

                    dimX = dimX > maxSimplexDim ? maxSimplexDim : dimX;
                    dimensionControl.Value = dimX;
                    break;
                case Polytope.Hypercube:
                    dimensionControl.Maximum = maxHypercubeDim;

                    dimX = dimX > maxHypercubeDim ? maxHypercubeDim : dimX;
                    dimensionControl.Value = dimX;
                    break;
                default:
                    break;
            }

            ResetSimulation(null, null);
        }

        private void MainWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (s != null)
                DrawSolution(s, zoom);
        }

        private void DimensionValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (dimensionControl.Value.HasValue)
                dimX = dimensionControl.Value.Value;

            ResetSimulation(null, null);
        }

        private void ClearCanvas(object sender, RoutedEventArgs e)
        {
            // Stop simulation
            simulationInProgress = false;
            timer.Stop();
            startButton.Content = "Start";

            // Clear canvas
            if (v != null)
                v.Clear();

            mds = null;
            s = null;
            v = null;
            minimalization = null;

            // Block Controls
            ToggleControlsClear(true);

            ToggleControlsSimulation(false);
        }

        private void ZoomChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            zoom = zoomSlider.Value;

            DrawSolution(s, zoom);
        }

        private void ResetSimulation(object sender, RoutedEventArgs e)
        {
            // Stop simulation
            simulationInProgress = false;
            timer.Stop();
            startButton.Content = "Start";

            // Clear canvas
            if (v != null)
                v.Clear();

            mds = null;
            s = null;
            v = null;
            minimalization = null;

            // Reset simulation
            InitSimulation();

            // Unblock Controls
            ToggleControlsClear(false);

            ToggleControlsSimulation(false);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            double oldStress = s.GetValue();
            s = mds.NextIteration();
            stressDisplay.Text = s.GetValue().ToString();
            DrawSolution(s, zoom);

            iterationDisplay.Text = mds.Iteration.ToString();

            // Stop if changes are smaller than epsilon
            if (Math.Abs(oldStress - s.GetValue()) < epsilon)
                ToggleSimulation(null, null);
        }

        private void ToggleSimulation(object sender, RoutedEventArgs e)
        {
            if (simulationInProgress == false)
            {
                // Start simulation
                simulationInProgress = true;
                ToggleControlsSimulation(true);
                timer.Start();
                startButton.Content = "Stop";
            }
            else
            {
                // Stop simulation
                simulationInProgress = false;
                ToggleControlsSimulation(false);
                timer.Stop();
                startButton.Content = "Start";
            }
        }

        private void ToggleControlsClear(bool clear)
        {
            if (clear == false)
            {
                startButton.IsEnabled = true;
                clearButton.IsEnabled = true;
                zoomSlider.IsEnabled = true;
                stepSizeSlider.IsEnabled = true;
                drawPointsControl.IsEnabled = true;
            }
            else
            {
                startButton.IsEnabled = false;
                clearButton.IsEnabled = false;
                zoomSlider.IsEnabled = false;
                stepSizeSlider.IsEnabled = false;
                drawPointsControl.IsEnabled = false;

                stressDisplay.Text = "";
                iterationDisplay.Text = "";
            }
        }

        private void ToggleControlsSimulation(bool running)
        {
            if (running)
            {
                polytopeTypeBox.IsEnabled = false;
                dimensionControl.IsEnabled = false;
            }
            else
            {
                polytopeTypeBox.IsEnabled = true;
                dimensionControl.IsEnabled = true;
            }
        }

        private void InitSimulation()
        {
            v = new Visualization(dimX, canvas, polytope);
            v.DrawPoints = drawPts;

            switch (polytope)
            {
                case Polytope.Simplex:
                    d0 = Simplex.GenerateDistanceMatrix(dimX);
                    minimalization = new GradientDescent(d0, alpha, Simplex.GetVerticesCount(dimX));
                    //minimalization = new SimulatedAnnealing(1000, Simplex.GetVerticesCount(dimX), d0, dimY);
                    break;
                case Polytope.Hypercube:
                    d0 = Hypercube.GenerateDistanceMatrix(dimX);
                    minimalization = new GradientDescent(d0, alpha, Hypercube.GetVerticesCount(dimX));
                    //minimalization = new SimulatedAnnealing(1000, Hypercube.GetVerticesCount(dimX), d0, dimY);
                    break;
                default:
                    break;
            }

            mds = new MDS(MDS.EuclideanDistance, MDS.KruskalStress, minimalization);

            s = mds.InitPos();
            stressDisplay.Text = s.GetValue().ToString();
            DrawSolution(s, zoom);

            iterationDisplay.Text = "0";
        }

        private void DrawSolution(ISolution s, double zoom)
        {
            int count = 0;
            switch (polytope)
            {
                case Polytope.Simplex:
                    count = Simplex.GetVerticesCount(dimX);
                    break;
                case Polytope.Hypercube:
                    count = Hypercube.GetVerticesCount(dimX);
                    break;
                default:
                    break;
            }

            double x_avg = 0, y_avg = 0;
            foreach (var pt in s.GetArgument())
            {
                x_avg += pt[0];
                y_avg += pt[1];
            }
            x_avg /= count;
            y_avg /= count;

            for (int i = 0; i < count; i++)
            {
                v.Points[i].x = (s.GetArgument()[i][0] - x_avg) * zoom + canvas.ActualWidth / 2;
                v.Points[i].y = (s.GetArgument()[i][1] - y_avg) * zoom + canvas.ActualHeight / 2;
            }

            v.Draw();
        }
    }
}
