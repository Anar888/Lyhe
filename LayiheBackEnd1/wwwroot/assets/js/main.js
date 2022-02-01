
$(document).ready(function () {
    $(document).on("click", ".open-product-modal", function (e) {
        e.preventDefault();

        let url = $(this).attr("href");

        fetch(url)
            .then(response => response.text())
            .then(data => {

                $("#quick_view .productmodal").html(data);
                $("#quick_view").modal(true)
            });


    });
})