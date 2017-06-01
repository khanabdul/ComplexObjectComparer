using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace equality
{
    class Program
    {
        //Test Method to compare
        public static void Main(string[] args)
        {
            //Testing Comaprer Extension  to test the values coming from different obects and different sub-objects having same fields.

            List<Product1> product = new List<Product1>();
            List<Product> product1 = new List<Product>();
            product.Add(new Product1()
            {
                Id = 1,
                Name = "2",
                LineUpList = new List<LineUp1>() { new LineUp1 { fir = 1, fir2 = "Lineupa" }, new LineUp1 { fir = 2, fir2 = "Lineupb" } },
                NameList = new List<string>() { "abdul", "swapnil" },
                ArrayOfTestID = new int[3] {4 ,5,6}

            });
            product.Add(new Product1()
            {
                Id = 2,
                Name = null,
                LineUpList = new List<LineUp1>() { new LineUp1 { fir = 1, fir2 = "Lineupa" }, new LineUp1 { fir = 2, fir2 = "Lineupb" } },
                NameList = new List<string>() { "karan", "vasu" },
                ArrayOfTestID = new int[3] {4 ,5,6}
            });

            product1.Add(new Product()
            {
                Id = 1,
                Name = "2"
                ,
                LineUpList = new List<LineUp>() { new LineUp { fir = 1, fir2 = "Lineupa" }, new LineUp { fir = 2, fir2 = "Lineupb" } }
                ,
                NameList = new List<string>() { "abdul", "swapnil" },
                ArrayOfTestID = new int[3] { 4, 5, 6 }
            });
            product1.Add(new Product()
            {
                Id = 2,
                Name = null,
                LineUpList = new List<LineUp>() { new LineUp { fir = 1, fir2 = "Lineupa" }, new LineUp { fir = 2, fir2 = "Lineupb" } },
                NameList = new List<string>() { "karan2", "vasu" },
                ArrayOfTestID = new int[3] { 4, 5, 6 }
            });

            Console.WriteLine();

            //Compare different complex IEnumerable or list of objects  having  by same properties.

            Console.WriteLine("Comparision Result : {0}", product.CompareEnumerableByValue(product1));
            Console.ReadLine();
        }

    }


    class Product1
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<LineUp1> LineUpList { get; set; }
        public List<string> NameList { get; set; }

        public int[] ArrayOfTestID { get; set; }
    }


    class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<LineUp> LineUpList { get; set; }
        public List<string> NameList { get; set; }
        public int[] ArrayOfTestID { get; set; }
    }

    class LineUp1
    {
        public int fir { get; set; }
        public string fir2 { get; set; }
    }


    class LineUp
    {
        public int fir { get; set; }
        public string fir2 { get; set; }
    }
}
