/// <reference path="./HttpClient.ts" />
var Tbus;
(function (Tbus) {
    var Web;
    (function (Web) {
        class TbusManager {
            constructor() {
                this.httpClient = new Web.HttpClient();
            }
            async get3Bus() {
                var httpRequest = new Web.HttpRequest("kansai_takatuki.limited1.json");
                var response = await this.httpClient.getAsync(httpRequest);
                if (response.status != 200) {
                    return "error";
                }
                var timeTable = JSON.parse(response.content);
                return timeTable.limited_time_option.start_day.getDay().toString();
            }
        }
        Web.TbusManager = TbusManager;
    })(Web = Tbus.Web || (Tbus.Web = {}));
})(Tbus || (Tbus = {}));
//# sourceMappingURL=TbusManager.js.map