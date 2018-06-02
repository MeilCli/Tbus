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
        special_days: { key: Date; value: DayTable }
    }

    export class TbusManager {
        private httpClient = new HttpClient();

        async get3Bus(): Promise<string> {
            var httpRequest = new HttpRequest("kansai_takatuki.limited1.json");
            var response = await this.httpClient.getAsync(httpRequest);
            if (response.status != 200) {
                return "error";
            }
            var timeTable: TimeTable = JSON.parse(response.content);
            return timeTable.limited_time_option.start_day.getDay().toString();
        }
    }
}