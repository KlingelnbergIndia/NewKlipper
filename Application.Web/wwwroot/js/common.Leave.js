﻿
//cancel leave
function CancelLeave(leaveId) {
    var controllerName = "Leave";
    var actionName = "CancelLeave";
    var newURL = window.location.protocol + "//" + window.location.host + "/" + controllerName + "/" + actionName;

    if (confirm('Do you want to really cancel this leave ?')) {
        $.ajax({
            type: "POST",
            url: newURL,
            data: {
                LeaveId: leaveId
            },
            dataType: "html",
            success: function (response) {
                alert('Leave canceled !');
                window.location.reload();
            }
        });
    }
}

//Update leave
function CancelLeave(leaveId) {
    var controllerName = "Leave";
    var actionName = "UpdateLeave";
    var newURL = window.location.protocol + "//" + window.location.host + "/" + controllerName + "/" + actionName;

    if (confirm('Do you want to really cancel this leave ?')) {
        $.ajax({
            type: "POST",
            url: newURL,
            data: {
                LeaveId: leaveId
            },
            dataType: "html",
            success: function (response) {
                alert('Leave canceled !');
                window.location.reload();
            }
        });
    }
}
