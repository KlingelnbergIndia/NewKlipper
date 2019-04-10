
//cancel leave
function CancelLeave(leaveId, isRealisedLeave, isCancelledLeave) {
    var controllerName = "Leave";
    var actionName = "CancelLeave";
    alert(window.location.href);
    var urlArray = window.location.href.split('/');
    urlArray[urlArray.length - 2] = controllerName;
    urlArray[urlArray.length - 1] = actionName;
    var urlArray = urlArray.join('/');
    alert(urlArray);
    //var newURL = window.location.origin  + "/" + controllerName + "/" + actionName;

    if (isRealisedLeave == "true") {
        alert('Passed leaves can not be cancelled !');
        return;
    }
    if (isCancelledLeave == "true") {
        alert('cancelled leaves can not be updated !');
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

function CancelAddedCompOff(leaveId, isRealised, isCancelled) {
    var controllerName = "Leave";
    var actionName = "CancelAddedCompOff";
    var urlArray = window.location.href.split('/');
    urlArray[urlArray.length - 2] = controllerName;
    urlArray[urlArray.length - 1] = actionName;
    var newURL = urlArray.join('/');

    if (isRealised == "true") {
        alert('Passed leaves can not be cancelled !');
        return;
    }
    if (isCancelled == "true") {
        alert('cancelled leaves can not be updated !');
        return;
    }


    if (confirm('Do you want to cancel this comp-off ?')) {
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


