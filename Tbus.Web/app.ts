/// <reference path="src/HttpClient.ts"/>
/// <reference path="src/TbusManager.ts"/>

class Greeter {
    element: HTMLElement;
    span: HTMLElement;
    timerToken: number;

    constructor(element: HTMLElement) {
        this.element = element;
        this.element.innerHTML += "The time is: ";
        this.span = document.createElement('span');
        this.element.appendChild(this.span);
        // this.span.innerText = new Date().toUTCString();
    }

    async s() {
        this.span.innerHTML = await new Tbus.Web.TbusManager().get3Bus()
    }

    start() {
        this.timerToken = setInterval(() => this.span.innerHTML = new Date().toUTCString(), 500);
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