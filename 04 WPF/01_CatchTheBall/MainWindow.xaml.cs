using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CatchTheBall
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Random rnd = new Random();
        /// <summary>
        /// Gibt an, ob das Spiel beendet wurde.
        /// </summary>
        private bool finished => clickCount >= 10;
        private int clickCount = 0;
        private int score = 0;

        /// <summary>
        /// Synchronisiert das Label ClickLabel automatisch mit dem gespeicherten ClickCounter.
        /// </summary>
        public int ClickCount
        {
            get => clickCount;
            set { clickCount = value; ClickLabel.Content = clickCount; }
        }

        /// <summary>
        /// Synchronisiert das Label ScoreLabel automatisch mit dem gespeicherten Score.
        /// </summary>
        public int Score
        {
            get => score;
            set { score = value; ScoreLabel.Content = score; }
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Hebt die Ellipse grün hervor, wenn man mit der Maus darüberfährt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ellipse_MouseEnter(object sender, MouseEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            ellipse.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
        }

        /// <summary>
        /// Löscht die Ellipse, auf die geklickt wurde und zeichnet eine neue an einer zufälligen
        /// Position innerhalb des Canvas.
        /// Man könnte sie auch verschieben, aber dies ist eine Demo, wie man Controls im Code
        /// erzeugen kann.
        /// </summary>
        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (finished) { return; }
            EllipseContainer.Children.Remove(e.Source as Ellipse);
            Score++;
            Ellipse ellipse = new Ellipse
            {
                Width = SizeSlider.Value,
                Height = SizeSlider.Value,
                Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                StrokeThickness = 4,
            };
            ellipse.SetBinding(Ellipse.WidthProperty, new Binding { Source = SizeSlider, Path = new PropertyPath(Slider.ValueProperty) });
            ellipse.SetBinding(Ellipse.HeightProperty, new Binding { Source = SizeSlider, Path = new PropertyPath(Slider.ValueProperty) });
            ellipse.MouseDown += Ellipse_MouseDown;
            ellipse.MouseEnter += Ellipse_MouseEnter;
            ellipse.MouseLeave += Ellipse_MouseLeave;
            // Achtung: Width und Height des Canvas sind nicht definiert, da wir sie nicht
            // in XAML explizit gesetzt haben. ActualHeight liefert die aktuelle Höhe.
            Canvas.SetTop(ellipse, rnd.Next((int)(ellipse.ActualHeight / 2.0), (int)(EllipseContainer.ActualHeight - ellipse.Height / 2.0)));
            Canvas.SetLeft(ellipse, rnd.Next((int)(ellipse.ActualWidth / 2.0), (int)(EllipseContainer.ActualWidth - ellipse.Width / 2.0)));
            EllipseContainer.Children.Add(ellipse);

        }
        /// <summary>
        /// Färbt die Ellipse weiß, wenn man mit der Maus rausfährt.
        /// </summary>
        private void Ellipse_MouseLeave(object sender, MouseEventArgs e)
        {
            Ellipse ellipse = e.Source as Ellipse;
            ellipse.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }

        /// <summary>
        /// Danebengegangene Klicks sowie die Klicks auf die Ellipse zum Erhöhen des Click Counters
        /// empfangen.
        /// </summary>
        private void EllipseContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (finished) { return; }
            ClickCount++;
            if (!(e.Source is Ellipse))
                Score--;
        }
    }
}
