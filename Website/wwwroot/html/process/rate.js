var Loading = `<div class="loadingio-spinner-spinner-uzlublexfob sending">
                    <div class="ldio-c4d59ljt0jh"><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div></div>
               </div>`;
var cid = $('#RateComment input[name=ContentID]').val();
var customeid = $('#UserID').val();
var fullname = $('#RateComment input[name=Fullname]').val();
var email = $('#RateComment input[name=Email]').val();
var phone = $('#RateComment input[name=Phone]').val();
var typecontent = $('#RateComment input[name=Type]').val();
$(function () {
    Rate();
    MoreRate();
    ShowReplyRate();
    ReplyReplyRate();
    MoreReplyRate()
});
function Rate() {
    var $frm_post_create = $("#RateComment");
    $("#btnRate").click(function () { $frm_post_create.submit(); });
    $("#RateComment").validate({
        rules: { Fullname: { required: true }, Phone: { required: true }, Email: { required: true, email: true }, Content: { required: true } },
        submitHandler: function () {
            $('.frm-rate .action .processing').addClass('show');
            $('.frm-rate .action button').hide();
            $frm_post_create.ajaxSubmit({
                success: function (data) {
                    if (data.errors === false) {
                        var html = `<div class="item">
                                    <div class="info">
                                        <div class="user-info">

                                        <div class="avt">
                                                <img src="/html/style/images/img-avatar.webp" alt="Admin">
                                            </div>
                                        <div class="user-text">
                                        <div><span class="by">` + data.data.fullname + `</span></div>`;
                        if (data.data.rate != null && data.data.rate > 0) {
                            html += `<p class="prRating">
                                            <span>
                                                <i class="`+ (data.data.rate > 0 ? 'starLight active' : 'starLight') + `"></i>
                                                <i class="`+ (data.data.rate > 1.5 ? 'starLight active' : 'starLight') + `"></i>
                                                <i class="`+ (data.data.rate > 2.5 ? 'starLight active' : 'starLight') + `"></i>
                                                <i class="`+ (data.data.rate > 3.5 ? 'starLight active' : 'starLight') + `"></i>
                                                <i class="`+ (data.data.rate > 4.5 ? 'starLight active' : 'starLight') + `"></i>
                                            </span>
                                        </p>`;
                        }

                        html += '</div> </div>';
                        html += `<p>` + data.data.content;
                        if (data.data.urlPicture !== null) {
                            html += `<br /><img src="` + data.data.urlPicture + `" alt="` + data.data.fullname + `" style="max-width:80px;max-height:80px;" />`;
                        }
                        html +=`</p>
                                        <div class="action item-`+ data.data.id + `">
                                            <span class="_like"><img src="/html/style/images/icon-like.webp" alt="`+ GetSource('HuuIch') + `" /> (0) ` + GetSource('HuuIch') +`</span>
                                            <span class="_show-reply-rate" data-id="`+ data.data.id + `" data-pid="` + data.data.id + `">
                                                <img src="/html/style/images/icon-cmt.webp" alt="`+ GetSource('ThaoLuan') +`" /> Thảo luận
                                            </span>
                                            <span class="time-span">`+ GetSource('VuaXong') +`</span>
                                        </div>
                                        <div class="lst-repply-rate reply-`+ data.data.id + ` hiden"></div>
                                    </div>
                                </div>`;
                        var lengthItem;
                        if (data.data.rate != null && data.data.rate > 0) {
                            lengthItem = $('.list-c-d-detail-rate>.item').length;
                            if (lengthItem > 0)
                                $('.list-c-d-detail-rate>.item:nth-of-type(1)').before(html);
                            else
                                $('.list-c-d-detail-rate').html(html);
                        }
                        else {
                            lengthItem = $('.list-c-d-detail>.item').length;
                            if (lengthItem > 0)
                                $('.list-c-d-detail>.item:nth-of-type(1)').before(html);
                            else
                                $('.list-c-d-detail').html(html);
                        }
                        $('.frm-rate .action .processing').removeClass('show');
                        $('.frm-rate .action button').show();
                        let totalRate = $('.num-cmt').attr('data-total-rate');
                        $('.num-cmt span').text(`(${parseInt(totalRate) + 1}  Đánh giá)`);
                        $('#RateComment').trigger('reset');
                        ReplyRate();
                    }
                    else {
                        $('.frm-rate .action .processing').removeClass('show');
                        $('.frm-rate .action button').show();
                        $('.alrt-rate').html(data.message).slideDown();
                        console.log(data.logs);
                        setInterval(function () {
                            $('.alrt-rate').slideUp();
                        }, 3000)
                    }
                },
                error: function () {
                    $('.frm-rate .action .processing').removeClass('show');
                    $('.alrt-rate').html(GetSource('DanhGiaThatBai')).slideDown();
                    setInterval(function () {
                        $('.alrt-rate').slideUp();
                    }, 3000)
                }
            });
        }
    });
}
function MoreRate() {
    $(".btn-viewrate").click(function (e) {
        e.preventDefault();
        var p = $('#PageRate').val();
        if (p > 0) {
            $('.list-c-d-detail-rate').append(Loading);
            $.ajax({
                url: encodeURI("/Ajax/Comment/AjaxRate?page=" + p + "&productId=" + cid),
                cache: false,
                type: "POST",
                success: function (data) {
                    $('.list-c-d-detail-rate .sending').remove();
                    $('.list-c-d-detail-rate script').remove();
                    $('.list-c-d-detail-rate').append(data);
                    $('.list-c-d-detail-rate').find('.hidden').removeClass('hidden').fadeIn();
                    //$("html,body").animate({ scrollTop: $('._comment-rate').offset().top - 200 }, 1e3);
                    ShowReplyRate();
                    MoreReplyRate();
                    ReplyReplyRate();
                }
            });
        }
    });
}
function ReplyRate() {
    $('._replay-rate').click(function () {
        var frm = $(this).next('.frm-reply-rate');
        if (frm.length > 0) {
            $('._replay-rate').next('.frm-reply-rate').remove();
        }
        else {
            var id = $(this).data('id');
            var parentid = $(this).data('pid');
            var name = $(this).data('name');
            $('.list-c-d-detail-rate').find('.frm-rate').remove();
            HtmlFormReplyRate(parentid, typecontent, (name != null ? "@" + name + " - " : ''), '.list-c-d-detail-rate .item .action.item-' + id);
        }
    });
}
function ReplyReplyRate() {
    $('._replay_cmt-rate').click(function () {
        var frm = $(this).nextAll('.frm-reply-rate');
        if (frm.length > 0) {
            $('._replay_cmt-rate').nextAll('.frm-reply-rate').remove();
        }
        else {                                 
            var id = $(this).data('id');
            var parentid = $(this).data('pid');
            var name = $(this).data('name');
            $('.list-c-d-detail-rate').find('.frm-reply-rate').remove();
            HtmlFormReplyRate(parentid, typecontent, (name != null ? "@" + name + " - " : ''), '.list-c-d-detail-rate .item .action.item-' + id);
        }
    });
}
function HtmlFormReplyRate(parentid, type, name, container) {
    var html = ``;
    $.get("/html/template/rate.html", function (data, ) {
        html = data;
        html = html.replace("[ContentID]", cid).replace("[Type]", type).replace("[ParentID]", parentid).replace("[Fullname]", fullname).replace("[Email]", email).replace("[Content]", name).replace("[Phone]", phone);
        html = html.replace("[EnterComment]", GetSource("NhapNoiDung"));
        html = html.replace("[Readonly]", customeid > 0 ? "Readonly" : fullname);
        html = html.replace("[Readonly]", customeid > 0 ? "Readonly" : email);
        html = html.replace("[Readonly]", customeid > 0 ? "Readonly" : phone);
        html = html.replace("[SendImage]", GetSource("DinhKemAnh"));
        html = html.replace("[FullName]", GetSource("HoTen"));
        html = html.replace("[Phone]", GetSource("SoDienThoai"));
        html = html.replace("[Send]", GetSource("GuiDanhGia"));
        html = html.replace("[ParentID]", parentid);
        html = html.replace("[Xoa]", GetSource("Xoa"));
        $(container).append(html);
    });
}
function ShowReplyRate() {
    $('._show-reply-rate').click(function () {
        var id = $(this).data('id');
        var parentid = $(this).data('pid');
        var name = $(this).data('name');
        var frm = $(this).next('.frm-reply-rate');
        if (frm.length > 0) {
            $('._replay-rate').next('.frm-reply-rate').remove();
        }
        else {
            $('.list-c-d-detail-rate').find('.frm-reply-rate').remove();
            HtmlFormReplyRate(parentid, typecontent, (name != null ? "@" + name + " - " : ''), '.list-c-d-detail-rate .item .action.item-' + id);
        }
    });
}
function ReplyRateAction() {
    var $frm_post_create = $("#ReplyRate");
    $("#btnReplyRate").click(function () { $frm_post_create.submit(); });
    $("#ReplyRate").validate({
        rules: { Content: { required: true }, Phone: { required: true }, Email: { required: true, email: true }, Fullname: { required: true } },
        submitHandler: function () {
            debugger
            $('.frm-comment.frm-reply-rate .action .input .processing').addClass('show');
            $('.frm-comment.frm-reply-rate .action .input button').hide();
            $frm_post_create.ajaxSubmit({
                success: function (data) {
                    if (data.errors === false) {
                        var html = `<div class="item">
                                                                <div class="info">
                                                                    <span><span class="by">` + data.data.fullname + `</span></span>`;
                        if (data.data.urlPicture !== null) {
                            html += `<p>` + data.data.content + `<br/><img src="` + data.data.urlPicture + `" title="` + data.data.fullname + `" alt="` + data.data.fullname + `" style="max-width:80px;max-height:80px;" /></p>`;
                        }
                        else {
                            html += `<p>` + data.data.content + `</p>`;
                        }
                        html += `<div class="action item-` + data.data.id + `">
                                                                        <span class="time-span">`+ GetSource('VuaXong') +`</span>
                                                                    </div>
                                                                </div>
                                                            </div>`;

                        $('.list-c-d-detail-rate').find('.frm-reply-rate').remove();
                        var length = $('.lst-repply-rate.reply-' + data.data.parentID + ' .item').length;
                        if (length > 0)
                            $('.lst-repply-rate.reply-' + data.data.parentID).prepend(html).removeClass('hiden');
                        else {
                            $('.lst-repply-rate.reply-' + data.data.parentID).append(html).removeClass('hiden');
                        }
                        var total = $('.action.item-' + data.data.parentID + ' ._show-reply-rate>span').text();
                        var newtotal = parseInt(total) + 1;
                        $('.action.item-' + data.data.parentID + ' ._show-reply-rate>span').text(newtotal);
                        ReplyReplyRate();
                    }
                    else {
                        $('.frm-comment.frm-reply-rate .action .input .processing').removeClass('show');
                        $('.frm-comment.frm-reply-rate .action .input button').show();
                        $('.alrt-cmt-rate-' + data.data.parentID).html(data.message);
                    }
                }
            });
        }
    });
}
function MoreReplyRate() {
    $(".page-reply-rate").click(function (e) {
        e.preventDefault();
        var cls = $(this).parents('.lst-repply-rate').attr('class');
        var pid = cls.replace('lst-repply-rate reply-', '');
        $(this).remove();
        $('.lst-repply-rate .reply-' + pid).append(Loading);
        var url = "/Ajax/Comment/AjaxReplyRate?contentId=" + cid + "&parentId=" + pid;
        if (typecontent == "Product")
            url = "/Ajax/Comment/AjaxReplyRate?productId=" + cid + "&parentId=" + pid;
        $.ajax({
            url: encodeURI(url),
            cache: false,
            type: "POST",
            success: function (data) {
                $('.lst-repply-rate.reply-' + pid).append(data);
                ReplyReply();
            }
        });
    });
}
function LikeRate(id) {
    var like = $('.action.item-' + id + ' ._like').children('span').text();
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