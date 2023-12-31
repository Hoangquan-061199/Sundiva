$(document).ready(function () {
    $('.menu-icon').click(function() {
        $(this).toggleClass('change');
        $('header .container .right').toggleClass('active');
    });

    $('.menu ul li .btn-down').click(function() {
        $('.menu ul li .btn-down').not(this).siblings('.sub-menu').slideUp();
        $(this).siblings('.sub-menu').slideToggle();
    });

    $("header .search .icon").click(function () {
        $("header .search .icon").addClass("active");
        $(this).removeClass("active");
        $("header .search form").toggleClass("active");
    });

    $('.slide-index').owlCarousel({
        items: 1,
        loop: 1,
        margin: 0,
        nav: 1,
        navText: ['<svg xmlns="http://www.w3.org/2000/svg" width="50px" height="50px" fill="currentColor" class="bi bi-chevron-left" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M11.354 1.646a.5.5 0 0 1 0 .708L5.707 8l5.647 5.646a.5.5 0 0 1-.708.708l-6-6a.5.5 0 0 1 0-.708l6-6a.5.5 0 0 1 .708 0z"/></svg>',
            '<svg xmlns="http://www.w3.org/2000/svg" width="50px" height="50px" fill="currentColor" class="bi bi-chevron-right" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M4.646 1.646a.5.5 0 0 1 .708 0l6 6a.5.5 0 0 1 0 .708l-6 6a.5.5 0 0 1-.708-.708L10.293 8 4.646 2.354a.5.5 0 0 1 0-.708z"/></svg>'],
        dots: 0,
        autoplay: 1,
        autoplayTimeout: 5000,
        smartSpeed: 1000,
        responsive: {
            0: {
                items: 1,
                dots: 0,
                nav: 0,
            },
            767: {
                items: 1,
                dots: 0,
                nav: 0,
            },
            992: {
                items: 1,
            },
        }
    });

    $('.slide-news-index').owlCarousel({
        items: 3,
        loop: 1,
        margin: 35,
        nav: 0,
        navText: ['<svg xmlns="http://www.w3.org/2000/svg" width="50px" height="50px" fill="currentColor" class="bi bi-chevron-left" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M11.354 1.646a.5.5 0 0 1 0 .708L5.707 8l5.647 5.646a.5.5 0 0 1-.708.708l-6-6a.5.5 0 0 1 0-.708l6-6a.5.5 0 0 1 .708 0z"/></svg>',
            '<svg xmlns="http://www.w3.org/2000/svg" width="50px" height="50px" fill="currentColor" class="bi bi-chevron-right" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M4.646 1.646a.5.5 0 0 1 .708 0l6 6a.5.5 0 0 1 0 .708l-6 6a.5.5 0 0 1-.708-.708L10.293 8 4.646 2.354a.5.5 0 0 1 0-.708z"/></svg>'],
        dots: 1,
        autoplay: 1,
        autoplayTimeout: 5000,
        smartSpeed: 1000,
        responsive: {
            0: {
                items: 1,
                dots: 0,
                nav: 0,
            },
            767: {
                items: 2,
                margin: 20,
                dots: 0,
                nav: 0,
            },
            992: {
                items: 3,
            },
        }
    });

    $('.slide-construction-index').owlCarousel({
        items: 2.5,
        loop: 1,
        margin: 20,
        nav: 1,
        navText: ['<svg xmlns="http://www.w3.org/2000/svg" height="20px" viewBox="0 0 512 512"><path d="M9.4 233.4c-12.5 12.5-12.5 32.8 0 45.3l128 128c12.5 12.5 32.8 12.5 45.3 0s12.5-32.8 0-45.3L109.3 288 480 288c17.7 0 32-14.3 32-32s-14.3-32-32-32l-370.7 0 73.4-73.4c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0l-128 128z"/></svg>',
            '<svg xmlns="http://www.w3.org/2000/svg" height="20px" viewBox="0 0 512 512"><path d="M502.6 278.6c12.5-12.5 12.5-32.8 0-45.3l-128-128c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3L402.7 224 32 224c-17.7 0-32 14.3-32 32s14.3 32 32 32l370.7 0-73.4 73.4c-12.5 12.5-12.5 32.8 0 45.3s32.8 12.5 45.3 0l128-128z"/></svg>'],
        dots: 0,
        autoplay: 1,
        autoplayTimeout: 5000,
        smartSpeed: 1000,
        responsive: {
            0: {
                items: 1,
                dots: 0,
                nav: 0,
            },
            767: {
                items: 1.5,
                margin: 20,
                dots: 0,
                nav: 0,
            },
            992: {
                items: 2.5,
            },
        }
    });

    $('.slide-construction').owlCarousel({
        items: 2,
        loop: 1,
        margin: 20,
        nav: 1,
        navText: ['<svg xmlns="http://www.w3.org/2000/svg" height="20px" viewBox="0 0 512 512"><path d="M9.4 233.4c-12.5 12.5-12.5 32.8 0 45.3l128 128c12.5 12.5 32.8 12.5 45.3 0s12.5-32.8 0-45.3L109.3 288 480 288c17.7 0 32-14.3 32-32s-14.3-32-32-32l-370.7 0 73.4-73.4c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0l-128 128z"/></svg>',
            '<svg xmlns="http://www.w3.org/2000/svg" height="20px" viewBox="0 0 512 512"><path d="M502.6 278.6c12.5-12.5 12.5-32.8 0-45.3l-128-128c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3L402.7 224 32 224c-17.7 0-32 14.3-32 32s14.3 32 32 32l370.7 0-73.4 73.4c-12.5 12.5-12.5 32.8 0 45.3s32.8 12.5 45.3 0l128-128z"/></svg>'],
        dots: 0,
        autoplay: 1,
        autoplayTimeout: 5000,
        smartSpeed: 1000,
        responsive: {
            0: {
                items: 1,
                dots: 0,
                nav: 0,
            },
            767: {
                items: 1.5,
                margin: 20,
                dots: 0,
                nav: 0,
            },
            992: {
                items: 2,
            },
        }
    });

    $('.slide-capacity').owlCarousel({
        items: 1,
        loop: 1,
        margin: 20,
        nav: 1,
        navText: ['<svg xmlns="http://www.w3.org/2000/svg" height="20px" viewBox="0 0 512 512"><path d="M9.4 233.4c-12.5 12.5-12.5 32.8 0 45.3l128 128c12.5 12.5 32.8 12.5 45.3 0s12.5-32.8 0-45.3L109.3 288 480 288c17.7 0 32-14.3 32-32s-14.3-32-32-32l-370.7 0 73.4-73.4c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0l-128 128z"/></svg>',
            '<svg xmlns="http://www.w3.org/2000/svg" height="20px" viewBox="0 0 512 512"><path d="M502.6 278.6c12.5-12.5 12.5-32.8 0-45.3l-128-128c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3L402.7 224 32 224c-17.7 0-32 14.3-32 32s14.3 32 32 32l370.7 0-73.4 73.4c-12.5 12.5-12.5 32.8 0 45.3s32.8 12.5 45.3 0l128-128z"/></svg>'],
        dots: 0,
        autoplay: 1,
        autoplayTimeout: 5000,
        smartSpeed: 1000,
        responsive: {
            0: {
                items: 1,
                dots: 0,
                nav: 0,
            },
            767: {
                items: 1,
                margin: 20,
                dots: 0,
                nav: 0,
            },
            992: {
                items: 1,
            },
        }
    });


    var sync1 = $(".slide-product-detail");
    var sync2 = $(".slide-product-detail1");
    var slidesPerPage = 3; //globaly define number of elements per page
    var syncedSecondary = true;

    sync1.owlCarousel({
        items: 1,
        slideSpeed: 2000,
        nav: 1,
        autoplay: 0,
        dots: 0,
        loop: 1,
        responsiveRefreshRate: 200,
        navText: ['<svg xmlns="http://www.w3.org/2000/svg" height="25" viewBox="0 0 320 512"><path d="M9.4 233.4c-12.5 12.5-12.5 32.8 0 45.3l192 192c12.5 12.5 32.8 12.5 45.3 0s12.5-32.8 0-45.3L77.3 256 246.6 86.6c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0l-192 192z"/></svg>', '<svg xmlns="http://www.w3.org/2000/svg" height="25" viewBox="0 0 320 512"><path d="M310.6 233.4c12.5 12.5 12.5 32.8 0 45.3l-192 192c-12.5 12.5-32.8 12.5-45.3 0s-12.5-32.8 0-45.3L242.7 256 73.4 86.6c-12.5-12.5-12.5-32.8 0-45.3s32.8-12.5 45.3 0l192 192z"/></svg>'],
    }).on('changed.owl.carousel', syncPosition);

    sync2
        .on('initialized.owl.carousel', function () {
            sync2.find(".owl-item").eq(0).addClass("current");
        })
        .owlCarousel({
            items: slidesPerPage,
            dots: 0,
            nav: 0,
            smartSpeed: 200,
            slideSpeed: 500,
            slideBy: slidesPerPage,
            responsiveRefreshRate: 100,
            margin: 10
        }).on('changed.owl.carousel', syncPosition2);

    function syncPosition(el) {
        //if you set loop to false, you have to restore this next line
        //var current = el.item.index;

        //if you disable loop you have to comment this block
        var count = el.item.count - 1;
        var current = Math.round(el.item.index - (el.item.count / 2) - .5);

        if (current < 0) {
            current = count;
        }
        if (current > count) {
            current = 0;
        }

        //end block

        sync2
            .find(".owl-item")
            .removeClass("current")
            .eq(current)
            .addClass("current");
        var onscreen = sync2.find('.owl-item.active').length - 1;
        var start = sync2.find('.owl-item.active').first().index();
        var end = sync2.find('.owl-item.active').last().index();

        if (current > end) {
            sync2.data('owl.carousel').to(current, 100, true);
        }
        if (current < start) {
            sync2.data('owl.carousel').to(current - onscreen, 100, true);
        }
    }

    function syncPosition2(el) {
        if (syncedSecondary) {
            var number = el.item.index;
            sync1.data('owl.carousel').to(number, 100, true);
        }
    }

    sync2.on("click", ".owl-item", function (e) {
        e.preventDefault();
        var number = $(this).index();
        sync1.data('owl.carousel').to(number, 300, true);
    });

    $('.product-detail .item .btn-down').click(function() {
        $(this).toggleClass('active');
        $(this).parent('.title').siblings(".content").slideToggle();
    });

    $('.slide-other-curtain').owlCarousel({
        items: 3,
        loop: 1,
        margin: 15,
        nav: 1,
        navText: ['<svg xmlns="http://www.w3.org/2000/svg" width="30px" height="30px" fill="currentColor" class="bi bi-chevron-left" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M11.354 1.646a.5.5 0 0 1 0 .708L5.707 8l5.647 5.646a.5.5 0 0 1-.708.708l-6-6a.5.5 0 0 1 0-.708l6-6a.5.5 0 0 1 .708 0z"/></svg>',
            '<svg xmlns="http://www.w3.org/2000/svg" width="30px" height="30px" fill="currentColor" class="bi bi-chevron-right" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M4.646 1.646a.5.5 0 0 1 .708 0l6 6a.5.5 0 0 1 0 .708l-6 6a.5.5 0 0 1-.708-.708L10.293 8 4.646 2.354a.5.5 0 0 1 0-.708z"/></svg>'],
        dots: 0,
        autoplay: 1,
        autoplayTimeout: 5000,
        smartSpeed: 1000,
        responsive: {
            0: {
                items: 1,
                dots: 0,
                nav: 0,
            },
            767: {
                items: 2,
                margin: 20,
                dots: 0,
                nav: 0,
            },
            992: {
                items: 3,
            },
        }
    });

    $('.slide-news').owlCarousel({
        items: 1,
        loop: 1,
        margin: 10,
        nav: 0,
        navText: ['<svg xmlns="http://www.w3.org/2000/svg" width="30px" height="30px" fill="currentColor" class="bi bi-chevron-left" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M11.354 1.646a.5.5 0 0 1 0 .708L5.707 8l5.647 5.646a.5.5 0 0 1-.708.708l-6-6a.5.5 0 0 1 0-.708l6-6a.5.5 0 0 1 .708 0z"/></svg>',
            '<svg xmlns="http://www.w3.org/2000/svg" width="30px" height="30px" fill="currentColor" class="bi bi-chevron-right" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M4.646 1.646a.5.5 0 0 1 .708 0l6 6a.5.5 0 0 1 0 .708l-6 6a.5.5 0 0 1-.708-.708L10.293 8 4.646 2.354a.5.5 0 0 1 0-.708z"/></svg>'],
        dots: 1,
        autoplay: 1,
        autoplayTimeout: 5000,
        smartSpeed: 1000,
        responsive: {
            0: {
                items: 1,
                dots: 0,
                nav: 0,
            },
            767: {
                items: 1,
                margin: 20,
                dots: 0,
                nav: 0,
            },
            992: {
                items: 1,
            },
        }
    });

    $('.slide-related-news').owlCarousel({
        items: 3,
        loop: 1,
        margin: 30,
        nav: 1,
        navText: ['<svg xmlns="http://www.w3.org/2000/svg" width="30px" height="30px" fill="currentColor" class="bi bi-chevron-left" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M11.354 1.646a.5.5 0 0 1 0 .708L5.707 8l5.647 5.646a.5.5 0 0 1-.708.708l-6-6a.5.5 0 0 1 0-.708l6-6a.5.5 0 0 1 .708 0z"/></svg>',
            '<svg xmlns="http://www.w3.org/2000/svg" width="30px" height="30px" fill="currentColor" class="bi bi-chevron-right" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M4.646 1.646a.5.5 0 0 1 .708 0l6 6a.5.5 0 0 1 0 .708l-6 6a.5.5 0 0 1-.708-.708L10.293 8 4.646 2.354a.5.5 0 0 1 0-.708z"/></svg>'],
        dots: 0,
        autoplay: 1,
        autoplayTimeout: 5000,
        smartSpeed: 1000,
        responsive: {
            0: {
                items: 1,
                dots: 0,
                nav: 0,
            },
            767: {
                items: 2,
                margin: 20,
                dots: 0,
                nav: 0,
            },
            992: {
                items: 3,
            },
        }
    });

    $('.slide-history').owlCarousel({
        items: 3,
        loop: 1,
        margin: 20,
        nav: 1,
        navText: ['<svg xmlns="http://www.w3.org/2000/svg" width="50" height="50" fill="currentColor" class="bi bi-arrow-left" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M15 8a.5.5 0 0 0-.5-.5H2.707l3.147-3.146a.5.5 0 1 0-.708-.708l-4 4a.5.5 0 0 0 0 .708l4 4a.5.5 0 0 0 .708-.708L2.707 8.5H14.5A.5.5 0 0 0 15 8z"/></svg>',
            '<svg xmlns="http://www.w3.org/2000/svg" width="50" height="50" fill="currentColor" class="bi bi-arrow-right" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M1 8a.5.5 0 0 1 .5-.5h11.793l-3.147-3.146a.5.5 0 0 1 .708-.708l4 4a.5.5 0 0 1 0 .708l-4 4a.5.5 0 0 1-.708-.708L13.293 8.5H1.5A.5.5 0 0 1 1 8z"/></svg>'],
        dots: 0,
        autoplay: 1,
        autoplayTimeout: 5000,
        smartSpeed: 1000,
        responsive: {
            0: {
                items: 1,
                dots: 0,
                nav: 0,
            },
            767: {
                items: 2,
                margin: 20,
                dots: 0,
                nav: 0,
            },
            992: {
                items: 3,
            },
        }
    });

    $('.slide-personnel').owlCarousel({
        items: 3,
        loop: 1,
        margin: 0,
        nav: 1,
        navText: ['<svg xmlns="http://www.w3.org/2000/svg" width="30px" height="30px" fill="currentColor" class="bi bi-chevron-left" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M11.354 1.646a.5.5 0 0 1 0 .708L5.707 8l5.647 5.646a.5.5 0 0 1-.708.708l-6-6a.5.5 0 0 1 0-.708l6-6a.5.5 0 0 1 .708 0z"/></svg>',
            '<svg xmlns="http://www.w3.org/2000/svg" width="30px" height="30px" fill="currentColor" class="bi bi-chevron-right" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M4.646 1.646a.5.5 0 0 1 .708 0l6 6a.5.5 0 0 1 0 .708l-6 6a.5.5 0 0 1-.708-.708L10.293 8 4.646 2.354a.5.5 0 0 1 0-.708z"/></svg>'],
        dots: 0,
        autoplay: 1,
        autoplayTimeout: 5000,
        smartSpeed: 1000,
        center: 1,
        responsive: {
            0: {
                items: 1,
                dots: 0,
                nav: 0,
            },
            767: {
                items: 2,
                margin: 20,
                dots: 0,
                nav: 0,
            },
            992: {
                items: 3,
            },
        }
    });

    $('.slide-achievements').owlCarousel({
        items: 4,
        loop: 1,
        margin: 25,
        nav: 1,
        navText: ['<svg xmlns="http://www.w3.org/2000/svg" width="30px" height="30px" fill="currentColor" class="bi bi-chevron-left" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M11.354 1.646a.5.5 0 0 1 0 .708L5.707 8l5.647 5.646a.5.5 0 0 1-.708.708l-6-6a.5.5 0 0 1 0-.708l6-6a.5.5 0 0 1 .708 0z"/></svg>',
            '<svg xmlns="http://www.w3.org/2000/svg" width="30px" height="30px" fill="currentColor" class="bi bi-chevron-right" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M4.646 1.646a.5.5 0 0 1 .708 0l6 6a.5.5 0 0 1 0 .708l-6 6a.5.5 0 0 1-.708-.708L10.293 8 4.646 2.354a.5.5 0 0 1 0-.708z"/></svg>'],
        dots: 0,
        autoplay: 1,
        autoplayTimeout: 5000,
        smartSpeed: 1000,
        responsive: {
            0: {
                items: 1,
                dots: 0,
                nav: 0,
            },
            767: {
                items: 2,
                margin: 20,
                dots: 0,
                nav: 0,
            },
            992: {
                items: 4,
            },
        }
    });
});
