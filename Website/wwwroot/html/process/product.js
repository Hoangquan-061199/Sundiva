var seo = $('#seoUrl').val();
$(function () {
    ViewMore();
    $('#OrderProductTour').change(function () {
        $('#grid-load .item').remove();
        let sort = $(this).val()
        more(1, sort);
    })
});

function ViewMore() {
    $('.btn-page').click(function (e) {
        e.preventDefault();
        let p = $(this).data('page');
        let sort = $('#OrderProductTour').val();
        $('#page').val(p);
        $(this).remove();
        more(p, sort);
    });
}

function more(p, sort) {
    seo = $('#seoUrl').val();
    moduleId = $('.ModuleId').val();
    var url = '/Ajax/Content/PartialProduct?seoUrl=' + seo + '&page=' + p + '&moduleId=' + moduleId + '&sort=' + sort;
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


