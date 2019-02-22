
//cancel leave
function CancelLeave(leaveId,isRealisedLeave) {
    var controllerName = "Leave";
    var actionName = "CancelLeave";
    var newURL = window.location.protocol + "//" + window.location.host + "/" + controllerName + "/" + actionName;

    if (isRealisedLeave == "true") {
        alert('Realised leave can not be regularized !');
        return;
    }

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


