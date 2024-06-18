﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerCore.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace TaskManager.App.UWP.Converters
{
    public class TaskTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TaskType)
            {
                int taskType = (int)value;
                return (taskType == 2 || taskType == 3) ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}