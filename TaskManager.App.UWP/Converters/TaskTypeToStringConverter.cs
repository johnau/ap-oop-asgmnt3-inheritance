using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using TaskManagerCore.Model;

namespace TaskManager.App.UWP.Converters
{
    public class TaskTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TaskType type)
            {
                switch (type)
                {
                    case TaskType.SINGLE:
                        return "Once-off Task";
                    case TaskType.REPEATING:
                        return "Recurring Task";
                    case TaskType.REPEATING_STREAK:
                        return "Tracked Task";
                    default:
                        break;
                }
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
