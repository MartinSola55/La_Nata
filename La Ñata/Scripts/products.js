function increaseValue(id, quant) {
    let value = parseInt(document.getElementById('number' + id).value, 10);
    value = isNaN(value) ? 0 : value;
    value >= quant ? value = quant - 1 : '';
    value++;
    document.getElementById('number' + id).value = value;
}

function decreaseValue(id) {
    let value = parseInt(document.getElementById('number' + id).value, 10);
    value = isNaN(value) ? 0 : value;
    value < 1 ? value = 1 : '';
    value--;
    document.getElementById('number' + id).value = value;
}

$("#btnSearch").on("click", function () {
    let name = $("#txtSearch").val();
    $.get("../../Products/FilterProductsByName/?name=" + name, function (data) {
        createTable(data);
    })
});

$('#txtSearch').keypress(function (e) {
    if (e.which == 13) {
        $(this).blur();
        $('#btnSearch').click();
    }
});

function createTable(data) {
    let content = "";
    for (let i = 0; i < data.length; i++) {
        content += "<tr class='clickable-row row100 body' data-href='/Products/Edit/" + data[i].product_id + "'>";
        content += "<td class='column20 ps-3'>" + data[i].description + "</td>";
        content += "<td class='column10 text-center'>" + data[i].stock + "</td>";
        content += "<td class='column10 text-center'>$" + data[i].price + "</td>";
        content += "<td class='column10 text-center'>$" + data[i].break_price + "</td>";
        content += "</tr>";
    }
    $("#contentTable").html(content);

    $(".clickable-row").click(function () {
        window.location = $(this).data("href");
    });
}