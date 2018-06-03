using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Tbus.Parser.NETStandard;
using static System.Console;

namespace Tbus.Parser.NETCore.Console
{
    class Program
    {
        private static List<(string id, List<TimeTableData> timeTableData, string fileNameWithoutExtension)> createTimeTableDataList()
        {
            var result = new List<(string, List<TimeTableData>, string)>();
            {
                // 関西大学 高槻方面行き
                var list = new List<TimeTableData>();
                list.Add(new TimeTableData
                {
                    Url = "https://transfer.navitime.biz/takatsuki/pc/diagram/BusDiagram?orvCode=00150093&course=0003700096&stopNo=1"
                });
                list.Add(new TimeTableData
                {
                    Url = "https://transfer.navitime.biz/takatsuki/pc/diagram/BusDiagram?orvCode=00150093&course=0003700096&stopNo=1&date=2018-08-01",
                    LimitedTimeOption = new LimitedTimeOption
                    {
                        StartDay = dateFrom("2018/8/1"),
                        EndDay = dateFrom("2018/9/20")
                    }
                });
                list.Add(new TimeTableData
                {
                    Url = "https://transfer.navitime.biz/takatsuki/pc/diagram/TemporaryBusDiagram?orvCode=00150093&course=0003700096&stopNo=1&date=2018-08-13",
                    LimitedTimeOption = new LimitedTimeOption
                    {
                        StartDay = dateFrom("2018/8/13"),
                        EndDay = dateFrom("2018/8/15")
                    }
                });
                result.Add(("関西大学 高槻方面行き", list, "kansai_takatuki"));
            }
            {
                // 関西大学 富田方面行き
                var list = new List<TimeTableData>();
                list.Add(new TimeTableData
                {
                    Url = "https://transfer.navitime.biz/takatsuki/pc/diagram/BusDiagram?orvCode=00150093&course=0003700142&stopNo=8&date=2018-06-02"
                });
                list.Add(new TimeTableData
                {
                    Url = "https://transfer.navitime.biz/takatsuki/pc/diagram/BusDiagram?orvCode=00150093&course=0003700142&stopNo=8&date=2018-08-01",
                    LimitedTimeOption = new LimitedTimeOption
                    {
                        StartDay = dateFrom("2018/8/1"),
                        EndDay = dateFrom("2018/9/20")
                    }
                });
                list.Add(new TimeTableData
                {
                    Url = "https://transfer.navitime.biz/takatsuki/pc/diagram/TemporaryBusDiagram?orvCode=00150093&course=0003700142&stopNo=8&date=2018-08-13",
                    LimitedTimeOption = new LimitedTimeOption
                    {
                        StartDay = dateFrom("2018/8/13"),
                        EndDay = dateFrom("2018/8/15")
                    }
                });
                result.Add(("関西大学 富田方面行き", list, "kansai_tonda"));
            }
            {
                // JR高槻駅北 関西大学方面行き
                var list = new List<TimeTableData>();
                list.Add(new TimeTableData
                {
                    Url = "https://transfer.navitime.biz/takatsuki/pc/diagram/BusDiagram?orvCode=00150029&course=0003700044&stopNo=1"
                });
                list.Add(new TimeTableData
                {
                    Url = "https://transfer.navitime.biz/takatsuki/pc/diagram/BusDiagram?orvCode=00150029&course=0003700044&stopNo=1&date=2018-08-01",
                    LimitedTimeOption = new LimitedTimeOption
                    {
                        StartDay = dateFrom("2018/8/1"),
                        EndDay = dateFrom("2018/9/20")
                    }
                });
                list.Add(new TimeTableData
                {
                    Url = "https://transfer.navitime.biz/takatsuki/pc/diagram/TemporaryBusDiagram?orvCode=00150029&course=0003700044&stopNo=1&date=2018-08-13",
                    LimitedTimeOption = new LimitedTimeOption
                    {
                        StartDay = dateFrom("2018/8/13"),
                        EndDay = dateFrom("2018/8/15")
                    }
                });
                result.Add(("JR高槻駅北 関西大学方面行き", list, "takatuki_kansai"));
            }
            {
                // JR富田駅 関西大学方面行き
                var list = new List<TimeTableData>();
                list.Add(new TimeTableData
                {
                    Url = "https://transfer.navitime.biz/takatsuki/pc/diagram/BusDiagram?orvCode=00150241&course=0003700072&stopNo=1"
                });
                list.Add(new TimeTableData
                {
                    Url = "https://transfer.navitime.biz/takatsuki/pc/diagram/BusDiagram?orvCode=00150241&course=0003700072&stopNo=1&date=2018-08-01",
                    LimitedTimeOption = new LimitedTimeOption
                    {
                        StartDay = dateFrom("2018/8/1"),
                        EndDay = dateFrom("2018/9/20")
                    }
                });
                list.Add(new TimeTableData
                {
                    Url = "https://transfer.navitime.biz/takatsuki/pc/diagram/TemporaryBusDiagram?orvCode=00150241&course=0003700072&stopNo=1&date=2018-08-13",
                    LimitedTimeOption = new LimitedTimeOption
                    {
                        StartDay = dateFrom("2018/8/13"),
                        EndDay = dateFrom("2018/8/15")
                    }
                });
                result.Add(("JR富田駅 関西大学方面行き", list, "tonda_kansai"));
            }
            return result;
        }

        static async Task Main(string[] args)
        {
            string applicationDirectory = AppDomain.CurrentDomain.BaseDirectory;
            int index = applicationDirectory.IndexOf("\\Tbus.Parser.NETCore.Console");
            string solutionPath = applicationDirectory.Substring(0, index);
            WriteLine($"solution path: {solutionPath}");
            var outputDirectory = Directory.CreateDirectory($"{solutionPath}/docs/timetable");
            foreach (var deleteFile in outputDirectory.EnumerateFiles())
            {
                deleteFile.Delete();
            }
            WriteLine($"output path: {outputDirectory.FullName}");

            var timeTableParser = new TimeTableParser();
            foreach (var data in createTimeTableDataList())
            {
                int number = 1;
                foreach (var t in data.timeTableData)
                {
                    TimeTable timeTable = await timeTableParser.ParseUrlAsync(t.Url, data.id, t.LimitedTimeOption);
                    if (t.LimitedTimeOption == null)
                    {
                        // default
                        output(timeTable, outputDirectory, $"{data.fileNameWithoutExtension}.json");
                    }
                    else
                    {
                        output(timeTable, outputDirectory, $"{data.fileNameWithoutExtension}.limited{number}.json");
                        number++;
                    }
                }
            }

            WriteLine("finish");
        }

        private static DateTime dateFrom(string date)
        {
            return DateTime.Parse(date, CultureInfo.CreateSpecificCulture("ja-JP"), DateTimeStyles.AssumeLocal);
        }

        private static void output(TimeTable timeTable, DirectoryInfo directoryInfo, string fileName)
        {
            string path = $"{directoryInfo.FullName}/{fileName}";
            File.WriteAllText(path, JsonConvert.SerializeObject(timeTable, Formatting.Indented));
            WriteLine($"output: {path}");
        }
    }

    class TimeTableData
    {
        public string Url { get; set; }

        public LimitedTimeOption LimitedTimeOption { get; set; }
    }
}
