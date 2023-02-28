function parseDate(date) {
    return moment(date).format("DD/MM/YYYY").toUpperCase()
}

let hoy = new Date();
let dd = String(hoy.getDate()).padStart(2, '0');
let mm = String(hoy.getMonth() + 1).padStart(2, '0');
let yyyy = hoy.getFullYear();
hoy = dd + '/' + mm + '/' + yyyy;
$("#date").removeAttr("data-val-date");


$(document).ready(function () {
    let dates = hoy + "," + hoy;
    $.get("../../Expenses/ExpensesByDate/?dates=" + dates, function (data) {
        createTable(data);
    })
    $("#printReport").attr("href", "../../Expenses/Print/?dates_range=" + dates);
});

$("#datepicker").on("apply.daterangepicker", function (ev, picker) {
    let date_from = picker.startDate.format('YYYY-MM-DD');
    let date_to = picker.endDate.format('YYYY-MM-DD');
    let dates = [date_from, date_to];
    $.get("../../Expenses/ExpensesByDate/?dates=" + dates, function (data) {
        createTable(data);
    })
    $("#printReport").attr("href", "../../Expenses/Print/?dates_range=" + dates);
});

function createTable(data) {
    let content = "";
    for (let i = 0; i < data.length; i++) {
        content += "<tr class='clickable-row row100 body' onclick='openModalEdit(" + data[i].id + ")' data-bs-toggle='modal' data-bs-target='#modalExpense' >";
        content += "<td class='column20 ps-2'>" + data[i].description + "</td>";
        content += "<td class='column10 text-center'>$" + data[i].price + "</td>";
        content += "<td class='column10 text-center'>" + parseDate(data[i].date) + "</td>";
        content += "<td class='column10 text-center'>";
        content += `<button id="btnSearch" class="btn" style="background-color: #ff5969; color: white; padding: 0.1rem 0.3rem" data-bs-toggle="modal" data-bs-target="#modalExpenseDelete"`
            + `onclick='openModalDelete(` + data[i].id + `)'>
                        <i class="bi bi-x" style="color: white"></i>
                    </button>`;
        content += "</td>";
        content += "</tr>";
    }
    $("#contentTable").html(content);
}

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
        singleDatePicker: false,
        opens: 'right',
        autoUpdateInput: true,
        autoApply: true
    })
});

function cleanFields() {
    $(".cleanField").val("");
}

function openModal() {
    $("#formExpense").attr("action", "/Expenses/Create");
    $("#modalExpenseLabel").text("Agregar gasto");
    cleanFields();
    $("#id_expense").val("");
    $("#date").daterangepicker({
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
        autoApply: true,
    })
}

function openModalEdit(id) {
    $("#formExpense").attr("action", "/Expenses/Edit");
    $("#modalExpenseLabel").text("Editar gasto");
    $("#id_expense").val(id);
    $.get("../Expenses/GetOne/?id=" + id, function (data) {
        $("#date").daterangepicker({
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
            autoApply: true,
            startDate: parseDate(data['date']),
            endDate: parseDate(data['date']),
        })
        $("#date").val(parseDate(data['date']));
        $("#description").val(data['description']);
        $("#price").val(data['price']);
    });
}

function openModalDelete(id) {
    $("#expense_id").val(id);
}