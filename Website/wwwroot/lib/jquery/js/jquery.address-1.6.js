/*! jQuery Address v1.6 | (c) 2009, 2013 Rostislav Hristov | jquery.org/license */
(function (c) {
    c.address = function () {
        var s = function (a) { a = c.extend(c.Event(a), function () { for (var b = {}, f = c.address.parameterNames(), m = 0, p = f.length; m < p; m++) b[f[m]] = c.address.parameter(f[m]); return { value: c.address.value(), path: c.address.path(), pathNames: c.address.pathNames(), parameterNames: f, parameters: b, queryString: c.address.queryString() } }.call(c.address)); c(c.address).trigger(a); return a }, g = function (a) { return Array.prototype.slice.call(a) }, k = function () {
            c().bind.apply(c(c.address), Array.prototype.slice.call(arguments));
            return c.address
        }, da = function () { c().unbind.apply(c(c.address), Array.prototype.slice.call(arguments)); return c.address }, G = function () { return A.pushState && d.state !== h }, T = function () { return ("/" + n.pathname.replace(new RegExp(d.state), "") + n.search + (H() ? "#" + H() : "")).replace(S, "/") }, H = function () { var a = n.href.indexOf("#"); return a != -1 ? n.href.substr(a + 1) : "" }, q = function () { return G() ? T() : H() }, U = function () { return "javascript" }, M = function (a) { a = a.toString(); return (d.strict && a.substr(0, 1) != "/" ? "/" : "") + a }, t = function (a,
        b) { return parseInt(a.css(b), 10) }, C = function () { if (!I) { var a = q(); if (decodeURI(e) != decodeURI(a)) if (v && x < 7) n.reload(); else { v && !J && d.history && u(N, 50); e = a; B(o) } } }, B = function (a) { u(ea, 10); return s(V).isDefaultPrevented() || s(a ? W : X).isDefaultPrevented() }, ea = function () {
            if (d.tracker !== "null" && d.tracker !== D) {
                var a = c.isFunction(d.tracker) ? d.tracker : i[d.tracker], b = (n.pathname + n.search + (c.address && !G() ? c.address.value() : "")).replace(/\/\//, "/").replace(/^\/$/, ""); if (c.isFunction(a)) a(b); else if (c.isFunction(i.urchinTracker)) i.urchinTracker(b);
                else if (i.pageTracker !== h && c.isFunction(i.pageTracker._trackPageview)) i.pageTracker._trackPageview(b); else i._gaq !== h && c.isFunction(i._gaq.push) && i._gaq.push(["_trackPageview", decodeURI(b)])
            }
        }, N = function () {
            var a = U() + ":" + o + ";document.open();document.writeln('<html><head><title>" + l.title.replace(/\'/g, "\\'") + "</title><script>var " + y + ' = "' + encodeURIComponent(q()).replace(/\'/g, "\\'") + (l.domain != n.hostname ? '";document.domain="' + l.domain : "") + "\";<\/script></head></html>');document.close();"; if (x < 7) j.src =
            a; else j.contentWindow.location.replace(a)
        }, Z = function () { if (E && Y != -1) { var a, b, f = E.substr(Y + 1).split("&"); for (a = 0; a < f.length; a++) { b = f[a].split("="); if (/^(autoUpdate|history|strict|wrap)$/.test(b[0])) d[b[0]] = isNaN(b[1]) ? /^(true|yes)$/i.test(b[1]) : parseInt(b[1], 10) !== 0; if (/^(state|tracker)$/.test(b[0])) d[b[0]] = b[1] } E = D } e = q() }, aa = function () {
            if (!$) {
                $ = r; Z(); c('a[rel*="address:"]').address(); if (d.wrap) {
                    var a = c("body"); c("body > *").wrapAll('<div style="padding:' + (t(a, "marginTop") + t(a, "paddingTop")) + "px " +
                    (t(a, "marginRight") + t(a, "paddingRight")) + "px " + (t(a, "marginBottom") + t(a, "paddingBottom")) + "px " + (t(a, "marginLeft") + t(a, "paddingLeft")) + 'px;" />').parent().wrap('<div id="' + y + '" style="height:100%;overflow:auto;position:relative;' + (K && !window.statusbar.visible ? "resize:both;" : "") + '" />'); c("html, body").css({ height: "100%", margin: 0, padding: 0, overflow: "hidden" }); K && c('<style type="text/css" />').appendTo("head").text("#" + y + "::-webkit-resizer { background-color: #fff; }")
                } if (v && !J) {
                    a = l.getElementsByTagName("frameset")[0];
                    j = l.createElement((a ? "" : "i") + "frame"); j.src = U() + ":" + o; if (a) { a.insertAdjacentElement("beforeEnd", j); a[a.cols ? "cols" : "rows"] += ",0"; j.noResize = r; j.frameBorder = j.frameSpacing = 0 } else { j.style.display = "none"; j.style.width = j.style.height = 0; j.tabIndex = -1; l.body.insertAdjacentElement("afterBegin", j) } u(function () { c(j).bind("load", function () { var b = j.contentWindow; e = b[y] !== h ? b[y] : ""; if (e != q()) { B(o); n.hash = e } }); j.contentWindow[y] === h && N() }, 50)
                } u(function () { s("init"); B(o) }, 1); if (!G()) if (v && x > 7 || !v && J) if (i.addEventListener) i.addEventListener(F,
                C, o); else i.attachEvent && i.attachEvent("on" + F, C); else fa(C, 50); "state" in window.history && c(window).trigger("popstate")
            }
        }, ga = function (a) { a = a.toLowerCase(); a = /(chrome)[ \/]([\w.]+)/.exec(a) || /(webkit)[ \/]([\w.]+)/.exec(a) || /(opera)(?:.*version|)[ \/]([\w.]+)/.exec(a) || /(msie) ([\w.]+)/.exec(a) || a.indexOf("compatible") < 0 && /(mozilla)(?:.*? rv:([\w.]+)|)/.exec(a) || []; return { browser: a[1] || "", version: a[2] || "0" } }, h, D = null, y = "jQueryAddress", F = "hashchange", V = "change", W = "internalChange", X = "externalChange",
        r = true, o = false, d = { autoUpdate: r, history: r, strict: r, wrap: o }, z = function () { var a = {}, b = ga(navigator.userAgent); if (b.browser) { a[b.browser] = true; a.version = b.version } if (a.chrome) a.webkit = true; else if (a.webkit) a.safari = true; return a }(), x = parseFloat(z.version), K = z.webkit || z.safari, v = !c.support.opacity, i = function () { try { return top.document !== h && top.document.title !== h ? top : window } catch (a) { return window } }(), l = i.document, A = i.history, n = i.location, fa = setInterval, u = setTimeout, S = /\/{2,9}/g; z = navigator.userAgent; var J =
        "on" + F in i, j, E = c("script:last").attr("src"), Y = E ? E.indexOf("?") : -1, O = l.title, I = o, $ = o, ba = r, L = o, e = q(); if (v) { x = parseFloat(z.substr(z.indexOf("MSIE") + 4)); if (l.documentMode && l.documentMode != x) x = l.documentMode != 8 ? 7 : 8; var ca = l.onpropertychange; l.onpropertychange = function () { ca && ca.call(l); if (l.title != O && l.title.indexOf("#" + q()) != -1) l.title = O } } if (A.navigationMode) A.navigationMode = "compatible"; if (document.readyState == "complete") var ha = setInterval(function () { if (c.address) { aa(); clearInterval(ha) } }, 50); else {
            Z();
            c(aa)
        } c(window).bind("popstate", function () { if (decodeURI(e) != decodeURI(q())) { e = q(); B(o) } }).bind("unload", function () { if (i.removeEventListener) i.removeEventListener(F, C, o); else i.detachEvent && i.detachEvent("on" + F, C) }); return {
            bind: function () { return k.apply(this, g(arguments)) }, unbind: function () { return da.apply(this, g(arguments)) }, init: function () { return k.apply(this, ["init"].concat(g(arguments))) }, change: function () { return k.apply(this, [V].concat(g(arguments))) }, internalChange: function () {
                return k.apply(this,
                [W].concat(g(arguments)))
            }, externalChange: function () { return k.apply(this, [X].concat(g(arguments))) }, baseURL: function () { var a = n.href; if (a.indexOf("#") != -1) a = a.substr(0, a.indexOf("#")); if (/\/$/.test(a)) a = a.substr(0, a.length - 1); return a }, autoUpdate: function (a) { if (a !== h) { d.autoUpdate = a; return this } return d.autoUpdate }, history: function (a) { if (a !== h) { d.history = a; return this } return d.history }, state: function (a) {
                if (a !== h) {
                    d.state = a; var b = T(); if (d.state !== h) if (A.pushState) b.substr(0, 3) == "/#/" && n.replace(d.state.replace(/^\/$/,
                    "") + b.substr(2)); else b != "/" && b.replace(/^\/#/, "") != H() && u(function () { n.replace(d.state.replace(/^\/$/, "") + "/#" + b) }, 1); return this
                } return d.state
            }, strict: function (a) { if (a !== h) { d.strict = a; return this } return d.strict }, tracker: function (a) { if (a !== h) { d.tracker = a; return this } return d.tracker }, wrap: function (a) { if (a !== h) { d.wrap = a; return this } return d.wrap }, update: function () { L = r; this.value(e); L = o; return this }, title: function (a) {
                if (a !== h) {
                    u(function () {
                        O = l.title = a; if (ba && j && j.contentWindow && j.contentWindow.document) {
                            j.contentWindow.document.title =
                            a; ba = o
                        }
                    }, 50); return this
                } return l.title
            }, value: function (a) { if (a !== h) { a = M(a); if (a == "/") a = ""; if (e == a && !L) return; e = a; if (d.autoUpdate || L) { if (B(r)) return this; if (G()) A[d.history ? "pushState" : "replaceState"]({}, "", d.state.replace(/\/$/, "") + (e === "" ? "/" : e)); else { I = r; if (K) if (d.history) n.hash = "#" + e; else n.replace("#" + e); else if (e != q()) if (d.history) n.hash = "#" + e; else n.replace("#" + e); v && !J && d.history && u(N, 50); if (K) u(function () { I = o }, 1); else I = o } } return this } return M(e) }, path: function (a) {
                if (a !== h) {
                    var b = this.queryString(),
                    f = this.hash(); this.value(a + (b ? "?" + b : "") + (f ? "#" + f : "")); return this
                } return M(e).split("#")[0].split("?")[0]
            }, pathNames: function () { var a = this.path(), b = a.replace(S, "/").split("/"); if (a.substr(0, 1) == "/" || a.length === 0) b.splice(0, 1); a.substr(a.length - 1, 1) == "/" && b.splice(b.length - 1, 1); return b }, queryString: function (a) { if (a !== h) { var b = this.hash(); this.value(this.path() + (a ? "?" + a : "") + (b ? "#" + b : "")); return this } a = e.split("?"); return a.slice(1, a.length).join("?").split("#")[0] }, parameter: function (a, b, f) {
                var m,
                p; if (b !== h) { var P = this.parameterNames(); p = []; b = b === h || b === D ? "" : b.toString(); for (m = 0; m < P.length; m++) { var Q = P[m], w = this.parameter(Q); if (typeof w == "string") w = [w]; if (Q == a) w = b === D || b === "" ? [] : f ? w.concat([b]) : [b]; for (var R = 0; R < w.length; R++) p.push(Q + "=" + w[R]) } c.inArray(a, P) == -1 && b !== D && b !== "" && p.push(a + "=" + b); this.queryString(p.join("&")); return this } if (b = this.queryString()) {
                    f = []; p = b.split("&"); for (m = 0; m < p.length; m++) { b = p[m].split("="); b[0] == a && f.push(b.slice(1).join("=")) } if (f.length !== 0) return f.length !=
                    1 ? f : f[0]
                }
            }, parameterNames: function () { var a = this.queryString(), b = []; if (a && a.indexOf("=") != -1) { a = a.split("&"); for (var f = 0; f < a.length; f++) { var m = a[f].split("=")[0]; c.inArray(m, b) == -1 && b.push(m) } } return b }, hash: function (a) { if (a !== h) { this.value(e.split("#")[0] + (a ? "#" + a : "")); return this } a = e.split("#"); return a.slice(1, a.length).join("#") }
        }
    }(); c.fn.address = function (s) {
        this.data("address") || this.on("click", function (g) {
            if (g.shiftKey || g.ctrlKey || g.metaKey || g.which == 2) return true; var k = g.currentTarget; if (c(k).is("a")) {
                g.preventDefault();
                g = s ? s.call(k) : /address:/.test(c(k).attr("rel")) ? c(k).attr("rel").split("address:")[1].split(" ")[0] : c.address.state() !== undefined && !/^\/?$/.test(c.address.state()) ? c(k).attr("href").replace(new RegExp("^(.*" + c.address.state() + "|\\.)"), "") : c(k).attr("href").replace(/^(#\!?|\.)/, ""); c.address.value(g)
            }
        }).on("submit", function (g) { var k = g.currentTarget; if (c(k).is("form")) { g.preventDefault(); g = c(k).attr("action"); k = s ? s.call(k) : (g.indexOf("?") != -1 ? g.replace(/&$/, "") : g + "?") + c(k).serialize(); c.address.value(k) } }).data("address",
        true); return this
    }
})(jQuery);
