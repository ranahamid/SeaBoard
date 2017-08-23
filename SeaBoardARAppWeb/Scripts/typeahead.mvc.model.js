$(document).ready($(function () {

    function autocompletewrapper(obj) {        
        var autos = new Bloodhound({
            datumTokenizer: function (datum) {
                return Bloodhound.tokenizers.whitespace(datum.value);
            },
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            remote: {
                wildcard: "%QUERY",            
                url: $(obj).data("autocomplete-url") + "?query=%QUERY",
                ajax: {
                            beforeSend: function(xhr, settings) {
                                $("#pplLoder").show();
                            },
                        complete: function(xhr, status) {
                            $("#pplLoder").hide();
                        }
                    },
                filter: function (autos) {
                  
                    // Map the remote source JSON array to a JavaScript object array
                    return $.map(autos, function (auto) {
                        return {
                            value: auto.LookupValue,
                            id: auto.LookupId
                        };
                    });
                }
            },
            limit: 1000
        });

        autos.initialize();

        $(obj).typeahead({ highlight: true, minLength: 0, hint: true }, {
            name: 'autos', displayKey: 'value', source: autos.ttAdapter()
        }).on('typeahead:selected', function (obj, datum) {
            onselected(obj, datum);
            $("#pplLoder").hide();
        });        
        
        if ($(obj).hasClass("focus")) {
            $(obj).focus();
        }

    };

    function onselected(obj, datum) {
        if (!obj || !obj.target || !datum) return;
        $('#' + jQuery(obj.target).data("autocomplete-id-field")).val(datum.id.toString());
        $("#PMOwner").val(datum.id.toString());
    }

    $('*[data-autocomplete-url]').each(function () {                
                autocompletewrapper($(this));
            });
})
)