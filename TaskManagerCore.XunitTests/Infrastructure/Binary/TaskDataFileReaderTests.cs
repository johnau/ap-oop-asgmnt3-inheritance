using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerCore.Infrastructure.BinaryFile;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.XunitTests.Infrastructure.Binary
{
    public class TaskDataFileReaderTests
    {
        /// <summary>
        ///  This method needs to be replaced since it curently relies on the result from the other test
        /// </summary>
        [Fact]
        public void ReadDataFile_TempTestMethod()
        {
            //var filePath = "C:\\Users\\John\\AppData\\Local\\Temp\\testing-task-data.bin";

            var reader = new TaskDataFileReader("testing-task-data");
            var content = reader.ReadDataFile(); // will throw exception with the current way tests are setup
            // TODO, create an example file that can be stored in the assembly to run this test
            // Also need to consolidate tehe checks for the type

            Assert.Equal(2, content.Count);

            foreach (var item in content)
            {
                Debug.WriteLine($"ID={item.Id}, Desc={item.Description}, Notes={item.Notes}, Completed={item.Completed}, DueDate={item.DueDate.ToString()}");
            }
        }

        /// <summary>
        ///  This method needs to be replaced since it curently relies on the result from the other test
        /// </summary>
        [Fact]
        public void ReadDataFile_TempTestMethod2()
        {
            //var filePath = "C:\\Users\\John\\AppData\\Local\\Temp\\testing-task-data.bin";

            var reader = new TaskDataFileReader("testing-task-data2");
            var content = reader.ReadDataFile(); // will throw exception with the current way tests are setup
            // TODO, create an example file that can be stored in the assembly to run this test
            // Also need to consolidate tehe checks for the type

            Assert.Equal(4, content.Count);

            foreach (var item in content)
            {
                if (item is HabitualTaskDataEntity habitual)
                {
                    Debug.WriteLine($"ID={habitual.Id}, Desc={habitual.Description}, Notes={habitual.Notes}, Completed={habitual.Completed}, DueDate={habitual.DueDate.ToString()}, Interval={habitual.RepeatingInterval.ToString()}, Repititions={habitual.Repititions}, Streak={habitual.Streak}");
                }
                else if (item is RepeatingTaskDataEntity repeating)
                {
                    Debug.WriteLine($"ID={repeating.Id}, Desc={repeating.Description}, Notes={repeating.Notes}, Completed={repeating.Completed}, DueDate={repeating.DueDate.ToString()}, Interval={repeating.RepeatingInterval.ToString()}, Repititions={repeating.Repititions}");
                } else
                {
                    Debug.WriteLine($"ID={item.Id}, Desc={item.Description}, Notes={item.Notes}, Completed={item.Completed}, DueDate={item.DueDate.ToString()}");
                }
            }
        }
    }
}
