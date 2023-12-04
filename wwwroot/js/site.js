(function () {
    'use strict';

    let APP_JS = {
        init: function () {
            this.product_quantity();
            this.product_btn_size();
            this.product_links_wap();
            this.accordion();
            this.add_to_cart();
        },
        product_quantity: function () {
            if ($(".quantity-input").length > 0) {
                $(".quantity-input").on("click", ".btn", function (event) {
                    event.preventDefault();
                    let _this = $(this),
                        _input = _this.siblings("input[name=product-quantity]"),
                        _value = _this.siblings("span[name=var-value]"),
                        _current_value = _this
                            .siblings("input[name=product-quantity]")
                            .val(),
                        _max_value = _this
                            .siblings("input[name=product-quantity]")
                            .attr("data-max");

                    const current_value = parseInt(_current_value, 10) * 1;
                    if (_this.hasClass("btn-minus")) {
                        if (current_value > 1) {
                            _input.val(current_value - 1);
                            _value.html(current_value - 1);
                            return false;
                        }
                    } else if (_this.hasClass("btn-plus")) {
                        if (current_value < parseInt(_max_value, 10)) {
                            _input.val(current_value + 1);
                            _value.html(current_value + 1);
                            return false;
                        }
                    }
                    return false;
                });
            }
        },
        product_btn_size: function () {
            $('.btn-size').click(function () {
                let this_val = $(this).html();
                $("#product-size").val(this_val);
                $(".btn-size").removeClass('btn-secondary');
                $(".btn-size").addClass('btn-success');
                $(this).removeClass('btn-success');
                $(this).addClass('btn-secondary');
                return false;
            });
        },
        product_links_wap: function () {
            $('.product-links-wap a').click(function () {
                let this_src = $(this).children('img').attr('src');
                $('#product-detail').attr('src', this_src);
                return false;
            });
        },
        accordion: function () {
            // Accordion
            let all_panels = $('.templatemo-accordion > li > ul').hide();

            $('.templatemo-accordion > li > a').click(function () {
                let target = $(this).next();
                if (!target.hasClass('active')) {
                    all_panels.removeClass('active').slideUp();
                    target.addClass('active').slideDown();
                } else {
                    target.removeClass('active').slideUp();
                }
                return false;
            });
            // End accordion
        },
        add_to_cart: function () {
            //create cart
            $('.add-to-cart').click(function (e) {
                e.preventDefault();
                let pid = $(this).data("id") ?? '';
                let quantity = $('#product_quantity_' + pid).val() ?? 1;
                console.log(pid);
                console.log(quantity);
                var data = {
                    pid: pid,
                    quantity: quantity
                };
                postAjax(data);
            });

            function postAjax(data) {
                $.ajax({
                    type: "POST",
                    url: "/update-cart",
                    data: data,
                    success: function (result) {
                        console.log(result);
                        //show site-cart
                        // if (data.status == 'error') {
                        //     $('#error_add').append(data.pid + " " + data.message);
                        //     wrapper_cart.classList.add('active-popup');
                        //     setTimeout(function () {
                        //         $('#error_add').remove();
                        //         wrapper_cart.classList.remove('active-popup');

                        //     }, 3000);
                        //     return;
                        // }
                        // wrapper_cart.classList.add('active-popup');
                        // getCart();
                    },
                    error: function (e) {
                        console.log(e);
                    }
                });
            }


        },

    }

    $(document).ready(function () {
        APP_JS.init();
    });

})();