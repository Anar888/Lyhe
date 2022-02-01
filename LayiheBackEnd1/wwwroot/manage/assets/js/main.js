$(document).on("click", "#butdelet", function (e) {
    e.preventDefault();
    swal({
        title: "Silmek istediyinize eminsiniz?!",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                swal("Muveffeqiyyetle silindi!");
                setTimeout(function () {
                    $('form').submit();
                    return true;
                }, 1300);



            } else {
                swal("Silinmedi!");
            }
        });
});