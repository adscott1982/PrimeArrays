using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;

namespace PrimeArrays
{
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter maximum prime number: ");
            var maxPrime = Convert.ToInt32(Console.ReadLine());
            var primeArray = CreatePrimeArray(maxPrime);
            var sw = new Stopwatch();
            sw.Start();
            RemoveNonPrimes(primeArray);
            Console.WriteLine($"Complete in {sw.ElapsedMilliseconds} ms.");

            var fileName = "primes.csv";

            Console.Write("Saving to file ...");
            SavePrimesToFile(primeArray, fileName);
            Console.WriteLine(" done.");

            Console.ReadKey();
        }

        static BitArray CreatePrimeArray(int size)
        {
            var array = new BitArray(size + 1, true);
            array[0] = false;
            array[1] = false;
            return array;
        }

        static void RemoveFactors(int number, BitArray array)
        {
            var startIndex = number + number;

            for (int i = startIndex; i < array.Length; i += number)
            {
                // If the number is large enough it may wrap around the largest int and become negative
                if (i < 1) break;
                array[i] = false;
            }
        }

        static void RemoveFactorsParallel(int number, BitArray array)
        {
            var startIndex = 2;
            var iterationCount = array.Length / number;

            Parallel.For(
                startIndex,
                iterationCount + 1,
                (i) =>
                    {
                        var nonPrime = i * number;
                        if (nonPrime > 1 && nonPrime < array.Length) array[nonPrime] = false;

                        //if (i * number > 1 && i * number < array.Length) array[i * number] = false;
                    });
        }

        static void SavePrimesToFile(BitArray primeArray, string fileName)
        {
            using (var fileStream = new StreamWriter(fileName, false))
            {
                for (int i = 2; i < primeArray.Length; i++)
                {
                    if (!primeArray[i]) continue;
                    fileStream.WriteLine(i);
                }
            }      
        }

        static void RemoveNonPrimes(BitArray array)
        {
            var sw = new Stopwatch();
            for (int i = 2; i < array.Length; i++)
            {
                if (array[i])
                {
                    //Console.WriteLine($"Prime: {i}");

                    //sw.Reset();
                    //sw.Start();
                    //Console.Write($"Removing factors for {i} ...");

                    RemoveFactors(i, array);
                    //RemoveFactorsParallel(i, array);

                    //Console.WriteLine($"done [{sw.ElapsedMilliseconds} ms]");
                }
            }
        }
    }
}