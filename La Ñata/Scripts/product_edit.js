let quant = 0;

function increaseValue() {
    let value = parseInt(document.getElementById('quantity').value, 10);
    value = isNaN(value) ? 0 : value;
    value >= quant ? value = quant - 1 : '';
    value++;
    document.getElementById('quantity').value = value;
}

function decreaseValue() {
    let value = parseInt(document.getElementById('quantity').value, 10);
    value = isNaN(value) ? 0 : value;
    value < 1 ? value = 1 : '';
    value--;
    document.getElementById('quantity').value = value;
}

$("#btnSearch").on("click", function () {
    let name = $("#txtSearch").val();
    let date = $("#datepicker").val();
    $.get("../../Products/ProductsStock/?name=" + name + "&date=" + date, function (data) {
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
        content += "<tr class='clickable-row row100 body' onclick='openModal(" + data[i].product_id + ", " + data[i].stock + ")' data-bs-toggle='modal' data-bs-target='#exampleModal'>";
        content += "<td class='column20 ps-3'>" + data[i].description + "</td>";
        let stock = data[i].stock != null ? data[i].stock : "-"
        content += "<td class='column10 text-center'>" + stock + "</td>";
        content += "<td class='column10 text-center'>$" + data[i].price + "</td>";
        content += "<td class='column10 text-center'>$" + data[i].break_price + "</td>";
        content += "</tr>";
    }
    $("#contentTable").html(content);
}

function openModal(id, stock) {
    $("#btnSendFormSubstract").css("display", "none");
    $("#quantity").val("");
    $("#id_product").val(id);
    $("#real_stock").val(stock);
    $("#old_price").val(null);
    $("#currentStock").html("Stock existente: " + stock);
    quant = stock;
}

function openModalOldPrice(id, stock, old_price) {
    $("#btnSendFormSubstract").css("display", "block");
    $("#quantity").val("");
    $("#id_product").val(id);
    $("#real_stock").val(stock);
    $("#old_price").val(old_price);
    $("#currentStock").html("Stock existente: " + stock);
    quant = stock;
}

$("#btnSendFormAdd").on("click", function (e) {
    e.preventDefault();
    let quant = parseInt($("#quantity").val());
    let stock = parseInt($("#real_stock").val());
    if (quant <= stock && quant > 0) {
        $("#price").attr("name", "old_price");
        $("#formEditOrder").attr("action", "/Orders/AddToExistingOrder");
        $("#formEditOrder").submit();
    } else if (quant <= 0) {
        alert("Ingrese una cantidad mayor a 0");
    } else {
        alert("Ingrese una cantidad menor o igual al stock existente")
    }
});

$("#btnSendFormSubstract").on("click", function (e) {
    e.preventDefault();
    let quant = parseInt($("#quantity").val());
    if (quant > 0) {
        $("#old_price").attr("name", "price");
        $("#formEditOrder").attr("action", "/Orders/RemoveFromExistingOrder");
        $("#formEditOrder").submit();
    } else if (quant <= 0) {
        alert("Ingrese una cantidad mayor a 0");
    } else {
        alert("Ingrese una cantidad menor o igual al stock existente")
    }
});