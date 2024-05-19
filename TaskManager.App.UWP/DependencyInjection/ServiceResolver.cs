using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerCore.Configuration;
using TaskManagerCore.Infrastructure.BinaryFile;
using TaskManagerCore.Model.Repository;

namespace TaskManager.App.UWP.DependencyInjection
{
    internal delegate ITaskDataRepository ServiceResolver(string key);
}
