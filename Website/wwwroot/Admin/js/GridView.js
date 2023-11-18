var arrOrder = [], urlList = "", arrPrice = [], arrPriceOld = []; arrSource = []; arrSale = [];
$(function () {
    $("#btnAdd").click(function () {
        loadAjax(urlForm, "#tab_add");
    });

});
function registerGridView(selector) {
    $('a.resize').click(function (e) {
        e.preventDefault();
        $('.loading_admin').addClass('active');
        let total = $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").length;
        let start = 0;
        $('#status').show();
        $('#status-total').text(total);
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            var thisurl = $(this).data('url');
            var path = '';
            var name = '';
            if (thisurl != '') {
                var listpath = thisurl.split('/');
                name = listpath[(listpath.length - 1)];
                path = thisurl.replace('/' + name, '');
            }
            var url = "/Adminadc/Image/ConvertReSize?path=" + path + "&name=" + name;
            $.ajax({
                url: encodeURI(url), cache: false, type: "Post",
                success: function (data) {
                    let sc = $('#status-success').text();
                    $('#status-success').text(parseInt(sc) + 1);
                    start++;
                    if (start == total) {
                        $('.loading_admin').removeClass('active');
                        setTimeout(function () {
                            window.location.reload();
                        },1500)
                    }
                },
                error: function (data) {
                    let sc = $('#status-failed').text();
                    $('#status-failed').text(parseInt(sc) + 1);
                    start++;
                    if (start == total) {
                        $('.loading_admin').removeClass('active');
                        setTimeout(function () {
                            window.location.reload();
                        }, 1500)
                    }
                }
            });
        });
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', '.checkAll', function () {
        $(selector + " input.check[type='checkbox']").not(".checkAll").not("#checkAll").prop("checked", $(this).is(":checked"));
    });
    $('.EditOrderAll').click(function () {
        $(this).hide();
        $('.SaveAll').show();
        $(selector + " input.InputOrderDisplay").removeAttr('disabled');
    });
    $('.SaveAll').click(function () {
        $(this).hide();
        $('.EditOrderAll').show();
        $(selector + " input.InputOrderDisplay").attr('disabled', "");
    });
    $('.EditPriceAll').click(function () {
        $(this).hide();
        $('.SavePriceAll').show();
        $(selector + " input.InputPrice").removeAttr('disabled');
    });
    $('.SavePriceAll').click(function () {
        $(this).hide();
        $('.EditPriceAll').show();
        $(selector + " input.InputPrice").attr('disabled', "");
    });
    $('.EditPriceOldAll').click(function () {
        $(this).hide();
        $('.SavePriceOldAll').show();
        $(selector + " input.InputPriceOld").removeAttr('disabled');
        $(selector + " input.InputPrice").removeAttr('disabled');
    });
    $('.SavePriceOldAll').click(function () {
        $(this).hide();
        $('.EditPriceOldAll').show();
        $(selector + " input.InputPriceOld").attr('disabled', "");
        $(selector + " input.InputPrice").attr('disabled', "");
    });
    $('.EditSource').click(function () {
        $(this).hide();
        $('.SaveSource').show();
        $(selector + " input.Source").removeAttr('disabled');
    });
    $('.SaveSource').click(function () {
        $(this).hide();
        $('.EditSource').show();
        $(selector + " input.Source").attr('disabled', "");
    });
    $('.EditSaleAll').click(function () {
        $(this).hide();
        $('.SaveSaleAll').show();
        $(selector + " input.InputSale").removeAttr('disabled');
    });
    $('.SaveSaleAll').click(function () {
        $(this).hide();
        $('.EditSaleAll').show();
        $(selector + " input.InputSale").attr('disabled', "");
    });
    //Đăng ký Hiển thị nhiều
    $(selector).on('click', 'a.showAll', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=ShowAll&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    //initAjaxLoadGridView(urlList, "#loadGridView");
                    window.location.reload();
                    //$.growl.notice({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.hideAll', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=HiddenAll&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    //initAjaxLoadGridView(urlList, "#loadGridView");
                    window.location.reload();
                    //$.growl.notice({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.deleteAll', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=DeleteAll&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.deleteAllJson', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        var type = $(this).data("type");
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=DeleteAll&TypeJson=" + type + "&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.shock', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=IsShock&ItemId=" + arrRowId, function (data) {   
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.notshock', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=NotShock&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    //initAjaxLoadGridView(urlList, "#loadGridView");
                    window.location.reload();
                    //$.growl.notice({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.isvatall', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=IsVatAll&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    window.location.reload();      
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.notvatall', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=NotIsVatALL&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.issitemapall', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=IsSitemapAll&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.notsitemapall', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=NotIsSitemapAll&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.bestSell', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=BestSell&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.notbestSell', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=NotBestSell&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.isNew', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=IsNew&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.notNew', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=NotNew&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.getlisttoarticle', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            
            var txt = "<div class=\"generate-promotion-products\" data-listid=\"" + arrRowId + "\"></div>"
            var textarea = document.createElement("textarea");
            textarea.textContent = txt;
            textarea.style.position = "fixed";  // Prevent scrolling to bottom of page in Microsoft Edge.
            document.body.appendChild(textarea);
            /* Select the text field */
            textarea.select();
            textarea.setSelectionRange(0, 99999); /*For mobile devices*/
            /* Copy the text inside the text field */
            document.execCommand("copy");
            swal({
                title: "",
                text: txt,
                icon: "success",
                button: "Copy và tắt"
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn sản phẩm!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.getproducttoarticle', function (e) {
        e.preventDefault();
        var arrRowId = $(this).data("id");

        if (arrRowId != "") {
            var txt = "<div class=\"generate-productbox\" data-id=\"" + arrRowId + "\"></div>"
            var textarea = document.createElement("textarea");
            textarea.textContent = txt;
            textarea.style.position = "fixed";  // Prevent scrolling to bottom of page in Microsoft Edge.
            document.body.appendChild(textarea);
            /* Select the text field */
            textarea.select();
            textarea.setSelectionRange(0, 99999); /*For mobile devices*/
            /* Copy the text inside the text field */
            document.execCommand("copy");
            swal({
                title: "",
                text: txt,
                icon: "success",
                button: "Copy và tắt"
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn sản phẩm!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.isHot', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=IsHot&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.notHot', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=NotHot&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.isHome', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=IsHome&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.notHome', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=NotHome&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.isSelling', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=IsSelling&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.notNotSelling', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=NotIsSelling&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.removeframes', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=RemoveFrames&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });    
    $(selector).on('click', 'a.approvedAll', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=ApprovedAll&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    //initAjaxLoadGridView(urlList, "#loadGridView");
                    window.location.reload();
                    //$.growl.notice({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.notapprovedAll', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=NotApprovedAll&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    //initAjaxLoadGridView(urlList, "#loadGridView");
                    window.location.reload();
                    //$.growl.notice({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    //Update nhiều
    $(selector).on('click', 'a.allsale', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            ModalADC.Open({
                title: "Cập nhật giảm giá",
                urlLoad: '/Adminadc/Product/AjaxSale?ids=' + arrRowId,
                bottom: false
            });
            //$.post(urlPostAction, "Do=UpdateSale&ItemId=" + arrRowId, function (data) {
            //    if (data.errors == false) {
            //        window.location.reload();
            //    } else {
            //        $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
            //        $("#btnOrder").attr("disabled", false);
            //    }
            //});
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.allmodule', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            ModalADC.Open({
                title: "Thêm danh mục",
                urlLoad: '/Adminadc/Product/AjaxChangeModule?Do=AddModule&ids=' + arrRowId,
                bottom: false
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.notallmodule', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            ModalADC.Open({
                title: "Thêm danh mục",
                urlLoad: '/Adminadc/Product/AjaxChangeModule?Do=RemoveModule&ids=' + arrRowId,
                bottom: false
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.status-change', function (e) {
        var val = $(this).data('type');
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=ChangeStatusAll&Status=" + val + "&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    $(selector).on('click', 'a.addframes', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            ModalADC.Open({
                title: "Thêm khung ảnh đại diện",
                urlLoad: '/Adminadc/Product/AjaxFrame?ids=' + arrRowId,
                bottom: false
            });
            //$.post(urlPostAction, "Do=UpdateSale&ItemId=" + arrRowId, function (data) {
            //    if (data.errors == false) {
            //        window.location.reload();
            //    } else {
            //        $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
            //        $("#btnOrder").attr("disabled", false);
            //    }
            //});
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
    //
    $("a.edit").click(function () {
        var itemId = $(this).attr("href").substring(1);
        var tab = $(this).data('tab');
        var url = urlForm + "?Do=Edit&ItemId=" + itemId + "&tab=" + tab;
        if (urlForm.indexOf("?") > -1) {
            url = urlForm + "&Do=Edit&ItemId=" + itemId + "&tab=" + tab;
        }
        loadAjax(url, "#tab_add");
        $("#btnEdit").click();
        return false;
    });
    $("a.edit-js").click(function () {
        var itemId = $(this).attr("href").substring(1);
        var type = $(this).data("type");
        var url = urlForm + "?Do=Edit&TypeJson=" + type + "&ItemId=" + itemId;
        if (urlForm.indexOf("?") > -1) {
            url = urlForm + "&Do=Edit&ItemId=" + itemId;
        }
        loadAjax(url, "#tab_add");
        $("#btnEdit").click();
        return false;
    });
    $(".act_delete a.add").click(function () {
        var itemId = $(this).attr("href").substring(1);
        var url = urlForm + "?Do=Add&ItemId=" + itemId;
        loadAjax(url, "#tab_add");
        $("#btnEdit").click();
        return false;
    });
    $('.quickTool a.add').click(function () {
        var itemId = $(this).attr("href").substring(1);
        loadAjax(urlForm + "?Do=Add&ItemId=" + itemId, "#tab_add");
        $("#btnEdit").click();
        return false;
    });
    $('a.delete').click(function (e) {
        var itemId = $(this).attr("href").substring(1);
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
                    $.post(urlPostAction, "Do=Delete&ItemId=" + itemId, function (data) {
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
                                    window.location.reload();
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
    });
    $('a.delete-js').click(function (e) {
        var itemId = $(this).attr("href").substring(1);
        var type = $(this).data("type");
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
                    $.post(urlPostAction, "Do=Delete&TypeJson=" + type + "&ItemId=" + itemId, function (data) {
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
                                    window.location.reload();
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
    });
    $(selector + " a.hiddens").click(function (e) {
        e.preventDefault();
        var itemId = $(this).attr("href").substring(1);
        var title = $(this).attr("title");
        title = isEmpty(title) ? "Ẩn" : title;
        $.post(urlPostAction, "Do=Hidden&ItemId=" + itemId, function (data) {
            if (data.errors == false) {
                window.location.reload();
            } else {
                swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
                $("#btnOrder").attr("disabled", false);
            }
        });
    });
    $(selector + " a.show").click(function () {
        var itemId = $(this).attr("href").substring(1);
        $.post(urlPostAction, "Do=Show&ItemId=" + itemId, function (data) {
            if (data.errors == false) {
                window.location.reload();
            } else {
                swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
                $("#btnOrder").attr("disabled", false);
            }
        });
    });
    $(selector + " a.featured").click(function (e) {
        e.preventDefault();
        var itemId = $(this).attr("href").substring(1);
        var title = $(this).attr("title");
        title = isEmpty(title) ? "Ẩn" : title;
        $.post(urlPostAction, "Do=Feature&ItemId=" + itemId, function (data) {
            if (data.errors == false) {
                window.location.reload();
            } else {
                swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
                $("#btnOrder").attr("disabled", false);
            }
        });
    });
    $(selector + " a.isvat").click(function (e) {
        e.preventDefault();
        var itemId = $(this).attr("href").substring(1);
        var title = $(this).attr("title");
        title = isEmpty(title) ? "Ẩn" : title;
        $.post(urlPostAction, "Do=IsVat&ItemId=" + itemId, function (data) {
            if (data.errors == false) {
                window.location.reload();
            } else {
                swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
            }
        });
    });
    $(selector + " a.sitemaps").click(function (e) {
        e.preventDefault();
        var itemId = $(this).attr("href").substring(1);
        var title = $(this).attr("title");
        title = isEmpty(title) ? "Ẩn" : title;
        $.post(urlPostAction, "Do=IsSitemap&ItemId=" + itemId, function (data) {
            if (data.errors == false) {
                window.location.reload();
            } else {
                swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
            }
        });
    });
    $(selector + " a.notaHome").click(function (e) {
        e.preventDefault();
        var itemId = $(this).attr("href").substring(1);
        var title = $(this).attr("title");
        title = isEmpty(title) ? "Bỏ trang chủ" : title;
        $.post(urlPostAction, "Do=NotHome&ItemId=" + itemId, function (data) {
            if (data.errors == false) {
                window.location.reload();
            } else {
                swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
                $("#btnOrder").attr("disabled", false);
            }
        });
    });
    $(selector + " a.isaHome").click(function () {
        var itemId = $(this).attr("href").substring(1);
        $.post(urlPostAction, "Do=IsHome&ItemId=" + itemId, function (data) {
            if (data.errors == false) {
                window.location.reload();
            } else {
                swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
                $("#btnOrder").attr("disabled", false);
            }
        });
    });
    $(selector + " a.notaHot").click(function (e) {
        e.preventDefault();
        var itemId = $(this).attr("href").substring(1);
        var title = $(this).attr("title");
        title = isEmpty(title) ? "Bỏ trang chủ" : title;
        $.post(urlPostAction, "Do=NotHot&ItemId=" + itemId, function (data) {
            if (data.errors == false) {
                window.location.reload();
            } else {
                swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
                $("#btnOrder").attr("disabled", false);
            }
        });
    });
    $(selector + " a.isaHot").click(function () {
        var itemId = $(this).attr("href").substring(1);
        $.post(urlPostAction, "Do=IsHot&ItemId=" + itemId, function (data) {
            if (data.errors == false) {
                window.location.reload();
            } else {
                swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
                $("#btnOrder").attr("disabled", false);
            }
        });
    });
    $(selector + " a.notaNew").click(function (e) {
        e.preventDefault();
        var itemId = $(this).attr("href").substring(1);
        var title = $(this).attr("title");
        title = isEmpty(title) ? "Bỏ mới" : title;
        $.post(urlPostAction, "Do=NotNew&ItemId=" + itemId, function (data) {
            if (data.errors == false) {
                window.location.reload();
            } else {
                swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
                $("#btnOrder").attr("disabled", false);
            }
        });
    });
    $(selector + " a.isaNew").click(function () {
        var itemId = $(this).attr("href").substring(1);
        $.post(urlPostAction, "Do=IsNew&ItemId=" + itemId, function (data) {
            if (data.errors == false) {
                window.location.reload();
            } else {
                swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
                $("#btnOrder").attr("disabled", false);
            }
        });
    });
    $(selector + " a.notaBestSale").click(function (e) {
        e.preventDefault();
        var itemId = $(this).attr("href").substring(1);
        var title = $(this).attr("title");
        title = isEmpty(title) ? "Bỏ bán chạy" : title;
        $.post(urlPostAction, "Do=NotBestSell&ItemId=" + itemId, function (data) {
            if (data.errors == false) {
                window.location.reload();
            } else {
                swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
                $("#btnOrder").attr("disabled", false);
            }
        });
    });
    $(selector + " a.isaBestSale").click(function () {
        var itemId = $(this).attr("href").substring(1);
        $.post(urlPostAction, "Do=BestSell&ItemId=" + itemId, function (data) {
            if (data.errors == false) {
                window.location.reload();
            } else {
                swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
                $("#btnOrder").attr("disabled", false);
            }
        });
    });
    $(selector + " a.notaShock").click(function (e) {
        e.preventDefault();
        var itemId = $(this).attr("href").substring(1);
        var title = $(this).attr("title");
        title = isEmpty(title) ? "Bỏ giá sốc" : title;
        $.post(urlPostAction, "Do=NotShock&ItemId=" + itemId, function (data) {
            if (data.errors == false) {
                window.location.reload();
            } else {
                swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
                $("#btnOrder").attr("disabled", false);
            }
        });
    });
    $(selector + " a.isaShock").click(function () {
        var itemId = $(this).attr("href").substring(1);
        $.post(urlPostAction, "Do=IsShock&ItemId=" + itemId, function (data) {
            if (data.errors == false) {
                window.location.reload();
            } else {
                swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
                $("#btnOrder").attr("disabled", false);
            }
        });
    });
    $(selector + " .change-status").change(function () {
        var itemId = $(this).data("id");
        var val = $(this).val();
        if (val == 1) {
            ModalADC.Open({
                title: "Cập nhật số lượng",
                urlLoad: '/Adminadc/Product/AjaxAmount?ids=' + itemId,
                bottom: false
            });
        }
        else {
            $.post(urlPostAction, "Do=ChangeStatus&Status=" + val + "&ItemId=" + itemId, function (data) {
                if (data.errors == false) {
                    window.location.reload();
                } else {
                    swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
                    $("#btnOrder").attr("disabled", false);
                }
            });
        }
    });
    $(selector + " a.notapproved").click(function () {
        var itemId = $(this).attr("href").substring(1);
        var title = $(this).attr("title");
        title = isEmpty(title) ? "Ẩn" : title;
        $.post(urlPostAction, "Do=NotApproved&ItemId=" + itemId, function (data) {
            if (data.errors == false) {
                window.location.reload();
            } else {
                swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
                $("#btnOrder").attr("disabled", false);
            }
        });
    });
    $(selector + " a.approved").click(function () {
        var itemId = $(this).attr("href").substring(1);
        $.post(urlPostAction, "Do=Approved&ItemId=" + itemId, function (data) {
            if (data.errors == false) {
                window.location.reload();
            } else {
                swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
                $("#btnOrder").attr("disabled", false);
            }
        });
    });
    // Sắp xếp lại thứ tự qua textbox
    $(selector).on('click', 'a.SaveAll', function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (arrOrder.length > 0) {
            $.post(urlPostAction, { OrderByValues: JSON.stringify(arrOrder), "Do": "OrderBy" }, function (data) {
                if (!data.errors) {
                    $.growl.notice({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    arrOrder = [];
                    setInterval(function () { window.location.reload(); }, 1000);
                } else
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
            });
        }
        return false;
    });
    $(selector).on('click', 'a.SavePriceAll', function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (arrPrice.length > 0) {
            $.post(urlPostAction, { OrderByValues: JSON.stringify(arrPrice), "Do": "ChangePrice" }, function (data) {
                if (!data.errors) {
                    $.growl.notice({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    arrPrice = [];
                    setInterval(function () { window.location.reload(); }, 1000);
                } else
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
            });
        }
        return false;
    });
    $(selector).on('click', 'a.SavePriceOldAll', function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (arrPriceOld.length > 0 || arrPrice.length>0) {
            $.post(urlPostAction, { OrderByValues: JSON.stringify(arrPrice), OrderByValuesOld: JSON.stringify(arrPriceOld), "Do": "ChangePriceOld" }, function (data) {
                if (!data.errors) {
                    $.growl.notice({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    arrPriceOld = [];
                    arrPrice = [];
                    setInterval(function () { window.location.reload(); }, 1000);
                } else
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
            });
        }
        return false;
    });
    $(selector).on('click', 'a.SaveSaleAll', function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (arrSale.length > 0) {
            $.post(urlPostAction, { OrderByValues: JSON.stringify(arrSale), "Do": "ChangeSale" }, function (data) {
                if (!data.errors) {
                    $.growl.notice({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    arrSale = [];
                    setInterval(function () { window.location.reload(); }, 1000);
                } else
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
            });
        }
        return false;
    });
    $(selector).on('click', 'a.SaveSource', function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (arrSource.length > 0) {
            $.post(urlPostAction, { OrderByValues: JSON.stringify(arrSource), "Do": "ChangeSource" }, function (data) {
                if (!data.errors) {
                    $.growl.notice({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                    arrSource = [];
                    setInterval(function () { window.location.reload(); }, 1000);
                } else
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
            });
        }
        return false;
    });
    //active
    $(selector).on('click', 'a.active', function (e) {
        e.preventDefault();
        var arrRowId = '';
        $(selector + " input.check[type='checkbox']:checked").not("#checkAll").not(".checkAll").each(function () {
            arrRowId += $(this).val() + ",";
        });
        arrRowId = (arrRowId.length > 0) ? arrRowId.substring(0, arrRowId.length - 1) : arrRowId;
        if (arrRowId != "") {
            $.post(urlPostAction, "Do=Active&ItemId=" + arrRowId, function (data) {
                if (data.errors == false) {
                    //initAjaxLoadGridView(urlList, "#loadGridView");
                    window.location.reload();
                    //$.growl.notice({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });
                } else {
                    $.growl.error({ title: "Thông báo", message: data.message, location: 'br', size: 'large', duration: 3000 });

                }
            });
        }
        else {
            $.growl.error({ title: "Thông báo", message: "Hãy chọn bài viết!", location: 'br', size: 'large', duration: 3000 });
        }
        $(this).parents('.btn-group').removeClass('open');
        return false;
    });
}
function changeOrder(id, value) {
    if (id && value && !isNaN(id) && !isNaN(value)) {
        id = parseInt(id);
        value = parseInt(value);
        if (arrOrder.length === 0 || !checkIDInOrderArr(id)) {
            arrOrder.push({ ID: id, OrderDisplay: value });
        } else {
            for (var keys in arrOrder) {
                if (arrOrder.hasOwnProperty(keys) && arrOrder[keys].ID === id) {
                    arrOrder[keys].OrderDisplay = value;
                    break;
                }
            }
        }
    } else {
        alert("Thứ tự bản ghi không được để trống và phải là kiểu số!");
    }
    return true;
}
function changePrice(id, value) {
    if (id && !isNaN(id) && value != undefined) {
        id = parseInt(id);
        if (arrPrice.length === 0 || !checkIDInPriceArr(id)) {
            arrPrice.push({ ID: id, Price: value });
        } else {
            for (var keys in arrPrice) {
                if (arrPrice.hasOwnProperty(keys) && arrPrice[keys].ID === id) {
                    arrPrice[keys].Price = value;
                    break;
                }
            }
        }
    } else {
        alert("Giá bản ghi không được để trống!");
    }
    return true;
}
function changePriceOld(id, value) {
    if (id && !isNaN(id) && value != undefined) {
        id = parseInt(id);
        if (arrPriceOld.length === 0 || !checkIDInPriceOldArr(id)) {
            arrPriceOld.push({ ID: id, Price: value });
        } else {
            for (var keys in arrPriceOld) {
                if (arrPriceOld.hasOwnProperty(keys) && arrPriceOld[keys].ID === id) {
                    arrPriceOld[keys].Price = value;
                    break;
                }
            }
        }
    } else {
        alert("Giá bản ghi không được để trống!");
    }
    return true;
}
function changeSale(id, value) {
    if (id && !isNaN(id) && value != undefined) {
        id = parseInt(id);
        if (arrSale.length === 0 || !checkIDInSaleArr(id)) {
            arrSale.push({ ID: id, Price: value });
        } else {
            for (var keys in arrPriceOld) {
                if (arrSale.hasOwnProperty(keys) && arrSale[keys].ID === id) {
                    arrSale[keys].Price = value;
                    break;
                }
            }
        }
    } else {
        alert("Giá bản ghi không được để trống!");
    }
    return true;
}
function changeSource(id, value, type) {
    if (id && value != undefined) {
        if (arrSource.length === 0 || !checkIDInSourceArr(id)) {
            arrSource.push({ Key: id, Price: value, Type: type });
        } else {
            for (var keys in arrSource) {
                if (arrSource.hasOwnProperty(keys) && arrSource[keys].Key === id) {
                    arrSource[keys].Price = value;
                    arrSource[keys].Type = type;
                    break;
                }
            }
        }
    } else {
        alert("Giá trị không được để trống!");
    }
    return true;
}
function checkIDInOrderArr(id) {
    for (var keys in arrOrder) {
        if (arrOrder.hasOwnProperty(keys) && arrOrder[keys].ID === id) {
            return true;
        }
    }
    return false;
}
function checkIDInPriceArr(id) {
    for (var keys in arrPrice) {
        if (arrPrice.hasOwnProperty(keys) && arrPrice[keys].ID === id) {
            return true;
        }
    }
    return false;
}
function checkIDInSourceArr(id) {
    for (var keys in arrPrice) {
        if (arrSource.hasOwnProperty(keys) && arrSource[keys].ID === id) {
            return true;
        }
    }
    return false;
}
function checkIDInPriceOldArr(id) {
    for (var keys in arrPriceOld) {
        if (arrPriceOld.hasOwnProperty(keys) && arrPriceOld[keys].ID === id) {
            return true;
        }
    }
    return false;
}
function checkIDInSaleArr(id) {
    for (var keys in arrSale) {
        if (arrSale.hasOwnProperty(keys) && arrSale[keys].ID === id) {
            return true;
        }
    }
    return false;
}
function PageClient(id) {
    $(id).DataTable();
}
function PageClient1(id) {
    $(id).DataTable({
        "paging": true,
        "lengthChange": false,
        "searching": false,
        "ordering": true,
        "info": true,
        "autoWidth": false
    });
}
var ModalADC = (function (w, d, $) {
    var modalContent = [],                                           // modal content html
        variable = {
            b: document.body,
            create: document.createElement,
            settings: [],                                                // options after merge
            item: -1,                                                    // index current modal item
            width: 900,
            keyboard: false                                              // true : press esc to close modal - false : not

        },
        defautlt = {
            title: "Cửa sổ mới",                                         // modal title
            urlPost: null,                                      // send ajax request with this url
            urlLoad: null,                                               // retrive html to fill to .modal-content
            params: {},                                                  // params of urlLoad request
            bottom: true,
            actionText: "Thêm mới"
        },
        processer = {
            init: function () {
                this
                    .mergeOption();
                this
                    .htmlContent();
            },
            extend: function () {
                for (var i = 1, arg = arguments.length; i < arg; i++) {
                    for (var key in arguments[i]) {
                        if (arguments[i].hasOwnProperty(key)) {
                            arguments[0][key] = arguments[i][key];
                        }
                    }
                }
                return arguments[0];
            },
            mergeOption: function () {
                variable.settings.push(this.extend({}, defautlt, variable.options));
                delete variable.options;
                variable.item++;
            },
            htmlContent: function () {
                modalContent[variable.item] =
                    $('<div class="modal fade in" style="display:block" id="modal-pop-' + variable.item + '">' +
                        '<div class="modal-dialog modal-lg">' +
                        '<div class="modal-content">' +
                        '<div class="modal-body">' +

                        //'<div class="modal-header">' +
                        //'<div class="box-footer">' +
                        //    '<button id="submit" type="submit" class="primaryAction btn btn-primary" id="submit">' + variable.settings[variable.item].actionText + '</button>' +
                        //    '<button id="reset" type="reset" class="primaryAction btn btn-primary" id="reset">Nhập lại</button>' +
                        //    '<button id="close" type="button" class="primaryAction btn btn-primary" data-dismiss="modal" aria-label="Close">Đóng lại</button>' +
                        //    '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">×</span></button>' +
                        //'</div>' +
                        '<h4 class="modal-title">' + '<div class="button-zoomout">' +
                        '</div>' + variable.settings[variable.item].title + '<a class="anchorjs-link"><span class="anchorjs-icon"></span></a></h4>' +
                        '</div>' +
                        '<div class="content-modal">' +
                        '</div>' +
                        '</div></div></div>');
                if (variable.settings[variable.item].bottom) {
                    modalContent[variable.item].find(".modal-body").append('<div class="modal-footer">' +
                        '<a href="#" id="okButton" class="btn btn-primary close" data-dismiss="modal">Tiếp tục</a>' +
                        '<a href="#" id="cancelButton" class="btn btn-danger close" data-dismiss="modal">Hủy</a>' +
                        '</div>');
                }
                //modalContent[variable.item].on('click', "#submit", function() {
                //    //modalContent[variable.item].find("form").submit();
                //});
                modalContent[variable.item].on('click', "#reset", function () {
                    $.each(modalContent[variable.item].find("form").find("input,select,textarea"), function (idx, el) {
                        var fieldType = el.type.toLowerCase();
                        switch (fieldType) {
                            case "text":
                                el.value = "";
                                break;
                            case "password":
                                el.value = "";
                                break;
                            case "textarea":
                                el.value = "";
                                break;
                            case "hidden":
                                //do nothing
                                break;
                            case "checkbox":
                                el.checked = false;
                                break;
                            case "select-one":
                                el.selectedIndex = 0;
                                break;
                            case "radio":
                                el.selectedIndex = 0;
                                break;
                            case "select-multi":
                                el.selectedIndex = 0;
                                break;
                            default:
                                break;
                        }
                    });
                });
                modalContent[variable.item].on('click', '.close,#close,#cancelButton', function () {
                    processer.close();
                });
            },
            load: function () {
                var def = $.Deferred();
                $.get(variable.settings[variable.item].urlLoad, variable.settings[variable.item].params).done(function (data) {
                    modalContent[variable.item].find(".content-modal").html(data);
                    def.resolve();
                }).fail(function (error) {
                    modalContent[variable.item].find(".content-modal").html(error);
                    def.reject();
                });
                return def.promise();
            },
            open: function () {
                this.load().done(function () {
                    modalContent[variable.item].modal({
                        keyboard: variable.settings[variable.item].keyboard,
                        backdrop: 'static'
                    });
                });
            },
            close: function () {
                modalContent[variable.item].modal("hide");
                setTimeout(function () {
                    modalContent[variable.item].remove();
                    modalContent.splice(variable.item, 1);
                    variable.settings.splice(variable.item, 1);
                    variable.item--;
                }, 500);
            }
        };
    return {
        Open: function (options) {
            variable.options = options;
            processer.init();
            processer.open();
        },
        Close: function () {
            processer.close();
        }
    };

})(window, document, jQuery);