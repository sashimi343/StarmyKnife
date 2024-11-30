using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace StarmyKnife.Converters
{
    public class ErrorsToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
            {
                return null;
            }

            var errors = values[0] as IEnumerable<string>;
            if (errors == null)
            {
                return null;
            }

            return string.Join(Environment.NewLine, errors);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
