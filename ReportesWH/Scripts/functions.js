function getVirtDir() {
    var Path = location.host;
    var VirtualDirectory;

    if (Path.indexOf("localhost") >= 0 && Path.indexOf(":") >= 0) {
        VirtualDirectory = "";
    }

    else {
        var pathname = window.location.pathname;
        var VirtualDir = pathname.split('/');
        VirtualDirectory = VirtualDir[1];
        VirtualDirectory = '/' + VirtualDirectory;
    }
    return VirtualDirectory;
}

function getRpwh() {
    $.ajax({
        method: "POST",
        url: getVirtDir() + "/Controllers/getReport.ashx",
        data: { ds: $("#date").val() },
        success: function (data) {
            $("#getTable").toggle();
            r = jQuery.parseJSON(data);
            if (r.result == "true") {
                $("#getTable").html(r.html);
                $("#Export").removeAttr("disabled");
            }
            else
                $("#getTable").html(r.MessageError);
            $("#loader").toggle();
            //return false;

        },
        error: function () { }
    });
}

function logIn() {
    $.ajax({
        method: "POST",
        url: getVirtDir() + "/Controllers/Accounts.ashx",
        data: { user: $("#user").val(), pass: $("#pass").val() },
        success: function (data) {
            r = jQuery.parseJSON(data);
            if (r.result == "true") {
                location.reload();
            }
            else {
                alertify.error('Error de logeo...');
            }
            
        },
        error: function () { }
    });
}

function logOUT() {
    $.ajax({
        method: "POST",
        url: getVirtDir() + "/Controllers/Letmeout.ashx",
        data: {},
        success: function (data) {
            r = jQuery.parseJSON(data);
            if (r.result == "true") {
                location.reload();
            }
            else {
                alertify.error('Error...');
            }

        },
        error: function () { }
    });
}

function fnExcelReport() {
    var tab_text = "<meta http-equiv='content-type' content='application/vnd.ms-excel; charset=UTF-8'>" +
        "<table border='2px'>" + "<tr bgcolor='#5ab8ff'>";
    var j = 0;
    tab = document.getElementById('report'); // id of table
    for (j = 0; j < tab.rows.length; j++) {
        tab_text = tab_text + tab.rows[j].innerHTML + "</tr>";

    }
    var ua = window.navigator.userAgent;
    sa = window.open('data:application/vnd.ms-excel,' + encodeURIComponent(tab_text));
    return (sa);
}