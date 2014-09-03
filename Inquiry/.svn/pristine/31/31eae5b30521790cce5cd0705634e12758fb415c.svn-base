
// This function changes the page to the passed url. All filters are passed as query string.
function ChangePage(url) {
    // _prefix is a global variable set by the page. This prefix is removed from all input names.
    // So if _prefix = 'Filters', we change query string 'Filters.a=1&Filters.b=2' to 'a=1&b=2'
    var data = $('#mypage select, input')
        .filter(function () {
            // Only consider those inputs which have a non null value
            return $(this).val();
        }).serialize().split(_prefix).join('');
    //var url = $(this).attr('action');
    if (data) {
        url += '?' + data;
    }
    //alert(url);
    window.location = url;
}

$(document).ready(function () {
    // Manually enhance popup outside the page
    $('#datesPopup,#statusPopup').enhanceWithin().popup();

    // When any form is submitted, all input control values are posted to the form's action
    $('#mypage').on('click', '#btnExcel', function (e) {
        ChangePage($(this).attr('data-url'));
        return false;
    });

    // Postback the page when any filter changes: Type, Sewing Plant, Variance
    $('#responsivegroup').on('change', 'input, select', function (e) {
        ChangePage($(e.delegateTarget).attr('data-url'));
    });

    // User confirms date entry. Post pack page.
    $('#datesPopup').on('click', '#btnApplyFilter', function (e) {
        if (!$(this).closest('form').valid()) {
            alert('Not Valid');
            return;
        }
        $('#hfStatus').val($(this).attr('data-status-value'));
        ChangePage($('#responsivegroup').attr('data-url'));        
        return false;
    });

    $('#statusPopup').on('click', 'li > a:not([data-rel])', function (e) {
        // User chose status Open or Closed.
        $('#hfStatus').val($(this).attr('data-value'));
        ChangePage($('#responsivegroup').attr('data-url'));
    }).on('click', 'li > a[data-rel]', function (e) {
        // User wants to set dates. Show date dialog.
        $('#statusPopup').one('popupafterclose', function (e) {
            // Open the new popup only after the previous popup has fully closed
            $('#datesPopup').popup('open');
        }).popup('close');
        return false;
    });

}).on('pagecontainertransition', function (event, ui) {
    $('#responsivegroup', ui.toPage).responsivegroup();
});
