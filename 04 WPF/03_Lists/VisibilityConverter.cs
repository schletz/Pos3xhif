using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ListDemo
{
    /// <summary>
    /// Converter für die Sichtbarkeit der Controls.
    /// </summary>
    public class VisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Wandelt den value in eine Visibility Eigenschaft um.
        /// </summary>
        /// <param name="value">Über das Binding aus XAML übergebener Wert (das aktuelle Geschlecht z. B.).</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">Über ConverterParameter aus XAML übergebener wert, wann Visible geliefert werden soll.</param>
        /// <param name="culture"></param>
        /// <returns>true, wenn value = parameter. Sonst false.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string activeMenuitem = value?.ToString() ?? "";
            string targetMenuitem = parameter?.ToString() ?? "";
            return activeMenuitem == targetMenuitem ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
