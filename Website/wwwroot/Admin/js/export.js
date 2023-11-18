
//https://datatables.net/extensions/buttons/examples/html5/simple.html
$(document).ready(function () {
    $(".noexport").remove();
    $('#tblContent').DataTable({
        dom: 'Bfrtip',
        buttons: [
            //'copyHtml5',
            'excelHtml5',
            'csvHtml5',
            'pdfHtml5'
        ]
    });
});
$(document).ready(function () {
  
    var excel = GetValueUrl(window.location.href, "IsExcel");
    var pdf = GetValueUrl(window.location.href, "IsPdf");
    var csv = GetValueUrl(window.location.href, "IsCsv");
    if (excel == "true") {
        setTimeout(function () {
            $(".buttons-excel").click();
        }, 1000);
    } else if (pdf == "true") {
        setTimeout(function () { $(".buttons-pdf").click(); }, 1000);
    } else if (csv == "true") {
        setTimeout(function () { $(".buttons-csv").click(); }, 1000);
    }
});