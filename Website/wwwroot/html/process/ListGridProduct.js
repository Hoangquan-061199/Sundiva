$(() => {
    let wattage = getUrlParameter('wattage');
    let sort = getUrlParameter('sort');
    let page = getUrlParameter('page');

    if (wattage && sort) {
        page = page ? page : 1;
        hanlderFillterProduct(wattage, sort, page)
        $('.list-select-filter #orderby').val(sort)
        $('.list-select-filter #power-capacity').val(wattage)
        $("#page").val(page);
    }
    if (!wattage && sort) {
        page = page ? page : 1;
        hanlderFillterProduct('', sort, page)
        $('.list-select-filter #orderby').val(sort)
        $('.list-select-filter #power-capacity').val('')
        $("#page").val(page);
    }
    if (wattage && !sort) {
        page = page ? page : 1;
        hanlderFillterProduct(wattage, '', page)
        $('.list-select-filter #orderby').val('')
        $('.list-select-filter #power-capacity').val(wattage)
        $("#page").val(page);
    }

    if (page && page >= 1 && !wattage && !sort) {
        hanlderFillterProduct('', '', page)
        $('.list-select-filter #orderby').val('')
        $('.list-select-filter #power-capacity').val('')
        $("#page").val(page);
    }


    $('.list-select-filter #power-capacity').change(function () {
        wattage = this.value;
        page = $("#page").val();
        hanlderFillterProduct(wattage, sort, page)
        $("#page").val(page);
    })

    $('.list-select-filter #orderby').change(function () {
        sort = this.value;
        page = $("#page").val();
        hanlderFillterProduct(wattage, sort, page)
        $("#page").val(page);
    })
    viewMore(wattage, sort);
})

function viewMore(wattage, sort) {
    $(".page div").click(function (n) {
        n.preventDefault();
        var p = $(this).data("page");
        $("#page").val(p);
        let w = wattage ? wattage : '';
        let s = sort ? sort : '';
        hanlderFillterProduct(w, s, p);
    })
}

function hanlderFillterProduct(wattage, sort, p) {
    let hastag = getHastag(wattage, sort);
    let seo = $('#seoUrl').val();
    let container = '#load-produt';
    let url = '/Ajax/Content/PartialFillterProduct?seoUrl=' + seo + '&page=' + p + '&' + hastag;
    $(".load").addClass('show');
    $.ajax({
        url: encodeURI(url),
        cache: false,
        type: "POST",
        dataType: 'html',
        success: function (data) {
            $(".load").removeClass('show');
            $(container).html(data);
            RenewUrl(wattage, sort, p);
            viewMore(wattage, sort);
        },
        errors: function () {
            window.alert("Tải dữ liệu không thành công");
        }
    });
}

function RenewUrl(wattage, sort, p) {
    let seo = $('#seoUrl').val();
    let page = $('#page').val();
    let hastag = getHastag(wattage, sort);
    let newUrl = '';
    if (seo.startsWith("/")) {
        newUrl = seo;
    }
    else {
        newUrl = '/' + seo;
    }
    if (parseInt(p) === 1) {
        newUrl += hastag != '' ? '?' + hastag : '';
    } else {
        newUrl += '?page=' + p;
        newUrl += hastag != '' ? '&' + hastag : '';

    }
    window.history.pushState('', '', newUrl);
}
function getHastag(wattage, sort) {
    let hastag = ''
    if (wattage && sort) hastag = 'wattage=' + wattage + '&sort=' + sort;
    else if (wattage) hastag = 'wattage=' + wattage;
    else if (sort) hastag = 'sort=' + sort;
    else hastag = '';

    return hastag;
}