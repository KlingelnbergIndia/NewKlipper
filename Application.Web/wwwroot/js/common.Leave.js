
//cancel leave
function CancelLeave(leaveId, isRealisedLeave, isCancelledLeave) {
    var controllerName = "Leave";
    var actionName = "CancelLeave";
    var newURL = window.location.protocol + "//" + window.location.host + "/" + controllerName + "/" + actionName;

    if (isRealisedLeave == "true" || isCancelledLeave=="true") {
        alert('leave can not be cancel!');
        return;
    }

    if (confirm('Do you want to cancel this leave ?')) {
        $.ajax({
            type: "POST",
            url: newURL,
            data: {
                LeaveId: leaveId
            },
            dataType: "html",
            success: function (response) {
                window.location.reload();
            }
        });
    }
}


