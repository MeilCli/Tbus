using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using Tbus.Calendar.NETStandard;
using static System.Console;

namespace Tbus.Calendar.NETCore.Console
{
    class Program
    {
        public static void Main(string[] args)
        {
            string applicationDirectory = AppDomain.CurrentDomain.BaseDirectory;
            int index = applicationDirectory.IndexOf("\\Tbus.Calendar.NETCore.Console");
            string solutionPath = applicationDirectory.Substring(0, index);
            WriteLine($"solution path: {solutionPath}");

            var calendar = new JapaneseCalendar();

            {
                var outputDirectory = Directory.CreateDirectory($"{solutionPath}/docs/holiday");
                foreach (var deleteFile in outputDirectory.EnumerateFiles())
                {
                    deleteFile.Delete();
                }
                WriteLine($"output path: {outputDirectory.FullName}");

                foreach (var year in Enumerable.Range(2018, 3))
                {
                    var holidays = calendar.GetHolidaysOfYear(year);
                    string json = JsonConvert.SerializeObject(holidays, Formatting.Indented);
                    string path = $"{outputDirectory.FullName}/{year}.json";
                    File.WriteAllText(path, json);
                    WriteLine($"output: {path}");
                }
            }

            {
                var outputDirectory = Directory.CreateDirectory($"{solutionPath}/docs/calendar");
                foreach (var deleteFile in outputDirectory.EnumerateFiles())
                {
                    deleteFile.Delete();
                }
                WriteLine($"output path: {outputDirectory.FullName}");

                foreach (var year in Enumerable.Range(2018, 3))
                {
                    var dayTypes = calendar.GetDayTypesOfYear(year).Select(x => new { x.date, day_type = x.dayType });
                    string json = JsonConvert.SerializeObject(dayTypes, Formatting.Indented);
                    string path = $"{outputDirectory.FullName}/{year}.json";
                    File.WriteAllText(path, json);
                    WriteLine($"output: {path}");
                }
            }
        }
    }
}
