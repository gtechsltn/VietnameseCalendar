using System;
using System.Globalization;

namespace VietnameseCalendarDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var dt = new DateTime(2024, 09, 24, 0, 0, 0);
            var vietnameseCalendar = new VietnameseCalendar();

            vietnameseCalendar.FromDateTime(dt, out int y, out int m, out int d);
            Console.WriteLine($"Year: {y}, Month: {m}, Day: {d}");

            dt = new DateTime(1983, 06, 25, 0, 0, 0);
            vietnameseCalendar.FromDateTime(dt, out y, out m, out d);
            Console.WriteLine($"Year: {y}, Month: {m}, Day: {d}");

            Console.WriteLine("DONE. Press any key to exit...");
            Console.ReadKey();
        }
    }
}