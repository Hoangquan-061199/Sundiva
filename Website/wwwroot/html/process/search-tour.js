var seo = $('#seoUrl').val();
$(function () {
    ViewMore();
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

function more(p) {
    keyword = $('#keyword').val();
    TourType = $('#TourType').val();
    AddressStart = $('#AddressStart').val();
    Times = $('#Times').val();
    var url = '/Ajax/Content/PartialSearchTour?q=' + keyword + '&page=' + p + '&TourType=' + TourType + '&AddressStart=' + AddressStart + '&Times=' + Times;
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
