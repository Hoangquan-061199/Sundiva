var seo = $('#seoUrl').val();
$(function () {
    ViewMore();
    ViewMoreR();
});

function ViewMore() {
    $('.btn-page').click(function (e) {
        e.preventDefault();
        var p = $(this).data('page');
        $('#page').val(p);
        $(this).remove();
        more(p);
    });
}

function ViewMoreR() {
    $('.btn-recuitment').click(function (e) {
        e.preventDefault();
        var p = $(this).data('page');
        $('#page').val(p);
        $(this).remove();
        moreR(p);
    });
}

function more(p) {
    seo = $('#seoUrl').val();
    moduleId = $('#moduleId').val();
    var url = '/Ajax/Content/PartialNews?seoUrl=' + seo + '&page=' + p + '&moduleId=' + moduleId;
    var container = '#grid-load';
    $(container).find('script').remove();
    $('.load').addClass('show');
    $.ajax({
        url: encodeURI(url),
        cache: false,
        type: "POST",
        dataType: 'html',
        success: function (data) {
            $(container).find('.btn-block').remove();
            $('.load').removeClass('show');
            $(container).append(data);
            ViewMore();
        },
        errors: function () {
            window.alert("Tải dữ liệu không thành công")
        }
    });
}

function moreR(p) {
    seo = $('#seoUrl').val();
    moduleId = $('#moduleId').val();
    number = $('#number').val();
    var url = '/Ajax/Content/PartialRecuitment?page=' + p + '&moduleId=' + moduleId + '&number=' + number;
    var container = '#grid-load';
    $(container).find('script').remove();
    $('.load').addClass('show');
    $.ajax({
        url: encodeURI(url),
        cache: false,
        type: "POST",
        dataType: 'html',
        success: function (data) {
            $('.load').removeClass('show');
            $(container).append(data);
            ViewMoreR();
        },
        errors: function () {
            window.alert("Tải dữ liệu không thành công")
        }
    });
}
