$(() => {


    Comments();
    ShowReplyComment();
    ReplyCommentAction();
    MoreReplyComment();
    MoreComment();
})

function showFormComment(e) {
    $(e).addClass('active');
}

function Comments() {
    let formComment = $("#form-comment");
    $("#btn-comment").click(function () { formComment.submit(); });
    formComment.validate({
        rules: {
            FullName: { required: true },
            Phone: { required: true, minlength: 10, maxlength: 12 },
            Email: { required: true, email: true },
            Content: { required: true }
        },
        messages: {
            FullName: { required: GetSource("VuiLongNhapDayDuHoTen") },
            Phone: { required: GetSource("VuiLongNhapSoDienThoai"), minlength: GetSource("Tu10Den12KyTu"), maxlength: GetSource("Tu10Den12KyTu") },
            Email: { required: GetSource("VuiLongNhapEmail"), email: GetSource("EmailKhongChinhXac") },
            Content: { required: GetSource("VuiLongNoiDung") }
        },
        submitHandler: function () {
            let d = formComment.serialize();
            $(".btn-comment").prop("disabled", true).hide();
            $(".load").addClass('show');
            $("body").addClass('disable');
            formComment.ajaxSubmit({
                success: function (data) {
                    if (data.errors === false) {
                        let img = `
                            <div class="img-cmt" data-src="${data.data.urlPicture}" data-fancybox="comment" data-caption="${data.data.fullname}">
                                <img src="${data.data.urlPicture}" alt="${data.data.fullname}"/>
                            </div>
                        `;
                        let html = `
                            <div class="item">
                                <div class="info">
                                    <div class="avt">
                                        <i class="fa-solid fa-user"></i>
                                    </div>
                                    <div class="content">
                                        <div class="user-text">${data.data.fullname}</div>
                                        <p>${data.data.content}</p>
                                        ${data.data.urlPicture !== null ? img : ''}
                                    </div>
                                    <div class="action item-${data.data.id}">
                                        <span class="_like"><i class="fa-solid fa-thumbs-up"></i> (0) ${GetSource('Thich')}</span>
                                        <span class="show-repply-comment" data-id="${data.data.id}" data-pid="${data.data.id}">
                                            <i class="fa-regular fa-comment-dots"></i> ${GetSource('TraLoi')}
                                        </span>
                                        <span class="time-span">${GetSource('VuaXong')}</span>
                                    </div>
                                    <div class="list-repply-comment reply-${data.data.id}"></div>
                                </div>
                            </div>
                        `;
                        var lengthItem;
                        if (data.data.rate != null && data.data.rate > 0) {
                            lengthItem = $('.list-comment > .item').length;
                            if (lengthItem > 0)
                                $('.list-comment > .item:nth-of-type(1)').before(html);
                            else
                                $('.list-comment').html(html);
                        }
                        else {
                            lengthItem = $('.list-comment > .item').length;
                            if (lengthItem > 0)
                                $('.list-comment > .item:nth-of-type(1)').before(html);
                            else
                                $('.list-comment').html(html);
                        }
                        $(".btn-comment").prop("disabled", false).show();
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        let totalRate = $('.num-cmt').attr('data-total-comment');
                        $('.num-cmt span').text(`(${parseInt(totalRate) + 1} ${GetSource('BinhLuan')})`);
                        $('#form-comment').trigger('reset');
                        $("#AvatarComment").val("");
                        $('.input-file .img-block .img').html('');
                        $('.input-file .img-block').hide();
                        $('.input-file label span').text(GetSource("DinhKemAnh"));
                    }
                    else {
                        $(".btn-comment").prop("disabled", false).show();
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        OpenAlert(data.message, false);
                        console.log(data.logs);
                    }
                },
                error: function () {
                    $(".btn-comment").prop("disabled", false).show();
                    $(".load").removeClass('show');
                    $("body").removeClass('disable');
                    OpenAlert(data.message, false);
                    console.log(data.logs);
                }
            })
            return false;
        }
    });
}

function HtmlFormReplyComment(parentid, type, name, container) {
    let cid = $('#form-comment input[name=ContentID]').val();
    var customeid = $('#UserID').val();
    var fullname = $('#form-comment input[name=Fullname]').val();
    var email = $('#form-comment input[name=Email]').val();
    var phone = $('#form-comment input[name=Phone]').val();
    var html = ``;
    $.get("/html/template/comment.html", function (data,) {
        html = data;
        html = html.replace("[ProductID]", cid)
            .replace("[Type]", type)
            .replace("[ParentID]", parentid)
            .replace("[Fullname]", fullname)
            .replace("[Email]", email).replace("[Content]", name);
        html = html.replace("[EnterComment]", GetSource("NhapNoiDung"));
        html = html.replace("[DinhKemAnh]", GetSource("DinhKemAnh"));
        html = html.replace("[FullName]", GetSource("HoTen"));
        html = html.replace("[Send]", GetSource("BinhLuan"));
        html = html.replace("[ParentID]", parentid);
        html = html.replace("[Xoa]", GetSource("Xoa"));
        $(container).append(html);
    });
}

function ShowReplyComment() {
    $('.show-repply-comment').click(function () {
        var id = $(this).data('id');
        var parentid = $(this).data('pid');
        var name = $(this).data('name');
        $('.list-comment').find('.reply-comment').remove();
        HtmlFormReplyComment(parentid, 'Product', (name != null ? "@" + name + " - " : ''), '.list-comment .item .action.item-' + id);
        showFormComment();
        ReplyCommentAction();
    });
}

function ReplyCommentAction() {
    var formrepplycomment = $("#form-reply-comment");
    $("#btn-reply-comment").click(function () { formrepplycomment.submit(); });
    formrepplycomment.validate({
        rules: {
            FullName: { required: true },
            Email: { required: true, email: true },
            Content: { required: true }
        },
        messages: {
            FullName: { required: GetSource("VuiLongNhapDayDuHoTen") },
            Email: { required: GetSource("VuiLongNhapEmail"), email: GetSource("EmailKhongChinhXac") },
            Content: { required: GetSource("VuiLongNoiDung") }
        },
        submitHandler: function () {
            let d = formrepplycomment.serialize();
            $("#btn-reply-comment").prop("disabled", true).hide();
            $(".load").addClass('show');
            $("body").addClass('disable');
            formrepplycomment.ajaxSubmit({
                success: function (data) {
                    if (data.errors === false) {
                        let img = `
                            <div class="img-cmt" data-src="${data.data.urlPicture}" data-fancybox="comment" data-caption="${data.data.fullname}">
                                <img src="${data.data.urlPicture}" alt="${data.data.fullname}"/>
                            </div>
                        `;
                        let html = `
                            <div class="item">
                                <div class="info">
                                    <div class="avt">
                                        <i class="fa-solid fa-user"></i>
                                    </div>
                                    <div class="content">
                                        <div class="user-text">${data.data.fullname}</div>
                                        <p>${data.data.content}</p>
                                        ${data.data.urlPicture !== null ? img : ''}
                                    </div>
                                    <div class="action item-${data.data.id}">
                                        <span class="_like"><i class="fa-solid fa-thumbs-up"></i> (0) ${GetSource('Thich')}</span>
                                        <span class="time-span">${GetSource('VuaXong')}</span>
                                    </div>
                                </div>
                            </div>
                        `;
                        var lengthItem;
                        lengthItem = $('.list-repply-comment.reply-' + data.data.parentID + ' > .item').length;
                        if (lengthItem > 0)
                            $('.list-repply-comment.reply-' + data.data.parentID + ' > .item:nth-of-type(1)').before(html);
                        else
                            $('.list-repply-comment.reply-' + data.data.parentID + '').html(html);
                        $("#btn-reply-comment").prop("disabled", false).show();
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        $("#AvatarComment").val("");
                        $('.input-file .img-block .img').html('');
                        $('.input-file .img-block').hide();
                        $('.input-file label span').text(GetSource("DinhKemAnh"));
                        //ReplyComment();
                    }
                    else {
                        $("#btn-reply-comment").prop("disabled", false).show();
                        $(".load").removeClass('show');
                        $("body").removeClass('disable');
                        OpenAlert(data.message, false);
                        console.log(data.logs);
                    }
                },
                error: function () {
                    $("#btn-reply-comment").prop("disabled", false).show();
                    $(".load").removeClass('show');
                    $("body").removeClass('disable');
                    OpenAlert(data.message, false);
                    console.log(data.logs);
                }
            })
            return false;
        }
    });
}

function MoreReplyComment() {
    $(".page-reply").click(function (e) {
        let productid = $(this).attr('data-product-id');
        let parentid = $(this).attr('data-parentid');
        e.preventDefault();
        $(".load").addClass('show');
        $(this).remove();
        let url = "/Ajax/Comment/AjaxReply?productId=" + productid + "&parentId=" + parentid;
        $.ajax({
            url: encodeURI(url),
            cache: false,
            type: "POST",
            success: function (data) {
                $('.list-repply-comment.reply-' + parentid).append(data);
                $(".load").removeClass('show');
            }
        });
    });
}

function MoreComment() {
    $(".page div").click(function (n) {
        n.preventDefault();
        let productId = $('#productId').val();
        var p = $(this).data("page");
        $(".load").addClass('show');
        $("#page-comment").val(p);
        $.ajax({
            url: encodeURI("/AjaxComment?page=" + p + "&productId=" + productId),
            cache: false,
            type: "POST",
            success: function (data) {
                $('.list-comment').html(data);
                ShowReplyComment();
                ReplyCommentAction();
                MoreReplyComment();
                MoreComment();
                $(".load").removeClass('show');
            }
        });
    })
}

function LikeComment(id) {
    var like = $('.action.item-' + id + ' ._like').children('span').text().replace("(", "").replace(")", "");
    var n = parseInt(like) + 1;
    $('.action.item-' + id + ' ._like').children('span').text(n);
    var url = "/Ajax/Comment/Good?parentId=" + id;
    $.ajax({
        url: encodeURI(url),
        cache: false,
        type: "POST",
        success: function (data) { }
    });
}