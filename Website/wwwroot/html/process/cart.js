function checkBrowserEnableCookie() { var cookieEnabled = (navigator.cookieEnabled) ? true : false; if (typeof navigator.cookieEnabled == "undefined" && !cookieEnabled) { document.cookie = "testcookie"; cookieEnabled = (document.cookie.indexOf("testcookie") != -1) ? true : false; } if (cookieEnabled) return true; else return false; }
function createCookie(name, value, days) { if (days) { var date = new Date(); date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000)); var expires = "; expires=" + date.toGMTString(); } else var expires = ""; document.cookie = name + "=" + value + expires + "; path=/"; }
function readCookie(name) { var nameEQ = name + "="; var ca = document.cookie.split(';'); for (var i = 0; i < ca.length; i++) { var c = ca[i]; while (c.charAt(0) == ' ') c = c.substring(1, c.length); if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length); } return ""; }
function eraseCookie(name) { createCookie(name, "", -1); }
function formatFrice(n, currency) {
    return n.toFixed(0).replace(/./g, function (c, i, a) {
        return i > 0 && c !== "." && (a.length - i) % 3 === 0 ? "," + c : c;
    }) + '' + currency;
}
function saveCartToCustomer() {
    var checkLogin = $("#userId").val();
    if (readCookie("shopping_cart") != "" && checkLogin != 0) {
        $.ajax({
            url: "/Ajax/Cart/SaveCartToCustomer",
            cache: false,
            dataType: "html",
            success: function (data) {
        
            }
        });
    }
}
function CancelOrder(id) {
    $.confirm({
        title: "",
        content: 'Bạn có chắc chắn hủy đơn hàng này?',
        theme: 'material',
        buttons: {
            hey: {
                text: 'Có',
                action: function () {
                    $.ajax({
                        url: "/Ajax/Cart/CancelOrder",
                        cache: false,
                        data: { id: id },
                        dataType: "html",
                        success: function (data) {
                            window.location.href = "/";
                        }
                    });
                }
            },
            heyThere: {
                text: 'Không'
            }
        }
    });

}
function UpdatePaymentTypeOrder(id, type) {
    $.ajax({
        url: "/Ajax/Cart/UpdatePaymentTypeOrder",
        cache: false,
        data: { id: id, type: type },
        dataType: "html",
        success: function (data) {
            $("#paymentTypeT").html(data)
        }
    });

}
function countShoppingCart(name) {
    if (readCookie(name) == "") {
        createCookie(name, '', 1);
        $(".count_shopping_cart").text("0");
    } else {
        var current_cart = readCookie(name);
        var ca = current_cart.split('%2c');
        number_product = 0;
        for (i = 0; i < ca.length; i++) {
            if (ca[i] !== undefined && ca[i] != '') {
                var caPart1 = ca[i].split('&');
                if (caPart1[0] !== undefined && caPart1[0] != '') {
                    var caPart2 = caPart1[0].split('-');
                    if (caPart2[1] !== undefined && caPart2[1] != '') {
                        number_product = number_product + parseInt(caPart2[1]);
                    }
                }
            }

        }
        $(".count_shopping_cart").text(number_product);
    }
}
function emptyShoppingCart(name) { createCookie(name, '-', 1); }
function changeAttrChangePrice() {
    //$(".choose-color span").click(function () {
    //    $(".choose-color span").removeClass("active");
    //    var dataC = $(this).data("color");
    //    $("." + dataC).addClass("active");
    //    var priceAll = parseInt($("#PriceAll").val());
    //    var priceOldAll = parseInt($("#PriceOldAll").val());
    //    var valPriceTemp = $(this).data('price');
    //    if (priceAll !== undefined && priceAll != 0) {
    //        if (valPriceTemp !== undefined && valPriceTemp != "0") {
    //            priceAll = priceAll + parseInt(valPriceTemp);
    //        }
    //        $(".change-pr").text(formatFrice(priceAll, "đ"));
    //    }
    //    if (priceOldAll !== undefined && priceOldAll != 0) {
    //        if (valPriceTemp !== undefined && valPriceTemp != "0") {
    //            priceOldAll = priceOldAll + parseInt(valPriceTemp);
    //        }
    //        $('.gt-dbfix').text(formatFrice(priceOldAll, "đ"));
    //        var percent = 100 - (priceAll / priceOldAll * 100);
    //        $(".percent-change").text("-" + parseInt(percent) + "%");
    //    }
    //});
}
function getAllSttAttrOrderByClass(classC) {
    var re = [];
    classC.each(function () {
        var valThis = $(this).val();
        if (valThis !== undefined && valThis != "0" && valThis != "") {
            re.push(parseInt(valThis));
        }
    });
    var sult = re.sort((a, b) => a - b);
    return sult.join("-");
}
function sortAttrBySigture(str, sig) {
    var re = str.split(sig);
    var sult = re.sort((a, b) => a - b);
    return sult.join("-");
}
function getAllSttAttrOrderByRadio(arrName) {
    var re = [];
    for (i = 0; i < arrName.length; i++) {
        var valThis = $("input[name='" + arrName[i] + "']:checked").val();
        if (valThis !== undefined && valThis != "0" && valThis != "") {
            re.push(parseInt(valThis));
        }
    }
    var sult = re.sort((a, b) => a - b);
    return sult.join("-");
}
function checkExistByKey(fullStr, checkStr1, checkStr2, key) {
    var re = 0;
    if (fullStr != '' && checkStr1 != '' && checkStr2 != '' && key != '') {
        var spl = fullStr.split(key);
        for (i = 1; i <= spl.length - 1; i++) {
            if (spl[i].search(checkStr1) != -1 && spl[i].search(checkStr2) != -1) {
                re = 1;
                return re;
            }
        }
    }
    return re;
}
//them sp vao gio hang
function addToCartAttr(productSubId, quantity, strAttr, from) {
    //%2c50608-1&5555&1.33&1
    //%2c50612-11&5752&&0
    //%2c42979-3&&&0    
    if (readCookie('shopping_cart') == null) {
        createCookie('shopping_cart', '', 1);
    }
    var current_cart = readCookie('shopping_cart');
    var keySearchOldProduct1 = productSubId + '-';
    var keySearchOldProduct2 = '&' + strAttr;
    //neu san pham va thuoc tinh chua co trong gio hang thi add them
    if (checkExistByKey(current_cart, keySearchOldProduct1, keySearchOldProduct2, "%2c") == 0) {
        var new_cart = current_cart + '%2c' + productSubId + '-' + quantity + '&' + strAttr;
        createCookie('shopping_cart', new_cart, 1);
    } else {
        //neu da co trong gio hang thi check theo id pro va sttr => tang so luong
        var strCurrrentCart = "";
        var lstStr = current_cart.split('%2c');
        for (i = 1; i <= lstStr.length - 1; i++) {
            eleChild = lstStr[i];
            var eleSplitChild = eleChild.split('&');
            part0 = eleSplitChild[0]; //part product + quantity
            part1 = eleSplitChild[1]; // part attr         
            thisPro = part0.split('-');
            thisProId = thisPro[0];
            thisProCount = thisPro[1];
            if (thisProId == productSubId && part1 == strAttr) {
                thisProCountN = parseInt(quantity) + parseInt(thisProCount);
            } else {
                thisProCountN = thisProCount;
            }
            strCurrrentCart = strCurrrentCart + '%2c' + thisProId + '-' + thisProCountN + '&' + part1;
        }
        createCookie('shopping_cart', strCurrrentCart, 1);
    }
    if (from == "buynow") {
        window.location.href = "/gio-hang";
    } else {
        $('#Cart').load("/Ajax/Cart/CartData");
    }
}
//update cart
function updateCart(productId, quantity, namePro, from, attrpart) {

    var elementThis = "#item_" + productId;
    if (attrpart != "" && attrpart !== undefined) {
        elementThis = "#item_" + productId + "_" + attrpart;
    }
    var newquantity = $(elementThis).val();
    newquantity = parseFloat(newquantity);
    if (newquantity <= 0) {
        $.confirm({
            title: "",
            content: 'Nếu số lượng <= 0, sản phẩm ' + namePro + ' sẽ bị xóa khỏi giỏ hàng!',
            theme: 'supervan',
            buttons: {
                hey: {
                    text: 'Có',
                    action: function () {
                        var current_cart = readCookie('shopping_cart');
                        if (type == "0") {
                            re = "%2c" + productId + '-' + quantity + '&' + attrpart;
                        }
                        else {
                            re = "%2c" + productId + '-' + quantity + '&' + attrpart;
                        }
                        var new_cart = currentcart.replace(re, "");
                        createCookie('shopping_cart', new_cart, 1);
                        window.location.reload();
                    }
                },
                heyThere: {
                    text: 'Không',
                    action: function () {
                        newquantity = quantity; window.location.reload();
                    }
                }
            }
        });
    } else {
        if (newquantity > 999) {
            newquantity = 999;
            $.confirm({
                title: "",
                content: '<h4>Không thành công</h4><p>Bạn chỉ được phép mua tối đa số lượng 999 cho mỗi sản phẩm</p>',
                autoClose: 'heyThere|2000',
                theme: 'supervan',
                heyThere: {
                    text: 'Thoát'
                }
            });
        }
        var current_cart = readCookie('shopping_cart');
        attrpartN = "";
        if (attrpart == "" || attrpart === undefined) {
            attrpartN = "";
        } else {
            attrpartN = attrpart
            //var tempAttrpart = attrpart.split('-');
            //if (tempAttrpart.length <= 2) {
            //    for (i = 0; i < tempAttrpart.length; i++) {
            //        if (tempAttrpart[i] != "" && tempAttrpart[i] !== undefined) {
            //            attrpartN = attrpartN + "-" + tempAttrpart[i]
            //        }
            //    }
            //}
            //else {
            //    attrpartN = attrpart
            //}
        }
        var re1 = "%2c" + productId + '-' + quantity + '&' + attrpartN;
        var re2 = "%2c" + productId + '-' + newquantity + '&' + attrpartN;
        new_cart = current_cart.replace(re1, re2);
        createCookie('shopping_cart', new_cart, 1);
        countShoppingCart("shopping_cart");
        var vorchercode = $("#VorcherCode").val();
        $.ajax({
            url: "/Ajax/Cart/CartAjax",
            cache: false,
            dataType: "html",
            data: { vorchercode: vorchercode },
            success: function (data) {
                $('#Cart').html(data);
            }
        });
        $.ajax({
            url: "/Ajax/Cart/TotalPriceCart",
            cache: false,
            dataType: "html",
            data: { vorchercode: vorchercode },
            success: function (data) {
                $('#Total').html(data);
            }
        });
        //$.ajax({
        //    url: "/Ajax/Cart/TotalPriceCart",
        //    cache: false,
        //    dataType: "html",
        //    success: function (data) {
        //        $('#totalmoney').html(data);
        //    }
        //});
        //window.location.href = "/gio-hang?nocache=" + (new Date()).getTime();
        //if (from = "guidonhang") {
        //    $.ajax({
        //        url: "/Ajax/Cart/CartAjax",
        //        cache: false,
        //        dataType: "html",
        //        success: function (data) {
        //            $('#Cart').html(data);
        //        }
        //    });
        //} else {
        //    window.location.href = "/gio-hang?nocache=" + (new Date()).getTime();
        //}
    }
}

function setCartUrlBack(url) {
    $.cookie('CartUrlBack', url, { expires: 1, path: '/' });
}
//update all cart
function updateAllCart() {
    $(".quanlity").each(function () {
        var id = $(this).data('id');
        var quality = $(this).data('quality');
        var newquality = $(this).val();
        updateEachCart(id, quality, newquality);
    });
    $('#Cart').load("/Ajax/Cart/CartData");
}
function updateEachCart(productId, quantity, newquantity) {
    newquantity = parseInt(newquantity);
    var current_cart = readCookie('shopping_cart');
    new_cart = current_cart.replace("%2c" + productId + '-' + quantity, "%2c" + productId + '-' + newquantity);
    createCookie('shopping_cart', new_cart, 1);
}
function deleteAllCart() {
    $.confirm({
        title: "",
        content: 'Bạn có muốn xóa sản phẩm tất cả trong giỏ hàng?',
        theme: 'material',
        buttons: {
            hey: {
                text: 'Có',
                action: function () {
                    $.removeCookie('shopping_cart');

                    window.location.href = "/gio-hang?nocache=" + (new Date()).getTime();
                }
            },
            heyThere: {
                text: 'Không'
            }
        }
    });
}
function deleteAllCartNoWarning() {
    eraseCookie('shopping_cart');

}
//Xóa 1 sp trong giỏ hàng, id pro, so luong va ten
function deleteFromCart(productId, quantity, namePro, attrpart) {
    $.confirm({
        title: "",
        content: 'Bạn có muốn xóa sản phẩm ' + namePro + ' trong giỏ hàng?',
        theme: 'material',
        buttons: {
            hey: {
                text: 'Có',
                action: function () {
                    var currentcart = readCookie('shopping_cart');
                    re = "%2c" + productId + '-' + quantity + '&' + attrpart;

                    var newcart = currentcart.replace(re, "");
                    createCookie('shopping_cart', newcart, 1);
                    countShoppingCart("shopping_cart");

                    //window.location.href = "/gio-hang?nocache=" + (new Date()).getTime();
                    $.ajax({
                        url: "/Ajax/Cart/CartAjax",
                        cache: false,
                        dataType: "html",
                        success: function (data) {
                            $('#Cart').html(data);
                        }
                    });
                }
            },
            heyThere: {
                text: 'Không'
            }
        }
    });
}
function deleteFromCartNotWarnNotReloadAndCreateNew(productId, quantity, type, weightOld, weightGold, TG) {
    //delete
    var currentcart = readCookie('shopping_cart');
    re = "%2c" + productId + '-' + quantity + '&' + TG.toString().replace("000", "") + '&' + weightOld + '&' + type;
    var newcart = currentcart.replace(re, "");
    createCookie('shopping_cart', newcart, 1);
    //create
    if (readCookie('shopping_cart') == null) {
        createCookie('shopping_cart', '', 1);
    }
    var current_cart = readCookie('shopping_cart');
    var keySearchOldProduct1 = productId + '-';
    var keySearchOldProduct2 = '&' + weightGold;
    //neu san pham va thuoc tinh chua co trong gio hang thi add them
    if (checkExistByKey(current_cart, keySearchOldProduct1, keySearchOldProduct2, "%2c") == 0) {
        var new_cart = current_cart + '%2c' + productId + '-' + quantity + '&' + TG + '&' + weightGold + '&1';
        createCookie('shopping_cart', new_cart, 1);
    } else {
        //neu da co trong gio hang thi check theo id pro va sttr => tang so luong
        var strCurrrentCart = "";
        var lstStr = current_cart.split('%2c');
        for (i = 1; i <= lstStr.length - 1; i++) {
            eleChild = lstStr[i];
            var eleSplitChild = eleChild.split('&');
            part0 = eleSplitChild[0]; //part product + quantity
            part1 = eleSplitChild[1]; // part attr or ti gia vang
            part2 = eleSplitChild[2]; // part weight gold 
            part3 = eleSplitChild[3];//part type
            if (part3 == "0") {
                strCurrrentCart = strCurrrentCart + '%2c' + part0 + '&' + part1 + '&' + '&' + part3;
            } else {
                thisPro = part0.split('-');
                thisProId = thisPro[0];
                thisProCount = thisPro[1];
                if (thisProId == productSubId && part2 == weightGold) {
                    thisProCountN = parseInt(quantity) + parseInt(thisProCount);
                } else {
                    thisProCountN = thisProCount;
                }
                strCurrrentCart = strCurrrentCart + '%2c' + thisProId + '-' + thisProCountN + '&' + part1 + '&' + part2 + '&1';
            }

        }
        createCookie('shopping_cart', strCurrrentCart, 1);
    }
    window.location.reload();

}
function deleteFromCartNotWarn(productId, quantity, namePro, attrpart, type) {
    var currentcart = readCookie('shopping_cart');
    var re = "%2c" + productId + '-' + quantity + '&' + attrpart + '&' + type;
    var newcart = currentcart.replace(re, "");
    createCookie('shopping_cart', newcart, 1);
    countShoppingCart("shopping_cart")
    $.ajax({
        url: "/Ajax/Cart/CartAjax",
        cache: false,
        dataType: "html",
        success: function (data) {
            $('#Cart').html(data);
            $('.ttct-a,.xtskt').click(function (e) {
                e.preventDefault();
                $('.popup-sample').fadeIn();
            });
            $("img.lazy").lazyload({ effect: "fadeIn" });
        }
    });
}
$(".wan-spinner input").keypress(function (e) { if (e.which < 48 || e.which > 57) { e.preventDefault(); } });
//keydown
$(".wan-spinner-detail-pro .total-mask").keydown(function (e) {
    var $input = $(this);
    switch (e.which) {
        case 38: upInputVal($input); e.preventDefault(); break;
        case 40: downInputVal($input); e.preventDefault(); break;
    }
});
//xóa giỏ hàng
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery'], factory);
    } else if (typeof exports === 'object') {
        factory(require('jquery'));
    } else { factory(jQuery); }
}(function ($) {
    var pluses = /\+/g;
    function encode(s) { return config.raw ? s : encodeURIComponent(s); }
    function decode(s) { return config.raw ? s : decodeURIComponent(s); }
    function stringifyCookieValue(value) { return encode(config.json ? JSON.stringify(value) : String(value)); }
    function parseCookieValue(s) {
        if (s.indexOf('"') === 0) { s = s.slice(1, -1).replace(/\\"/g, '"').replace(/\\\\/g, '\\'); }
        try { s = decodeURIComponent(s.replace(pluses, ' ')); return config.json ? JSON.parse(s) : s; } catch (e) { }
    }
    function read(s, converter) { var value = config.raw ? s : parseCookieValue(s); return $.isFunction(converter) ? converter(value) : value; }
    var config = $.cookie = function (key, value, options) {
        if (value !== undefined && !$.isFunction(value)) {
            options = $.extend({}, config.defaults, options);
            if (typeof options.expires === 'number') { var days = options.expires, t = options.expires = new Date(); t.setTime(+t + days * 864e+5); }
            return (document.cookie = [encode(key), '=', stringifyCookieValue(value), options.expires ? '; expires=' + options.expires.toUTCString() : '', options.path ? '; path=' + options.path : '', options.domain ? '; domain=' + options.domain : '', options.secure ? '; secure' : ''].join(''));
        }
        var result = key ? undefined : {};//read
        var cookies = document.cookie ? document.cookie.split('; ') : [];
        for (var i = 0, l = cookies.length; i < l; i++) {
            var parts = cookies[i].split('=');
            var name = decode(parts.shift());
            var cookie = parts.join('=');
            if (key && key === name) { result = read(cookie, value); break; }
            if (!key && (cookie = read(cookie)) !== undefined) { result[name] = cookie; }
        }
        return result;
    };
    config.defaults = {};
    $.removeCookie = function (key, options) {
        if ($.cookie(key) === undefined) { return false; }
        $.cookie(key, '', $.extend({}, options, { expires: -1 }));
        return !$.cookie(key);
    };
}));
//voucher
//Check mã giảm giá
function applyVoucher() {
    $("#VorcherCodeSubmit").click(function () {
        var vorchercode = $("#VorcherCode").val();
        if (vorchercode == undefined || vorchercode == '') {
            $.alert({
                title: 'Thông báo',
                type: 'red',
                animation: 'zoom',
                closeAnimation: 'zoom',
                content: 'Vui lòng nhập mã giảm giá của bạn'
            });
        }
        else {
            var urlLoadCheckVorcherCode = "/Ajax/Cart/CheckVorcherCode";
            $.ajax({
                type: "POST",
                url: urlLoadCheckVorcherCode,
                data: { vorchercode: vorchercode },
                success: function (data) {
                    if (data.errorcode == 0) {
                        $("#VoucherCodeCheck").html(data.errormessage);
                    }
                    else {
                        $("#VoucherCodeCheck").html(data.errormessage).addClass('success');
                        $("#VorcherCode").addClass('disabled');
                        $("#VorcherCodeSubmit").hide();
                        $('#CancelVoucher').show();
                        setTimeout(function () {
                            $("#VoucherCodeCheck").html('').removeClass('success');
                        }, 2000);
                        //$.ajax({
                        //    url: "/Ajax/Cart/CartAjax",
                        //    cache: false,
                        //    dataType: "html",
                        //    data: { vorchercode: vorchercode },
                        //    success: function (data) {
                        //        $('#Cart').html(data);
                        //    }
                        //});
                        $.ajax({
                            url: "/Ajax/Cart/TotalPriceCart",
                            cache: false,
                            dataType: "html",
                            data: { vorchercode: vorchercode },
                            success: function (data) {
                                $('#Total').html(data);
                            }
                        });
                    }

                }
            });
        }        
    });
    $("#CancelVoucher").click(function () {
        $("#VorcherCode").val('');
        var urlLoadCheckVorcherCode = "/Ajax/Cart/CheckVorcherCode";
        $.ajax({
            type: "POST",
            url: urlLoadCheckVorcherCode,
            data: { vorchercode: '' },
            success: function (data) {
                $("#VorcherCode").removeClass('disabled');
                $("#VorcherCodeSubmit").show();
                $('#CancelVoucher').hide();
                $.ajax({
                    url: "/Ajax/Cart/TotalPriceCart",
                    cache: false,
                    dataType: "html",
                    data: { vorchercode: '' },
                    success: function (data) {
                        $('#Total').html(data);
                    }
                });
            }
        });
    });
}
$(function () {
    applyVoucher()
});
//Apply mã giảm giá
//uu dai mua kem
function checkProductMk() {
    $(".mk-product .check-mk-product").change(function () {
        count = 0;
        ids = "";
        priceAll = parseInt($("#PriceAll").val());
        priceAllOld = parseInt($("#PriceOldAll").val());
        discountCombo = parseInt($("#DiscountCombo").val());
        countCombo = parseInt($("#CountCombo").val());
        $(".change-pr").text(formatFrice(priceAll, "đ"));
        $(".mk-product .check-mk-product:checked").each(function () {
            count++;
            ids = ids + "," + $(this).val();
            var valPriceTemp = $(this).data('price');
            if (valPriceTemp === undefined || valPriceTemp == "0") {
                valPriceTemp = 0;
            }
            priceAll = priceAll + parseInt(valPriceTemp);
            var valPriceOldTemp = $(this).data('oldprice');
            if (valPriceOldTemp === undefined || valPriceOldTemp == "0") {
                valPriceOldTemp = 0;
            }
            priceAllOld = priceAllOld + parseInt(valPriceOldTemp);

        })
        if (count == countCombo) {
            priceAll = priceAll - discountCombo;
            console.log(priceAll);
            priceAllOld = priceAllOld - discountCombo;
            console.log(priceAllOld);
        }
        $('#mk-price').text(formatFrice(priceAll, "đ"));
        $('#mk-price-old').text(formatFrice(priceAllOld, "đ"));
        $('#count-mk').text(count);
        $("#all-ids-mk").val(ids);
    })
}
function addListToCart() {
    count = 0;
    $(".mk-product .check-mk-product:checked").each(function () {
        count++;
    })
    if (count == 0) {
        $(".alrt-mk").removeClass("hidden");
    } else {
        var productSubId = $("input[name=ContentID]").val();
        var keySearchOldProduct1 = productSubId + '-';
        var ids = $("#all-ids-mk").val();
        var current_cart = readCookie('shopping_cart');
        if (checkExistByKey(current_cart, keySearchOldProduct1, "", "%2c") == 0) {
            ids = ids + "," + productSubId;
        }
        var idSplitResult = ids.split(',');
        for (i = 0; i < idSplitResult.length; i++) {
            if (idSplitResult[i] !== undefined && idSplitResult[i] != '') {
                addToCartAttr(idSplitResult[i], 1, '', '');
            }

        }
        window.location.href = "/gio-hang";
    }
}
$(document).ready(function () {
    $("#apply-voucher").click(function () {
        var allow = $("#apply-voucher").attr("data-allow");
        if (allow == "0") {
            var vorchercode = $("#VorcherCode").val();
            if (vorchercode.length <= 4) {
                Lobibox.alert("info",
                    {
                        msg: "Mã Voucher phải có ít nhất 4 ký tự!"
                    });
            } else {
                $(this).text("Hủy");
                var urlLoadCheckVorcherCode = "/Ajax/Cart/ApplyVorcherCode";
                $.ajax({
                    type: "POST",
                    url: urlLoadCheckVorcherCode,
                    data: { vorchercode: vorchercode },
                    success: function (data) {
                        $("#apply-voucher").attr("data-allow", "1");
                        $("#VoucherApplyHtml").html(data);
                        $("#UseVoucher").val("1");
                    }
                });
            }
        } else {
            $(this).attr("data-allow", "0");
            $(this).text("Áp dụng");
            $("#VorcherCode").val("");
            var urlLoadCheckVorcherCodeCancel = "/Ajax/Cart/ApplyVorcherCodeCancel";
            $.ajax({
                type: "POST",
                url: urlLoadCheckVorcherCodeCancel,
                data: { vorchercode: "" },
                success: function (data) {
                    $("#apply-voucher").attr("data-allow", "0");
                    $("#VoucherApplyHtml").html(data);
                    $("#UseVoucher").val("0");
                }
            });
        }
    });
});
$(function () {
    checkProductMk();
    changeAttrChangePrice();
    $(".item-dsdc").click(function () {
        $(".item-dsdc").removeClass("active");
        $(this).addClass("active");

    })
    $(".act_add").click(function (e) {
        var id = $(this).data("id");
        var name = $(this).data("name");
        var quality = 1;
        var sttr = $(".choose-color span.active").data("id");
        if (sttr === undefined) {
            sttr = "";
        }
        addToCartAttr(id, quality, sttr, "buynow");
        countShoppingCart("shopping_cart")
        //$(".show-noti-cart").removeClass("hidden");
        $("html, body").animate({ scrollTop: 0 }, 2000);
    });

    $(".btn-order-now").click(function (e) {
        //deleteAllCartNoWarning();
        var id = $(this).data("id");
        var name = $(this).data("name");
        var quality = $('#Quantity').val();
        var sttr = $(".choose-color span.active").data("id");
        if (sttr === undefined) {
            sttr = "";
        }
        addToCartAttr(id, quality, sttr, "buynow");
        countShoppingCart("shopping_cart")
        //$(".show-noti-cart").removeClass("hidden");
    });
    countShoppingCart("shopping_cart")

});




