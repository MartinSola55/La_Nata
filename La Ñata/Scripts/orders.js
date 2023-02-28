$("#datepicker").on("change", function () {
    let date = $("#datepicker").val();
    $.get("../../Orders/OrdersByDate/?date=" + date, function (data) {
        createTable(data);
    })
});

function createTable(data) {
    let content = "";
    for (let i = 0; i < data.length; i++) {
        content += "<tr class='clickable-row row100 body' data-href='/Orders/Details/" + data[i].id + "'>";
        content += "<td class='column10 ps-3'>" + data[i].client_name + "</td>";
        content += "<td class='column10 text-center pe-sm-0 pe-4'>" + (data[i].phone != null ? data[i].phone : "-") + "</td>";
        content += "<td class='column10 text-center'>" + data[i].event_adress + "</td>";
        content += "</tr>";
    }
    $("#contentTable").html(content);

    $(".clickable-row").click(function () {
        window.location = $(this).data("href");
    });
}

let hoy = new Date();
let dd = String(hoy.getDate()).padStart(2, '0');
let mm = String(hoy.getMonth() + 1).padStart(2, '0');
let yyyy = hoy.getFullYear();
hoy = dd + '/' + mm + '/' + yyyy;
$("#datepicker").removeAttr("data-val-date");

moment.locale('es');
$(function () {
    $("#datepicker").daterangepicker({
        "autoApply": true,
        "locale": {
            "applyLabel": "Aplicar",
            "cancelLabel": "Cancelar",
            "fromLabel": "Hasta",
            "toLabel": "Desde",
        },
        singleDatePicker: true,
        opens: 'right',
        autoUpdateInput: true,
        autoApply: true
    })
});