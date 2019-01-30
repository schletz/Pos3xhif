using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CashApp
{
    /// <summary>
    /// Converter für die Sichtbarkeit von Controls je nach State. Wird direkt im ViewModel
    /// ein Property dafür gemacht, muss bei jeder Änderung des State mit PropertyChanged
    /// für jedes Visibility Flag aufgerufen werden. Hier ist der Converter die allgemeinere
    /// Lösung.
    /// </summary>
    public class VisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Prüft, ob value gleich dem übergebenen Parameter ist.
        /// </summary>
        /// <param name="value">Der aktuelle State, wird über das Binding mittels Binding State
        /// in XAML eingelesen.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">Der State, in dem das Control sichtbar ist.</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == parameter.ToString() ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
