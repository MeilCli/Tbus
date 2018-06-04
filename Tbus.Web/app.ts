/// <reference path="src/HttpClient.ts"/>
/// <reference path="src/TbusManager.ts"/>

class BusTimer {
    private element: HTMLElement;
    private timerToken: number;
    private busManager: Tbus.Web.TbusManager;
    private sleep = ms => new Promise(resolve => setTimeout(resolve, ms));
    private isRunning: boolean = false;
    private second = 1000;
    private minitu = this.second * 60;
    private hour = this.minitu * 60;

    constructor(element: HTMLElement, fileName: string) {
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
                } else if (0 < dm) {
                    html += `<span class="badge badge-primary">あと${dm}分${ds}秒</span>`;
                } else {
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