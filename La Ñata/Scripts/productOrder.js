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
    $("#id_product").val(id);
    $("#stock").val(stock);
    $("#currentStock").html("Stock existente: " + stock);
    quant = stock;
}

$("#btnSendForm").on("click", function (e) {
    e.preventDefault();
    let quant = parseInt($("#quantity").val());
    let stock = parseInt($("#stock").val());
    let id_product = $("#id_product").val();
    if (quant <= stock) {
        $.get("../../Orders/AddToOrder/?id_prod=" + id_product + "&quant=" + quant + "&real_stock=" + stock, function () {
            location.reload();
        })
    } else {
        alert("Ingrese una cantidad menor o igual al stock existente")
    }
});

$("#formCreateOrder").on("submit", function (e) {
    e.preventDefault();
    let obs = $("#observation").val();
    console.log(obs)
    $("#Order_observation").val(obs);
    //$("#formCreateOrder").submit();
});