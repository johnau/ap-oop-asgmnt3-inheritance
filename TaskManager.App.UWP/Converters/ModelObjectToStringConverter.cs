using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.App.UWP.Core.Models;
using Windows.UI.Xaml.Data;

namespace TaskManager.App.UWP.Converters
{
    internal class ModelObjectToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TaskViewObject taskVO)
            {
                return taskVO.GlobalId;
            }
            else if (value is TaskFolderViewObject folderVO)
            {
                return folderVO.Name;
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
