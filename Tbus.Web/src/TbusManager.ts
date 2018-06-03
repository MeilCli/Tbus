/// <reference path="./HttpClient.ts" />

namespace Tbus.Web {

    interface Bus {
        hour: number
        minute: number
        destination: string
    }

    interface DayTable {
        buses: Bus[]
    }

    interface IndexedHour {
        index: number
        hour: number
    }

    interface HourTable {
        indexed_hours: IndexedHour[]
    }

    interface LimitedTimeOption {
        start_day: Date
        end_day: Date
    }

    interface TimeTable {
        limited_time_option: LimitedTimeOption
        weekday_table: DayTable
        saturday_table: DayTable
        sunday_table: DayTable
        special_days: { [key: string]: DayTable; }
    }

    export class TbusManager {
        private httpClient = new HttpClient();
        private host = "https://meilcli.github.io/Tbus/timetable/";

        async getNextBusAsync(fileName: string): Promise<Bus> {
            var dayTime = await this.getTodayTableAsync(fileName);

            var result: Bus = null;
            var date = new Date();
            var time = date.getHours() * 100 + date.getMinutes();
            for (var i = 0; i < dayTime.buses.length; i++) {
                var bus = dayTime.buses[i];
                if (bus.hour * 100 + bus.minute < time) {
                    continue;
                }
                result = bus;
                break;
            }

            return result;
        }

        async getTodayTableAsync(fileName: string): Promise<DayTable> {
            var defaultTimeTable = await this.getTimeTableAsync(`${this.host}${fileName}.json`);
            var limitedTimeTable: TimeTable[] = [];
            for (var i = 0; i < 10; i++) {
                // 10ぐらいで妥当なはず
                var timeTable = await this.getTimeTableAsync(`${this.host}${fileName}.limited${i + 1}.json`);
                if (timeTable == null) {
                    break;
                }
                limitedTimeTable[i] = timeTable;
            }

            return this.getTodayTimeTable(defaultTimeTable, limitedTimeTable);
        }

        private getTodayTimeTable(defaultTimeTable: TimeTable, limitedTimeTable: TimeTable[]): DayTable {
            var date = new Date();
            for (var i = 0; i < limitedTimeTable.length; i++) {
                var iso = date.toISOString()
                var dateString = `${iso.substring(0, iso.indexOf("T"))}T00:00:00+09:00`;
                var dayTime = limitedTimeTable[i].special_days[dateString];
                if (dayTime != null) {
                    return dayTime;
                }
            }

            for (var i = 0; i < limitedTimeTable.length; i++) {
                if (this.getDayTime(date) < this.getDayTime(limitedTimeTable[i].limited_time_option.start_day)) {
                    // 期間前
                    continue;
                }
                if (this.getDayTime(limitedTimeTable[i].limited_time_option.end_day) < this.getDayTime(date)) {
                    // 期間後
                    continue;
                }

                var dayTable: DayTable
                if (date.getDay() == 0) {
                    // 日曜日
                    dayTable = limitedTimeTable[i].sunday_table;
                } else if (date.getDay() == 6) {
                    // 土曜日
                    dayTable = limitedTimeTable[i].saturday_table;
                } else {
                    dayTable = limitedTimeTable[i].weekday_table;
                }
                if (dayTable != null) {
                    return dayTable;
                }
            }

            if (date.getDay() == 0) {
                // 日曜日
                return defaultTimeTable.sunday_table;
            }
            if (date.getDay() == 6) {
                // 土曜日
                return defaultTimeTable.saturday_table;
            }
            return defaultTimeTable.weekday_table;
        }

        private async getTimeTableAsync(url: string): Promise<TimeTable> {
            var httpRequest = new HttpRequest(url);
            var response = await this.httpClient.getAsync(httpRequest);
            if (response.status != 200) {
                return null;
            }
            var timeTable: TimeTable = JSON.parse(response.content, (key, value) => {
                if (typeof (value) == "string" && value.match(/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\+\d{2}:\d{2}$/)) {
                    return new Date(Date.parse(value as string));
                }
                return value;
            });
            return timeTable;
        }

        private getDayTime(date: Date): number {
            return date.getFullYear() * 10000 + date.getMonth() * 100 + date.getDate();
        }
    }
}