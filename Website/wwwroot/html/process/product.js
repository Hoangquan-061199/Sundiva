let min = "";
let max = "";
$(function () {
    min = $('#Min').text();
    max = $('#Max').text();
    ViewMore();
    $('.more_attr').click(function () {
        $(this).prevAll('.hides').show();
        $(this).remove();
    });
    var wt = 80;
    $('.rangeslider-price').jRange({
        showScale: false,
        scale: [min, max],
        format: '%s',
        width: wt + '%',
        step: 1,
        showLabels: true,
        isRange: true,
        from: min,
        to: max,
        onstatechange: function (data) {
            var values = data.split(',');
            $('#PriceFrom').val(values[0]);
            $('#PriceTo').val(values[1]);
            if (values[0] == min && values[1] == max) {
                $('input[name=gia]').val('');
            }
            else {
                $('input[name=gia]').val(data);
            }
        },
        onbarclicked: function (data) {
            $('#page').val(1);
        },
        ondragend: function (data) {
            $('#page').val(1);
        }
    });
    $('#PriceTo').change(function () {
        var from = $('#PriceFrom').val();
        var to = $(this).val();
        if (parseInt(to) > max) {
            to = max;
            $(this).val(max);
        }
        if (isNaN(to)) {
            to = max;
            $(this).val(max);
        }
        var ran = from + ',' + to;
        $('.rangeslider-price').jRange('setValue', ran);
        if (from == min && to == max) $('input[name=gia]').val(''); else $('input[name=gia]').val(ran);
        $('#page').val(1);
    });
    $('#PriceFrom').change(function () {
        var to = $('#PriceTo').val();
        var from = $(this).val();
        if (parseInt(from) < min) {
            from = min;
            $(this).val(min);
        }
        if (isNaN(from)) {
            from = min;
            $(this).val(min);
        }
        var ran = from + ',' + to;
        $('.rangeslider-price').jRange('setValue', ran);
        $('#page').val(1);
    });
    $('.filter_price').click(function () {
        let seo = $('#seoUrl').val();
        let newurl = '/' + seo;
        window.history.pushState('', '', newurl);
        let p = $('#page').val();
        moreProduct(p);
    });
});

function ViewMore() {
    $('.page div').click(function (e) {
        let seo = $('#seoUrl').val();
        e.preventDefault();
        //$(this).parent('.more_product').remove();
        var p = $(this).data('page');
        $('#page').val(p);
        let newurl = '/' + seo + '/page/' + p;
        window.history.pushState('', '', newurl);
        moreProduct(p);
    });
}

function moreProduct(p) {
    let price = $('#price-rank').val();
    let seo = $('#seoUrl').val();
    let url = '/Ajax/Content/PartialProduct?seoUrl=' + seo + '&page=' + p + '&gia=' + price;
    let container = '.grid-load-product';
    $(container).find('script').remove();
    $(".load").addClass('show');
    $.ajax({
        url: encodeURI(url),
        cache: false,
        type: "POST",
        dataType: 'html',
        success: function (data) {
            $(container).find('.load').remove();
            $(container).html(data);
            ViewMore();
            $(".load").removeClass('show');

            if ($(window).width() < 991) {
                $('.module-filter').removeClass('active');
                $('.overlay-fillter').removeClass('active');
                $(window).scrollTop($('.price-quote .container .right').offset().top - 60)
            }
        },
        errors: function () {
        }
    });
}

