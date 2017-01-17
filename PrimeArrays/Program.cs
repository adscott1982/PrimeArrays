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

            var sw = new Stopwatch();
            var primeArray = CreatePrimeArray(maxPrime);
            sw.Start();
            RemoveNonPrimes(primeArray);
            Console.WriteLine($"Prime array complete in {sw.ElapsedMilliseconds} ms.");

            sw.Reset();
            sw.Start();
            CalculatePrimesAlternative(maxPrime);
            Console.WriteLine($"Alternative method complete in {sw.ElapsedMilliseconds} ms.");

            
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
                    RemoveFactors(i, array);
                }
            }
        }

        static List<int> CalculatePrimesAlternative(int number)
        {
            int Until = number;
            BitArray PrimeBits = new BitArray(Until, true);

            /*
             * Sieve of Eratosthenes
             * PrimeBits is a simple BitArray where all bit is an integer
             * and we mark composite numbers as false
             */

            PrimeBits.Set(0, false); // You don't actually need this, just
            PrimeBits.Set(1, false); // remindig you that 2 is the smallest prime

            for (int P = 2; P < (int)Math.Sqrt(Until) + 1; P++)
                if (PrimeBits.Get(P))
                    // These are going to be the multiples of P if it is a prime
                    for (int PMultiply = P * 2; PMultiply < Until; PMultiply += P)
                        PrimeBits.Set(PMultiply, false);

            // We use this to store the actual prime numbers
            var Primes = new List<int>();

            for (int i = 2; i < Until; i++)
                if (PrimeBits.Get(i))
                    Primes.Add(i);

            return Primes;
        }
    }
}