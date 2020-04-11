using NipahTokenizer;
using System;

namespace Nipah_Tokenizer_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Tokenizer tokenizer = new Tokenizer();
            while(true)
            {
                Console.WriteLine("Write some text here...");
                string input = Console.ReadLine();
                Console.WriteLine("///////////////////////////");
                var output = tokenizer.Tokenize(input);
                foreach (var element in output)
                    Console.Write($"[{element.text}] ");
                Console.WriteLine("...Press any key to continue");
                Console.ReadKey(true);
                Console.Clear();
            }
        }
    }
}
