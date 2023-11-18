function tooltip() {
    /* CONFIG */
    xOffset = 10;
    yOffset = 20;
    // these 2 variable determine popup's distance from the cursor
    // you might want to adjust to get the right result		
    /* END CONFIG */
    $(".tooltipImage").hover(function (e) {
        this.t = this.src;
        var size = this.fileSize;
        var filename = this.src.replace(/^.*[\\\/]/, '');
        $("body").append("<p id='tooltip'> <span class='tooltipimg'> <img src='" + this.t + "'/> </span>" +
            " <span><b>File Name:" + filename + "</b></span>" +
            " <span><b>Dimensions: " + this.naturalWidth + " x " + this.naturalHeight + "</b></span></p>");
        $("#tooltip")
            .css("top", (e.pageY - xOffset) + "px")
            .css("left", (e.pageX + yOffset) + "px")
            .fadeIn("fast");
    },
        function () {
            this.title = this.t;
            $("#tooltip").remove();
        });
    $("a.tooltip").mousemove(function (e) {
        $("#tooltip")
            .css("top", (e.pageY - xOffset) + "px")
            .css("left", (e.pageX + yOffset) + "px");
    });
};
//load File start
//0: là load ảnh, 1: load album
function SelectFileTyniMce(id, name, type) {
    var share_link = "/lib/tinymce/index.html?integration=folderadmin&view=" + id + "&name=" + name + "&typeview=" + type;
    popupwindow(share_link, "Manager file", 1600, 800);
}
function popupwindow(url, title, w, h) {
    var left = (screen.width / 2) - (w / 2);
    var top = (screen.height / 2) - (h / 2);
    return window.open(url, title, 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
}
function ChangeUrlTinyMce(thisNow, id, name, type) {
    var url = thisNow.val();
    var file = {
        'fullPath': url
    };
    if (type == 2) {
        FileTinyMce(file, id, name);
    } else {
        if (type == 1) {
            AlbumTinyMce(file, id, name);
        }
        else if (type == 3) {
            ImageTinyMce(file, id, name);
        }
        else if (type == 4) {
            MultiFileTinyMce(file, id, name);
        }
        else if (type == 5) {
            ColorTableTinyMce(file, id, name);
        }
        else if (type == 6) {
            AlbumModuleProductTinyMce(file, id, name);
        }
        else if (type == 7) {
            SlideDetailTinyMce(file, id, name);
        }
        else {
            UpdatePictureTinyMce(file, id, name);
        }
    }
}
function UpdatePictureTinyMce(file, id, name) {
    var html = '<table class="removeParent">' +
        '<tr>' +
        '<td><img style="width: auto;height: 50px;" src="' + file.fullPath + '">' +
        '<input type="hidden" name="' + name + '" value="' + file.fullPath + '"><a href="javascript:void(0);" class="removeObject"><i class="fa fa-trash"></i></a></td>' +
        '</tr></table>';
    $("#" + id).html(html);
    $("#" + id).parents(".changeUrlTinyMceParent").find(".changeUrlTinyMce").val(file.fullPath);
    console.log(file);
    removeObject();
    tooltip();
    if (file.type == "image") {
        var url = "/Adminadc/Image/ConvertReSize?path=" + file.path + "&name=" + file.name;
        $.ajax({
            url: encodeURI(url), cache: false, type: "Post",
            success: function (data) {
                console.log(data);
            },
            error: function (data) {
                console.log(data);
            }
        });
    }
}
function FileTinyMce(file, id, name) {
    $("#" + id).val(file.fullPath);
}
function MultiFileTinyMce(file, id, name) {
    if (file.type == "pdf") {
        var html = `<div class="item-file">
                                                <span><i class="fa fa-file-pdf-o"></i></span>
                                                <input type="text" class="form-control" placeholder="Tên file" name="FileName" value="[FileName]" />
                                                <input type="text" class="form-control" name="FileUrl" value="[FileUrl]" />
                                                <div class="removeFile"><i class="fa fa-trash-o"></i></div>
                                            </div>`;
        let fileName = file.name.split('.')[0];
        html = html.replaceAll("[FileUrl]", file.fullPath);
        html = html.replaceAll("[FileName]", fileName);
        $("#" + id).append(html);
        removeObjectFile();
    }
}
function removeObjectFile() {
    $('.removeFile').click(function () {
        $(this).parents('.item-file').remove();
    });
}
function AlbumTinyMce(file, id, name) {
    var html = `<table class="table removeParent">`;
    html += `<tr><td rowspan="2" style="width:50px;"><img class="tooltipImage" style="width: 50px; height: 50px;" src="[AlbumUrl]"><a href="javascript:void(0);" class="removeObject"><i class="fa fa-trash"></i></a></td><td style="width:100px;text-align:left;"><b>Tiêu đề</b></td><td><input class="form-control" type="text" name="AlbumTitle" placeholder="[AlbumTitle]" value=""></td></tr>`;
    html += `<tr><td style="text-align:left;"><b>Link</b></td><td><input type="text" class="form-control" name="AlbumUrl" value="[AlbumUrl]"></td></tr>`;
    html += `<tr><td><input type="text" style="text-align:center;width:70px;display: inline-block;" class="form-control" name="AlbumOrderDisplay" value="0" placeholder="Thứ tự"></td><td style="text-align:left;"><b>Loại</b></td><td>` +
        `<select name="AlbumType" class="form-control" style="width:200px;display:inline-block;">` +
        // `<option value="4">Slide thư viện</option>` +
        `<option value="0">Ảnh/Background</option>` +
        `<option value="1">Icon/Ảnh nhỏ</option>` +
        //`<option value="2">Icon trang chủ</option>` +
        `<option value="3">Banner</option>` +
        /*`<option value="4">Slide thư viện</option>` +*/
        //`<option value="5">Banner ngoài danh mục cha</option>` +
        //`<option value="6">Popup</option>` +
        `</select>
<input type="radio" class="IsAvatar" name="IsAvatar" value="true" style="margin:0 5px;" data-url="[AlbumUrl]" />Ảnh đại diện
</td></tr></table>`;
    html = html.replaceAll("[AlbumUrl]", file.fullPath);
    html = html.replaceAll("[AlbumTitle]", file.fullPath);
    $("#" + id).prepend(html);
    removeObject();
    $('.IsAvatar').change(function () {
        var src = $(this).data('url');
        $('#Avatar').val(src);
        $('#AddAvatar').find('input').val(src);
        $('#AddAvatar').find('img').attr('src', src);
    });
    tooltip();
    if (file.type == "image") {
        var url = "/Adminadc/Image/ConvertReSize?path=" + file.path + "&name=" + file.name;
        $.ajax({
            url: encodeURI(url), cache: false, type: "Post",
            success: function (data) {
                console.log(data);
            },
            error: function (data) {
                console.log(data);
            }
        });
    }
}

function SlideDetailTinyMce(file, id, name) {
    var html = `<table class="table removeParent">`;
    html += `<tr><td rowspan="2" style="width:50px;"><img class="tooltipImage" style="width: 50px; height: 50px;" src="[AlbumUrl]"><a href="javascript:void(0);" class="removeObject"><i class="fa fa-trash"></i></a></td><td style="width:100px;text-align:left;"><b>Tiêu đề</b></td><td><input class="form-control" type="text" name="AlbumTitle" placeholder="Tiêu đề" value=""></td></tr>`;
    html += `<tr><td style="text-align:left;"><b>Link</b></td><td><input type="text" class="form-control" name="AlbumUrl" value="[AlbumUrl]"></td></tr>`;
    html += `<tr><td><input type="text" style="text-align:center;width:70px;display: inline-block;" class="form-control" name="AlbumOrderDisplay" value="0" placeholder="Thứ tự"></td><td style="text-align:left;"><b>Loại</b></td><td>` +
        `<select name="AlbumType" class="form-control" style="width:200px;display:inline-block;">` +
         `<option value="7">Project Images</option>` +
        `<option value="0">Ảnh/Background</option>` +
        `<option value="3">Banner</option>` +
        `</select>
<input type="radio" class="IsAvatar" name="IsAvatar" value="true" style="margin:0 5px;" data-url="[AlbumUrl]" />Ảnh đại diện
</td></tr></table>`;
    html = html.replaceAll("[AlbumUrl]", file.fullPath);
    $("#" + id).prepend(html);
    removeObject();
    $('.IsAvatar').change(function () {
        var src = $(this).data('url');
        $('#Avatar').val(src);
        $('#AddAvatar').find('input').val(src);
        $('#AddAvatar').find('img').attr('src', src);
    });
    tooltip();
    if (file.type == "image") {
        var url = "/Adminadc/Image/ConvertReSize?path=" + file.path + "&name=" + file.name;
        $.ajax({
            url: encodeURI(url), cache: false, type: "Post",
            success: function (data) {
                console.log(data);
            },
            error: function (data) {
                console.log(data);
            }
        });
    }
}

function AlbumModuleProductTinyMce(file, id, name) {
    var html = `<table class="table removeParent">`;
    html += `<tr><td rowspan="2" style="width:50px;"><img class="tooltipImage" style="width: 50px; height: 50px;" src="[AlbumUrl]"><a href="javascript:void(0);" class="removeObject"><i class="fa fa-trash"></i></a></td><td style="width:100px;text-align:left;"><b>Tiêu đề</b></td><td><input class="form-control" type="text" name="AlbumTitle" placeholder="[AlbumTitle]" value=""></td></tr>`;
    html += `<tr><td style="text-align:left;"><b>Link</b></td><td><input type="text" class="form-control" name="AlbumUrl" value="[AlbumUrl]"></td></tr>`;
    html += `<tr><td><input type="text" style="text-align:center;width:70px;display: inline-block;" class="form-control" name="AlbumOrderDisplay" value="0" placeholder="Thứ tự"></td><td style="text-align:left;"><b>Loại</b></td><td>` +
        `<select name="AlbumType" class="form-control" style="width:200px;display:inline-block;">` +
        // `<option value="4">Slide thư viện</option>` +
        `<option value="0">Ảnh/Background</option>` +
        `<option value="1">Icon/Ảnh nhỏ</option>` +
        //`<option value="2">Icon trang chủ</option>` +
        `<option value="3">Banner</option>` +
        `<option value="4">Banner nhỏ danh mục sản phẩm</option>` +
        //`<option value="5">Banner ngoài danh mục cha</option>` +
        //`<option value="6">Popup</option>` +
        `</select>
<input type="radio" class="IsAvatar" name="IsAvatar" value="true" style="margin:0 5px;" data-url="[AlbumUrl]" />Ảnh đại diện
</td></tr></table>`;
    html = html.replaceAll("[AlbumUrl]", file.fullPath);
    html = html.replaceAll("[AlbumTitle]", file.fullPath);
    $("#" + id).prepend(html);
    removeObject();
    $('.IsAvatar').change(function () {
        var src = $(this).data('url');
        $('#Avatar').val(src);
        $('#AddAvatar').find('input').val(src);
        $('#AddAvatar').find('img').attr('src', src);
    });
    tooltip();
    if (file.type == "image") {
        var url = "/Adminadc/Image/ConvertReSize?path=" + file.path + "&name=" + file.name;
        $.ajax({
            url: encodeURI(url), cache: false, type: "Post",
            success: function (data) {
                console.log(data);
            },
            error: function (data) {
                console.log(data);
            }
        });
    }
}

function ColorTableTinyMce(file, id, name) {
    var html2 = `<table class="table removeParent">`;
    html2 += `<tr>
                <td rowspan="4" style="width:50px;">
                    <img class="tooltipImage" style="width: 50px; height: 50px;" src="[ColorTableUrl]">
                    <input type="hidden" name="ColorTableUrl" value="[ColorTableUrl]">
                <a href="javascript:void(0);" class="removeObject">
                    <i class="fa fa-trash"></i>
                </a>
                </td>
                <td style="width:100px;text-align:left;">
                    <b>Tiêu đề</b>
                    </td><td>
                    <input class="form-control" type="text" name="ColorTableTitle" placeholder="Tiêu đề" value="">
                </td>
            </tr>`;
    html2 += `<tr>
                <td style="text-align:left;">
                    <b>Link ảnh</b>
                    </td>
                    <td>
                    <input type="text" class="form-control" value="[ColorTableUrl]">
                </td>
            </tr>`;
    html2 += `<tr>
                <td style="text-align:left;">
                    <b>Alt</b>
                </td>
                <td>
                    <input type="text" class="form-control" name="ColorTableAlt" value="">
                </td>
            </tr>`;
    html2 += `<tr>
                <td style="text-align:left;">
                    <b>Thứ tự</b>
                </td>
                <td>
                    <input type="text" style="text-align:center;width:70px;display: inline-block;" class="form-control" name="ColorTableOrderDisplay" value="0" placeholder="Thứ tự">
                </td>
            </tr>
        </table>`;

    html2 = html2.replaceAll("[ColorTableUrl]", file.fullPath);
    $("#" + id).prepend(html2);
    removeObject();
    tooltip();
    if (file.type == "image") {
        var url = "/Adminadc/Image/ConvertReSize?path=" + file.path + "&name=" + file.name;
        $.ajax({
            url: encodeURI(url), cache: false, type: "Post",
            success: function (data) {
                console.log(data);
            },
            error: function (data) {
                console.log(data);
            }
        });
    }
}
function ImageTinyMce(file, id, name) {
    var html = `<table class="table removeParent">
                                        <tr>
                                            <td rowspan="4" style="width:50px;">
                                                <img style="width: 50px; height: 50px;" src="[ImageUrl]">
                                                <input type="hidden" name="ImageUrl" value="[ImageUrl]">
                                                <a href="javascript:void(0);" class="removeObject"><i class="fa fa-trash"></i></a>
                                            </td>
                                            <td style="width:100px;text-align:left;"><b>Tiêu đề</b></td>
                                            <td><input class="form-control" type="text" name="ImageTitle" value=""></td>
                                        </tr>
                                        <tr>
                                            <td style="text-align:left;"><b>Video</b></td>
                                            <td><input type="text" class="form-control" name="VideoUrl" value="[VideoUrl]"></td>
                                        </tr>
                                        <tr>
                                            <td style="text-align:left;"><b>Link</b></td>
                                            <td><input type="text" class="form-control" name="ImageLink" value=""></td>
                                        </tr>
                                        <tr>
                                            <td style="text-align:left;"><b>Kiểu</b></td>
                                            <td>
                                                <select name='ImageType' class="form-control" style="width:100px;display:inline-block;">
                                                    <option value='3'>Banner</option>
                                                    <option value='4'>Mobile</option>
                                                </select>
                                                <input type="number" style="text-align:center;width:100px;display: inline-block;" class="form-control" name="ImageOrder" value="" placeholder="Thứ tự">
                                                <input type="number" style="text-align:center;width:100px;display: inline-block;" class="form-control" name="ImagePosition" value="" placeholder="Vị trí">
                                                <select name='ImageSize' class="form-control" style="width:100px;display:inline-block;">
                                                    <option value=''>Kích thước</option>
                                                    <option value='1x1'>1x1</option><option value='1x2'>1x2</option><option value='1x3'>1x3</option><option value='1x4'>1x4</option><option value='1x5'>1x5</option>
                                                    <option value='2x1'>2x1</option><option value='2x2'>2x2</option><option value='2x3'>2x3</option><option value='2x4'>2x4</option><option value='2x5'>2x5</option>
                                                    <option value='3x1'>3x1</option><option value='3x2'>3x2</option><option value='3x3'>3x3</option><option value='3x4'>3x4</option><option value='3x5'>3x5</option>
                                                    <option value='4x1'>4x1</option><option value='4x2'>4x2</option><option value='4x3'>4x3</option><option value='4x4'>4x4</option><option value='4x5'>4x5</option>
                                                    <option value='5x1'>5x1</option><option value='5x2'>5x2</option><option value='5x3'>5x3</option><option value='5x4'>5x4</option><option value='5x5'>5x5</option>
                                                    <option value='6x1'>6x1</option><option value='6x2'>6x2</option><option value='6x3'>6x3</option><option value='6x4'>6x4</option><option value='6x5'>6x5</option>
                                                    <option value='7x1'>7x1</option><option value='7x2'>7x2</option><option value='7x3'>7x3</option><option value='7x4'>7x4</option><option value='7x5'>7x5</option>
                                                    <option value='8x1'>8x1</option><option value='8x2'>8x2</option><option value='8x3'>8x3</option><option value='8x4'>8x4</option><option value='8x5'>8x5</option>
                                                </select>

                                            </td>                                            
                                        </tr>
                                    </table>`;
    if (file.type == "image") {
        html = html.replaceAll("[ImageUrl]", file.fullPath);
        html = html.replaceAll("[VideoUrl]", "");
    }
    else {
        html = html.replaceAll("[ImageUrl]", "/Admin/images/imagepreview.png");
        html = html.replaceAll("[VideoUrl]", file.fullPath);
    }
    html = html.replaceAll("[ImageUrl]", file.fullPath);
    $("#" + id).prepend(html);
    removeObject();
    tooltip();
    if (file.type == "image") {
        var url = "/Adminadc/Image/ConvertReSize?path=" + file.path + "&name=" + file.name;
        $.ajax({
            url: encodeURI(url), cache: false, type: "Post",
            success: function (data) {
                console.log(data);
            },
            error: function (data) {
                console.log(data);
            }
        });
    }
}
function removeAllAlbum(id) {
    $('#' + id).find('.removeParent').remove();
}

function AddSubContentIcon() {
    let n = 1;
    let arr = new Array();
    let ids = $('#NewSubContent').val();
    if (ids)
        arr = ids.split(',');
    if ($('.new-list-subcontent .item_sub').length > 0) {
        let cls = $('.new-list-subcontent .item_sub:first-child').attr('class');
        cls = cls.replace('item_sub item_', '');
        n = parseInt(cls) + 1;
        if (!containsObject(n, arr)) {
            arr.push(n);
        }
        $('#NewSubContent').val(arr.join(','));
    }
    else
        arr.push(n);

    let tem = `
        <div class="item_sub item_${n}">
            <div class="value-2 changeUrlTinyMceParent">
                <div id="AddIcon_${n}">
                </div>
                <div class="input-group">
                    <input type="text" class="changeUrlTinyMce link form-control" placeholder="Link ảnh" onchange="ChangeUrlTinyMce($(this),'AddIcon_${n}', 'UrlPicture_${n}',0)" value="" />
                    <span class="input-group-addon" style="background:transparent;padding-right:10px;">
                        <button type="button" onclick="SelectFileTyniMce('AddIcon_${n}','UrlPicture_${n}',0);" class="btn btn-info btn-bnl btn-sm"><span class="lnr lnr-upload" style="margin-right:5px;"></span>Chọn ảnh</button>
                    </span>
                </div>
            </div>
            <div class="value-1">
                <label>Tên</label>
                <input type="text" name="Name_${n}" class="form-control" />
            </div>
            <div class="value-2" style="flex: 1">
                <label>Nội dung</label>
                <input type="text" name="Content_${n}" class="form-control" />
            </div>
            <div class="value-3">
                <label>Thứ tự</label>
                <input type="number" name="OrderDisplay_${n}" class="form-control" />
            </div>
            <div class="value-4">
                <input type="checkbox" name="IsShow_${n}" @(item.IsShow == true ? "checked" : string.Empty) /> Ẩn/Hiện
            </div>
            <div class="value-5"><div class="badge badge-info" onclick="RemoveContentSub('${n}');" style="cursor:pointer;">Xóa</div></div>
        </div>
    `;
    $('#NewSubContent').val(arr.join(','));
    $('#NewSubContentList').prepend(tem);
}

function AddSubContentIconProduct() {
    let n = 1;
    let arr = new Array();
    let ids = $('#NewSubContent').val();
    if (ids)
        arr = ids.split(',');
    if ($('.new-list-subcontent .item_sub').length > 0) {
        let cls = $('.new-list-subcontent .item_sub:first-child').attr('class');
        cls = cls.replace('item_sub item_', '');
        n = parseInt(cls) + 1;
        if (!containsObject(n, arr)) {
            arr.push(n);
        }
        $('#NewSubContent').val(arr.join(','));
    }
    else
        arr.push(n);

    let tem = `
        <div class="item_sub item_${n}">
            <div class="value-2 changeUrlTinyMceParent">
                <div id="AddIcon_${n}">
                </div>
                <div class="input-group">
                    <input type="text" class="changeUrlTinyMce link form-control" placeholder="Link ảnh" onchange="ChangeUrlTinyMce($(this),'AddIcon_${n}', 'UrlPicture_${n}',0)" value="" />
                    <span class="input-group-addon" style="background:transparent;padding-right:10px;">
                        <button type="button" onclick="SelectFileTyniMce('AddIcon_${n}','UrlPicture_${n}',0);" class="btn btn-info btn-bnl btn-sm"><span class="lnr lnr-upload" style="margin-right:5px;"></span>Chọn ảnh</button>
                    </span>
                </div>
            </div>
            <div class="value-1" style="flex: 1">
                <label>Tên</label>
                <input type="text" name="Name_${n}" class="form-control" />
            </div>
            <div class="value-1" style="flex: 1">
                <label>Nội dung</label>
                <input type="text" name="Content_${n}" class="form-control" />
            </div>
            <div class="value-3">
                <label>Thứ tự</label>
                <input type="number" name="OrderDisplay_${n}" class="form-control" />
            </div>
            <div class="value-4">
                <input type="checkbox" name="IsShow_${n}" @(item.IsShow == true ? "checked" : string.Empty) /> Ẩn/Hiện
            </div>
            <div class="value-5"><div class="badge badge-info" onclick="RemoveContentSub('${n}');" style="cursor:pointer;">Xóa</div></div>
        </div>
    `;
    $('#NewSubContent').val(arr.join(','));
    $('#NewSubContentList').prepend(tem);
}

function AddSubProduct() {
    let n = 1;
    let arr = new Array();
    let ids = $('#NewSubContent').val();
    if (ids)
        arr = ids.split(',');
    if ($('#NewSubContentList .item_sub').length > 0) {
        let cls = $('#NewSubContentList .item_sub:first-child').attr('class');
        cls = cls.replace('col-sm-6 item_sub bg-color-item-sub item_', '');
        n = parseInt(cls) + 1;
        if (!containsObject(n, arr)) {
            arr.push(n);
        }
        $('#NewSubContent').val(arr.join(','));
    }
    else
        arr.push(n);

    let tem = `
        <div class="col-sm-12 item_sub bg-color-item-sub item_${n}">
            <div class="col-sm-11">
                <div class="form-group">
                    <div class="col-sm-1">
                        <label>Tên</label>
                    </div>
                    <div class="col-sm-5">
                        <input type="text" name="Name_${n}" class="form-control" />
                    </div>
                    <div class="col-sm-1">
                        <label>Thứ tự</label>
                    </div>
                    <div class="col-sm-2">
                        <input type="number" name="OrderDisplay_${n}" class="form-control" />
                    </div>
                    <div class="col-sm-2">
                        <input type="checkbox" name="IsShow_${n}" /> Ẩn/Hiện
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-sm-1">
                        <label>
                            Ảnh
                        </label>
                    </div>
                    <div class="value-2 changeUrlTinyMceParent col-sm-11">
                        <div id="AddIcon_${n}">
                        </div>
                        <div class="input-group">
                            <input type="text" class="changeUrlTinyMce link form-control" placeholder="Link ảnh"
                                onchange="ChangeUrlTinyMce($(this),'AddIcon_${n}', 'UrlPicture_${n}',0)" value="" />
                            <span class="input-group-addon" style="background:transparent;padding-right:10px;">
                                <button type="button" onclick="SelectFileTyniMce('AddIcon_${n}','UrlPicture_${n}',0);"
                                    class="btn btn-info btn-bnl btn-sm"><span class="lnr lnr-upload"
                                        style="margin-right:5px;"></span>Chọn ảnh</button>
                            </span>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-sm-1">
                        <label>Nội dung</label>
                    </div>
                    <div class="col-sm-11">
                        <input type="text" name="Content_${n}" class="form-control" />
                    </div>
                </div>
            </div>
            <div class="col-sm-1">
                    <div class="btn btn-danger" onclick="RemoveContentSub('${n}');" style="cursor:pointer;">Xóa</div>
            </div>
        </div>
    `;
    $('#NewSubContent').val(arr.join(','));
    $('#NewSubContentList').prepend(tem);
    LoadCKEDITOR(`Content_${n}`, true, 200);
}

function RemoveContentSub(n) {
    let idsNew = $('#NewSubContent').val();
    let idsOld = $('#OldSubContent').val();
    let arrNew = idsNew.split(',');
    let arrOld = idsOld.split(',');
    if (containsObject(n, arrNew)) {
        removeElement(arrNew, n);
    }
    if (containsObject(n, arrOld)) {
        removeElement(arrOld, n);
    }
    $('#NewSubContent').val(arrNew.join(','));
    $('#NewSubContentList .item_' + n).remove();
    $('#OldSubContent').val(arrOld.join(','));
    $('#OldSubContentList .item_' + n).remove();
}