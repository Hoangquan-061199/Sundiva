function ValidInput() {
    $('input[type=text]:not(.notvalid)').on('keyup', function (e) {
        e.preventDefault();
        var old = $(this).val();
        var newval = removeTagHTML(old);
        $(this).val(newval);
    });
    $('textarea.valid').on('keyup', function (e) {
        e.preventDefault();
        var old = $(this).val();
        var newval = removeTagHTML(old);
        $(this).val(newval);
    });
    $('input.title').on('keyup', function (e) {
        e.preventDefault();
        var old = $(this).val();
        var newval = removeTagHTML(old);
        newval = newval.replace(/\$|\{|\}|@|\^|\*|\+|\=|\;|\'|\#|~|$|`/g, "");
        newval = newval.replace(/-+-/g, "");
        $(this).val(newval);
    });
    $('input.name').on('keyup', function (e) {
        e.preventDefault();
        var old = $(this).val();
        var newval = removeTagHTML(old);
        newval = newval.replace(/!|\$|\\|\{|\}|\||@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\:|\;|\'|\"|\&|\#|\[|\]|~|$|_|”|“|`/g, "");
        newval = newval.replace(/-+-/g, "");
        $(this).val(newval);
    });
    $('input.code').on('keyup', function (e) {
        e.preventDefault();
        var old = $(this).val();
        var newval = removeTagHTML(old);
        newval = newval.replace(/[^\w\s]/g, "");
        newval = newval.replace(/-+-/g, "").replace(" ", "");
        $(this).val(newval);
    });
    $('input.code').on('keyup', function (e) {
        e.preventDefault();
        var old = $(this).val();
        var newval = removeTagHTML(old);
        newval = newval.replace(/[^\w\s]/g, "");
        newval = newval.replace(/-+-/g, "").replace(" ", "");
        $(this).val(newval);
    });
    $('input.link').on('keyup', function (e) {
        e.preventDefault();
        var old = $(this).val();
        var newval = removeTagHTML(old);
        newval = newval.replace(/!|\$|\\|\{|\@|\}|\^|\*|\(|\)|\+|\<|\>|,|\;|\'|\"|\[|\]|~|$|”|“|`|®/g, "");
        //newval = newval.replace(/-+-/g, "");
        $(this).val(newval);
    });
    $('input.number').on('keyup', function (e) {
        e.preventDefault();
        var old = $(this).val();
        var newval = removeTagHTML(old);
        newval = newval.replace(/[^0-9^.]/g, "");
        newval = newval.replace(/-+-/g, "").replace(" ", "");
        $(this).val(newval);
    });
}

function removeTagHTML(str) {
    if ((str === null) || (str === ''))
        return '';
    else
        str = str.toString();
    // Regular expression to identify HTML tags in 
    // the input string. Replacing the identified 
    // HTML tag with a null string.
    return str.replace(/(<([^>]+)>)/ig, '');
}

function removeSpecialText(str) {
    str = str.replace(/\$|\\|\{|\}|\||@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|\:|\;|\'|\"|\&|\#|\[|\]|~|$|”|“|`/g, "");
    str = str.replace(/-+-/g, "");
    return str;
}

function ShowGridProduct(str) {
    var selected = $('#' + str + 'Ids').val();
    loadAjax('/Adminadc/Product/ListItemsAjax?code=' + str + '&ids=' + selected, "#AjaxGrid" + str)
}

function ShowGridContent(str) {
    var selected = $('#' + str + 'Ids').val();
    loadAjax('/Adminadc/WebsiteContent/ListItemsAjax?code=' + str + '&ids=' + selected, "#AjaxGrid" + str)
}

function ShowGridContentNews(str) {
    var selected = $('#' + str + 'Ids').val();
    loadAjax('/Adminadc/WebsiteContent/ListItemsAjax?code=' + str + '&ids=' + selected, "#AjaxGrid" + str)
}

function ShowGridContentDocument(str) {
    var selected = $('#' + str + 'Ids').val();
    loadAjax('/Adminadc/WebsiteContent/ListItemsDocumentAjax?code=' + str + '&ids=' + selected, "#AjaxGrid" + str)
}

function ShowGridTag(str) {
    var selected = $('#' + str + 's').val();
    loadAjax('/Adminadc/SystemTag/ListItemsAjax?code=' + str + '&ids=' + selected, "#AjaxGrid" + str)
}

function HideGridProduct(str) {
    $("#AjaxGrid" + str).html('');
}

function containsObject(obj, list) {
    var i;
    for (i = 0; i < list.length; i++) {
        if (list[i] == obj) {
            return true;
        }
    }
    return false;
}

function removeElement(array, elem) {
    var index = array.indexOf(elem);
    if (index > -1) {
        array.splice(index, 1);
    }
}
$('.IsAvatar').change(function () {
    var src = $(this).data('url');
    $('#Avatar').val(src);
    $('#AddAvatar').find('input').val(src);
    $('#AddAvatar').find('img').attr('src', src);
});

function isEmpty(val) { return val === undefined || val == null || val.length <= 0 ? true : false; }

function updateDateDDMMYYYY() {
    $(".dateddmmyyy").each(function () {
        var today = $(this).val();
        var name = $(this).attr("id");
        var parts = today.split("/");
        var now = parts[1] + "/" + parts[0] + "/" + parts[2];
        if ($('input[name=' + name + ']').length == 0) {
            $(this).before('<input type="hidden" value="' + now + '" name="' + name + '"/>');
        }
        else {
            $('input[name=' + name + ']').val(now);
        }
    });
}

function DateVi(id) { $(id).datetimepicker({ format: 'd/m/Y', timepicker: false }); }

function DateTimeVi(id) { $(id).datetimepicker({ format: 'd/m/Y H:i', timepicker: true }); }

function DateViNow(id) { $(id).datetimepicker({ format: 'd/m/Y', timepicker: false, minDate: 0}); }

function LoadAgency() {
    $.getJSON("/DataJson/AgencyData.json", function (data) {
        var agencyid = $('#ValueAgency').val();
        var items = [];
        items.push("<option value=''>Tất cả</li>");
        $.each(data, function (name) {
            if (data[name].ID == agencyid) {
                items.push("<option value='" + data[name].ID + "' selected>" + data[name].Name + "</li>");
            } else {
                items.push("<option value='" + data[name].ID + "'>" + data[name].Name + "</li>");
            }
        });
        $('#Agency').html(items.join(""));
    });
}

function Count() {
    $('.count').each(function () {
        var attr = $(this).attr('name');
        var v = $(this).val();
        var w = CountWord(v);
        $('.Val-' + attr + ' span').html("<b>" + v.length + "</b> ký tự, <b>" + w + "</b> từ");
    });
    $('.count').keyup(function () {
        var attr = $(this).attr('name');
        var v = $(this).val();
        var w = CountWord(v);
        $('.Val-' + attr + ' span').html("<b>" + v.length + "</b> ký tự, <b>" + w + "</b> từ");
    });
}

function CountWord(str) {
    if (str == '' && str != undefined)
        return 0;
    return str.split(' ').length;
}

function Post(urlPostAction, fromId) {
    $('.loading_admin').addClass('active');
    $.post(urlPostAction, $(fromId).serialize(), function (data) {
        if (data.errors) {
            swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
            $('.loading_admin').removeClass('active');
        } else {
            $('.loading_admin').removeClass('active');
            if (data.url != null) {
                window.location.href = data.url;
            } else {
                window.location.reload();
            }
        }
    }).fail(function () {
        swal({ title: "Thông báo", text: "Xử lý thất bại!", type: "error", showConfirmButton: true, animation: false });
        $('.loading_admin').removeClass('active');
    });
}

function PostWithAlert(urlPostAction, urlForm, fromId) {
    $('.loading_admin').addClass('active');
    $.post(urlPostAction, $(fromId).serialize(), function (data) {
        if (data.errors) {
            swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
            $('.loading_admin').removeClass('active');
        } else {
            var url = urlForm + "?Do=Edit&ItemId=" + data.obj.id;
            if (urlForm.indexOf("?") > -1) {
                url = urlForm + "&Do=Edit&ItemId=" + data.obj.id;
            }
            $('.loading_admin').removeClass('active');
            loadAjax(url, "#tab_add");
            $.growl.notice({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
        }
    }).fail(function () {
        swal({ title: "Thông báo", text: "Xử lý thất bại!", type: "error", showConfirmButton: true, animation: false });
        $('.loading_admin').removeClass('active');
    });
}

function PostAjaxQuickAttr(urlPostAction, fromId, elemetResult) {
    $.post(urlPostAction, $(fromId).serialize(), function (data) {
        if (data.errors) { swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false }); } else {
            var htmlAppend = `
                <div class="item-attr changeUrlTinyMceParent col-sm-6"
                    style="height:175px;margin-bottom:15px;padding-bottom: 10px;padding-top:15px;border-top:1px solid #cdcdcd;border-bottom: 1px solid #cdcdcd; border-right: 1px solid #cdcdcd">
                    <div class="form-group">
                        <div class="col-sm-1">
                            <label><input checked type="checkbox" class="child-attr" name="AttributeProductIds" style="margin-right:5px;"
                                    value="${data.id}"/></label>
                            Tên
                        </div>
                        <div class="col-sm-5">
                            <input type="text" name="AttributeName_${data.id}" class="form-control"
                                value="${data.name}" placeholder="Tên" />
                        </div>
                        <div class="col-sm-1">
                            <label>
                                Màu
                            </label>
                        </div>
                        <div class="col-sm-5">
                            <input type="text" name="AttributeColor_${data.id}" class="form-control" value=""
                                placeholder="Mã màu" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-1">
                            <label>
                                Ảnh
                            </label>
                        </div>
                        <div class="col-sm-5">
                            <div class="input-group " style="margin-top:5px;">
                                <input type="text" class="changeUrlTinyMce link form-control"
                                    onchange="ChangeUrlTinyMce($(this),'AttributeUrlPicture_${data.id}', 'AttributeUrlPicture_${data.id}',0)"
                                    value="" placeholder="Ảnh" />
                                <span class="input-group-addon"><button type="button"
                                        onclick="SelectFileTyniMce('AttributeUrlPicture_${data.id}','AttributeUrlPicture_${data.id}',0);"
                                        class="btn btn-info btn-sm"><i class="lnr lnr-upload"></i> ảnh</button></span>
                            </div>
                            <div id="AttributeUrlPicture_${data.id}">
                            </div>
                        </div>
                        <div class="col-sm-1">
                            <label>
                                Thứ tự
                            </label>
                        </div>
                        <div class="col-sm-5">
                            <input type="text" name="AttributeOrderDisplay_${data.id}" class="form-control" value=""
                                placeholder="Thứ tự" />
                        </div>
                    </div>
                </div>
                `;
            $(elemetResult).append(htmlAppend)
            $(".child-attr").change(function () {
                var parrentE = $(this).parents(".parent-ittr-select");
                var id = $(this).val();
                if ($(this).is(':checked')) {
                    parrentE.find(".parr-attr").prop('checked', true);
                    $('input[name=AttributePrice_' + id + ']').removeAttr('disabled');
                } else {
                    var checked = 0;
                    parrentE.find(".child-attr").each(function () {
                        if ($(this).is(':checked')) {
                            checked = 1;
                            return;
                        }
                    })
                    if (checked == 0) {
                        parrentE.find(".parr-attr").prop('checked', false);
                    } else {
                        parrentE.find(".parr-attr").prop('checked', true);
                    }
                    $('input[name=AttributePrice_' + id + ']').attr('disabled', 'disabled');
                }
            })
            ModalADC.Close();

        }
    });
}

function PostAjaxQuickSub(urlPostAction, fromId, elemetResult) {
    $.post(urlPostAction, $(fromId).serialize(), function (data) {
        if (data.errors) { swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false }); } else {
            if (data.action == 'Add') {
                let htmlAppend = `
                 <div class="col-sm-3 item-sub-${data.id}">
                    <div class="editsub edit-subitem-quick" style="padding: 10px; border: 1px solid #cdcdcd; display: flex; margin-bottom: 15px;" >
                        <span class="text-line-1">
                            ${data.name}
                        </span>
                        <span style="margin-left: auto">
                            <a href="#" class="lnr lnr-pencil" onclick="editSubItemQuick(${data.productId}, ${data.id})" title="sửa"></a>
                            <a href="#" class="lnr lnr-trash" onclick="deleteSubItemQuick(${data.id})" title="xoá"></a>
                        </span>
                    </div>
                </div>
                `;
                $(elemetResult).append(htmlAppend)
            }
            else {
                let htmlAppend = `
                     <div class="editsub edit-subitem-quick" style="padding: 10px; border: 1px solid #cdcdcd; display: flex; margin-bottom: 15px;" >
                        <span class="text-line-1">
                            ${data.name}
                        </span>
                        <span style="margin-left: auto">
                            <a href="#" class="lnr lnr-pencil" onclick="editSubItemQuick(${data.productId}, ${data.id})" title="sửa"></a>
                            <a href="#" class="lnr lnr-trash" onclick="deleteSubItemQuick(${data.id})" title="xoá"></a>
                        </span>
                    </div>
                `;
                $('.col-sm-3.item-sub-' + data.id).html(htmlAppend);
            }
            ModalADC.Close();
        }
    });
}
function editSubItemQuick(productid, idsub) {
    ModalADC.Open({
        title: "Sửa nội dung phụ nhanh",
        urlLoad: '/adminadc/SubItem/QuickAdd?productId=' + productid + '&idSub=' + idsub,
        bottom: false
    });
}
function deleteSubItemQuick(itemId) {
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
                $.post("/adminadc/SubItem/Actions?", "Do=Delete&ItemId=" + itemId, function (data) {
                    if (data.errors == false) {
                        swal({
                            title: "Thông báo",
                            text: data.message,
                            type: "success",
                            showConfirmButton: true,
                            animation: false,
                            timer: 2000
                        },
                            function () {
                                $('.col-sm-3.item-sub-' + itemId).remove();
                            });

                    } else {
                        swal({
                            title: "Thông báo",
                            text: data.message,
                            type: "error",
                            showConfirmButton: true,
                            animation: false
                        });
                        $("#btnOrder").attr("disabled", false);
                    }
                });
            }
        });
    return false;
}
(function ($) {
    $.fn.extend({
        donetyping: function (callback, timeout) {
            timeout = timeout || 1e3; // 1 second default timeout
            var timeoutReference,
                doneTyping = function (el) {
                    if (!timeoutReference) return;
                    timeoutReference = null;
                    callback.call(el);
                };
            return this.each(function (i, el) {
                var $el = $(el);
                $el.is(':input') && $el.on('keyup keypress paste', function (e) {
                    if (e.type == 'keyup' && e.keyCode != 8) return;
                    if (timeoutReference) clearTimeout(timeoutReference);
                    timeoutReference = setTimeout(function () {
                        doneTyping(el);
                    }, timeout);
                }).on('blur', function () {
                    doneTyping(el);
                });
            });
        }
    });
})(jQuery);

function loadAjax(urlContent, container) {
    $(container).html("Đang tải dữ liệu...");
    $.ajax({
        url: encodeURI(urlContent),
        cache: false,
        type: "GET",
        dataType: 'html',
        success: function (data) {
            $(container).html(data);
        }
    });
}
Array.prototype.remove = function () {
    for (var t, r, e = arguments, i = e.length; i && this.length;)
        for (t = e[--i]; - 1 !== (r = this.indexOf(t));) this.splice(r, 1);
    return this;
}; // remove
function RemoveUnicode(str) {
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'| |\"|\&|\#|\[|\]|~|$|_|\||–|”|“|`/g, "-");
    str = str.replace(/-+-/g, "-"); //thay thế 2- thành 1- 
    str = str.replace(/^\-+|\-+$/g, "");
    return str;
} // chuyển name tới NameAscii
function RemoveToUnicode(str) {
    str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
    str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
    str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    str = str.replace(/Đ/g, "D");
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'| |\"|\&|\#|\[|\]|~|$|_|–|”|“|`/g, "");
    str = str.replace(/-+-/g, ""); //thay thế 2- thành 1- 
    str = str.replace(/^\-+|\-+$/g, "");
    return str;
}

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
        valNow.parents('.input-group').next().html("<lable class='lbFormatPrice' style='color:#ed1f24'>" + amount + " " + unit + " </label>");
    });
} //format price khi chuyển đổi.
function paging() {
    $(".pagingData").each(function () {
        var a = $(this).attr("href"),
            e = a.replaceAll("#page=", "");
        a = replaceUrlParam(window.location.href, "page", e), $(this).attr("href", a);
    });
} //Load lại phân trang.
function showHiden() {
    $(".showHiden").click(function () {
        var n = $(this).parents(".showHidenParent").data("name");
        $(this).parents('.showHidenParent[data-name="' + n + '"]').addClass("hidden");
        var a = $(this).data("open");
        $("." + a).removeClass("hidden");
    }), $(".btnResetShowHide").click(function () { window.location.reload(); });
} //Hiển thị và ẩn nội dung
function select2() { $(".js-select2").select2({ placeholder: "Vui lòng chọn", allowClear: true }); } // View select2
function replaceUrlParam(url, paramName, paramValue) {
    if (paramValue == null) { paramValue = ''; }
    if (paramName == null) { return url; }
    var pattern = new RegExp('\\b(' + paramName + '=).*?(&|$)');
    if (url.search(pattern) >= 0) { return url.replace(pattern, '$1' + paramValue + '$2'); }
    url = url.replace(/\#$/, '');
    return url + (url.indexOf('#') > 0 ? '&' : '#') + paramName + '=' + paramValue;
} //replace giá trị param
function removeObject() {
    $(".removeObject").click(function () {
        var thisRemove = $(this);
        swal({ title: "", text: "Bạn chắc chắn muốn xóa?", type: "warning", showCancelButton: true, confirmButtonClass: "btn-danger", confirmButtonText: "OK", closeOnConfirm: false },
            function (isConfirm) {
                if (isConfirm) {
                    thisRemove.parents(".removeParent").find('input[type=hidden]').val('');
                    thisRemove.parents(".removeParent").remove();
                }
                swal.close();
            });
    });
} // xóa trong thẻ div
function removeObjectClick(thisRemove) {
    swal({ title: "", text: "Bạn chắc chắn muốn xóa?", type: "warning", showCancelButton: true, confirmButtonClass: "btn-danger", confirmButtonText: "OK", closeOnConfirm: false },
        function (isConfirm) {
            if (isConfirm) {
                thisRemove.parents(".removeParent").remove();
            }
            swal.close();
        });
}

function formatPrice(val) { var amount = Number(val).toLocaleString('en', { maximumSignificantDigits: 21 }); return amount; } //format số
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
            setTimeout(
                function () {
                    paging();
                }, 300);
        }).fail(function () {
            $(container).html("Tải dữ liệu thất bại...");
        });
    });
}

function initAjaxLoadGridView(urlListsLoad, container) {
    var imageLoading = "Đang tải dữ liệu...";
    $.address.init(function (event) {
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
}
//load dữ liêu ajax
function removeTag(id, idRemove, idListRemove) {
    var arrId = $('#' + idRemove).val();
    if (arrId != '') {
        var arrIds = arrId.split(',');
        arrIds.remove(id);
        $('#' + idRemove).val(arrIds.toString());
    }
    $("#" + idListRemove + " span[id='" + id + "']").parent().remove();
}
// remove liên quan
function removeRelate(id, idRemove, idListRemove) {
    var arrId = $('#' + idRemove + 'Ids').val();
    if (arrId != '') {
        var arrIds = arrId.split(',');
        arrIds.remove(id.toString());
        $('#' + idRemove + "Ids").val(arrIds.toString());
    }
    $('#' + idListRemove + "Remove" + ' li[data-id=' + id + ']').remove();
    $('#tblContentAjax' + idListRemove).find('input.check[value=' + id + ']').prop("checked", false);
}

function removeTags(id, idRemove, idListRemove) {
    var arrId = $('#' + idRemove + 's').val();
    if (arrId != '') {
        var arrIds = arrId.split(',');
        arrIds.remove(id.toString());
        $('#' + idRemove + "s").val(arrIds.toString());
    }
    $('#' + idListRemove + "Remove" + ' li[data-id=' + id + ']').remove();
    $('#tblContentAjax' + idListRemove).find('input.check[value=' + id + ']').prop("checked", false);
}

function initAutoComplete(tagControls, urlRouters, id, idList) {
    $("#" + tagControls).keydown(function (e) {
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
                __arrID.push(el.data);
                $('#' + tagControls).val("");
                $("#" + id).val(__arrID.join(","));
                var _product = "<li><span title=" + el.value + " id=" + el.data + ">" + el.value + "</span><a title=\"Xóa: " + el.value + "\" href=\"javascript:removeTag(" + el.data + ",'" + id + "','" + idList + "');\"><i class='fa fa-times text-red'></i></a></li>";
                $('#' + idList).append(_product);
            },
            transformResult: function (response) {
                var obj = JSON.parse(response);
                return {
                    suggestions: $.map(obj, function (dataItem) {
                        return dataItem;
                    })
                };
            }
        });
    });
} // autocomplete
function AutoCompleteInput(tagControls, urlRouters, idView, idValue) {
    $(tagControls).autocomplete({
        serviceUrl: urlRouters,
        minLength: 3,
        onSelect: function (el) {
            event.preventDefault();
            $(tagControls).val('');
            $(idView).text(el.value);
            $(idValue).val(el.data);
        },
        transformResult: function (response) {
            var obj = JSON.parse(response);
            return {
                suggestions: $.map(obj, function (dataItem) {
                    return dataItem;
                })
            };
        }
    });
}
//load CKEditor start
config = {};

function LoadCKEDITOR(n, o, h) {
    var i = CKEDITOR.instances[n];
    i && i.destroy(!0), CKEditorConfig(n, o, h);
}

function CKEditorConfig(instanceName, fullEditor, height) {
    config.language = 'vi';
    //config.extraPlugins = 'youtube';
    config.allowedContent = true;
    config.filebrowserBrowseUrl = '/lib/tinymce/index.html?integration=ckeditor&typeview=3';
    config.filebrowserImageBrowseUrl = '/lib/tinymce/index.html?integration=ckeditor&typeview=3';
    config.filebrowserFlashBrowseUrl = '/lib/tinymce/index.html?integration=ckeditor&typeview=3';
    config.filebrowserUploadUrl = '/lib/tinymce/index.html?integration=ckeditor&typeview=3';
    config.filebrowserImageUploadUrl = '/api/TinyMce/UploadImage';
    config.filebrowserFlashUploadUrl = '/lib/tinymce/index.html?integration=ckeditor&typeview=3';
    config.entities_latin = false;
    config.image2_alignClasses = ['image-left', 'image-center', 'image-right'];
    config.image2_captionedClass = 'image-captioned';

    if (fullEditor) {
        config.toolbarGroups = [
            { name: 'document', groups: ['mode', 'document', 'doctools'] },
            { name: 'clipboard', groups: ['clipboard', 'undo'] },
            //{ name: 'forms', groups: ['forms'] },
            { name: 'editing', groups: ['find', 'selection', 'spellchecker', 'editing'] },
            { name: 'links', groups: ['links'] },
            { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
            { name: 'insert', groups: ['insert'] },
            { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi', 'paragraph'] },
            { name: 'styles', groups: ['styles'] },
            { name: 'colors', groups: ['colors'] },
            { name: 'tools', groups: ['tools'] },
            '/',
            { name: 'others', groups: ['others'] }
        ];
        config.extraPlugins = 'image2,youtube,html5audio,html5video,widget,widgetselection,clipboard,lineutils,removeformat';
        //config.removeDialogTabs = 'image:advanced;link:advanced';
        config.removeButtons = 'Radio,TextField,Textarea,Select,Button,HiddenField,ImageButton,Language,PageBreak,CreateDiv,Anchor,Outdent,Indent,Replace,SelectAll,Scayt,Checkbox,Find';
        config.skin = 'office2013';
        config.height = height;
        CKEDITOR.replace(instanceName, config);
    } else {
        config.toolbarGroups = [
            { name: 'document', groups: ['mode', 'document', 'doctools'] },
            { name: 'clipboard', groups: ['clipboard', 'undo'] },
            { name: 'forms', groups: ['forms'] },
            { name: 'editing', groups: ['find', 'selection', 'spellchecker', 'editing'] },
            { name: 'links', groups: ['links'] },
            { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
            { name: 'insert', groups: ['insert'] },
            { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi', 'paragraph'] },
            { name: 'styles', groups: ['styles'] },
            { name: 'colors', groups: ['colors'] },
            { name: 'tools', groups: ['tools'] },
            { name: 'about', groups: ['about'] },
            '/',
            { name: 'others', groups: ['others'] }
        ];
        config.removeButtons = 'Radio,TextField,Textarea,Select,Button,HiddenField,ImageButton,Language,PageBreak,CreateDiv,Anchor,Outdent,Indent,Replace,SelectAll,Scayt,Checkbox,Find,Save,NewPage,Preview,Print,Templates,Subscript,Superscript,CopyFormatting,Image,Flash,Table,HorizontalRule,Smiley,SpecialChar,Iframe,NumberedList,BulletedList,Blockquote,BidiLtr,ShowBlocks,About,Cut,Undo,Redo,Copy,Paste,PasteText,Styles,Font,BGColor,PasteFromWord,BidiRtl';
        CKEDITOR.replace(instanceName, config);
    }
}

function updateEditor() { for (var n in CKEDITOR.instances) CKEDITOR.instances[n].updateElement(); }
//load CKEditor end
function getValueFormMutilSelect(t) { var e, i = ""; return $(t).find("input,textarea,hidden,select").not("input[type='checkbox'], input[type='radio']:checked, input[name='selectItem'], .ms-search input, .mutil").each(function () { e = $(this).attr("name"), "" != $(this).val() && "" != $(this).val() && (i += "&" + e + "=" + $(this).val()); }), "" != i && (i = i.substring(1)), i; }

function copyText(c, o) {
    document.querySelector(c).addEventListener("click", function (c) {
        document.querySelector(o).select();
        try {
            var e = document.execCommand("copy") ? "successful" : "unsuccessful";
            console.log("Copying text command was " + e);
        } catch (c) { console.log("Oops, unable to copy"); }
    });
}

function ReplaceSpencail(val) { val = val.replaceAll(",", "^2c"); return val; } // chuyển ký tự đặc biệt 
function ReplaceUnSpencail(val) { val = val.replaceAll(",", "^2c"); return val; } // lấy lại ký tự đặc biệt 
function SpencailSubmitForm() { $("form input").each(function () { null != this.value && "" != this.value && -1 < this.value.toString().indexOf(",") && (this.value = ReplaceSpencail(this.value)); }), $("form textarea").each(function () { null != this.value && "" != this.value && -1 < this.value.toString().indexOf(",") && (this.value = ReplaceSpencail(this.value)); }); } // chuyển tất cả ký tự đặc biệt trong form
function UnSpencailSubmitForm() { $("form input").each(function () { null != this.value && "" != this.value && -1 < this.value.toString().indexOf(",") && (this.value = ReplaceUnSpencail(this.value)) }), $("form textarea").each(function () { null != this.value && "" != this.value && -1 < this.value.toString().indexOf(",") && (this.value = ReplaceUnSpencail(this.value)); }); } //lấy lại tất cả các ký tự đặc biệt trong form
function AutoSetValueForReviewForm(t) {
    var e = "";
    $(t).find("input[type=text],input[type=number],input[type=radio]:checked,textarea,select").each(function () { "" != (e = "radio" == $(this).attr("type") ? $(this).attr("rel") : $(this).is("select") ? $(this).find("option:selected").text() : $(this).val()) && $("#spl_" + $(this).attr("name")).text(e); });
} //set review dữ liệu
String.prototype.getParamFromUrl = String.prototype.getParamFromUrl || function (r) { var t = new RegExp("[?&#]" + r + "=([^&#]*)").exec(this); return t ? t[1] : ""; };
String.prototype.replaceAll = function (e, r) { return this.replace(new RegExp(e.replace(/[-\/\\^$*+?.()|[\]{}]/g, "\\$&"), "g"), r); };

function GetValueUrl(e, r) { return e = e.replaceAll("#", "?"), new URL(e).searchParams.get(r); }


$(function () {
    $('.treeview-menu li a').each(function () {
        if (this.href.trim() == window.location) {
            $(this).parent().addClass('active');
            $(this).closest('ul.treeview-menu').show();
            $(this).closest('li.treeview').addClass('active');
        }
    });
    removeObject();
});

function checkAllCheckBox() {
    $("#SelectTreeCategory input[type=checkbox]").each(function () {
        $(this).prop("checked", true);
    });
}

function unCheckAllCheckBox() {
    $("#SelectTreeCategory input[type=checkbox]").each(function () {
        $(this).prop("checked", false);
    });
}

function RandomCode(id) {
    var randomstring = Math.random().toString(36).slice(-10);
    $('#' + id).val(randomstring);
}

function replaceCommaFirstEnd(str) {
    if (str != null || str != "" || str != undefined) {
        if (str.startsWith(",")) {
            str = str.substring(1);
        }
        if (str.endsWith(",")) {
            str = str.substring(0, str.length - 1);
        }
        return str;
    }
}