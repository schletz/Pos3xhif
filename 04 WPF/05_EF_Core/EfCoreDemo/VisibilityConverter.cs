using ListDemo.Model;
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
    /// Zeigt das Gendericon an, wenn der Gendername des übergebenen Gender Objekt gleich dem
    /// Parameter ist.
    /// </summary>
    public class GenderIconVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Gender? actualValue = value as Gender;
            string visibleValue = parameter?.ToString() ?? "";
            return actualValue?.Name == visibleValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
