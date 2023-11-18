function paymentReceiveF() {
    $("#CityID").prop('required', true);
    $("#DistrictID").prop('required', true);
    $("#WardID").prop('required', true);
    $("#addr").prop('required', true);
}
function TimeReceive() {
    $("#other-day-re").click(function () {
        $("#timereceivecheck").val("2");
    })
    $(".selectdayreal span").click(function () {
        $(".selectdayreal span").removeClass("active");
        $(this).addClass("active");
        var data = $(this).data("day");
        $("#dayreceive").val(data);
        var text = $(this).text();
        $("#day-re-real").text(text);

    })
}
function selectPaymentReceive(val, ele) {
    $(".chostrans .transorhere a").removeClass("active");
    ele.addClass("active");
    $("#paymentreceive").val(val);
    $(".receive-pl").addClass("hide-receive");
    if (val == "1") {
        //giao hang tan noi
        $("#atHome").removeClass("hide-receive");

        $("#CityID").prop('required', true);
        $("#DistrictID").prop('required', true);
        $("#WardID").prop('required', true);
        $("#addr").prop('required', true);

        $("#AreaAgencyParentID").prop('required', false);
        $("#AreaAgencyChildID").prop('required', false);
        $("#store").prop('required', false);
    } else {
        $("#atStore").removeClass("hide-receive");
        $("#CityID").prop('required', false);
        $("#DistrictID").prop('required', false);
        $("#WardID").prop('required', false);
        $("#addr").prop('required', false);

        $("#AreaAgencyParentID").prop('required', true);
        $("#AreaAgencyChildID").prop('required', true);
        $("#store").prop('required', true);
    }
}
//city
function ChageCityPayment() {
    $("#CityID").change(function () {
        var val = $(this).val();
        if (val == undefined || val == "") {
            LoadDistrict("");
            LoadWard("");
        }
        else {
            LoadDistrict(val);
            LoadWard("");
        }
    });
}
function ChageDistrictPayment() {
    $("#DistrictID").change(function () {
        var val = $(this).val();
        if (val == undefined || val == "") {
            LoadWard("");
        }
        else {
            LoadWard("");
            LoadWard(val);
        }
    });
}
function LoadDistrict(id) {
    if (id != "") {
        $("#DistrictID").removeAttr("disabled");
        $.ajax({
            type: "POST",
            url: "/Ajax/Cart/LoadDistrict",
            dataType: "text",
            data: { id: id },
            success: function (data) {
                $('#DistrictID').html(data);
            }
        });
    }
    else {
        $("#DistrictID").attr("disabled", "true");
        $('#DistrictID').html("<option value=\"\">Chọn Quận/Huyện</option>");
    }
}
function LoadWard(id) {
    if (id != "") {
        $("#WardID").removeAttr("disabled");
        $.ajax({
            type: "POST",
            url: "/Ajax/Cart/LoadWard",
            dataType: "text",
            data: { id: id },
            success: function (data) {
                $('#WardID').html(data);
            }
        });
    }
    else {
        $("#WardID").attr("disabled", "true");
        $('#WardID').html("<option value=\"\">Chọn Phường/Xã</option>");
    }
}
function SendPayment() {
    var $formPayment = $("#SendOrder");
    $formPayment.validate({
        rules: {
            gen: { required: true },
            fullname: { required: true },
            paymentmobile: { required: true, number: true, minlength: 10 },
            paymentEmail: { required: true, email: true },
            CityID: { required: true },
            DistrictID: { required: true },
            WardID: { required: true },
            paymentadd: { required: true }
        },
        messages: {
            gen: { required: "Vui lòng chọn" },
            fullname: { required: "Vui lòng nhập họ và tên" },
            paymentmobile: { required: "Vui lòng nhập số điện thoại", number: "Vui lòng nhập số", minlength: "Độ dài tối thiểu 10 số" },
            paymentEmail: { required: "Vui lòng nhập email", email: "vui long nhập đúng định dạng email" },
            CityID: { required: "Vui lòng chọn Tỉnh/TP" },
            DistrictID: { required: "Vui lòng chọn Quận/Huyện" },
            WardID: { required: "Vui lòng chọn Phường/Xã" },
            paymentadd: { required: "Vui lòng nhập địa chỉ" },
        },
        submitHandler: function () {
            var d = $formPayment.serialize();
            $(".btnSendOrder").hide();
            $(".btnSendOrder-1").show().addClass("pd-le-35");
            $(".spin-payment").show();
            $.post("/Ajax/Cart/SendPayment", d, function (data) {
                if (data.errors) {
                    $(".btnSendOrder").show();
                    $(".btnSendOrder-1").hide();
                    $('.payment-error').html(data.message).show();
                    setInterval(function () { $('.payment-error').hide(); }, 1000);
                    //noty({
                    //    layout: 'top',
                    //    theme: 'bootstrapTheme',
                    //    text: data.message,
                    //    type: 'error',
                    //    timeout: 10000,

                    //    animation: {
                    //        open: { height: 'toggle' }, // or Animate.css class names like: 'animated bounceInLeft'
                    //        close: { height: 'toggle' }, // or Animate.css class names like: 'animated bounceOutLeft'
                    //        easing: 'swing',
                    //        speed: 500 // opening & closing animation speed
                    //    },
                    //});
                } else {
                    $.removeCookie('shopping_cart');
                    window.location.href = "/thong-tin-dat-hang/" + data.id;
                    //setInterval(function () { window.location.reload(); }, 1000);
                }
            });
            return false;
        }
    });
};
function ChangeAreaAgencyParent() {
    $("#AreaAgencyParentID").change(function (e) {
        var pa = $(this).val();
        $.ajax({
            url: encodeURI("/Ajax/Stores/ChangeCityPayment"),
            data: ({ pa: parseInt(pa) }),
            cache: false,
            type: "POST",
            success: function (data) {
                $("#AreaAgencyChildID").removeAttr("disabled").html(data);
                ChangeAreaAgencyChild();
            }
        });

    });
}
function ChangeAreaAgencyChild() {
    $("#AreaAgencyChildID").change(function (e) {
        var pa = $(this).val();
        $.ajax({
            url: encodeURI("/Ajax/Stores/StoreByCityIdPayment"),
            data: ({ cityPa: parseInt(pa) }),
            cache: false,
            type: "POST",
            success: function (data) {
                $("#store").removeAttr("disabled").html(data);
            }
        });
    });
}
$(document).ready(function () {
    $(".anotheroption ul li label.otherreceive").click(function () {
        var active = $(this).data("active");
        if (active == "0") {
            $(this).data("active", "1");
            $(this).addClass("active");
            $("#otherreceive").val("1");
            $(".infouser").fadeIn();
            $("#ReceivePerName").prop('required', true);
            $("#ReceivePerAdd").prop('required', true);
        } else {
            $(this).data("active", "0");
            $(this).removeClass("active");
            $("#otherreceive").val("0");
            $(".infouser").fadeOut();
            $("#ReceivePerName").prop('required', false);
            $("#ReceivePerAdd").prop('required', false);
        }

    });
    $(".anotheroption ul li label.billcompany").click(function () {
        var active = $(this).data("active");
        if (active == "0") {
            $(this).data("active", "1");
            $(this).addClass("active");
            $("#IsExportBill").val("1");
            $(".infocompany").fadeIn();
            $("#PhoneBill").prop('required', true);
            $("#EmailBill").prop('required', true);
            $("#FullNameBill").prop('required', true);
            $("#EmailBill1").prop('required', true);

        } else {
            $(this).data("active", "0");
            $(this).removeClass("active");
            $("#IsExportBill").val("0");
            $(".infocompany").fadeOut();
            $("#PhoneBill").prop('required', false);
            $("#EmailBill").prop('required', false);
            $("#FullNameBill").prop('required', false);
        }

    });
    $("#IsExportBill").change(function () {

        if ($("#IsExportBill").is(':checked')) {

            $("#FullNameBill").prop('required', true);
            $("#PhoneBill").prop('required', true);
            $("#EmailBill").prop('required', true);
        } else {
            $("#FullNameBill").prop('required', false);
            $("#PhoneBill").prop('required', false);
            $("#EmailBill").prop('required', false);
        }
    })
    paymentReceiveF();
    ChangeAreaAgencyParent();
    ChangeAreaAgencyChild();
    ChageCityPayment();
    ChageDistrictPayment();
    SendPayment();
    TimeReceive();
    //$('#dateofbirth').datepicker({
    //    calendarWeeks: true,
    //    todayHighlight: true,
    //    autoclose: true,
    //    format: 'dd/mm/yyyy',
    //    startView: 2,
    //    endDate: new Date(new Date().setDate(new Date().getDate()))
    //});
    //$('#timereceive').datetimepicker({
    //    autoclose: true,
    //    format: 'dd/mm/yyyy hh:ii',
    //    startDate: "+1d",
    //});
    $('input[name ="timereceivecheck"]').change(function () {
        var val = $('input[name ="timereceivecheck"]:checked').val();
        if (val == "1") {
            $("#timereceive").attr("disabled", "disabled");
        } else {
            $("#timereceive").removeAttr("disabled");
        }
    })
});