/*
Control group is always horizontal within a header

HTML
----
<div id="responsivegroup">
    <h3>@Model.Categories[i].Name</h3>
    <div data-role="controlgroup" data-type="horizontal" style="text-align:center">
        @for (var j = 0; j < Model.Categories.Count; ++j)
        {
            <a class="ui-btn ui-btn-active">@(j + 1). @Html.DisplayFor(m => m.Categories[j].Description)</a>
        }
    </div>
</div>

Javascript
----------
$(document).on('pagecontainertransition', function (event, ui) {
    $('#responsivegroup', ui.toPage).responsivegroup();
});
*/
(function ($, undefined) {
    $.widget("dcmsmobile.responsivegroup", {
        options: {
            heading: "h1,h2,h3,h4,h5,h6,legend"
        },
        _create: function () {
            this._on(this.window, {
                resize: '_delayRefresh'
            });
            $(this.options.heading, this.element).eq(0).hide();
            this.refresh();
        },

        _timer: null,
        _delayRefresh: function () {
            if (this._timer) {
                // Do nothing
                return;
            }
            if (this.element.is('.ui-collapsible')) {
                this._destroyCollapsible();
            }
            this._timer = setTimeout(function (self) {
                //adaptMenu2();
                self.refresh();
            }, 500, this);
        },
        _createCollapsible: function () {
            $(this.options.heading, this.element).eq(0).show();
            this.element.collapsible({
                heading: this.options.heading
            });
            $('.ui-controlgroup', this.element).controlgroup('option', 'type', 'vertical');
        },
        _destroyCollapsible: function () {
            this.element.collapsible('destroy');
            $(this.options.heading, this.element).eq(0).hide();
            $('.ui-controlgroup', this.element).controlgroup('option', 'type', 'horizontal');
        },
        refresh: function () {
            if (this._timer) {
                // Clear the timer so that we do not refresh again. We should get here only when being called by _delayRefresh
                clearTimeout(this._timer);
                this._timer = null;
            }
            var x;
            var wrapped = false;
            $(".ui-controlgroup-controls >", this.element).each(function () {
                var top = Math.round($(this).position().top);
                if (x == null) {
                    x = top
                } else if (x != top) {
                    wrapped = true;
                }
            });
            if (this.element.is('.ui-collapsible')) {
                if (!wrapped) {
                    this._destroyCollapsible();
                }
            } else {
                if (wrapped) {
                    this._createCollapsible();
                }
            }
        }
    });
})(jQuery);


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

