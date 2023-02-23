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