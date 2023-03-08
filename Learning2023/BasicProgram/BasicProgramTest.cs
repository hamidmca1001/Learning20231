using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning2023.BasicProgram
{
    public class BasicProgramTest
    {
        public static void Test()
        {

        }
    }


    public class FibonacciTest
    {
        int i = 0; int j = 1; int n = 10;
        public void Test1()
        {
            Console.Write($"{ i}  { j}");
            for (int k = 2; k < n; k++)
            {
                int temp = i+j;
                Console.Write(temp);
                i = j;
                j= temp;
            }
        }
        // 0 1,1, 2 3,5, 8

    }
}
