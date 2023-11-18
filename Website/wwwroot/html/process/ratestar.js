$(() => {
    $('.rate input').change(function () {
        let star = ""
        let isRate = false;
        let rates = localStorage.getItem('rates');
        let id = $('#ProductId').val();
        if (rates) {
            let list = rates.split(',');
            for (let i = 0; i < list.length; i++) {
                if (list[i] == id) {
                    isRate = true;
                }
            }
        }
        if (!isRate) {
            if (this.checked)
                star = $(this).val()


            let rate = $('.rate .total-rate').text();
            let url = '/Ajax/Content/RateStar?star=' + star + '&ProductId=' + id;
            $.ajax({
                url: encodeURI(url),
                cache: false,
                type: "POST",
                dataType: 'html',
                success: function (data) {
                    localStorage.setItem('rates', rates ? rates + ',' + id : id)
                    $('.rate .total-star').text(data)
                    $('.rate .total-rate').text(Number(rate) + 1)
                    alert("Cảm ơn bạn đã đánh giá!");
                },
                errors: function () {
                    window.alert("Tải dữ liệu không thành công")
                }
            });
        } else {
            alert("Bạn đã đánh giá rồi!");
        }


    })
    let isHover = false;
    $('.rate .star.first').hover(function () {
        if (!isHover) {
            isHover = true;
            $(this).addClass('none')
            $(this).next().removeClass('none')
        }
    })

})