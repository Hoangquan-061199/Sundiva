var lang = Cookies.get('lang');
if (lang == null || lang == undefined) lang = 'vi';
// add the rule here
$.validator.addMethod("valueNotEquals", function (value, element, arg) {
    return arg !== value;
}, "Value must not equal arg.");

function ProcessData() {
    //#region Submit
    this.SendContact = function () {
        let $formContact = $("#SendContact");
        $formContact.validate({
            rules: {
                FullName: { required: true },
                Phone: { required: true, minlength: 10, maxlength: 12 },
                Email: { required: true, email: true },
                Title: { required: true }
            },
            messages: {
                FullName: { required: GetSource("NhapHoTen") },
                Phone: { required: GetSource("NhapSoDienThoai"), minlength: GetSource("Tu10Den12KyTu"), maxlength: GetSource("Tu10Den12KyTu") },
                Email: { required: GetSource("NhapEmail"), email: GetSource("EmailKhongChinhXac") },
                Title: { required: GetSource("NhapTieuDe") }
            },
            submitHandler: function () {
                let d = $formContact.serialize();
                $(".btnSendContact").prop("disabled", true).hide();
                $(".load").addClass('show');
                $("body").addClass('disable');
                let action = $formContact.attr('action');
                $.post(action, d, function (data) {
                    if (data.errors) {
                        $(".btnSendContact").prop("disabled", false).show();
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        OpenAlert(data.message, false);
                        console.log(data.logs);
                    } else {
                        OpenAlert(data.message, true);
                        setInterval(function () { window.location.reload(); }, 3000);
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        $(".btnSendContact").show();
                    }
                }).fail(function () {
                    $(".btnSendContact").prop("disabled", false).show();
                    $(".load").removeClass('show');
                    $("body").removeClass('disable');
                    OpenAlert(GetSource("GuiThatBai"), false);
                });
                return false;
            }
        });
    };
    this.SendBookHotel = function () {
        let $formContact = $("#BookHotel");
        $formContact.validate({
            rules: {
                FullName: { required: true },
                Phone: { required: true, minlength: 10, maxlength: 12 },
                Email: { required: true, email: true },
            },
            messages: {
                FullName: { required: GetSource("NhapHoTen") },
                Phone: { required: GetSource("NhapSoDienThoai"), minlength: GetSource("Tu10Den12KyTu"), maxlength: GetSource("Tu10Den12KyTu") },
                Email: { required: GetSource("NhapEmail"), email: GetSource("EmailKhongChinhXac") },
            },
            submitHandler: function () {
                let d = $formContact.serialize();
                $(".btnBookHotel").prop("disabled", true).hide();
                $(".load").addClass('show');
                $("body").addClass('disable');
                let action = $formContact.attr('action');
                $.post(action, d, function (data) {
                    if (data.errors) {
                        $(".btnBookHotel").prop("disabled", false).show();
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        OpenAlert(data.message, false);
                        console.log(data.logs);
                    } else {
                        OpenAlert(data.message, true);
                        setInterval(function () { window.location.reload(); }, 3000);
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        $(".btnBookHotel").show();
                    }
                }).fail(function () {
                    $(".btnBookHotel").prop("disabled", false).show();
                    $(".load").removeClass('show');
                    $("body").removeClass('disable');
                    OpenAlert(GetSource("GuiThatBai"), false);
                });
                return false;
            }
        });
    };
    this.SendCarRental = function () {
        let $formContact = $("#CarRental");
        $formContact.validate({
            rules: {
                FullName: { required: true },
                Phone: { required: true, minlength: 10, maxlength: 12 },
                Email: { required: true, email: true },
                StartDate: { required: true },
                EndDate: { required: true },
                Destination: { required: true },
                PointGo: { required: true },
                Service: { valueNotEquals: "0" }
            },
            messages: {
                FullName: { required: GetSource("NhapHoTen") },
                Phone: { required: GetSource("NhapSoDienThoai"), minlength: GetSource("Tu10Den12KyTu"), maxlength: GetSource("Tu10Den12KyTu") },
                Email: { required: GetSource("NhapEmail"), email: GetSource("EmailKhongChinhXac") },
                StartDate: { required: "" },
                EndDate: { required: "" },
                Destination: { required: "" },
                PointGo: { required: "" },
                Service: { valueNotEquals: "" }

            },
            submitHandler: function () {
                updateDateDDMMYYYY();
                let d = $formContact.serialize();
                $(".btnCarRental").prop("disabled", true).hide();
                $(".load").addClass('show');
                $("body").addClass('disable');
                let action = $formContact.attr('action');
                $.post(action, d, function (data) {
                    if (data.errors) {
                        $(".btnCarRental").prop("disabled", false).show();
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        OpenAlert(data.message, false);
                        console.log(data.logs);
                    } else {
                        OpenAlert(data.message, true);
                        setInterval(function () { window.location.reload(); }, 3000);
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        $(".btnCarRental").show();
                    }
                }).fail(function () {
                    $(".btnCarRental").prop("disabled", false).show();
                    $(".load").removeClass('show');
                    $("body").removeClass('disable');
                    OpenAlert(GetSource("GuiThatBai"), false);
                });
                return false;
            }
        });
    };
    this.SendBookTour = function () {
        let $formContact = $("#BookTour");
        $formContact.validate({
            rules: {
                FullName: { required: true },
                Phone: { required: true, minlength: 10, maxlength: 12 },
                Email: { required: true, email: true },
            },
            messages: {
                FullName: { required: GetSource("NhapHoTen") },
                Phone: { required: GetSource("NhapSoDienThoai"), minlength: GetSource("Tu10Den12KyTu"), maxlength: GetSource("Tu10Den12KyTu") },
                Email: { required: GetSource("NhapEmail"), email: GetSource("EmailKhongChinhXac") },
            },
            submitHandler: function () {
                updateDateDDMMYYYY();
                let d = $formContact.serialize();
                $(".book-tour-submit").prop("disabled", true).hide();
                $(".load").addClass('show');
                $("body").addClass('disable');
                let action = $formContact.attr('action');
                $.post(action, d, function (data) {
                    if (data.errors) {
                        $(".book-tour-submit").prop("disabled", false).show();
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        OpenAlert(data.message, false);
                        console.log(data.logs);
                    } else {
                        OpenAlert(data.message, true);
                        setInterval(function () { window.location.reload(); }, 3000);
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        $(".book-tour-submit").show();
                    }
                }).fail(function () {
                    $(".book-tour-submit").prop("disabled", false).show();
                    $(".load").removeClass('show');
                    $("body").removeClass('disable');
                    OpenAlert(GetSource("GuiThatBai"), false);
                });
                return false;
            }
        });
    };
    this.SendContactOrder = function () {
        let $formContact = $("#contact-order");
        $formContact.validate({
            rules: {
                FullName: { required: true },
                Phone: { required: true, minlength: 10, maxlength: 12 },
                Email: { required: true, email: true },
                Number: { required: true },
            },
            messages: {
                FullName: { required: GetSource("VuiLongNhapDayDuHoTen") },
                Phone: { required: GetSource("VuiLongNhapSoDienThoai"), minlength: GetSource("Tu10Den12KyTu"), maxlength: GetSource("Tu10Den12KyTu") },
                Email: { required: GetSource("VuiLongNhapEmail"), email: GetSource("EmailKhongChinhXac") },
                Number: { required: GetSource("VuiLongNhapSoLuong") },
            },
            submitHandler: function () {
                let d = $formContact.serialize();
                $(".btn-submit-order").prop("disabled", true).hide();
                $(".load").addClass('show');
                $("body").addClass('disable');
                let action = $formContact.attr('action');
                $.post(action, d, function (data) {
                    if (data.errors) {
                        $(".btn-submit-order").prop("disabled", false).show();
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        OpenAlert(data.message, false);
                        console.log(data.logs);
                    } else {
                        OpenAlert(data.message, true);
                        setInterval(function () { window.location.reload(); }, 3000);
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        $(".btn-submit-order").show();
                    }
                }).fail(function () {
                    $(".btn-submit-order").prop("disabled", false).show();
                    $(".load").removeClass('show');
                    $("body").removeClass('disable');
                    OpenAlert(GetSource("GuiThatBai"), false);
                });
                return false;
            }
        });
    };
    this.SendTuVan = function () {
        var $formTuVan = $("#SendTuVan");
        $formTuVan.validate({
            rules: {
                FullName: { required: true },
                Address: { required: true },
                Phone: { required: true, minlength: 10, maxlength: 12 },
                Email: { required: true, email: true },
            },
            messages: {
                FullName: { required: GetSource("VuiLongNhapDayDuHoTen") },
                Address: { required: GetSource("VuiLongNhapDiaChi") },
                Phone: { required: GetSource("VuiLongNhapSoDienThoai"), minlength: GetSource("Tu10Den12KyTu"), maxlength: GetSource("Tu10Den12KyTu") },
                Email: { required: GetSource("VuiLongNhapEmail"), email: GetSource("EmailKhongChinhXac") },
            },
            submitHandler: function () {
                var d = $formTuVan.serialize();
                $(".btnSendTuVan").prop("disabled", true).hide();
                $(".load").addClass('show');
                $("body").addClass('disable');
                var action = $formTuVan.attr('action');
                $.post(action, d, function (data) {
                    if (data.errors) {
                        $(".btnSendTuVan").prop("disabled", false).show();
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        OpenAlert(data.message, false);
                        console.log(data.logs);
                    } else {
                        OpenAlert(data.message, true);
                        setInterval(function () { window.location.reload(); }, 3000);
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        $(".btnSendTuVan").show();
                    }
                }).fail(function () {
                    $(".btnSendTuVan").prop("disabled", false).show();
                    $(".load").removeClass('show');
                    $("body").removeClass('disable');
                    OpenAlert(GetSource("GuiThatBai"), false);
                });
                return false;
            }
        });
    };

    this.SendBaoGia = function () {
        var $formBaoGia = $("#SendBaoGia");
        $formBaoGia.validate({
            rules: {
                FullName: { required: true },
                Phone: { required: true, minlength: 10, maxlength: 12 },
                Email: { required: true, email: true },
            },
            messages: {
                FullName: { required: GetSource("VuiLongNhapDayDuHoTen") },
                Phone: { required: GetSource("VuiLongNhapSoDienThoai"), minlength: GetSource("Tu10Den12KyTu"), maxlength: GetSource("Tu10Den12KyTu") },
                Email: { required: GetSource("VuiLongNhapEmail"), email: GetSource("EmailKhongChinhXac") },
            },
            submitHandler: function () {
                var d = $formBaoGia.serialize();
                $(".btnSendBaoGia").prop("disabled", true).hide();
                $(".load").addClass('show');
                $("body").addClass('disable');
                var action = $formBaoGia.attr('action');
                $.post(action, d, function (data) {
                    if (data.errors) {
                        $(".btnSendBaoGia").prop("disabled", false).show();
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        OpenAlert(data.message, false);
                        console.log(data.logs);
                    } else {
                        OpenAlert(data.message, true);
                        setInterval(function () { window.location.reload(); }, 3000);
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        $(".btnSendBaoGia").show();
                    }
                }).fail(function () {
                    $(".btnSendBaoGia").prop("disabled", false).show();
                    $(".load").removeClass('show');
                    $("body").removeClass('disable');
                    OpenAlert(GetSource("GuiThatBai"), false);
                });
                return false;
            }
        });
    };

    //#region Submit
    this.SendApply = function () {
        var $formContact = $("#Apply");
        $formContact.validate({
            rules: {
                FullName: { required: true },
                Email: { required: true, email: true },
                Phone: { required: true, minlength: 10, maxlength: 12 }
            },
            messages: {
                FullName: { required: GetSource("VuiLongNhapDayDuHoTen") },
                Email: { required: GetSource("VuiLongNhapEmail"), email: GetSource("EmailKhongChinhXac") },
                Phone: { required: GetSource("VuiLongNhapSoDienThoai"), minlength: GetSource("Tu10Den12KyTu"), maxlength: GetSource("Tu10Den12KyTu") }
            },
            submitHandler: function () {
                $(".btnSendContact").hide();
                $(".load").addClass('show');
                $("body").addClass('disable');
                $formContact.ajaxSubmit({
                    success: function (data) {
                        if (data.errors === false) {
                            $formContact[0].reset();
                            OpenAlert(data.message, true);
                            setInterval(function () { window.location.reload(); }, 5000);
                            $(".load").removeClass('show');
                            $("body").removeClass('disable');
                            $(".btnSendContact").show().prop('disabled', true);
                        }
                        else {
                            $(".btnSendContact").show();
                            $(".load").removeClass('show');
                            $("body").removeClass('disable');
                            OpenAlert(data.message, false);
                            console.log(data.logs);
                        }
                    },
                    error: function () {
                        $(".btnSendContact").prop("disabled", false).show();
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        OpenAlert(GetSource("GuiThatBai"), false);
                    }
                });
                return false;
            }
        });
    };
    this.SendRequestPopup = function () {
        var $formContact = $("#SendRequestPopup");
        if (Jsonconfig.hasOwnProperty("IsFullName") && Jsonconfig['IsFullName'] == true) {
            $.validator.addMethod("nRequired", $.validator.methods.required, "Họ tên bắt buộc nhập!");
            $.validator.addClassRules("fullname", { nRequired: true });
        }
        if (Jsonconfig.hasOwnProperty("IsPhone") && Jsonconfig['IsPhone'] == true) {
            $.validator.addMethod("pRequired", $.validator.methods.required, "Số điện thoại bắt buộc nhập!");
            $.validator.addClassRules("phone", { pRequired: true });
        }
        if (Jsonconfig.hasOwnProperty("IsAddress") && Jsonconfig['IsAddress'] == true) {
            $.validator.addMethod("aRequired", $.validator.methods.required, "Địa chỉ bắt buộc nhập!");
            $.validator.addClassRules("address", { aRequired: true });
        }
        if (Jsonconfig.hasOwnProperty("IsEmail") && Jsonconfig['IsEmail'] == true) {
            $.validator.addMethod("eRequired", $.validator.methods.required, "Email bắt buộc nhập!");
            $.validator.addMethod("eEmail",
                function (value, element) {
                    return /^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/.test(value);
                },
                "Email không đúng"
            );
            $.validator.addClassRules("email", { eRequired: true, eEmail: true });
        }
        if (Jsonconfig.hasOwnProperty("IsCity") && Jsonconfig['IsCity'] == true) {
            $.validator.addMethod("cRequired", $.validator.methods.required, "Tỉnh/Thành bắt buộc chọn!");
            $.validator.addClassRules("city", { cRequired: true });
        }
        if (Jsonconfig.hasOwnProperty("IsDistrict") && Jsonconfig['IsDistrict'] == true) {
            $.validator.addMethod("dRequired", $.validator.methods.required, "Quận/Huyện bắt buộc chọn!");
            $.validator.addClassRules("district", { dRequired: true });
        }
        if (Jsonconfig.hasOwnProperty("IsContent") && Jsonconfig['IsContent'] == true) {
            $.validator.addMethod("ctRequired", $.validator.methods.required, "Nội dung bắt buộc nhập!");
            $.validator.addClassRules("content", { ctRequired: true });
        }
        $formContact.validate({
            submitHandler: function () {
                var d = $formContact.serialize();
                $(".btnSendContact").hide();
                $(".load").show();
                var action = $formContact.attr('action');
                var token = $('input[name="__RequestVerificationToken"]', $formContact).val();
                $.post(action, d, function (data) {
                    if (data.errors) {
                        $(".btnSendContact").show();
                        $(".load").hide();
                        $('.alrt-contact').html(data.message).slideDown().delay(10000).slideUp();
                    } else {
                        $formContact[0].reset();
                        $('.alrt-contact').html(data.message).addClass('text-success').slideDown();
                        setInterval(function () { window.location.reload(); }, 1000);
                        $(".load").hide();
                        $(".btnSendContact").show().attr('disabled', 'disabled');
                    }
                });
                return false;
            }
        });
    };
    //#endregion
    //#region Member
    this.SendCode = function () {
        var $formContact = $("#SendCode");
        $formContact.validate({
            rules: {
                Email: { required: true, email: true }
            },
            submitHandler: function () {
                var d = $formContact.serialize();
                $(".btnSendContact").hide();
                $(".sending").show();
                var action = $formContact.attr('action');
                $.post(action, d, function (data) {
                    if (data.errors) {
                        $('.alrt').addClass('alert-error').html(data.message);
                        $(".btnSendContact").show();
                        $(".sending").hide();
                        setInterval(function () { $('.alrt-1').slideUp().removeClass('alert-error').html(''); }, 1500);
                    } else {
                        $('.alrt').addClass('alert-success').html(data.message);
                        $(".btnSendContact").show();
                        $(".sending").hide();
                        $('#Email').val(data.email);
                        $formContact.hide();
                        $('#ForgotPassword').show();
                        setInterval(function () { $('.alrt-1').slideUp().removeClass('alert-success').html(''); }, 1500);
                    }
                });
                return false;
            }
        });
    };
    this.ForgotPassword = function () {
        var $formContact = $("#ForgotPassword");
        $formContact.validate({
            rules: {
                Email: { required: true, email: true },
                Code: { required: true },
                NewPassword: { required: true },
                ConfirmPassword: { required: true, equalTo: "#NewPassword" }
            },
            submitHandler: function () {
                var d = $formContact.serialize();
                $(".btnSendContact").hide();
                $(".sending").show();
                var action = $formContact.attr('action');
                $.post(action, d, function (data) {
                    if (data.errors) {
                        $('.alrt-2').addClass('alert-error').html(data.message);
                        $(".btnSendContact").show();
                        $(".sending").hide();
                        setInterval(function () { $('.alrt-2').slideUp().removeClass('alert-error').html(''); }, 1500);
                    } else {
                        $('.alrt-2').addClass('alert-success').html(data.message);
                        $(".btnSendContact").show();
                        $(".sending").hide();
                        setInterval(function () { window.location.href = "/sign-in"; }, 5000);
                    }
                });
                return false;
            }
        });
    };
    //#endregion
    this.LoadResultSearch = function () {
        $(".pagination ul li span").click(function (e) {
            e.preventDefault();
            var page = $(this).data('page');
            var q = $("#Query").val();
            if (page > 0) {
                loadAjax("/Ajax/Content/ListSearch?q=" + q + "&page=" + page, "#Result");
            }
        });
    };
    //#region LoadData
    this.LoadMoreAjax = function (urlContent, container, type) {
        $(container).append(TemplateLoading());
        $.ajax({
            url: encodeURI(urlContent),
            cache: false,
            type: "POST",
            success: function (data) {
                var html = ``;
                if (type === 'Product') {
                    if (data.listProductItem.length > 0) {
                        html += PartialProduct(data);
                    }
                    else {
                        $('.Total').html(0);
                    }
                }
                else {
                    if (data.listContentItem.length > 0) {
                        html += PartialNews(data);
                    }
                }
                if (data.page === 1) {
                    $(container).html(html);
                } else {
                    $(container).append(html);
                }
                $(container).find('.hidden').fadeIn().removeClass('hidden');
                $(container).children('.load').remove();
            },
            errors: function () {
            }
        });
    };
    this.LoadHtml = function (urlContent, container) {
        $.ajax({
            url: encodeURI(urlContent),
            cache: false,
            type: "POST",
            success: function (data) {
                $(container).html(data);
            },
            errors: function () {
            }
        });
    };
    //#endregion
};

$(function () {
    var e = $("#ReceiverEmailFrm");
    $("#btnReceiverEmail").click(function () {
        $("#btnReceiverEmail").attr("disabled", !0), e.valid() ? e.submit() : $("#btnReceiverEmail").attr("disabled", !1);
        console.log("0")
    }),
        e.validate({
            rules: { Email: { required: true, email: true }, },
            messages: { Email: { required: GetSource("VuiLongNhapEmail"), email: GetSource("EmailKhongChinhXac") }, },
            submitHandler: function () {
                $.post("/Ajax/Home/ReceiverEmail", e.serialize(), function (e) {
                    e.errors
                        ? OpenAlert(e.message, !1)
                        : (OpenAlert(e.message, !0),
                            setTimeout(function () {
                                null != e.url ? (window.location.href = e.url) : window.location.reload();
                            }, 2e3));
                });
            },
        });
});
//#region Utility
function loadAjax(urlContent, container) {
    // $(container).append(TemplateLoading);
    // $("html,body").animate({ scrollTop: $(container).offset().top - 200 }, 1e3);
    $.ajax({
        url: encodeURI(urlContent),
        cache: false,
        type: "POST",
        success: function (data) {
            $(container).html(data);
            $(container).children('.list-search').find('.hidden').removeClass('hidden').fadeIn();
        }
    });
}
function TemplateLoading() { return '<div class="load text-center"><img src="/html/style/images/loading.svg" /><p>' + GetSource('DangTai') + '</p></div>' }
function CheckLink(n, i) { return "" !== i && null != i ? i : "" == n || null != i && "" != i ? "javascript:void(0)" : "/" + n }
function formatDate(t) { if (null == t) return ""; t = new Date(t); return t.getDate() + "/" + (t.getMonth() + 1) + "/" + t.getFullYear() }
function formatFrice(n, e) { return null == n ? "Liên hệ" : n.toFixed(0).replace(/./g, function (n, e, r) { return 0 < e && "." !== n && (r.length - e) % 3 == 0 ? "," + n : n }) + e }
//#endregion
$(function () {
    $('.menu li a').each(function () {
        var link = window.location.origin + window.location.pathname;
        if (this.href.trim() === link)
            $(this).addClass('active');
    });
    $('.sub-menu li a').each(function () {
        var link = window.location.origin + window.location.pathname;
        if (this.href.trim() === link)
            $(this).parents('.drop-down li a').addClass('active');
    });
});

//#region Call function
var process = new ProcessData();
$(function () {
    //if ($("#Apply").length > 0) {
    //    process.SendApply();
    //}
    //$('.lang-vi').click(function (e) {
    //    e.preventDefault();
    //    if ('vi' === lang) return;
    //    else {
    //        Cookies.set('lang', 'vi', { expires: 1 });
    //        window.location.href = "/";
    //    }
    //});
    //$('.lang-en').click(function (e) {
    //    e.preventDefault();
    //    if ('en' === lang) return;
    //    else {
    //        Cookies.set('lang', 'en', { expires: 1 });
    //        window.location.href = "/";
    //    }
    //});
    //$('.lang-de').click(function (e) {
    //    e.preventDefault();
    //    if ('de' === lang) return;
    //    else {
    //        Cookies.set('lang', 'de', { expires: 1 });
    //        window.location.href = "/";
    //    }
    //});
    //let ck = Cookies.get('lang');
    //let dataLang = $('body').attr('data-lang');

    //if (ck !== dataLang && dataLang !== '' && dataLang !== undefined && dataLang !== null) {
    //    Cookies.set('lang', dataLang, { expires: 1 });
    //    window.location.reload();
    //}
    //if (ck == undefined)
    //    ck = 'en';
    //let current = $('html').attr('lang');
    //if (ck != current && current !== undefined && current !== null) {
    //    window.location.reload();
    //}
});
//#region hidden
var Resource = new Object();
LoadResource();
function LoadResource() { var o = "/DataJson/Resource/Resources_" + lang + ".json"; $.ajax({ url: o, dataType: "json", async: !1, success: function (o) { Resource = o }, error: function (o) { console.log("Dữ liệu không tồn tại") } }) }
function GetSource(e) { var r = Resource[e]; return null != r ? r : "[" + e + "]" }
var getUrlParameter = function (r) { for (var t, e = window.location.search.substring(1).split("&"), n = 0; n < e.length; n++)if ((t = e[n].split("="))[0] === r) return t[1], decodeURIComponent(t[1]); return !1 };
function RemoveUnicode(e) { return e = (e = (e = (e = (e = (e = (e = (e = (e = (e = (e = (e = e.toLowerCase()).replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a")).replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e")).replace(/ì|í|ị|ỉ|ĩ/g, "i")).replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o")).replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u")).replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y")).replace(/đ/g, "d")).replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'| |\"|\&|\#|\[|\]|~|$|_|–|”|“|`/g, "-")).replace(/\s+/g, "-")).replace(/-+-/g, "-")).replace(/^\-+|\-+$/g, "") }
function removeTagHTML(str) {
    if ((str === null) || (str === ''))
        return '';
    else
        str = str.toString();
    return str.replace(/(<([^>]+)>)/ig, '');
}
function resizeImage(e, s) {
    s = parseInt($(e).width()) * s;
    $(e).css({ height: s + "px" });
}

function getValueFormMutilSelect(t) { var e, i = ""; return $(t).find("input[type='checkbox']:checked, input[type='radio']:checked, input[type='text'],input[type='number'], input[type='hidden'], select").each(function () { e = $(this).attr("name"), "" != $(this).val() && (i += "&" + e + "=" + $(this).val()) }), "" != i && (i = i.substring(1)), i }
function getparram(t) { var n = new Object; return $(t).find("input[type='checkbox']:checked, input[type='radio']:checked, input[type='text'],input[type='number'], input[type='hidden'], select").each(function () { var t = $(this).attr("name"), e = $(this).val(); "" != $(this).val() && "" != t && null != t && (n[t] = e) }), n }
//function TemplateLoading() { return '<div class="load show text-center"><img src="/html/style/image2022/loading.svg" alt="' + GetSource('DangTai') + '" /><p>' + GetSource('DangTai') + '</p></div>' }
function formatPrice(n, e) { return null == n ? "Liên hệ" : n.toFixed(0).replace(/./g, function (n, e, r) { return 0 < e && "." !== n && (r.length - e) % 3 == 0 ? "," + n : n }) + e }
function OpenVideo(url) {
    var html = ``;
    if (url != '') {
        html = `<div class="popup-vd">`;
        if (url.includes('images/')) {
            html += `<div class="ifr-tv"><video width="400" autoplay controls><source src="` + url + `" type="video/mp4"></video></div><div class="bgblack"></div><div class="close-pu"></div>`;
        } else {
            var src = "https://www.youtube.com/embed/" + url + "?autoplay=1";
            html += `<div class="ifr-tv"><iframe src="` + src + `" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe></div><div class="bgblack"></div><div class="close-pu"></div>`;
        }
        html += `<script>$(".bgblack, .close-pu").click(function () { $('body').find(".popup-vd").remove();$('body').removeClass('nonescroll'); });</script></div>`;
        $('body').append(html);
        $('body').addClass('nonescroll');
        $('.popup-vd').addClass('active');
    }
}
function AlertError(r) { window.alert(r) }
function OpenAlert(msg, success) {
    $('.alrt-popup .main').html(msg);
    success == true ? $('.alrt-popup').addClass('success') : $('.alrt-popup').removeClass('success');
    $('.alrt-popup,.overlay').addClass('show');
    $('.close-alrt').click(function () {
        CloseAlert();
    });
}
function CloseAlert() {
    $('.alrt-popup').removeClass('success');
    $('.alrt-popup .main').html('');
    $('.alrt-popup,.overlay').removeClass('show');
}
//#endregion
function LoadContentIndex(id, pos) {
    $('.load-content-index-' + pos).append(TemplateLoading());
    var url = '/Ajax/Content/PartialContentIndex?moduleId=' + id + "&type=" + pos;
    setTimeout(function () {
        $.ajax({
            url: encodeURI(url),
            cache: true,
            type: "POST",
            async: true,
            dataType: 'html',
            success: function (data) {
                $('.news ul li .item-type').removeClass('active');
                $('.news ul li .item-type[data-module=' + id + ']').addClass('active');
                $('.load-content-index-' + pos).html(data);
                $('.load-content-index-' + pos).children('.load').remove();
                resizeImage('.load-content-index-' + pos + '.news-cnt .left.news-item .img', 385 / 592);
                resizeImage('.load-content-index-' + pos + ' .right .news-item .img', 181 / 282);
            },
            errors: function () {
                $('.load-content-index-' + pos).children('.load').remove();
                OpenAlert(GetSource('TaiKhongThanhCong'), false);
            }
        });
    }, 500);
}
function LoadProductIndex(id, pos) {
    var url = '/Ajax/Content/PartialProductIndex?moduleId=' + id + "&type=" + pos;
    $('.load-product-index-' + pos).append(TemplateLoading());
    $.ajax({
        url: encodeURI(url),
        cache: true,
        type: "POST",
        async: true,
        dataType: 'html',
        success: function (data) {
            $('.load-product-index-' + pos).find('.load').remove();
            $('.load-product-index-' + pos).html(data);
        },
        errors: function () {
        }
    });
}
function LoadPreview(id, n) {
    var url = '/Ajax/Content/ViewerPdf?contentId=' + id + '&type=' + n;
    $.ajax({
        url: encodeURI(url),
        cache: false,
        type: "POST",
        dataType: 'html',
        success: function (data) {
            $('.load_preview').html(data);
        },
        errors: function () {
        }
    });
}
$('input.valid').on('keyup', function (e) {
    e.preventDefault();
    var old = $(this).val();
    old = old.replace(/!|\$|\\|\{|\}|\||%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\:|\;|\'|\"|\&|\#|\[|\]|~|$|”|“|`/g, "");
    old = old.replace(/-+-/g, "");
    $(this).val(old);
});
$('textarea.valid').on('keyup', function (e) {
    e.preventDefault();
    var old = $(this).val();
    old = old.replace(/!|\$|\\|\{|\}|\||@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\:|\;|\'|\"|\&|\#|\[|\]|~|$|”|“|`/g, "");
    old = old.replace(/-+-/g, "");
    $(this).val(old);
});
$('textarea.valid-comment').on('keyup', function (e) {
    e.preventDefault();
    var old = $(this).val();
    old = old.replace(/!|\$|\\|\{|\}|\||@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\:|\;|\'|\"|\&|\#|\[|\]|~|$|”|“|`/g, "");
    old = old.replace(/-+-/g, "");
    $(this).val(old);
});
$('input.email').on('keyup', function (e) {
    e.preventDefault();
    var old = $(this).val();
    old = old.replace(/!|\$|\\|\{|\}|\||%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\:|\;|\'|\"|\&|\#|\[|\]|~|$|”|“|`/g, "");
    old = old.replace(/-+-/g, "");
    $(this).val(old);
});
$('input.number').on('keyup', function () {
    var old = $(this).val();
    old = old.replace(/\D/g, '');
    $(this).val(old);
})
$('input.fullname').on('keyup', function (e) {
    e.preventDefault();
    var old = $(this).val();
    old = old.replace(/!|\$|\\|\{|\}|\||@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\:|\;|\'|\"|\&|\#|\[|\]|~|$|_|–|”|“|`/g, "");
    old = old.replace(/-+-/g, "");
    old = old.replace(/0|1|2|3|4|5|6|7|8|9/g, '');
    old = old.replace(/^\-+|\-+$/g, "");
    $(this).val(old);
});
$('input.link').on('keyup', function (e) {
    e.preventDefault();
    var old = $(this).val();
    var newval = removeTagHTML(old);
    newval = newval.replace(/!|\$|\\|\{|\@|\}|\^|\*|\(|\)|\+|\<|\>|,|\;|\'|\"|\[|\]|~|$|”|“|`/g, "");
    $(this).val(newval);
});

// Format a number n using: 
//   p decimal places (two by default)
//   ts as the thousands separator (comma by default) and
//   dp as the  decimal point (period by default).
//
//   If p < 0 or p > 20 results are implementation dependent.
function formatNumber(n, p, ts, dp) {
    var t = [];
    // Get arguments, set defaults
    if (typeof p == 'undefined') p = 2;
    if (typeof ts == 'undefined') ts = '.';
    if (typeof dp == 'undefined') dp = ',';

    // Get number and decimal part of n
    n = Number(n).toFixed(p).split('.');

    // Add thousands separator and decimal point (if requied):
    for (var iLen = n[0].length, i = iLen ? iLen % 3 || 3 : 0, j = 0; i <= iLen; i += 3) {
        t.push(n[0].substring(j, i));
        j = i;
    }
    // Insert separators and return result
    return t.join(ts) + (n[1] ? dp + n[1] : '');
}


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

function DateViNow(id) { $(id).datetimepicker({ format: 'd/m/Y', timepicker: false, minDate: 0 }); }