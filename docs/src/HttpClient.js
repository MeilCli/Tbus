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
//# sourceMappingURL=HttpClient.js.map