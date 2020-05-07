using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace FileWriterApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void WriteFileButton_Click(object sender, RoutedEventArgs e)
        {
            WriteFileButton.IsEnabled = false;
            // await schreibt das Ergebnis des Tasks in das Array content
            byte[] content = await GenerateData(int.Parse(Anzahl.Text) * 1000000);
            string filename = Filename.Text;

            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                // Die vorgegebene Async Methode wird mit await synchronisiert, es wird also
                // nicht blockierend gewartet, bis sie fertig ist.
                await fs.WriteAsync(content, 0, content.Length);
            }
            WriteFileButton.IsEnabled = true;
        }

        /// <summary>
        /// Erstellt einen Task, der ein Bytearray mit der übergebenen Anzahl erstellt.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private Task<byte[]> GenerateData(int count)
        {
            // Task.Run führt Code in einem neuen Thread aus. Wir geben nicht das Ergebnis zurück,
            // sondern lediglich eine Referenz auf diesen laufenden Task.
            return Task.Run(() =>
            {
                string data = new string('x', count);
                byte[] content = Encoding.UTF8.GetBytes(data);
                return content;
            });
        }
    }
}
