var Tbus;
(function (Tbus) {
    var Web;
    (function (Web) {
        class HttpClient {
            async getAsync(request) {
                return new Promise(resolve => {
                    var http = new XMLHttpRequest();
                    http.open("GET", request.url);
                    http.onload = e => {
                        var response = new HttpResponse(http.responseText, http.status);
                        resolve(response);
                    };
                    http.send();
                });
            }
        }
        Web.HttpClient = HttpClient;
        class HttpRequest {
            constructor(url) {
                this.url = url;
            }
        }
        Web.HttpRequest = HttpRequest;
        class HttpResponse {
            constructor(content, status) {
                this.content = content;
                this.status = status;
            }
        }
        Web.HttpResponse = HttpResponse;
    })(Web = Tbus.Web || (Tbus.Web = {}));
})(Tbus || (Tbus = {}));
/// <reference path="./HttpClient.ts" />
var Tbus;
(function (Tbus) {
    var Web;
    (function (Web) {
        class TbusManager {
            constructor(fileName) {
                this.httpClient = new Web.HttpClient();
                this.host = "https://meilcli.github.io/Tbus/timetable/";
                this.holidayHost = "https://meilcli.github.io/Tbus/holiday/";
                this.dayTable = null;
                this.isHoliday = null;
                this.fileName = fileName;
            }
            async getNextBusAsync() {
                if (this.dayTable == null) {
                    this.dayTable = await this.getTodayTableAsync(this.fileName);
                }
                var result = [];
                var resultIndex = 0;
                var date = new Date();
                var time = date.getHours() * 100 + date.getMinutes();
                for (var i = 0; i < this.dayTable.buses.length; i++) {
                    var bus = this.dayTable.buses[i];
                    if (bus.hour * 100 + bus.minute <= time) {
                        continue;
                    }
                    result[resultIndex] = bus;
                    resultIndex++;
                    if (result.length == 3) {
                        break;
                    }
                }
                return result;
            }
            async getTodayTableAsync(fileName) {
                var defaultTimeTable = await this.getTimeTableAsync(`${this.host}${fileName}.json`);
                var limitedTimeTable = [];
                for (var i = 0; i < 10; i++) {
                    // 10ぐらいで妥当なはず
                    var timeTable = await this.getTimeTableAsync(`${this.host}${fileName}.limited${i + 1}.json`);
                    if (timeTable == null) {
                        break;
                    }
                    limitedTimeTable[i] = timeTable;
                }
                return await this.getTodayTimeTableAsync(defaultTimeTable, limitedTimeTable);
            }
            async getTodayTimeTableAsync(defaultTimeTable, limitedTimeTable) {
                var date = new Date();
                for (var i = 0; i < limitedTimeTable.length; i++) {
                    var iso = date.toISOString();
                    var dateString = `${iso.substring(0, iso.indexOf("T"))}T00:00:00+09:00`;
                    var dayTime = limitedTimeTable[i].special_days[dateString];
                    if (dayTime != null) {
                        return dayTime;
                    }
                }
                if (this.isHoliday == null) {
                    this.isHoliday = await this.isHolidayAsync();
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
                    var dayTable;
                    if (date.getDay() == 0 || this.isHoliday) {
                        // 日曜日
                        dayTable = limitedTimeTable[i].sunday_table;
                    }
                    else if (date.getDay() == 6) {
                        // 土曜日
                        dayTable = limitedTimeTable[i].saturday_table;
                    }
                    else {
                        dayTable = limitedTimeTable[i].weekday_table;
                    }
                    if (dayTable != null) {
                        return dayTable;
                    }
                }
                if (date.getDay() == 0 || this.isHoliday) {
                    // 日曜日
                    return defaultTimeTable.sunday_table;
                }
                if (date.getDay() == 6) {
                    // 土曜日
                    return defaultTimeTable.saturday_table;
                }
                return defaultTimeTable.weekday_table;
            }
            async getTimeTableAsync(url) {
                var httpRequest = new Web.HttpRequest(url);
                var response = await this.httpClient.getAsync(httpRequest);
                if (response.status != 200) {
                    return null;
                }
                var timeTable = JSON.parse(response.content, (key, value) => {
                    if (typeof (value) == "string" && value.match(/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\+\d{2}:\d{2}$/)) {
                        return new Date(Date.parse(value));
                    }
                    return value;
                });
                return timeTable;
            }
            getDayTime(date) {
                return date.getFullYear() * 10000 + date.getMonth() * 100 + date.getDate();
            }
            async isHolidayAsync() {
                var date = new Date();
                var year = date.getFullYear();
                var month = date.getMonth();
                var day = date.getDate();
                var httpRequest = new Web.HttpRequest(`${this.holidayHost}${year}.json`);
                var response = await this.httpClient.getAsync(httpRequest);
                if (response.status != 200) {
                    return false;
                }
                var holidays = JSON.parse(response.content, (key, value) => {
                    if (typeof (value) == "string" && value.match(/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\+\d{2}:\d{2}$/)) {
                        return new Date(Date.parse(value));
                    }
                    return value;
                });
                for (var i = 0; i < holidays.length; i++) {
                    if (holidays[i].getMonth() == month && holidays[i].getDate() == day) {
                        return true;
                    }
                }
                return false;
            }
        }
        Web.TbusManager = TbusManager;
    })(Web = Tbus.Web || (Tbus.Web = {}));
})(Tbus || (Tbus = {}));
/// <reference path="src/HttpClient.ts"/>
/// <reference path="src/TbusManager.ts"/>
class BusTimer {
    constructor(element, fileName) {
        this.sleep = ms => new Promise(resolve => setTimeout(resolve, ms));
        this.isRunning = false;
        this.second = 1000;
        this.minitu = this.second * 60;
        this.hour = this.minitu * 60;
        this.element = element;
        this.busManager = new Tbus.Web.TbusManager(fileName);
    }
    async start() {
        this.isRunning = true;
        while (this.isRunning) {
            var buses = await this.busManager.getNextBusAsync();
            var now = new Date();
            var html = "";
            for (var i = 0; i < buses.length; i++) {
                var bus = buses[i];
                var busTime = new Date(now.getFullYear(), now.getMonth(), now.getDate(), bus.hour, bus.minute);
                var d = busTime.getTime() - now.getTime();
                var dh = Math.floor(d / this.hour);
                var dm = Math.floor((d - dh * this.hour) / this.minitu);
                var ds = Math.floor((d - dh * this.hour - dm * this.minitu) / this.second);
                html += "<p>";
                html += `<span>${bus.hour}時${bus.minute}発</span>`;
                if (0 < dh) {
                    html += `<span class="badge badge-primary">あと${dh}時間${dm}分${ds}秒</span>`;
                }
                else if (0 < dm) {
                    html += `<span class="badge badge-primary">あと${dm}分${ds}秒</span>`;
                }
                else {
                    html += `<span class="badge badge-primary">あと${ds}秒</span>`;
                }
                html += "<br />";
                html += `<span class="ml-3">${bus.destination}</span>`;
                html += "</p>";
            }
            if (html.length == 0) {
                html = "バスはもうありません。";
            }
            this.element.innerHTML = html;
            await this.sleep(500);
        }
    }
    stop() {
        this.isRunning = false;
    }
}
window.onload = () => {
    {
        var el = document.getElementById('nav-kansai-takatuki');
        var busTimer = new BusTimer(el, "kansai_takatuki");
        busTimer.start();
    }
    {
        var el = document.getElementById('nav-kansai-tonda');
        var busTimer = new BusTimer(el, "kansai_tonda");
        busTimer.start();
    }
    {
        var el = document.getElementById('nav-takatuki-kansai');
        var busTimer = new BusTimer(el, "takatuki_kansai");
        busTimer.start();
    }
    {
        var el = document.getElementById('nav-tonda-kansai');
        var busTimer = new BusTimer(el, "tonda_kansai");
        busTimer.start();
    }
};
//# sourceMappingURL=app.js.map