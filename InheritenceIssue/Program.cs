namespace InheritenceIssue
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var clazz = new BaseClass();
            clazz.WrapMethod();
            Console.WriteLine($"Base class result value: {clazz.Result}");
            clazz = new SubClass();
            clazz.WrapMethod();
            Console.WriteLine($"Sub class result value: {clazz.Result}");
        }
    }
}


