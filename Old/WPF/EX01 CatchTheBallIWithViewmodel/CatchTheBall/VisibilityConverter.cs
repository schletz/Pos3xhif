using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CatchTheBall
{
    class VisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Liefert Visible, wenn der Wert gleich dem übergebenen Parameter ist.
        /// </summary>
        /// <param name="value">Finished</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Konvertierte Wert</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool finished = (bool)value;
            bool targetValue = parameter.ToString() == "true";
            return finished == targetValue ? Visibility.Visible : 
                Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
