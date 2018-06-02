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
            constructor() {
                this.httpClient = new Web.HttpClient();
                this.host = "https://meilcli.github.io/Tbus/";
            }
            async getNextBusAsync(fileName) {
                var dayTime = await this.getTodayTableAsync(fileName);
                var result = null;
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
                return this.getTodayTimeTable(defaultTimeTable, limitedTimeTable);
            }
            getTodayTimeTable(defaultTimeTable, limitedTimeTable) {
                var date = new Date();
                for (var i = 0; i < limitedTimeTable.length; i++) {
                    var iso = date.toISOString();
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
                    var dayTable;
                    if (date.getDay() == 0) {
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
        }
        Web.TbusManager = TbusManager;
    })(Web = Tbus.Web || (Tbus.Web = {}));
})(Tbus || (Tbus = {}));
/// <reference path="src/HttpClient.ts"/>
/// <reference path="src/TbusManager.ts"/>
class Greeter {
    constructor(element) {
        this.element = element;
        this.span1 = document.createElement('p');
        this.element.appendChild(this.span1);
        this.span2 = document.createElement('p');
        this.element.appendChild(this.span2);
        this.span3 = document.createElement('p');
        this.element.appendChild(this.span3);
        this.span4 = document.createElement('p');
        this.element.appendChild(this.span4);
    }
    async s() {
        var manager = new Tbus.Web.TbusManager();
        var bus1 = await manager.getNextBusAsync("kansai_takatuki");
        this.span1.innerHTML = `関西大学発 ${bus1.hour}:${bus1.minute} 行先: ${bus1.destination}`;
        var bus2 = await manager.getNextBusAsync("kansai_tonda");
        this.span2.innerHTML = `関西大学発 ${bus2.hour}:${bus2.minute} 行先: ${bus2.destination}`;
        var bus3 = await manager.getNextBusAsync("takatuki_kansai");
        this.span3.innerHTML = `JR高槻駅北発 ${bus3.hour}:${bus3.minute} 行先: ${bus3.destination}`;
        var bus4 = await manager.getNextBusAsync("tonda_kansai");
        this.span4.innerHTML = `JR富田駅発 ${bus4.hour}:${bus4.minute} 行先: ${bus4.destination}`;
    }
    start() {
        // タイマー使うことになればこれを使おう
        this.timerToken = setInterval(() => this.span1.innerHTML = new Date().toUTCString(), 500);
    }
    stop() {
        clearTimeout(this.timerToken);
    }
}
window.onload = () => {
    var el = document.getElementById('content');
    var greeter = new Greeter(el);
    greeter.s();
    // greeter.start();
};
//# sourceMappingURL=app.js.map