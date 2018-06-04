/// <reference path="src/HttpClient.ts"/>
/// <reference path="src/TbusManager.ts"/>

class BusContent {
    private nearBus: HTMLElement;
    private timeTable: HTMLElement;
    private busManager: Tbus.Web.TbusManager;
    private fileName: string;
    private sleep = ms => new Promise(resolve => setTimeout(resolve, ms));
    private isRunning: boolean = false;
    private second = 1000;
    private minitu = this.second * 60;
    private hour = this.minitu * 60;

    constructor(nearBusId: string, timeTableId: string, fileName: string) {
        this.nearBus = document.getElementById(nearBusId);
        this.timeTable = document.getElementById(timeTableId);
        this.fileName = fileName;
        this.busManager = new Tbus.Web.TbusManager(fileName);
    }

    async start() {
        this.isRunning = true;

        await this.setTimeTableAsync();

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

            this.nearBus.innerHTML = html;

            await this.sleep(500);
        }
    }

    async setTimeTableAsync() {
        var buses = await this.busManager.getBusesAsync();
        var html = `<div class="table-responsive"><table class="table">`;
        html += `<thead><tr><th scope="col" style="width:15%">時</th><th scope="col">分</th></tr></thead>`;
        html += `<tbody>`;

        var hour = -1;
        for (var i = 0; i < buses.length; i++) {
            var bus = buses[i];
            if (hour < bus.hour) {
                if (i != null) {
                    html += `</td></tr>`;
                }
                html += `<tr><th scope="row">${bus.hour}</th><td>`;
                hour = bus.hour;
            }

            html += `<a class="text-primary mr-4" data-toggle="modal" data-target=".bd-${this.fileName}-${bus.hour}-${bus.minute}">${bus.minute}</a>`;
            html += `<div class="modal fade bd-${this.fileName}-${bus.hour}-${bus.minute}" tabindex="-1" role="dialog" aria-hidden="true">
                        <div class="modal-dialog modal-sm">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h5 class="modal-title">${bus.hour}時${bus.minute}分発</h5>
                                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                        <span aria-hidden="true">&times;</span>
                                    </button>
                                </div>
                                <div class="modal-body">
                                    <p>${bus.destination}</p>
                                </div>
                            </div>
                        </div>
                    </div>`;
        }
        if (0 < buses.length) {
            html += `</td></tr>`;
        }

        html += `</tbody>`;
        html += `</table></div>`;

        this.timeTable.innerHTML = html;
    }

    stop() {
        this.isRunning = false;
    }
}

function createCard(title: string, num: number): string {
    return `<div class="card">
            <h5 class="card-header">${title}</h5>
            <div class="card-body">
                <ul class="nav nav-pills mb-3" id="nav-tab${num}" role="tablist">
                    <li class="nav-item">
                        <a class="nav-link active" id="nav-takatuki-tab${num}" data-toggle="pill" href="#nav-takatuki${num}" role="tab" aria-controls="nav-takatuki${num}"
                           aria-selected="true">JR高槻駅北</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" id="nav-tonda-tab${num}" data-toggle="pill" href="#nav-tonda${num}" role="tab" aria-controls="nav-tonda${num}" aria-selected="false">JR富田駅</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" id="nav-kansai-tab${num}" data-toggle="pill" href="#nav-kansai${num}" role="tab" aria-controls="nav-kansai${num}" aria-selected="false">関西大学</a>
                    </li>
                </ul>
                <div class="tab-content" id="nav-tabContent">
                    <div class="tab-pane fade show active" id="nav-takatuki${num}" role="tabpanel" aria-labelledby="nav-takatuki-tab${num}">
                        <ul class="nav nav-tabs mb-3" id="nav-tab${num}" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link active" id="nav-takatuki-kansai-tab${num}" data-toggle="pill" href="#nav-takatuki-kansai${num}" role="tab" aria-controls="nav-takatuki-kansai${num}"
                                   aria-selected="true">関西大学方面</a>
                            </li>
                        </ul>
                        <div class="tab-content" id="nav-tabContent">
                            <div class="tab-pane fade show active" id="nav-takatuki-kansai${num}" role="tabpanel" aria-labelledby="nav-takatuki-kansai-tab${num}"></div>
                        </div>
                    </div>
                    <div class="tab-pane fade" id="nav-tonda${num}" role="tabpanel" aria-labelledby="nav-tonda-tab${num}">
                        <ul class="nav nav-tabs mb-3" id="nav-tab${num}" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link active" id="nav-tonda-kansai-tab${num}" data-toggle="pill" href="#nav-tonda-kansai${num}" role="tab" aria-controls="nav-tonda-kansai${num}"
                                   aria-selected="true">関西大学方面</a>
                            </li>
                        </ul>
                        <div class="tab-content" id="nav-tabContent">
                            <div class="tab-pane fade show active" id="nav-tonda-kansai${num}" role="tabpanel" aria-labelledby="nav-tonda-kansai-tab${num}"></div>
                        </div>
                    </div>
                    <div class="tab-pane fade" id="nav-kansai${num}" role="tabpanel" aria-labelledby="nav-kansai-tab${num}">
                        <ul class="nav nav-tabs mb-3" id="nav-tab" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link active" id="nav-kansai-takatuki-tab${num}" data-toggle="pill" href="#nav-kansai-takatuki${num}" role="tab" aria-controls="nav-kansai-takatuki${num}"
                                   aria-selected="true">JR高槻駅北方面</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" id="nav-kansai-tonda-tab${num}" data-toggle="pill" href="#nav-kansai-tonda${num}" role="tab" aria-controls="nav-kansai-tonda${num}"
                                   aria-selected="true">JR富田駅方面</a>
                            </li>
                        </ul>
                        <div class="tab-content" id="nav-tabContent">
                            <div class="tab-pane fade show active" id="nav-kansai-takatuki${num}" role="tabpanel" aria-labelledby="nav-kansai-takatuki-tab${num}"></div>
                            <div class="tab-pane fade" id="nav-kansai-tonda${num}" role="tabpanel" aria-labelledby="nav-kansai-tonda-tab${num}"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>`;
}

window.onload = () => {
    document.getElementById('content1').innerHTML = createCard("直近の発着時間", 1);
    document.getElementById('content2').innerHTML = createCard("今日の時刻表", 2);

    {
        var busContent = new BusContent('nav-kansai-takatuki1', 'nav-kansai-takatuki2', "kansai_takatuki");
        busContent.start();
    }
    {
        var busContent = new BusContent('nav-kansai-tonda1', 'nav-kansai-tonda2', "kansai_tonda");
        busContent.start();
    }
    {
        var busContent = new BusContent('nav-takatuki-kansai1', 'nav-takatuki-kansai2', "takatuki_kansai");
        busContent.start();
    }
    {
        var busContent = new BusContent('nav-tonda-kansai1', 'nav-tonda-kansai2', "tonda_kansai");
        busContent.start();
    }
};