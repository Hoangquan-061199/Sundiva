$(function () {
    var form = $("#loginAdminFrm");
    $("#btnLogin").click(function () {
        $("#btnLogin").attr("disabled", true);
        if (form.valid()) {
            form.submit();
        } else {
            $("#btnLogin").attr("disabled", false);
        }
    });
    form.validate({
        rules: {
            userName: {
                required: true
            },
            password: {
                required: true
            }
        },
        messages: {
            userName: {
                required: "Tài khoản bắt buộc nhập."
            },
            password: {
                required: "Mật khẩu bắt buộc nhập."
            }
        },
        submitHandler: function () { //onSubmit
            form.ajaxSubmit({
                success: function (data) {               
                    if (data.errors) {
                        swal({
                            title: "Thông báo",
                            text: data.message,
                            type: "error",
                            showConfirmButton: true,
                            animation: false
                        });
                        $("#btnLogin").attr("disabled", false);
                    } else {
                        if (data.Url != null) {
                            window.location.href = data.Url;
                        } else {
                            window.location.reload();
                        }
                    }
                }
            });
            return false;
        }
    });
    //change pass
    var changepass = $("#ChangePassFrm");
    $("#btnChangePassFrm").click(function () {
        $("#btnChangePassFrm").attr("disabled", true);
        if (changepass.valid()) {
            changepass.submit();
        } else {
            $("#btnChangePassFrm").attr("disabled", false);
        }
    });
    changepass.validate({
        rules: {
            OldPassword: { required: true },
            NewPassword: { required: true },
            ConfirmPassword: { required: true, equalTo: "#NewPassword" }
        },
        messages: {
            OldPassword: { required: "Tài khoản cũ bắt buộc nhập." },
            NewPassword: { required: "Mật khẩu mới bắt buộc nhập." },
            ConfirmPassword: { required: "Xác nhận mật khẩu mới bắt buộc nhập.", equalTo: "Xác nhận mật khẩu không đúng." }
        },
        submitHandler: function () { //onSubmit
            changepass.ajaxSubmit({
                success: function (data) {
                    if (data.errors) {
                        swal({ title: "Thông báo", text: data.message, type: "error", showConfirmButton: true, animation: false });
                        $("#btnChangePassFrm").attr("disabled", false);
                    } else {
                        swal({ title: "Thông báo", text: data.message, type: "success", showConfirmButton: true, animation: false });
                        setInterval(function () {
                            window.location.href = "/Adminadc/HomeAdmin/LogOff";
                        }, 3000);
                    }
                }
            });
            return false;
        }
    });
});