
$(document).ready(function () {


    $(document).on("click", ".productdetail", function (e) {
        e.preventDefault()

        let url = $(this).attr("href");
        document.getElementById('productId').value = 0;
        document.getElementById('productcount').value = 1;
        document.getElementById('mainImage').src = '';
        var element = document.createElement('option');
        var sizeIdSelect = $('#sizeId');
        element.value = 0;
        element.innerHTML = 'Choose..';
        sizeIdSelect.html(element)

        //<span class="dec qtybtn">-</span>
        //<input type="text" id="productcount" name="count" value="1">
        //<span class="inc qtybtn">+</span>
        var qty_div = $('#pro_qty');

        var dec_element = document.createElement('span');
        dec_element.classList.add('dec');
        dec_element.classList.add('qtybtn');
        dec_element.innerHTML = '-';

        var productcount = document.createElement('input');
        productcount.type='text';
        productcount.id ='productcount';
        productcount.name = 'count';
        productcount.value = '1';

        var inc_element = document.createElement('span');
        inc_element.classList.add('inc');
        inc_element.classList.add('qtybtn');
        inc_element.innerHTML = '+';

        qty_div.html('');
        qty_div.append(dec_element)
        qty_div.append(productcount)
        qty_div.append(inc_element)


        $.ajax({
            method: 'GET',
            url: url
        }).then(function (res) {
            document.getElementById('productId').value = res.id;
            document.getElementById('mainImage').src = 'assets/images/product/' + res.mainImage;
            document.getElementById('name').innerHTML = res.name;
            document.getElementById('description').innerHTML = res.description;
            document.getElementById('product_price').innerHTML = '$' + res.price;
            document.getElementById('dist_price').innerHTML = '$' + res.discountPrice;
        })

        $.ajax({
            method: 'GET',
            url: '/Product/Getsizes'
        }).then(function (res) {
            for (let i = 0; i < res.length; i++) {
                var added = document.createElement('option');
                var sizeSelect = $('#sizeId');
                added.value = res[i].id;
                added.innerHTML = res[i].name;
                sizeSelect.append(added);
            }
        })

        $("#quick_view").modal("show")
        $('.btn-close').on('click', function (e) {
            e.preventDefault();
            $("#quick_view").modal("hide")

        })


        $('.product-view-mode a').on('click', function (e) {
            e.preventDefault();
            var shopProductWrap = $('.shop-product-wrap');
            var viewMode = $(this).data('target');
            $('.product-view-mode a').removeClass('active');
            $(this).addClass('active');
            shopProductWrap.removeClass('grid-view list-view').addClass(viewMode);
        })

        // quantity change js
       
        $('.qtybtn').on('click', function () {
            var $button = $(this);
            var oldValue = $button.parent().find('input').val();
            if ($button.hasClass('inc')) {
                var newVal = parseFloat(oldValue) + 1;
            } else {
                // Don't allow decrementing below zero
                if (oldValue > 0) {
                    var newVal = parseFloat(oldValue) - 1;
                } else {
                    newVal = 0;
                }
            }
            $button.parent().find('input').val(newVal);
            document.getElementById('pro_count').value = newVal;
        });

        document.getElementById('pro_count').value = 1;
    })




    $(document).on("click", "#addbasketbtn", function (e) {
        e.preventDefault()
        let url = $("#basketform").attr("action")
        let count = document.getElementById('pro_count').value;
        let id = document.getElementById('productId').value;
        if (count == null || count == undefined || count == 0) {
            alert('Increase product count');
        }
        let sizeId = $('#sizeId').val();

        if (sizeId == null || sizeId == undefined || sizeId == 0) {
        	alert('Select a size!');
        	return;
        }

        url = url + "?count=" + count + '&&sizeId=' + sizeId + '&&id=' + id;
        fetch(url).then(response => {
            return response.text();
        }).then(data => {
            $(".minicart-inner-content").html(data)

        })
    })

    $(document).on("click", ".addbasketlink", function (e) {

        e.preventDefault()

        let url = $(this).attr("href")+ 

        fetch(url).then(response => {
            return response.text();
        }).then(data => {
            $("#quick_view").modal("hide")
            //$("#quick_view2").modal("hide")
            $(".minicart-inner-content").html(data);
        })
    })

    $(document).on("click", ".basketUpdate", function (e) {
        e.preventDefault();
        let url = $(this).attr("href");
        let count = $(this).parent().children()[1].value;
        count = parseInt(count);

        if ($(this).hasClass("subCount")) {
            count--;
        }
        else if ($(this).hasClass("addCount")) {
            count++;
        }
        $(this).parent().children()[1].value = count
        url = url + "?count=" + count;

        fetch(url).then(response => {
            fetch("Basket/GetBasket").then(response => response.text()).then(data => $(".header-cart").html(data))
            return response.text()
        }).then(data => $(".basketContainer").html(data))
    })
    $(document).on("keyup", ".basketItemCount", function () {
        let url = $(this).next().attr("href");
        url = url + "?count=" + $(this).val();

        if ($(this).val().trim()) {
            fetch(url).then(response => response.text()).then(data => $(".basketContainer").html(data))

        }
    })
    $(document).on("click", ".deletecard", function (e) {
        e.preventDefault();

        let url = $(this).attr("href");

        fetch(url).then(response => {
            fetch("Basket/GetBasket").then(response => response.text()).then(data => $(".minicart-inner-content").html(data))

            return response.text()
        }).then(data => $(".basketContainer").html(data))
    })
    $(document).on("click", ".deletebasket", function (e) {
        e.preventDefault();

        let url = $(this).attr("href");

        fetch(url).then(response => {
            fetch("Basket/GetCard").then(response => response.text()).then(data => $(".basketContainer").html(data))

            return response.text()
        }).then(data => $(".minicart-inner-content").html(data))
    })
})






