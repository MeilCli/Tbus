/// <reference path="src/HttpClient.ts"/>
/// <reference path="src/TbusManager.ts"/>

class Greeter {
    element: HTMLElement;
    span1: HTMLElement;
    span2: HTMLElement;
    span3: HTMLElement;
    span4: HTMLElement;
    timerToken: number;

    constructor(element: HTMLElement) {
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