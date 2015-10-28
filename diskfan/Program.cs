using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Delver
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            if (args.Count() < 1)
            {
                Console.WriteLine("Insufficient arguments: diskdig PATH");
                Environment.Exit(1);
            }

            string path = new DirectoryInfo(args[0] + Path.DirectorySeparatorChar + ".").FullName;

            Console.WriteLine("Digging in {0}..", path);

            var dwork = new dObj(path);

            var stack = new Stack<dObj>();
            stack.Push(dwork);

            while (true)
            {
                var cur = stack.Peek();
                var total = cur.children.Select(x => x.size).Sum();

                Console.WriteLine("\nContents of {0}:", cur.path.FullName);

                var sortedList = cur.children.OrderByDescending(x => x.size).ToList();

                int i = 1;
                foreach (var o in sortedList)
                    if (o.children.Count > 0)
                    {
                        Console.WriteLine("{0,3}. {1,-50} {3,10:#,#0}MB ({2,5:0.0}%)", i, o.name, 100 * o.size / (double)total, o.size / 1024 / 1024);
                        i++;
                    }
                foreach (var o in sortedList)
                    if (o.children.Count == 0)
                    {
                        Console.WriteLine("     {1,-50} {3,10:#,#0}MB ({2,5:0.0}%)", i, o.name, 100 * o.size / (double)total, o.size / 1024 / 1024);
                        i++;
                    }

                string str = Console.ReadLine().Trim().ToLower();
                if (str == "..") {
                    if (stack.Count > 1)
                        stack.Pop();
                } else {
                    int N = 0;
                    if (int.TryParse(str, out N))
                    {
                        if (N > 0 && N <= cur.children.Count)
                        stack.Push(sortedList[N-1]);
                    }
                }
            }
        }
    }

    class dObj
    {
        public string name;
        public long size;
        public DirectoryInfo path;
        public List<dObj> children = new List<dObj>();
        public void Add(dObj ob) {
            children.Add(ob);
        }

        public IEnumerator<dObj> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        public dObj()
        { }

        public dObj(string path) : this(new DirectoryInfo(path))
        { }

        public dObj(DirectoryInfo d, int n = 0)
        {
            name = d.Name;
            size = 0;
            path = d;

            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                Add(new dObj { name = fi.Name, size = fi.Length });
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                try
                {
                    var child = new dObj(di, n + 1);

                    Add(child);
                    size += child.size;
                }
                catch
                {
                    Console.WriteLine(" > Error: Unable to access {0}", di.FullName);
                }
            }
        }
    }

}
