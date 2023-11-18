$(function () {
    paging();
});
function DateVi(id) { $(id).datetimepicker({ format: 'd/m/Y', timepicker: false }); }
function enterSubmit(id) {
    var idView = id + ' input';
    $(idView).keypress(function (e) {
        if (e.which == 13) {
            $(id).submit();
        }
    });
} //entersubmit

function loadAjax(urlContent, container) {
    $(container).html("Đang tải dữ liệu...");
    $.ajax({
        url: encodeURI(urlContent),
        cache: false,
        type: "POST",
        success: function (data) {
            $(container).html(data);
        }
    });
}//load ajax

function loadAjaxAppend(urlContent, container) {
    $.ajax({
        url: encodeURI(urlContent),
        cache: false,
        type: "POST",
        success: function (data) {
            if (data != '') {
                $(container).append(data);
            }
        }
    });
} //append html
function initAjaxLoad(urlListsLoad, container, callback) {
    //alert(container);
    $.address.unbind().externalChange(function (event) {
        var urlTransform = urlListsLoad;
        var urlHistory = event.value;
        if (urlHistory.length > 0) {
            urlHistory = urlHistory.substring(1, urlHistory.length);
            if (urlTransform.indexOf('?') > 0)
                urlTransform = urlTransform + '&' + urlHistory;
            else
                urlTransform = urlTransform + '?' + urlHistory;
        }
        //alert(urlTransform);
        $(container).html("Đang tải dữ liệu...");
        $.post(urlTransform, function (data) {
            //alert(data);
            $(container).html(data);
        }).complete(function () {
            if (callback && typeof (callback) === "function") {
                callback();
            }
        });
    });
}//load ajax grid
function Post(urlPostAction, fromId) {
    $.post(urlPostAction, $(fromId).serialize(), function (data) {
        if (data.errors) {
            swal({
                title: "",
                text: data.message,
                type: "success",
                showConfirmButton: true,
                animation: false
            });
        } else {
            swal({
                title: "",
                text: data.message,
                type: "success",
                showConfirmButton: true,
                animation: false,
                timer: 2000
            }, function () {
                if (data.url != null) {
                    window.location.href = data.url;
                } else {
                    window.location.reload();
                }
            });
        }
    });
} //post

//chuyển đổi khi format
function formatPriceChange() {
    $(".formatPrice").change(function () {
        var valNow = $(this);

        var unit = $(this).attr('unit');
        if (unit == undefined) {
            unit = "";
        }
        var amount = Number(valNow.val()).toLocaleString('en', { maximumSignificantDigits: 21 });
        if ($(this).parent().find('.lbFormatPrice').length != 0) {
            var removeThis = $(this).parent().find('.lbFormatPrice');
            removeThis.remove();
        }

        valNow.before("<lable class='lbFormatPrice' style='color:#ed1f24'>" + amount + " " + unit + " </label>");
    });
} //format price khi chuyển đổi.
function loadingBtn() {
    $('.loading_btn').click(function () {
        $('body').loading({
            stoppable: false,
            message: '<img src="Images/ajax-loading-gif-3.gif" alt=""><p>Loadding</p>',
            theme: 'dark',

        });
    });
}//Hiển thị loading khi click.
function paging() {
    $(".pagingData").each(function () {
        var link = $(this).attr("href");
        if (link.indexOf("http") > -1) {
            return;
        }
        var val = link.replaceAll("#page=", "");
        link = replaceUrlParam(window.location.href, "page", val);
        $(this).attr("href", link);
    });
} //Load lại phân trang.
function showHiden() {
    $(".showHiden").click(function () {
        var dataid = $(this).parents('.showHidenParent').data('name');
        $(this).parents('.showHidenParent[data-name="' + dataid + '"]').addClass('hidden');
        var idOpen = $(this).data('open');
        $("." + idOpen).removeClass("hidden");
    });
    $(".btnResetShowHide").click(function () {
        window.location.reload();
    });
} //Hiển thị và ẩn nội dung
function select2() {
    $(".js-select2").select2({
        placeholder: "Vui lòng chọn",
        allowClear: true,
    });
} // View select2
function viewYoutube() {
    $("a.videoYoutube").click(function (e) {

        try {
            var c = $(this).attr("href");
            $(this).attr("href", "javascript:void(0)");

            try {
                $('iframe.videoYoutube').each(function () {
                    var thisNow = $(this);
                    var c = thisNow[0].outerHTML;
                    var b = /^.*((youtube\/)|(v\/)|(\/u\/\w\/)|(embed\/)|(watch\?))\??v?=?([^#\&\?]*).*/;
                    var a = c.match(b);
                    var x = Math.floor((Math.random() * 10) + 1);
                    if (a && a[7].length == 11) {
                        var linkImage = "https://img.youtube.com/vi/" + a[7] + "/0.jpg?v=" + x;
                        var html = '<a class="videoYoutube" href="https://www.youtube.com/watch?v=' + a[7] + '" data-img="' + linkImage + '">';
                        html += ' <img src="' + linkImage + '" alt="" style="opacity: 1;"></a><script> viewYoutube();</script>';
                        thisNow.parents(".video").html(html);

                        //
                        //$(this).attr("data-img", link);
                        //$(this).children("img").attr("src", link);
                        //$(this).children("img").attr("data-src", link);
                    }
                    //if (thisNow[0].outerHTML.indexOf("autoplay=1") > -1) {
                    //    var html = thisNow[0].outerHTML.replaceAll("autoplay=1", "");
                    //    thisNow.parents(".video").html(html);
                    //}

                    // $(this)[0].contentWindow.postMessage('{"event":"command","func":"' + 'stopVideo' + '","args":""}', '*');
                });
            } catch (ex) { }

            var b = /^.*((youtu.be\/)|(v\/)|(\/u\/\w\/)|(embed\/)|(watch\?))\??v?=?([^#\&\?]*).*/;
            var a = c.match(b);
            if (a && a[7].length == 11) {
                var link = "https://www.youtube.com/embed/" + a[7] + "?rel=0&controls=0&showinfo=0&autoplay=1";
                var width = $(this).parents(".video").width();
                var height = $(this).parents(".video").height();
                $(this).parents(".video").html('<iframe class="videoYoutube" src="' +
                    link +
                    '" frameborder="0" width="' + width + '" height="' + height + '" allowfullscreen></iframe>');
            }
        } catch (err) {
        }
        e.preventdefault();
    });
} // view video youtube
function replaceUrlParam(url, paramName, paramValue) {
    if (paramValue == null) {
        paramValue = '';
    }
   
    var pattern = new RegExp('\\b(' + paramName + '=).*?(&|$)');
    if (url.search(pattern) >= 0) {
        return url.replace(pattern, '$1' + paramValue + '$2');
    }
    url = url.replace(/\?$/, '');
    return url + (url.indexOf('?') > 0 ? '&' : '?') + paramName + '=' + paramValue;
}//replace giá trị param
function loadEmoticonize() {
    $(".content_chatmess").each(function () {
        $(this).html($(this).html()).emoticonize(true).show();
    });
    $("br").remove();
} //icon chat
function removeObject() {
    $(".removeObject").click(function () {
        var thisRemove = $(this);
        swal({
            title: "",
            text: "Bạn chắc chắn muốn xóa?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "OK",
            closeOnConfirm: false
        },
            function (isConfirm) {
                if (isConfirm) {
                    thisRemove.parents(".removeParent").remove();
                }
                swal.close();
            });

    });
} // xóa trong thẻ div
function formatPrice(val) {
    var amount = Number(val).toLocaleString('en', { maximumSignificantDigits: 21 });
    return amount;
} //format số
function initAjaxLoad(urlListsLoad, container) {
    var imageLoading = "Đang tải dữ liệu...";
    $.address.init().change(function (event) {
        var urlTransform = urlListsLoad;
        var urlHistory = event.value;
        if (urlHistory.length > 0) {
            urlHistory = urlHistory.substring(1, urlHistory.length);
            if (urlTransform.indexOf('?') > 0)
                urlTransform = urlTransform + '&' + urlHistory;
            else
                urlTransform = urlTransform + '?' + urlHistory;
        }
        $(container).html(imageLoading);
        $.post(urlTransform, function (data) {
            $(container).html(data);
        });
    });
} //load dữ liêu ajax
Array.prototype.remove = function () {
    var what, a = arguments, L = a.length, ax;
    while (L && this.length) {
        what = a[--L];
        while ((ax = this.indexOf(what)) !== -1) {
            this.splice(ax, 1);
        }
    }
    return this;
}; // remove
function removeTag(id, idRemove, idListRemove) {
    var arrId = $('#' + idRemove).val();
    if (arrId != '') {
        var arrIds = arrId.split(',');
        arrIds.remove(id);
        $('#' + idRemove).val(arrIds.toString());
    }
    $("#" + idListRemove + " span[id='" + id + "']").parent().remove();
} // remove liên quan
function initAutoComplete(tagControls, urlRouters, id, idList) {

    $("#" + tagControls).keypress(function (e) {
        if (e.keyCode == 13) {
            addValues(tagControls, $(this).val(), urlRouters + "?do=Add&KeyID=" + labelKey, labelKey);
            return false;
        }
        $('#' + tagControls).autocomplete({
            serviceUrl: urlRouters,
            minChars: 1,
            maxHeight: 400,
            width: 500,
            onSelect: function (el) {
                var arrId = $('#' + id).val();
                var __arrID = [];
                if (arrId.length > 0)
                    __arrID = arrId.split(',');
                __arrID.push(el.IDs);
                $('#' + tagControls).val("");
                $("#" + id).val(__arrID.join(","));
                var _product = "<li><span title=" + el.value + " id=" + el.IDs + ">" + el.value + " " + el.code + "</span><a title=\"Xóa: " + el.value + "\" href=\"javascript:removeTag(" + el.IDs + ",'" + id + "','" + idList + "');\"><img border=\"0\" src=\"/Content/Admin/Images/gridview/act_filedelete.png\"></a></li>";
                $('#' + idList).append(_product);
            },
            transformResult: function (response) {
                var obj = JSON.parse(response);
                return {
                    suggestions: $.map(obj.Data, function (dataItem) {
                        return dataItem;
                    })
                };
            }
        });
    });
} // autocomplete

String.prototype.replaceAll = function (find, replace) {
    var str = this;
    return str.replace(new RegExp(find.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&'), 'g'), replace);
};
function ValidateRequired(btnId) {
    var listValid = $(".required").toArray();
    var val, title;
    for (var i = 0; i < listValid.length; i++) {
        val = listValid[i].value.trim();
        if (listValid[i].title != undefined && listValid[i].title != '') {
            title = listValid[i].title;
        } else {
            title = "Trường này";
        }

        if (val == '') {
            var n = noty({ text: '<div class="activity-item"> ' + title + ' bắt buộc nhập.</div>', type: "error" });
            $(btnId).attr("disabled", false);
            return false;
        }
    }
    return true;
}
function isURL(c, id) {
    var b = /^.*((youtu.be\/)|(v\/)|(\/u\/\w\/)|(embed\/)|(watch\?))\??v?=?([^#\&\?]*).*/;
    var a = c.match(b);
    if (a && a[7].length == 11) {
        var link = "https://www.youtube.com/embed/" + a[7] + "?rel=0&controls=0&showinfo=0";
        if (id != undefined && id != '') {
            $(id).html('<iframe id="video-url" width="187" height="120" src="' +
                link +
                '" frameborder="0" allowfullscreen></iframe>');
        } else {
            $(this).html('<iframe id="video-url" width="187" height="120" src="' +
                link +
                '" frameborder="0" allowfullscreen></iframe>');

        }
    }
}
function getValueFormMutilSelect(form) {
    var arrParam = '';
    var idMselect;
    $(form).find("input[type='checkbox']:checked, input[type='radio']:checked, input[type='text'],input[type='number'], input[type='hidden'], select").each(function () {
        idMselect = $(this).attr("name");
        if ($(this).val() != '' && $(this).val() != '')
            arrParam += "&" + idMselect + "=" + $(this).val();
    });
    if (arrParam != '')
        arrParam = arrParam.substring(1);
    return arrParam;
}
function getValueMutilSelect(selectName) {
    var arrID = '';
    $.each($('select.mutil').multiSelect("getSelects"), function (key, value) {
        arrID += value + ",";
    });
    arrID = (arrID.length > 0) ? arrID.substring(0, arrID.length - 1) : arrID;
    return arrID;
}
function copyText(btnId, textId) {
    var copyTextareaBtn = document.querySelector(btnId);

    copyTextareaBtn.addEventListener('click', function (event) {
        var copyTextarea = document.querySelector(textId);
        copyTextarea.select();

        try {
            var successful = document.execCommand('copy');
            var msg = successful ? 'successful' : 'unsuccessful';
            console.log('Copying text command was ' + msg);
        } catch (err) {
            console.log('Oops, unable to copy');
        }
    });
}
function ReplaceSpencail(val) {
    val = val.replaceAll(",", "^2c");
    return val;
}

function ReplaceUnSpencail(val) {
    val = val.replaceAll(",", "^2c");
    return val;
}
function SpencailSubmitForm() {
    $('form input').each(function () {
        if (this.value != undefined && this.value != ''
            && this.value.toString().indexOf(',') > -1) {
            this.value = ReplaceSpencail(this.value);
        }

    });
    $('form textarea').each(function () {

        if (this.value != undefined && this.value != '' && this.value.toString().indexOf(',') > -1) {
            this.value = ReplaceSpencail(this.value);
        }
    });
}
function UnSpencailSubmitForm() {
    $('form input').each(function () {
        if (this.value != undefined &&
            this.value != '' &&
            this.value.toString().indexOf(',') > -1) {
            this.value = ReplaceUnSpencail(this.value);
        }
    });
    $('form textarea').each(function () {
        if (this.value != undefined && this.value != '' && this.value.toString().indexOf(',') > -1) {
            this.value = ReplaceUnSpencail(this.value);
        }
    });
}

function AutoSetValueForReviewForm(frmfrom) {

    var value = '';
    $(frmfrom).find("input[type=text],input[type=number],input[type=radio]:checked,textarea,select").each(function () {
        if ($(this).attr("type") == "radio") {
            value = $(this).attr("rel");
        } else if ($(this).is("select")) {
            value = $(this).find('option:selected').text();
        } else {
            value = $(this).val();
        }
        if (value != '')
            $('#spl_' + $(this).attr("name")).text(value);
    });
    //$('input[type=text]').on('keyup', function (e) {
    //    e.preventDefault();
    //    $('#spl_'+$(this).attr("id")).text($(this).val());
    //});

}
function loadEmoticonize() {
    $(".content_chatmess").each(function () {
        $(this).html($(this).html()).emoticonize(true).show();
    });
    $("br").remove();
}




