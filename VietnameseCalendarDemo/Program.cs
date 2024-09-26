using System;
using System.Globalization;

namespace VietnameseCalendarDemo
{
    internal class Program
    {
        private static int y;
        private static int m;
        private static int d;
        private static DateTime dt;

        private static void Main(string[] args)
        {
            dt = new DateTime(2024, 09, 24, 0, 0, 0);
            VietnameseCalendar.FromDateTime(dt, out y, out m, out d);
            Console.WriteLine($"Year: {y}, Month: {m}, Day: {d}");

            dt = new DateTime(1983, 06, 25, 0, 0, 0);
            VietnameseCalendar.FromDateTime(dt, out y, out m, out d);
            Console.WriteLine($"Year: {y}, Month: {m}, Day: {d}");

            Console.WriteLine("DONE. Press any key to exit...");
            Console.ReadKey();
        }
    }
}