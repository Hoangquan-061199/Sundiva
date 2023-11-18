//city
function ChageCity() {
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
    $("#CityIDs").change(function () {

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
function ChageDistrict() {
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
    $("#DistrictIDs").change(function () {
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
        $("#DistrictIDs").removeAttr("disabled");
        $.ajax({
            type: "POST",
            url: "/Ajax/Cart/LoadDistrict",
            dataType: "text",
            data: { id: id },
            success: function (data) {
                $('#DistrictIDs').html(data);
            }
        });
    }
    else {
        $("#DistrictID").attr("disabled", "true");
        $('#DistrictID').html("<option value=\"\">Chọn Quận/Huyện</option>");
        $("#DistrictIDs").attr("disabled", "true");
        $('#DistrictIDs').html("<option value=\"\">Chọn Quận/Huyện</option>");
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
        $("#WardIDs").removeAttr("disabled");
        $.ajax({
            type: "POST",
            url: "/Ajax/Cart/LoadWard",
            dataType: "text",
            data: { id: id },
            success: function (data) {
                $('#WardIDs').html(data);
            }
        });
    }
    else {
        $("#WardID").attr("disabled", "true");
        $('#WardID').html("<option value=\"\">Chọn Phường/Xã</option>");
        $("#WardIDs").attr("disabled", "true");
        $('#WardIDs').html("<option value=\"\">Chọn Phường/Xã</option>");
    }
}
//
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