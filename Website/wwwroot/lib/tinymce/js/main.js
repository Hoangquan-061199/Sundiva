var fileTypeIcons = new Object();
fileTypeIcons["7z"] = "icon-zip.png";
fileTypeIcons.doc = "file-doc.png";
fileTypeIcons.docx = "file-doc.png";
fileTypeIcons.jpeg = "file-img.png";
fileTypeIcons.jpg = "file-img.png";
fileTypeIcons.svg = "file-img.png";
fileTypeIcons.gif = "file-img.png";
fileTypeIcons.pdf = "file-pdf.png";
fileTypeIcons.png = "file-img.png";
fileTypeIcons.webp = "file-img.png";
fileTypeIcons.rar = "icon-zip.png";
fileTypeIcons.torrent = "file_extension_torrent.png";
fileTypeIcons.xls = "file-excel.png";
fileTypeIcons.xlsx = "file-excel.png";
fileTypeIcons.ppt = "file-ppt.png";
fileTypeIcons.xpi = "file_extension_xpi.png";
fileTypeIcons.zip = "icon-zip.png";
var FileTypes = new Array();
FileTypes.image = new Array("jpg", "jpeg", "png", "gif");
FileTypes.media = new Array("avi", "flv", "swf", "wmv", "mp3", "mp4", "wma", "mpg", "mpeg");
FileTypes.document = new Array("doc", "docx", "txt", "rtf", "pdf", "xls", "mdb", "html", "htm", "db");

let isCtrl = false;
let isShift = false;
let fileElement = $('#fileActions');
let folderElement = $('#dirActions');
$(document).keydown(function (e) {
    if (e.keyCode === 17)
        isCtrl = true;
    if (e.keyCode === 16)
        isShift = true;
    if (isSelecting(e))
        setAllFile()();
    if (e.keyCode === 27)
        unSetAllFile();
    if (e.keyCode === 46) {
        if (fileElement.hasClass('active'))
            deleteFile();
        if (folderElement.hasClass('active'))
            deleteDir();
    }
    if (e.keyCode === 113) {
        if (fileElement.hasClass('active'))
            renameFile();
        if (folderElement.hasClass('active'))
            renameDir();
    }
    if (e.keyCode === 13)
        setFile();
})

fileElement.click(function () {
    $(this).addClass('active');
    if (folderElement.hasClass('active'))
        folderElement.removeClass('active');
})
folderElement.click(function () {
    $(this).addClass('active');
    if (fileElement.hasClass('active'))
        fileElement.removeClass('active');
})

$(document).keyup(function (e) {
    isCtrl = false;
    isShift = false;
})

$('pnlFiles')

function RoxyUtils() { }
RoxyUtils.FixPath = function (b) {
    if (!b) {
        return ""
    }
    var a = b.replace(/\\/g, "");
    a = a.replace(/\/\//g, "/");
    a = a.replace(":/", "://");
    return a
};
RoxyUtils.FormatDate = function (b) {
    var a = "";
    try {
        a = $.format.date(b, RoxyFilemanConf.DATEFORMAT)
    } catch (c) {
        a = b.toString();
        a = a.substr(0, a.indexOf("UTC"))
    }
    return a
};
RoxyUtils.GetPath = function (b) {

    var a = "";
    b = RoxyUtils.FixPath(b);
    if (b.indexOf("/") > -1) {
        a = b.substring(0, b.lastIndexOf("/"))
    }
    return a
};
RoxyUtils.GetUrlParam = function (d, b) {

    var a = "";
    if (!b) {
        b = self.location.href
    }
    if (b.indexOf("?") > -1) {
        b = b.substr(b.indexOf("?") + 1);
        b = b.split("&");
        for (i = 0; i < b.length; i++) {
            var c = b[i].split("=");
            if (c[0] && c[1] && c[0] == d) {
                a = c[1];
                break
            }
        }
    }
    return a
};
RoxyUtils.GetFilename = function (b) {

    var a = b;
    b = RoxyUtils.FixPath(b);
    if (b.indexOf("/") > -1) {
        a = b.substring(b.lastIndexOf("/") + 1)
    }
    return a
};
RoxyUtils.MakePath = function () {

    ret = "";
    if (arguments && arguments.length > 0) {
        for (var a = 0; a < arguments.length; a++) {
            ret += ($.isArray(arguments[a]) ? arguments[a].join("/") : arguments[a]);
            if (a < (arguments.length - 1)) {
                ret += "/"
            }
        }
        ret = RoxyUtils.FixPath(ret)
    }
    return ret
};
RoxyUtils.GetFileExt = function (b) {
    var a = "";
    b = RoxyUtils.GetFilename(b);
    if (b.indexOf(".") > -1) {
        a = b.substring(b.lastIndexOf(".") + 1)
    }
    return a
};
RoxyUtils.FileExists = function (b) {
    var a = false;
    $.ajax({
        url: b,
        type: "HEAD",
        async: false,
        dataType: "text",
        success: function () {
            a = true
        }
    });
    return a
};
RoxyUtils.GetFileIcon = function (a) {
    ret = "images/icon/file.png";
    if (fileTypeIcons[RoxyUtils.GetFileExt(a).toLowerCase()]) {
        ret = "images/icon/" + fileTypeIcons[RoxyUtils.GetFileExt(a).toLowerCase()]
    }
    return ret
};
RoxyUtils.GetFileSize = function (b) {

    var a = 0;
    $.ajax({
        url: b,
        type: "HEAD",
        async: false,
        success: function (f, c, e) {
            debugger
            a = e.getResponseHeader("Content-Length")
        }
    });
    if (!a) {
        a = 0
    }
    return a
};
RoxyUtils.GetFileType = function (b) {
    var a = RoxyUtils.GetFileExt(b).toLowerCase();
    if (a == "png" || a == "jpg" || a == "gif" || a == "jpeg" || a == "webp" || a == "svg" || a == "gif") {
        a = "image"
    }
    return a
};
RoxyUtils.IsImage = function (b) {
    var a = false;
    if (RoxyUtils.GetFileType(b) == "image") {
        a = true
    }
    return a
};
RoxyUtils.FormatFileSize = function (a) {
    var b = "B";
    if (!a) {
        a = 0
    }
    if (a > 1024) {
        a = a / 1024;
        b = "KB"
    }
    if (a > 1024) {
        a = a / 1024;
        b = "MB"
    }
    a = new Number(a);
    return a.toFixed(2) + " " + b
};
RoxyUtils.AddParam = function (b, c, a) {
    b += (b.indexOf("?") > -1 ? "&" : "?") + c + "=" + encodeURIComponent(a);
    return b
};
RoxyUtils.SelectText = function (d, f, b) {
    try {
        var e = document.getElementById(d);
        if (e.createTextRange) {
            var a = e.createTextRange();
            a.collapse(true);
            a.moveStart("character", f);
            a.moveEnd("character", b - f);
            a.select()
        } else {
            if (e.setSelectionRange) {
                e.setSelectionRange(f, b)
            } else {
                if (e.selectionStart) {
                    e.selectionStart = f;
                    e.selectionEnd = b
                }
            }
        }
        e.focus()
    } catch (c) { }
};

function RoxyFilemanConf() { }
RoxyUtils.LoadConfig = function () {
    $.ajax({
        url: "conf.json",
        dataType: "json",
        async: false,
        success: function (a) {
            RoxyFilemanConf = a
        },
        error: function (a) {
            alert(t("E_LoadingConf"))
        }
    })
};

function RoxyLang() { }
RoxyUtils.LoadLang = function () {
    var a = "";
    if (RoxyFilemanConf.LANG && RoxyFilemanConf.LANG.toLowerCase() == "auto") {
        if (RoxyUtils.GetUrlParam("langCode")) {
            a = "lang/" + RoxyUtils.GetUrlParam("langCode").substr(0, 2).toLowerCase() + ".json"
        } else {
            var b = window.navigator.userLanguage || window.navigator.language;
            a = "lang/" + b.substr(0, 2) + ".json"
        }
        if (!RoxyUtils.FileExists(a)) {
            a = ""
        }
    } else {
        if (RoxyFilemanConf.LANG) {
            a = "lang/" + RoxyFilemanConf.LANG.substr(0, 2).toLowerCase() + ".json";
            if (!RoxyUtils.FileExists(a)) {
                a = ""
            }
        }
    }
    if (!a) {
        a = "lang/en.json"
    }
    $.ajax({
        url: a,
        dataType: "json",
        async: false,
        success: function (c) {
            RoxyLang = c
        },
        error: function (c) {
            alert("Error loading language file")
        }
    })
};
RoxyUtils.Translate = function () {
    RoxyUtils.LoadLang();
    $("[data-lang-t]").each(function () {
        var a = $(this).attr("data-lang-t");
        $(this).prop("title", t(a))
    });
    $("[data-lang-v]").each(function () {
        var a = $(this).attr("data-lang-v");
        $(this).prop("value", t(a))
    });
    $("[data-lang]").each(function () {
        var a = $(this).attr("data-lang");
        $(this).html(t(a))
    })
};
RoxyUtils.GetCookies = function () {
    var a = new Object();
    var b = document.cookie.replace(" ", "");
    b = b.split(";");
    for (i in b) {
        var c = b[i].split("=");
        if (c.length > 1) {
            a[$.trim(c[0].toString())] = decodeURIComponent($.trim(c[1].toString())) || ""
        }
    }
    return a
};
RoxyUtils.GetCookie = function (b) {
    var a = RoxyUtils.GetCookies();
    return a[b] || ""
};
RoxyUtils.SetCookie = function (c, e, b, d) {
    var a = new Date();
    if (b) {
        a.setTime(a.getTime() + (b * 3600 * 1000))
    }
    if (!d) {
        d = "/"
    }
    document.cookie = c + "=" + encodeURIComponent(e) + "; path=" + d + (b ? "; expires=" + a.toGMTString() : "")
};
RoxyUtils.ToBool = function (b) {
    var a = false;
    b = b.toString().toLowerCase();
    if (b == "true" || b == "on" || b == "yes" || b == "1") {
        a = true
    }
    return a
};
RoxyUtils.UnsetCookie = function (a) {
    document.cookie = a + "=; expires=Thu, 01 Jan 1972 00:00:00 UTC"
};

function t(a) {
    var b = a;
    if (RoxyLang && RoxyLang[a]) {
        b = RoxyLang[a]
    }
    return b
}

function File(d, a, c, b, e) {
    this.fullPath = d;
    this.type = RoxyUtils.GetFileType(d);
    this.name = RoxyUtils.GetFilename(d);
    this.ext = RoxyUtils.GetFileExt(d);
    this.path = RoxyUtils.GetPath(d);
    this.icon = RoxyUtils.GetFileIcon(d);
    this.bigIcon = this.icon.replace("filetypes", "filetypes/big");
    this.image = d;
    this.size = (a ? a : RoxyUtils.GetFileSize(d));
    this.time = c;
    this.width = (b ? b : 0);
    this.height = (e ? e : 0);
    this.Show = function () {
        html = '<li data-path="' + this.fullPath + '" data-time="' + this.time + '" data-icon="' + this.icon + '" data-w="' + this.width + '" data-h="' + this.height + '" data-size="' + this.size + '" data-icon-big="' + (this.IsImage() ? this.fullPath : this.bigIcon) + '" title="' + this.name + '">';
        html += '<img src="' + this.icon + '" class="icon">';
        html += '<span class="time">' + RoxyUtils.FormatDate(new Date(this.time * 1000)) + "</span>";
        html += '<span class="name">' + this.name + "</span>";
        html += '<span class="size">' + RoxyUtils.FormatFileSize(this.size) + "</span>";
        html += "</li>";
        $("#pnlFileList").append(html);
        var f = $("#pnlFileList li:last");
        if (RoxyFilemanConf.MOVEFILE) {
            f.draggable({
                helper: makeDragFile,
                start: startDragFile,
                cursorAt: {
                    left: 10,
                    top: 10
                },
                delay: 200
            })
        }

        f.click(function (g) {
            if (g.type == 'click') {
                if (!isCtrl && !isShift)
                    selectOnlyFile(this);
                else if (isCtrl)
                    selectFileAll(this);
                else if (isShift) {
                    let indexFirst = $('#pnlFileList').find('li.selected').index();
                    if (indexFirst < $(this).index()) {
                        for (let i = indexFirst; i <= $(this).index(); i++) {
                            $($('#pnlFileList li')[i]).removeClass("selected");
                            selectFileAll($('#pnlFileList li')[i]);
                        }
                    }
                    else {
                        for (let i = $(this).index(); i <= indexFirst; i++) {
                            $($('#pnlFileList li')[i]).removeClass("selected");
                            selectFileAll($('#pnlFileList li')[i]);
                        }
                    }

                }
            }

        });
        f.dblclick(function (g) {
            selectFile(this)
            setFile()
        });
        f.tooltip({
            show: {
                delay: 700
            },
            track: true,
            content: tooltipContent
        });
        f.bind("contextmenu", function (h) {
            h.stopPropagation();
            h.preventDefault();
            closeMenus("dir");
            selectFile(this);
            $(this).tooltip("close");
            var g = h.pageY - $("#menuFile").height();
            if (g < 0) {
                g = 0
            }
            $("#menuFile").css({
                top: g + "px",
                left: h.pageX + "px"
            }).show();
            return false
        })
    };
    this.GetElement = function () {
        return $('li[data-path="' + this.fullPath + '"]')
    };
    this.IsImage = function () {
        var f = false;
        if (this.type == "image") {
            f = true
        }
        return f
    };
    this.Delete = function () {
        if (!RoxyFilemanConf.DELETEFILE) {
            alert(t("E_ActionDisabled"));
            return
        }
        var g = RoxyUtils.AddParam(RoxyFilemanConf.DELETEFILE, "f", this.fullPath);
        var f = this;
        $.ajax({
            url: g,
            type: "POST",
            data: {
                f: this.fullPath
            },
            dataType: "json",
            async: false,
            success: function (h) {
                if (h.res.toLowerCase() == "ok") {
                    $('li[data-path="' + f.fullPath + '"]').remove();
                    var j = Directory.Parse(f.path);
                    if (j) {
                        j.files--;
                        j.Update();
                        j.SetStatusBar()
                    }
                } else {
                    alert(h.msg)
                }
            },
            error: function (h) {
                alert(t("E_LoadingAjax") + " " + g)
            }
        })
    };
    this.Rename = function (f) {
        if (!RoxyFilemanConf.RENAMEFILE) {
            alert(t("E_ActionDisabled"));
            return false
        }
        if (!f) {
            return false
        }
        var h = RoxyUtils.AddParam(RoxyFilemanConf.RENAMEFILE, "f", this.fullPath);
        h = RoxyUtils.AddParam(h, "n", f);
        var j = this;
        var g = false;
        $.ajax({
            url: h,
            type: "POST",
            data: {
                f: this.fullPath,
                n: f
            },
            dataType: "json",
            async: false,
            success: function (l) {
                if (l.res.toLowerCase() == "ok") {
                    var k = RoxyUtils.MakePath(this.path, f);
                    $('li[data-path="' + j.fullPath + '"] .icon').attr("src", RoxyUtils.GetFileIcon(f));
                    $('li[data-path="' + j.fullPath + '"] .name').text(f);
                    $('li[data-path="' + k + '"]').attr("data-path", k);
                    g = true
                }
                if (l.msg) {
                    alert(l.msg)
                }
            },
            error: function (k) {
                alert(t("E_LoadingAjax") + " " + h)
            }
        });
        return g
    };
    this.Copy = function (j) {
        if (!RoxyFilemanConf.COPYFILE) {
            alert(t("E_ActionDisabled"));
            return
        }
        var g = RoxyUtils.AddParam(RoxyFilemanConf.COPYFILE, "f", this.fullPath);
        g = RoxyUtils.AddParam(g, "n", j);
        var h = this;
        var f = false;
        $.ajax({
            url: g,
            type: "POST",
            data: {
                f: this.fullPath,
                n: j
            },
            dataType: "json",
            async: false,
            success: function (k) {
                if (k.res.toLowerCase() == "ok") {
                    var l = Directory.Parse(j);
                    if (l) {
                        l.files++;
                        l.Update();
                        l.SetStatusBar();
                        l.ListFiles(true)
                    }
                    f = true
                }
                if (k.msg) {
                    alert(k.msg)
                }
            },
            error: function (k) {
                alert(t("E_LoadingAjax") + " " + g)
            }
        });
        return f
    };
    this.Move = function (j) {
        if (!RoxyFilemanConf.MOVEFILE) {
            alert(t("E_ActionDisabled"));
            return
        }
        newFullPath = RoxyUtils.MakePath(j, this.name);
        var g = RoxyUtils.AddParam(RoxyFilemanConf.MOVEFILE, "f", this.fullPath);
        g = RoxyUtils.AddParam(g, "n", newFullPath);
        var h = this;
        var f = false;
        $.ajax({
            url: g,
            type: "POST",
            data: {
                f: this.fullPath,
                n: newFullPath
            },
            dataType: "json",
            async: false,
            success: function (k) {
                if (k.res.toLowerCase() == "ok") {
                    $('li[data-path="' + h.fullPath + '"]').remove();
                    var l = Directory.Parse(h.path);
                    if (l) {
                        l.files--;
                        l.Update();
                        l.SetStatusBar();
                        l = Directory.Parse(j);
                        l.files++;
                        l.Update()
                    }
                    f = true
                }
                if (k.msg) {
                    alert(k.msg)
                }
            },
            error: function (k) {
                alert(t("E_LoadingAjax") + " " + g)
            }
        });
        return f
    }
}

File.Parse = function (c) {
    var b = false;
    var a = $("#pnlFileList").find('li[data-path="' + c + '"]');
    if (a.length > 0) {
        b = new File(a.attr("data-path"), a.attr("data-size"), a.attr("data-time"), a.attr("data-w"), a.attr("data-h"))
    }
    return b
};

function Directory(a, c, b) {

    if (!a) {
        a = ""
    }
    this.fullPath = a;
    this.name = RoxyUtils.GetFilename(a);
    if (!this.name) {
        this.name = "My files"
    }
    this.path = RoxyUtils.GetPath(a);
    this.dirs = (c ? c : 0);
    this.files = (b ? b : 0);
    this.filesList = new Array();
    this.Show = function () {
        var d = this.GetHtml();
        var e = null;
        e = $('li[data-path="' + this.path + '"]');
        if (e.length == 0) {
            e = $("#pnlDirList")
        } else {
            if (e.children("ul").length == 0) {
                e.append("<ul></ul>")
            }
            e = e.children("ul")
        }
        if (e) {
            e.append(d);
            this.SetEvents()
        }
    };
    this.SetEvents = function () {
        var d = this.GetElement();
        if (RoxyFilemanConf.MOVEDIR) {
            d.draggable({
                helper: makeDragDir,
                start: startDragDir,
                cursorAt: {
                    left: 10,
                    top: 10
                },
                delay: 200
            })
        }
        d = d.children("div");
        d.click(function (f) {
            selectDir(this)
        });
        d.bind("contextmenu", function (g) {
            g.stopPropagation();
            g.preventDefault();
            closeMenus("file");
            selectDir(this);
            var f = g.pageY - $("#menuDir").height();
            if (f < 0) {
                f = 0
            }
            $("#menuDir").css({
                top: f + "px",
                left: g.pageX + "px"
            }).show();
            return false
        });
        d.droppable({
            drop: moveObject,
            over: dragFileOver,
            out: dragFileOut
        });
        d = d.children(".dirPlus");
        d.click(function (f) {
            f.stopPropagation();
            var g = Directory.Parse($(this).closest("li").attr("data-path"));
            g.Expand()
        })
    };
    this.GetHtml = function () {
        var d = '<li data-path="' + this.fullPath + '" data-dirs="' + this.dirs + '" data-files="' + this.files + '" class="directory">';
        d += '<div><img src="images/' + (this.dirs > 0 ? "/icon/plus.png" : "blank.gif") + '" class="dirPlus" width="15" height="15">';
        d += '<img src="' + ((this.files == 0) ? "images/icon/folder.png" : "images/icon/folder-full.png") + '" class="dir"><span class="name">' + this.name + " (" + this.files + ")</span></div>";
        d += "</li>";
        return d
    };
    this.SetStatusBar = function () {
        $("#pnlStatus").html(this.files + " " + (this.files == 1 ? t("file") : t("files")))
    };
    this.SetSelectedFile = function (e) {
        if (e) {
            var d = File.Parse(e);
            if (d) {
                selectFile(d.GetElement())
            }
        }
    };
    this.Select = function (e) {
        var d = this.GetElement();
        d.children("div").addClass("selected");
        $('#pnlDirList li[data-path!="' + this.fullPath + '"] > div').removeClass("selected");
        d.children("img.dir").prop("src", "images/icon/folder.png");
        this.SetStatusBar();
        var f = this.GetParent();
        while (f) {
            f.Expand(true);
            f = f.GetParent()
        }
        this.Expand(true);
        this.ListFiles(true, e);
        setLastDir(this.fullPath)
    };
    this.GetElement = function () {
        return $('li[data-path="' + this.fullPath + '"]')
    };
    this.IsExpanded = function () {
        var d = this.GetElement().children("ul");
        return (d && d.is(":visible"))
    };
    this.IsListed = function () {
        if ($("#hdDir").val() == this.fullPath) {
            return true
        }
        return false
    };
    this.GetExpanded = function (e) {
        var d = new Array();
        if (!e) {
            e = $("#pnlDirList")
        }
        e.children("li").each(function () {
            var f = $(this).attr("data-path");
            var g = new Directory(f);
            if (g) {
                if (g.IsExpanded() && f) {
                    d.push(f)
                }
                d = d.concat(g.GetExpanded(g.GetElement().children("ul")))
            }
        });
        return d
    };
    this.RestoreExpanded = function (e) {
        for (i = 0; i < e.length; i++) {
            var f = Directory.Parse(e[i]);
            if (f) {
                f.Expand(true)
            }
        }
    };
    this.GetParent = function () {
        return Directory.Parse(this.path)
    };
    this.SetOpened = function () {
        var d = this.GetElement();
        if (d.find("li").length < 1) {
            d.children("div").children(".dirPlus").prop("src", "images/blank.gif")
        } else {
            if (this.IsExpanded()) {
                d.children("div").children(".dirPlus").prop("src", "images/icon/minus.png")
            } else {
                d.children("div").children(".dirPlus").prop("src", "images/icon/plus.png")
            }
        }
    };
    this.Update = function (e) {
        var d = this.GetElement();
        if (e) {
            this.fullPath = e;
            this.name = RoxyUtils.GetFilename(e);
            if (!this.name) {
                this.name = "My files"
            }
            this.path = RoxyUtils.GetPath(e)
        }
        d.attr("data-path", this.fullPath);
        d.attr("data-dirs", this.dirs);
        d.attr("data-files", this.files);
        d.children("div").children(".name").html(this.name + " (" + this.files + ")");
        this.SetOpened()
    };
    this.LoadAll = function (d) {
        var f = this.GetExpanded();
        var e = RoxyFilemanConf.DIRLIST;
        if (!e) {
            alert(t("E_ActionDisabled"));
            return
        }
        $("#pnlLoadingDirs").show();
        $("#pnlDirList").hide();
        e = RoxyUtils.AddParam(e, "type", RoxyUtils.GetUrlParam("type"));
        var g = this;
        $.ajax({
            url: e,
            type: "POST",
            dataType: "json",
            async: false,
            cache: false,
            success: function (h) {
                $("#pnlDirList").children("li").remove();
                for (i = 0; i < h.length; i++) {
                    var j = new Directory(h[i].p, h[i].d, h[i].f);
                    j.Show()
                }
                $("#pnlLoadingDirs").hide();
                $("#pnlDirList").show();
                g.RestoreExpanded(f);
                var j = Directory.Parse(d);
                if (j) {
                    j.Select()
                }
            },
            error: function (h) {
                $("#pnlLoadingDirs").hide();
                $("#pnlDirList").show();
                alert(t("E_LoadingAjax") + " " + RoxyFilemanConf.DIRLIST)
            }
        })
    };
    this.Expand = function (e) {
        var d = this.GetElement();
        var f = d.children("ul");
        if (this.IsExpanded() && !e) {
            f.hide()
        } else {
            f.show()
        }
        this.SetOpened()
    };
    this.Create = function (d) {
        if (!d) {
            return false
        } else {
            if (!RoxyFilemanConf.CREATEDIR) {
                alert(t("E_ActionDisabled"));
                return
            }
        }
        var f = RoxyUtils.AddParam(RoxyFilemanConf.CREATEDIR, "d", this.fullPath);
        f = RoxyUtils.AddParam(f, "n", d);
        var g = this;
        var e = false;
        $.ajax({
            url: f,
            type: "POST",
            data: {
                d: this.fullPath,
                n: d
            },
            dataType: "json",
            async: false,
            cache: false,
            success: function (h) {
                if (h.res.toLowerCase() == "ok") {
                    g.LoadAll(RoxyUtils.MakePath(g.fullPath, d));
                    e = true
                } else {
                    alert(h.msg)
                }
            },
            error: function (h) {
                alert(t("E_LoadingAjax") + " " + g.name)
            }
        });
        return e
    };
    this.Delete = function () {
        if (!RoxyFilemanConf.DELETEDIR) {
            alert(t("E_ActionDisabled"));
            return
        }
        var e = RoxyUtils.AddParam(RoxyFilemanConf.DELETEDIR, "d", this.fullPath);
        var f = this;
        var d = false;
        $.ajax({
            url: e,
            type: "POST",
            data: {
                d: this.fullPath
            },
            dataType: "json",
            async: false,
            cache: false,
            success: function (h) {
                if (h.res.toLowerCase() == "ok") {
                    var g = f.GetParent();
                    g.dirs--;
                    g.Update();
                    g.Select();
                    f.GetElement().remove();
                    d = true
                }
                if (h.msg) {
                    alert(h.msg)
                }
            },
            error: function (g) {
                alert(t("E_LoadingAjax") + " " + f.name)
            }
        });
        return d
    };
    this.Rename = function (d) {
        if (!d) {
            return false
        } else {
            if (!RoxyFilemanConf.RENAMEDIR) {
                alert(t("E_ActionDisabled"));
                return
            }
        }
        var f = RoxyUtils.AddParam(RoxyFilemanConf.RENAMEDIR, "d", this.fullPath);
        f = RoxyUtils.AddParam(f, "n", d);
        var g = this;
        var e = false;
        $.ajax({
            url: f,
            type: "POST",
            data: {
                d: this.fullPath,
                n: d
            },
            dataType: "json",
            async: false,
            cache: false,
            success: function (j) {
                if (j.res.toLowerCase() == "ok") {
                    var h = RoxyUtils.MakePath(g.path, d);
                    g.Update(h);
                    g.Select();
                    e = true
                }
                if (j.msg) {
                    alert(j.msg)
                }
            },
            error: function (h) {
                alert(t("E_LoadingAjax") + " " + g.name)
            }
        });
        return e
    };
    this.Copy = function (g) {
        if (!RoxyFilemanConf.COPYDIR) {
            alert(t("E_ActionDisabled"));
            return
        }
        var e = RoxyUtils.AddParam(RoxyFilemanConf.COPYDIR, "d", this.fullPath);
        e = RoxyUtils.AddParam(e, "n", g);
        var f = this;
        var d = false;
        $.ajax({
            url: e,
            type: "POST",
            data: {
                d: this.fullPath,
                n: g
            },
            dataType: "json",
            async: false,
            cache: false,
            success: function (h) {
                if (h.res.toLowerCase() == "ok") {
                    var j = Directory.Parse(g);
                    if (j) {
                        j.LoadAll(j.fullPath)
                    }
                    d = true
                }
                if (h.msg) {
                    alert(h.msg)
                }
            },
            error: function (h) {
                alert(t("E_LoadingAjax") + " " + e)
            }
        });
        return d
    };
    this.Move = function (g) {
        if (!g) {
            return false
        } else {
            if (!RoxyFilemanConf.MOVEDIR) {
                alert(t("E_ActionDisabled"));
                return
            }
        }
        var e = RoxyUtils.AddParam(RoxyFilemanConf.MOVEDIR, "d", this.fullPath);
        e = RoxyUtils.AddParam(e, "n", g);
        var f = this;
        var d = false;
        $.ajax({
            url: e,
            type: "POST",
            data: {
                d: this.fullPath,
                n: g
            },
            dataType: "json",
            async: false,
            cache: false,
            success: function (h) {
                if (h.res.toLowerCase() == "ok") {
                    f.LoadAll(RoxyUtils.MakePath(g, f.name));
                    d = true
                }
                if (h.msg) {
                    alert(h.msg)
                }
            },
            error: function (h) {
                alert(t("E_LoadingAjax") + " " + f.name)
            }
        });
        return d
    };
    this.ListFiles = function (e, d) {
        $("#pnlLoading").show();
        $("#pnlEmptyDir").hide();
        $("#pnlFileList").hide();
        $("#pnlSearchNoFiles").hide();
        this.LoadFiles(e, d)
    };
    this.FilesLoaded = function (g, d) {
        g = this.SortFiles(g);
        $("#pnlFileList").html("");
        for (i = 0; i < g.length; i++) {
            var e = g[i];
            e.Show()
        }
        $("#hdDir").val(this.fullPath);
        $("#pnlLoading").hide();
        if ($("#pnlFileList").children("li").length == 0) {
            $("#pnlEmptyDir").show()
        }
        this.files = $("#pnlFileList").children("li").length;
        this.Update();
        this.SetStatusBar();
        filterFiles();
        switchView();
        $("#pnlFileList").show();
        this.SetSelectedFile(d)
    };
    this.LoadFiles = function (g, f) {
        if (!RoxyFilemanConf.FILESLIST) {
            alert(t("E_ActionDisabled"));
            return
        }
        var e = new Array();
        var d = RoxyFilemanConf.FILESLIST;
        d = RoxyUtils.AddParam(d, "d", this.fullPath);
        d = RoxyUtils.AddParam(d, "type", RoxyUtils.GetUrlParam("type"));
        var h = this;
        if (!this.IsListed() || g) {
            $.ajax({
                url: d,
                type: "POST",
                data: {
                    d: this.fullPath,
                    type: RoxyUtils.GetUrlParam("type")
                },
                dataType: "json",
                async: true,
                cache: false,
                success: function (j) {
                    for (i = 0; i < j.length; i++) {
                        e.push(new File(j[i].p, j[i].s, j[i].t, j[i].w, j[i].h))
                    }
                    h.FilesLoaded(e, f)
                },
                error: function (j) {
                    alert(t("E_LoadingAjax") + " " + d)
                }
            })
        } else {
            $("#pnlFileList li").each(function () {
                e.push(new File($(this).attr("data-path"), $(this).attr("data-size"), $(this).attr("data-time"), $(this).attr("data-w"), $(this).attr("data-h")))
            });
            h.FilesLoaded(e, f)
        }
        return e
    };
    this.SortByName = function (e, d) {
        e.sort(function (h, g) {
            var f = (d == "desc" ? 0 : 2);
            h = h.name.toLowerCase();
            g = g.name.toLowerCase();
            if (h > g) {
                return -1 + f
            } else {
                if (h < g) {
                    return 1 - f
                } else {
                    return 0
                }
            }
        });
        return e
    };
    this.SortBySize = function (e, d) {
        e.sort(function (h, g) {
            var f = (d == "desc" ? 0 : 2);
            h = parseInt(h.size);
            g = parseInt(g.size);
            if (h > g) {
                return -1 + f
            } else {
                if (h < g) {
                    return 1 - f
                } else {
                    return 0
                }
            }
        });
        return e
    };
    this.SortByTime = function (e, d) {
        e.sort(function (h, g) {
            var f = (d == "desc" ? 0 : 2);
            h = parseInt(h.time);
            g = parseInt(g.time);
            if (h > g) {
                return -1 + f
            } else {
                if (h < g) {
                    return 1 - f
                } else {
                    return 0
                }
            }
        });
        return e
    };
    this.SortFiles = function (e) {
        var d = $("#ddlOrder").val();
        if (!d) {
            d = "name"
        }
        switch (d) {
            case "size":
                e = this.SortBySize(e, "asc");
                break;
            case "size_desc":
                e = this.SortBySize(e, "desc");
                break;
            case "time":
                e = this.SortByTime(e, "asc");
                break;
            case "time_desc":
                e = this.SortByTime(e, "desc");
                break;
            case "name_desc":
                e = this.SortByName(e, "desc");
                break;
            default:
                e = this.SortByName(e, "asc")
        }
        return e
    }
}
Directory.Parse = function (c) {
    var b = false;
    var a = $("#pnlDirList").find('li[data-path="' + c + '"]');
    if (a.length > 0) {
        b = new Directory(a.attr("data-path"), a.attr("data-dirs"), a.attr("data-files"))
    }
    return b
};
$.ajaxSetup({
    cache: false
});

function selectFile(b) {
    $(b).prop("class", "selected");
    var a = RoxyUtils.GetFilename($(b).attr("data-path"));
    a += " (" + t("Size") + ": " + RoxyUtils.FormatFileSize($(b).attr("data-size"));
    if ($(b).attr("data-w") > 0) {
        a += ", " + t("Dimensions") + ":" + $(b).attr("data-w") + "x" + $(b).attr("data-h")
    }
    a += ")";
    $("#pnlStatus").html(a)
}

function selectOnlyFile(b) {
    $('#pnlFileList').find('li').each(function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");
        }
    })
    $(b).not($(this)).removeClass('selected');
    $(b).prop("class", "selected");
    var a = RoxyUtils.GetFilename($(b).attr("data-path"));
    a += " (" + t("Size") + ": " + RoxyUtils.FormatFileSize($(b).attr("data-size"));
    if ($(b).attr("data-w") > 0) {
        a += ", " + t("Dimensions") + ":" + $(b).attr("data-w") + "x" + $(b).attr("data-h")
    }
    a += ")";
    $("#pnlStatus").html(a)
}

function selectFileAll(b) {

    if ($(b).hasClass("selected")) {
        $(b).removeClass("selected");
    } else {
        $(b).prop("class", "selected");
        var a = RoxyUtils.GetFilename($(b).attr("data-path"));
        a += " (" + t("Size") + ": " + RoxyUtils.FormatFileSize($(b).attr("data-size"));
        if ($(b).attr("data-w") > 0) {
            a += ", " + t("Dimensions") + ":" + $(b).attr("data-w") + "x" + $(b).attr("data-h")
        }
        a += ")";
        $("#pnlStatus").html(a)
    }
}

function getLastDir() {

    return RoxyUtils.GetCookie("roxyld")
}

function setLastDir(a) {

    RoxyUtils.SetCookie("roxyld", a, 10)
}

function selectDir(a) {

    var b = Directory.Parse($(a).parent("li").attr("data-path"));
    if (b) {
        b.Select()
    }
}

function startDragDir() { }

function startDragFile() {

    selectFile(this)
}

function dragFileOver() {

    $(this).children("img.dir").attr("src", "images/folder-green.png")
}

function dragFileOut() {

    $("#pnlDirList").find("img.dir").attr("src", "images/folder.png")
}

function makeDragFile(b) {

    var a = new File($(b.target).closest("li").attr("data-path"));
    return '<div class="pnlDragFile" data-path="' + a.fullPath + '"><img src="' + a.bigIcon + '" align="absmiddle">&nbsp;' + a.name + "</div>"
}

function makeDragDir(b) {

    var a = new Directory($(b.target).attr("data-path") ? $(b.target).attr("data-path") : $(b.target).closest("li").attr("data-path"));
    return '<div class="pnlDragDir" data-path="' + a.fullPath + '"><img src="images/folder.png" align="absmiddle">&nbsp;' + a.name + "</div>"
}

function moveDir(f, b, d) {

    var a = Directory.Parse(b.draggable.attr("data-path"));
    var c = Directory.Parse($(d).parent("li").attr("data-path"));
    if (c.fullPath != a.path) {
        a.Move(c.fullPath)
    }
}

function moveFile(g, b, c) {

    var a = new File(b.draggable.attr("data-path"));
    var j = Directory.Parse($(c).parent("li").attr("data-path"));
    var h = Directory.Parse(a.path);
    if (a.path != j.fullPath) {
        a.Move(j.fullPath)
    }
}

function moveObject(b, a) {

    b.stopPropagation();
    if (a.draggable.hasClass("directory")) {
        moveDir(b, a, this)
    } else {
        moveFile(b, a, this)
    }
    dragFileOut()
}

function clickFirstOnEnter(a) {
    debugger
    $("#" + a).unbind("keypress");
    $(".actions input").each(function () {
        this.blur()
    });
    $("#" + a).keypress(function (b) {
        if (b.keyCode == $.ui.keyCode.ENTER) {
            b.stopPropagation();
            $(this).parent().find(".ui-dialog-buttonset button").eq(0).trigger("click")
        }
    })
}

function addDir() {

    var b = getSelectedDir();
    if (!b) {
        return
    }
    clickFirstOnEnter("pnlDirName");
    $("#txtDirName").val("");
    var a = {};
    a[t("CreateDir")] = function () {
        var c = $.trim($("#txtDirName").val());
        if (!c) {
            alert(t("E_MissingDirName"))
        }
        if (b.Create(c)) {
            $("#pnlDirName").dialog("close")
        }
    };
    a[t("Cancel")] = function () {
        $("#pnlDirName").dialog("close")
    };
    $("#pnlDirName").dialog({
        title: t("T_CreateDir"),
        modal: true,
        buttons: a
    })
}
var uploadFileList = new Array();

function showUploadList(b) {

    var a = $("#uploadFilesList");
    a.html("");
    clearFileField();
    for (i = 0; i < b.length; i++) {
        a.append('<div class="fileUpload"><div class="fileName">' + b[i].name + " (" + RoxyUtils.FormatFileSize(b[i].size) + ')<span class="progressPercent"></span><div class="uploadProgress"><div class="stripes"></div></div></div><a class="removeUpload" onclick="removeUpload(' + i + ')"></a></div>')
    }
    if (b.length > 0) {
        $("#btnUpload").button("enable")
    } else {
        $("#btnUpload").button("disable")
    }
}

function listUploadFiles(a) {

    if (!window.FileList) {
        $("#btnUpload").button("enable")
    } else {
        if (a.length > 0) {
            uploadFileList = new Array();
            addUploadFiles(a)
        }
    }
}

function addUploadFiles(a) {

    for (i = 0; i < a.length; i++) {
        uploadFileList.push(a[i])
    }
    showUploadList(uploadFileList)
}

function removeUpload(b) {
    var c = findUploadElement(b);
    c.remove();
    try {
        uploadFileList.splice(b, 1);
        showUploadList(uploadFileList)
    } catch (a) { }
}

function findUploadElement(a) {

    return $("#uploadFilesList .fileUpload:eq(" + (a) + ")")
}

function updateUploadProgress(d, a) {

    var b = findUploadElement(a);
    var c = 99;
    if (d.lengthComputable) {
        c = Math.floor((d.loaded / d.total) * 100)
    }
    if (c > 99) {
        c = 99
    }
    b.find(".uploadProgress").css("width", c + "%");
    b.find(".progressPercent").html(" - " + c + "%")
}

function uploadComplete(b, a) {

    uploadFinished(b, a, "ok")
}

function uploadError(b, a) {

    setUploadError(a);
    uploadFinished(b, a, "error")
}

function setUploadError(a) {

    var b = findUploadElement(a);
    b.find(".uploadProgress").css("width", "100%").addClass("uploadError").removeClass("uploadComplete");
    b.find(".progressPercent").html(' - <span class="error">' + t("E_UploadingFile") + "</span>")
}

function setUploadSuccess(a) {

    var b = findUploadElement(a);
    b.find(".uploadProgress").css("width", "100%").removeClass("uploadError").addClass("uploadComplete");
    b.find(".progressPercent").html(" - 100%")
}

function uploadCanceled(b, a) {

    uploadFinished(b, a, "error")
}

function uploadFinished(g, d, c) {

    var f = findUploadElement(d);
    var a = null;
    try {
        a = JSON.parse(g.target.responseText)
    } catch (b) { }
    if ((a && a.res == "error") || c != "ok") {
        c = "error";
        setUploadError(d)
    } else {
        c = "ok";
        setUploadSuccess(d)
    }
    f.attr("data-ulpoad", c);
    checkUploadResult()
}

function checkUploadResult() {

    var b = $("#uploadFilesList .fileUpload").length;
    var a = $("#uploadFilesList .fileUpload[data-ulpoad]").length;
    var e = $('#uploadFilesList .fileUpload[data-ulpoad="ok"]').length;
    if (a == b) {
        uploadFileList = new Array();
        var c = Directory.Parse($("#hdDir").val());
        c.ListFiles(true);
        $("#btnUpload").button("disable")
    }
}

function fileUpload(e, c) {

    var b = new XMLHttpRequest();
    var a = new FormData();
    var d = findUploadElement(c);
    d.find(".removeUpload").remove();
    a.append("action", "upload");
    a.append("method", "ajax");
    a.append("d", $("#hdDir").attr("value"));
    a.append("files[]", e);
    b.upload.addEventListener("progress", function (f) {
        updateUploadProgress(f, c)
    }, false);
    b.addEventListener("load", function (f) {
        uploadComplete(f, c)
    }, false);
    b.addEventListener("error", function (f) {
        uploadError(f, c)
    }, false);
    b.addEventListener("abort", function (f) {
        uploadCanceled(f, c)
    }, false);
    b.open("POST", RoxyFilemanConf.UPLOAD, true);
    b.setRequestHeader("Accept", "*/*");
    b.send(a);
}

function dropFiles(b, a) {

    if (b && b.dataTransfer && b.dataTransfer.files) {
        addFile();
        if (a) {
            addUploadFiles(b.dataTransfer.files)
        } else {
            listUploadFiles(b.dataTransfer.files)
        }
    } else {
        addFile()
    }
}

function clearFileField(a) {

    if (!a) {
        a = "#fileUploads"
    }
    try {
        $(a).val("");
        $(a).val(null)
    } catch (b) { }
}

function addFileClick() {

    $("#uploadResult").html("");
    showUploadList(new Array());
    addFile()
}

function setAllFile() {
    let a;
    $('#pnlFileList').find('li').each(function () {
        if (!$(this).hasClass("selected")) {
            $(this).addClass("selected");
        }
        $(this).prop("class", "selected");
        a = RoxyUtils.GetFilename($(this).attr("data-path"));
        a += " (" + t("Size") + ": " + RoxyUtils.FormatFileSize($(this).attr("data-size"));
        if ($(this).attr("data-w") > 0) {
            a += ", " + t("Dimensions") + ":" + $(this).attr("data-w") + "x" + $(this).attr("data-h")
        }
        a += ")";
        $("#pnlStatus").html(a);
    })
}

function unSetAllFile() {
    $('#pnlFileList').find('li').each(function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");
        }
    })
}

function addFile() {
    debugger
    clickFirstOnEnter("dlgAddFile");
    $("#uploadResult").html("");
    clearFileField();
    var a = {};
    a[t("Upload")] = {
        id: "btnUpload",
        text: t("Upload"),
        disabled: true,
        click: function () {
            debugger
            if (!$("#fileUploads").val() && (!uploadFileList || uploadFileList.length == 0)) {
                alert(t("E_SelectFiles"))
            } else {
                if (!RoxyFilemanConf.UPLOAD) {
                    alert(t("E_ActionDisabled"))
                } else {
                    if (window.FormData && window.XMLHttpRequest && window.FileList && uploadFileList && uploadFileList.length > 0) {
                        for (i = 0; i < uploadFileList.length; i++) {
                            fileUpload(uploadFileList[i], i)
                        }
                    } else {
                        document.forms.addfile.action = RoxyFilemanConf.UPLOAD;
                        document.forms.addfile.submit()
                    }
                    $("#dlgAddFile").dialog("close")
                }
            }
        }
    };
    a[t("Cancel")] = function () {
        $("#dlgAddFile").dialog("close")
    };
    $("#dlgAddFile").dialog({
        title: t("T_AddFile"),
        modal: true,
        buttons: a,
        width: 400
    })
}

function fileUploaded(a) {

    if (a.res == "ok" && a.msg) {
        $("#dlgAddFile").dialog("close");
        var b = Directory.Parse($("#hdDir").val());
        b.ListFiles(true);
        alert(a.msg)
    } else {
        if (a.res == "ok") {
            $("#dlgAddFile").dialog("close");
            var b = Directory.Parse($("#hdDir").val());
            b.ListFiles(true)
        } else {
            alert(a.msg)
        }
    }
}

function renameDir() {

    var b = getSelectedDir();
    if (!b) {
        return
    }
    if ($('[data-path="' + b.fullPath + '"]').parents("li").length < 1) {
        alert(t("E_CannotRenameRoot"));
        return
    }
    clickFirstOnEnter("pnlDirName");
    $("#txtDirName").val(b.name);
    var a = {};
    a[t("RenameDir")] = function () {
        var c = $.trim($("#txtDirName").val());
        if (!c) {
            alert(t("E_MissingDirName"))
        }
        if (b.Rename(c)) {
            $("#pnlDirName").dialog("close")
        }
    };
    a[t("Cancel")] = function () {
        $("#pnlDirName").dialog("close")
    };
    $("#pnlDirName").dialog({
        title: t("T_RenameDir"),
        modal: true,
        buttons: a
    });
    RoxyUtils.SelectText("txtDirName", 0, new String(b.name).length)
}

function renameFile() {

    var b = getSelectedFile();
    if (!b) {
        return
    }
    clickFirstOnEnter("pnlRenameFile");
    $("#txtFileName").val(b.name);
    var a = {};
    a[t("RenameFile")] = function () {
        var c = $.trim($("#txtFileName").val());
        if (!c) {
            alert("Missing file name")
        } else {
            if (b.Rename(c)) {
                $('li[data-path="' + b.fullPath + '"] .name').text(c);
                $('li[data-path="' + b.fullPath + '"]').attr("data-path", RoxyUtils.MakePath(b.path, c));
                $("#pnlRenameFile").dialog("close")
            }
        }
    };
    a[t("Cancel")] = function () {
        $("#pnlRenameFile").dialog("close")
    };
    $("#pnlRenameFile").dialog({
        title: t("T_RenameFile"),
        modal: true,
        buttons: a
    });
    if (b.name.lastIndexOf(".") > 0) {
        RoxyUtils.SelectText("txtFileName", 0, b.name.lastIndexOf("."))
    }
}

function getSelectedFile() {

    var a = null;
    if ($("#pnlFileList .selected").length > 0) {
        a = new File($("#pnlFileList .selected").attr("data-path"))
    }
    return a
}

function getSelectedDir() {

    var a = null;
    if ($("#pnlDirList .selected")) {
        a = Directory.Parse($("#pnlDirList .selected").closest("li").attr("data-path"))
    }
    return a
}

function deleteDir(a) {

    var b = null;
    if (a) {
        b = Directory.Parse(a)
    } else {
        b = getSelectedDir()
    }
    if (b && confirm(t("Q_DeleteFolder"))) {
        b.Delete()
    }
}

function deleteFile() {

    // var a = getSelectedFile();
    if (confirm(t("Q_DeleteFile"))) {
        var myVals = [];
        $('#pnlFileList .selected').each(function () {
            myVals.push($(this).attr("data-path"));
        });
        if (myVals.length > 0) {

            for (var i = 0; i < myVals.length; i++) {
                var a = new File(myVals[i])
                a.Delete()
            }
        }
    }
}

function previewFile() {

    var a = getSelectedFile();
    if (a) {
        window.open(a.fullPath)
    }
}

function downloadFile() {

    var b = getSelectedFile();
    if (b && RoxyFilemanConf.DOWNLOAD) {
        var a = RoxyUtils.AddParam(RoxyFilemanConf.DOWNLOAD, "f", b.fullPath);
        window.frames.frmUploadFile.location.href = a
    } else {
        if (!RoxyFilemanConf.DOWNLOAD) {
            alert(t("E_ActionDisabled"))
        }
    }
}

function downloadDir() {

    var b = getSelectedDir();
    if (b && RoxyFilemanConf.DOWNLOADDIR) {
        var a = RoxyUtils.AddParam(RoxyFilemanConf.DOWNLOADDIR, "d", b.fullPath);
        window.frames.frmUploadFile.location.href = a
    } else {
        if (!RoxyFilemanConf.DOWNLOAD) {
            alert(t("E_ActionDisabled"))
        }
    }
}

function isApple() {
    var expression = /(Mac|iPhone|iPod|iPad)/i;
    return expression.test(navigator.platform);
}

function isControl(event) { // Returns true if Ctrl or cmd keys were pressed.
    if (isApple()) {
        return event.metaKey;
    }
    return event.ctrlKey; // Windows, Linux, UNIX
}

function isSelecting(event) { // Returns true if Ctrl+a or cmd+a were pressed (all text selection).
    if (isControl(event)) {
        return event.key === 'a';
    }
    return false;
}

function closeMenus(a) {

    if (!a || a == "dir") {
        $("#menuDir").fadeOut()
    }
    if (!a || a == "file") {
        $("#menuFile").fadeOut()
    }
}

function selectFirst() {

    var a = $("#pnlDirList li:first").children("div").first();
    if (a.length > 0) {
        selectDir(a)
    } else {
        window.setTimeout("selectFirst()", 300)
    }
}

function tooltipContent() {

    if ($("#menuFile").is(":visible")) {
        return ""
    }
    var a = "";
    var b = File.Parse($(this).attr("data-path"));
    if ($("#hdViewType").val() == "thumb" && b.IsImage()) {
        a = b.fullPath + '<br><span class="filesize">' + t("Size") + ": " + RoxyUtils.FormatFileSize(b.size) + " " + t("Dimensions") + ": " + b.width + "x" + b.height + "</span>"
    } else {
        if (b.IsImage()) {
            if (RoxyFilemanConf.GENERATETHUMB) {
                imgUrl = RoxyUtils.AddParam(RoxyFilemanConf.GENERATETHUMB, "f", b.fullPath);
                imgUrl = RoxyUtils.AddParam(imgUrl, "width", RoxyFilemanConf.PREVIEW_THUMB_WIDTH);
                imgUrl = RoxyUtils.AddParam(imgUrl, "height", RoxyFilemanConf.PREVIEW_THUMB_HEIGHT)
            } else {
                imgUrl = b.fullPath
            }
            a = '<img src="' + imgUrl + '" class="imgPreview"><br>' + b.name + ' <br><span class="filesize">' + t("Size") + ": " + RoxyUtils.FormatFileSize(b.size) + " " + t("Dimensions") + ": " + b.width + "x" + b.height + "</span>"
        } else {
            a = b.fullPath + ' <span class="filesize">' + t("Size") + ": " + RoxyUtils.FormatFileSize(b.size) + "</span>"
        }
    }
    return a
}

function filterFiles() {
    var b = $("#txtSearch").val();
    $("#pnlSearchNoFiles").hide();
    if ($("#pnlFileList li").length == 0) {
        return
    }
    if (!b) {
        $("#pnlFileList li").show();
        return
    }
    var a = 0;
    $("#pnlFileList li").each(function () {
        var c = $(this).children(".name").text();
        if (c.toLowerCase().indexOf(b.toLowerCase()) > -1) {
            a++;
            $(this).show()
        } else {
            $(this).removeClass("selected");
            $(this).hide()
        }
    });
    if (a == 0) {
        $("#pnlSearchNoFiles").show()
    }
}

function sortFiles() {
    var a = getSelectedDir();
    if (!a) {
        return
    }
    a.ListFiles();
    filterFiles();
    switchView($("#hdViewType").val())
}

function switchView(a) {

    if (a == $("#hdViewType").val()) {
        return
    }
    if (!a) {
        a = $("#hdViewType").val()
    }
    $(".btnView").removeClass("selected");
    if (a == "thumb") {
        $("#pnlFileList .icon").attr("src", "images/blank.gif");
        $("#pnlFileList").addClass("thumbView");
        if ($("#dynStyle").length == 0) {
            $("head").append('<style id="dynStyle" />');
            var b = "ul#pnlFileList.thumbView li{width:" + RoxyFilemanConf.THUMBS_VIEW_WIDTH + "px;}";
            b += "ul#pnlFileList.thumbView li{height:" + (parseInt(RoxyFilemanConf.THUMBS_VIEW_HEIGHT) + 20) + "px;}";
            b += "ul#pnlFileList.thumbView .icon{width:" + RoxyFilemanConf.THUMBS_VIEW_WIDTH + "px;}";
            b += "ul#pnlFileList.thumbView .icon{height:" + RoxyFilemanConf.THUMBS_VIEW_HEIGHT + "px;}";
            $("#dynStyle").html(b)
        }
        $("#pnlFileList li").each(function () {
            var c = $(this).attr("data-icon-big");
            if (RoxyFilemanConf.GENERATETHUMB && RoxyUtils.IsImage($(this).attr("data-path"))) {
                c = RoxyUtils.AddParam(RoxyFilemanConf.GENERATETHUMB, "f", c);
                c = RoxyUtils.AddParam(c, "width", RoxyFilemanConf.THUMBS_VIEW_WIDTH);
                c = RoxyUtils.AddParam(c, "height", RoxyFilemanConf.THUMBS_VIEW_HEIGHT)
            }
            $(this).children(".icon").css("background-image", "url(" + c + ")");
            $(this).tooltip("option", "show", {
                delay: 50
            })
        });
        $("#btnThumbView").addClass("selected")
    } else {
        $("#pnlFileList").removeClass("thumbView");
        $("#pnlFileList li").each(function () {
            $(this).children(".icon").css("background-image", "").attr("src", $(this).attr("data-icon"));
            $(this).tooltip("option", "show", {
                delay: 500
            })
        });
        $("#btnListView").addClass("selected")
    }
    $("#hdViewType").val(a);
    RoxyUtils.SetCookie("roxyview", a, 10)
}
var clipBoard = null;

function Clipboard(b, c) {
    this.action = b;
    this.obj = c
}

function cutDir() {
    var a = getSelectedDir();
    if (a) {
        setClipboard("cut", a);
        a.GetElement().addClass("pale")
    }
}

function copyDir() {
    var a = getSelectedDir();
    if (a) {
        setClipboard("copy", a)
    }
}

function cutFile() {
    var a = getSelectedFile();
    if (a) {
        setClipboard("cut", a);
        a.GetElement().addClass("pale")
    }
}

function copyFile() {
    var a = getSelectedFile();
    if (a) {
        setClipboard("copy", a)
    }
}

function pasteToFiles(b, a) {
    if ($(a).hasClass("pale")) {
        b.stopPropagation();
        return false
    }
    var c = getSelectedDir();
    if (!c) {
        c = Directory.Parse($("#pnlDirList li:first").children("div").first())
    }
    if (c && clipBoard && clipBoard.obj) {
        if (clipBoard.action == "copy") {
            clipBoard.obj.Copy(c.fullPath)
        } else {
            clipBoard.obj.Move(c.fullPath);
            clearClipboard()
        }
    }
    return true
}

function pasteToDirs(b, a) {
    if ($(a).hasClass("pale")) {
        b.stopPropagation();
        return false
    }
    var c = getSelectedDir();
    if (!c) {
        c = Directory.Parse($("#pnlDirList li:first").children("div").first())
    }
    if (clipBoard && c) {
        if (clipBoard.action == "copy") {
            clipBoard.obj.Copy(c.fullPath)
        } else {
            clipBoard.obj.Move(c.fullPath);
            clearClipboard();
            c.ListFiles(true)
        }
    } else {
        alert("error")
    }
    return true
}

function clearClipboard() {
    $("#pnlDirList li").removeClass("pale");
    $("#pnlFileList li").removeClass("pale");
    clipBoard = null;
    $(".paste").addClass("pale")
}

function setClipboard(b, c) {
    clearClipboard();
    if (c) {
        clipBoard = new Clipboard(b, c);
        $(".paste").removeClass("pale")
    }
}

function ResizeLists() {
    let a = $(window).innerHeight() - $("#fileActions .actions").outerHeight() - $(".bottomLine").outerHeight() - 15;
    $(".scrollPane").css("height", a)
}

function removeDisabledActions() {
    if (RoxyFilemanConf.CREATEDIR == "") {
        $("#mnuCreateDir").next().remove();
        $("#mnuCreateDir").remove();
        $("#btnAddDir").remove()
    }
    if (RoxyFilemanConf.DELETEDIR == "") {
        $("#mnuDeleteDir").prev().remove();
        $("#mnuDeleteDir").remove();
        $("#btnDeleteDir").remove()
    }
    if (RoxyFilemanConf.MOVEDIR == "") {
        $("#mnuDirCut").next().remove();
        $("#mnuDirCut").remove()
    }
    if (RoxyFilemanConf.COPYDIR == "") {
        $("#mnuDirCopy").next().remove();
        $("#mnuDirCopy").remove()
    }
    if (RoxyFilemanConf.COPYDIR == "" && RoxyFilemanConf.MOVEDIR == "") {
        $("#mnuDirPaste").next().remove();
        $("#mnuDirPaste").remove()
    }
    if (RoxyFilemanConf.RENAMEDIR == "") {
        $("#mnuRenameDir").next().remove();
        $("#mnuRenameDir").remove();
        $("#btnRenameDir").remove()
    }
    if (RoxyFilemanConf.UPLOAD == "") {
        $("#btnAddFile").remove()
    }
    if (RoxyFilemanConf.DOWNLOAD == "") {
        $("#mnuDownload").next().remove();
        $("#mnuDownload").remove()
    }
    if (RoxyFilemanConf.DOWNLOADDIR == "") {
        $("#mnuDownloadDir").next().remove();
        $("#mnuDownloadDir").remove()
    }
    if (RoxyFilemanConf.DELETEFILE == "") {
        $("#mnuDeleteFile").prev().remove();
        $("#mnuDeleteFile").remove();
        $("#btnDeleteFile").remove()
    }
    if (RoxyFilemanConf.MOVEFILE == "") {
        $("#mnuFileCut").next().remove();
        $("#mnuFileCut").remove()
    }
    if (RoxyFilemanConf.COPYFILE == "") {
        $("#mnuFileCopy").next().remove();
        $("#mnuFileCopy").remove()
    }
    if (RoxyFilemanConf.COPYFILE == "" && RoxyFilemanConf.MOVEFILE == "") {
        $("#mnuFilePaste").next().remove();
        $("#mnuFilePaste").remove()
    }
    if (RoxyFilemanConf.RENAMEFILE == "") {
        $("#mnuRenameFile").next().remove();
        $("#mnuRenameFile").remove();
        $("#btnRenameFile").remove()
    }
}

function getPreselectedFile() {

    var c = RoxyUtils.GetUrlParam("selected");
    if (!c) {
        switch (getFilemanIntegration()) {
            case "ckeditor":
                try {
                    var b = window.opener.CKEDITOR.dialog.getCurrent();
                    c = b.getValueOf("info", (b.getName() == "link" ? "url" : "txtUrl"))
                } catch (a) { }
                break;
            case "tinymce3":
                try {
                    var e = tinyMCEPopup.getWindowArg("window");
                    c = e.document.getElementById(tinyMCEPopup.getWindowArg("input")).value;
                    if (c.indexOf("..") == 0) {
                        c = c.substr(2)
                    }
                } catch (a) { }
                break;
            case "tinymce4":
                try {
                    var e = (window.opener ? window.opener : window.parent);
                    c = e.document.getElementById(RoxyUtils.GetUrlParam("input")).value;
                    if (c.indexOf("..") == 0) {
                        c = c.substr(2)
                    }
                } catch (a) { }
                break;
            default:
                // c = GetSelectedValue();
                break
        }
    }
    if (RoxyFilemanConf.RETURN_URL_PREFIX) {
        var d = RoxyFilemanConf.RETURN_URL_PREFIX;
        if (c.indexOf(d) == 0) {
            if (d.substr(-1) == "/") {
                d = d.substr(0, d.length - 1)
            }
            c = c.substr(d.length)
        }
    }
    return c
}

function initSelection(c) {

    var b = false,
        a = true;
    if (!c) {
        c = getPreselectedFile()
    }
    if (!c && RoxyUtils.ToBool(RoxyFilemanConf.OPEN_LAST_DIR)) {
        c = getLastDir();
        a = false
    }
    if (c) {
        var e = (a ? RoxyUtils.GetPath(c) : c);
        var f = tmp = Directory.Parse(e);
        do {
            if (tmp) {
                tmp.Expand(true);
                b = true
            }
            tmp = Directory.Parse(tmp.path)
        } while (tmp);
        if (f) {
            f.Select(c);
            b = true
        }
    }
    if (!b) {
        selectFirst()
    }
}
$(function () {
    RoxyUtils.LoadConfig();
    var e = new Directory();
    e.LoadAll();
    $("#wraper").show();
    window.setTimeout("initSelection()", 100);
    RoxyUtils.Translate();
    $("body").click(function () {
        closeMenus()
    });
    var a = RoxyUtils.GetCookie("roxyview");
    if (!a) {
        a = RoxyFilemanConf.DEFAULTVIEW
    }
    if (a) {
        switchView(a)
    }
    ResizeLists();
    $(".actions input").tooltip({
        track: true
    });
    $(window).resize(ResizeLists);
    document.oncontextmenu = function () {
        return false
    };
    removeDisabledActions();
    $("#copyYear").html(new Date().getFullYear());
    if (RoxyFilemanConf.UPLOAD && RoxyFilemanConf.UPLOAD != "") {
        var c = document.getElementById("fileActions");
        c.ondragover = function () {
            return false
        };
        c.ondragend = function () {
            return false
        };
        c.ondrop = function (d) {
            d.preventDefault();
            d.stopPropagation();
            dropFiles(d)
        };
        c = document.getElementById("dlgAddFile");
        c.ondragover = function () {
            return false
        };
        c.ondragend = function () {
            return false
        };
        c.ondrop = function (d) {
            d.preventDefault();
            d.stopPropagation();
            dropFiles(d, true)
        }
    }
    if (getFilemanIntegration() == "tinymce3") {
        try {
            $("body").append('<script src="js/tiny_mce_popup.js"><\/script>')
        } catch (b) { }
    }
});

function getFilemanIntegration() {

    var a = RoxyUtils.GetUrlParam("integration");
    if (!a) {
        a = RoxyFilemanConf.INTEGRATION
    }
    return a.toLowerCase()
}

function setFile() {
    var myVals = [];
    $('#pnlFileList .selected').each(function () {
        myVals.push($(this).attr("data-path"));
    });
    if (myVals.length > 0) {
        for (var i = 0; i < myVals.length; i++) {
            var c = new File(myVals[i])
            //  = getSelectedFile();
            if (!c) {
                alert(t("E_NoFileSelected"));
                return
            }
            var a = c.fullPath;
            if (RoxyFilemanConf.RETURN_URL_PREFIX) {
                var b = RoxyFilemanConf.RETURN_URL_PREFIX;
                if (b.substr(-1) == "/") {
                    b = b.substr(0, b.length - 1)
                }
                a = b + (a.substr(0, 1) != "/" ? "/" : "") + a
            }
            switch (getFilemanIntegration()) {
                case "ckeditor":
                    window.opener.CKEDITOR.tools.callFunction(RoxyUtils.GetUrlParam("CKEditorFuncNum"), a);
                    self.close();
                    break;
                case "tinymce3":
                    var d = tinyMCEPopup.getWindowArg("window");
                    d.document.getElementById(tinyMCEPopup.getWindowArg("input")).value = a;
                    if (typeof (d.ImageDialog) != "undefined") {
                        if (d.ImageDialog.getImageData) {
                            d.ImageDialog.getImageData()
                        }
                        if (d.ImageDialog.showPreviewImage) {
                            d.ImageDialog.showPreviewImage(a)
                        }
                    }
                    tinyMCEPopup.close();
                    break;
                case "tinymce4":
                    var d = (window.opener ? window.opener : window.parent);
                    d.document.getElementById(RoxyUtils.GetUrlParam("input")).value = a;
                    if (typeof (d.ImageDialog) != "undefined") {
                        if (d.ImageDialog.getImageData) {
                            d.ImageDialog.getImageData()
                        }
                        if (d.ImageDialog.showPreviewImage) {
                            d.ImageDialog.showPreviewImage(a)
                        }
                    }
                    d.tinyMCE.activeEditor.windowManager.close();
                    break;
                default:
                    FileSelected(c);
                    break
            }
        }
    }
};