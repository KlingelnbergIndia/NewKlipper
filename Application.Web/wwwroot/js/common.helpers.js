
//convert c# date to javascript date string
function dateToYMD(date) {
    try {
        date = new Date(date);
        var strArray = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        var d = date.getDate();
        var m = strArray[date.getMonth()];
        var y = date.getFullYear();
        return '' + (d <= 9 ? '0' + d : d) + ' ' + m + ' ' + y;
    } catch (e) {
        alert('Invalid date');
        return null;
    }
}

//HTML formt is yyyy-mm-dd
function convertDateToHtmlFormat(fromDate) {
    try {
        fromDate = new Date(fromDate)
        var d = fromDate.getDate();
        var m = fromDate.getMonth() + 1;
        var y = fromDate.getFullYear();
        return '' + y + '-' + (m <= 9 ? '0' + m : m) + '-' + (d <= 9 ? '0' + d : d);
    } catch (e) {
        alert('Invalid date');
        return null;
    }
}

function decodeString(encodedString) {
    var str = encodedString.replace("%", " ");
    for (i = 0; i < encodedString.length; i++) {
        str = str.replace("%", " ");
    }
    return str;
}