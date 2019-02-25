
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