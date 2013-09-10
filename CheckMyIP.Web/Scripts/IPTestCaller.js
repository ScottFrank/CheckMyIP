function ipCheckCallback(result) {
    ipChecker().onReady(result);
}

var ipChecker = function (options) {
    if (arguments.callee._singletonInstance) {
        return arguments.callee._singletonInstance;
    }
    arguments.callee._singletonInstance = this;

    var opts = options;

    this.onReady = function (result) {
        if (opts.onReady) {
            opts.onReady(result);
        }
    };

    this.initialize = function () {
        var sc = document.createElement("script");
        var qry = opts.target;
        qry += "?callback=ipCheckCallback";
        sc.src = qry;
        document.getElementsByTagName("head")[0].appendChild(sc);
    };
};