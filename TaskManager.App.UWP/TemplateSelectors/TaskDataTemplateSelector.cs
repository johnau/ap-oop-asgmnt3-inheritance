using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.App.UWP.Core.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace TaskManager.App.UWP.TemplateSelectors
{
    public class TaskDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TaskFolderTemplate { get; set; }

        public DataTemplate TaskTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return GetTemplate(item) ?? base.SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return GetTemplate(item) ?? base.SelectTemplateCore(item, container);
        }

        private DataTemplate GetTemplate(object item)
        {
            switch (item)
            {
                case TaskFolderViewObject folder:
                    return TaskFolderTemplate;
                case TaskViewObject task:
                    return TaskTemplate;
            }

            return null;
        }
    }
}
