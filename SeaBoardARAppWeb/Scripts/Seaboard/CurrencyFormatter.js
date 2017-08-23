$(function () {
          if (!$.browser.msie) {
              
              $(".nonIE").show();
              $(".IE").hide();
             
          }
          else {
              $(".nonIE").hide();
              $(".IE").show();
          }

          $('.rightAlign').each(function () {
              $(this).css("text-align", "right");
          });
            
         // Funds_Committed, In_Budget set to unchekced
          //  debugger
            var totalCost = $("#Total_Cost").val();
            if(totalCost<=0)
            {
                $('#Funds_Committed').attr('checked', false);
                $('#In_Budget').attr('checked', false);
            }

      });




$(function () {
    var lastCurrency=0;
    $('.number').blur(function () {
        //debugger;
        var thisValue=$(this).val();
        if (thisValue != "" && thisValue !== undefined)
        {
            var intext;
            if (isNaN(thisValue) || (!isFinite(thisValue))) {
                intext = '$' + parseFloat(lastCurrency, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
            }
            else {
                intext = '$' + parseFloat(thisValue, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
            }
          
            $(this).val(intext);
        }
        else
        {
            $(this).val("$0.00");
        }
        $(this).css("text-align", "right");
    });

    $('.number').each(function () {
        var intext = '$' + parseFloat($(this).val(), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
        $(this).val(intext);
        
        $(this).css("text-align", "right");
    });


    $(".number").focus(function () {
        //debugger
        var nm = $(this).val();
        var nmbValue = nm.replace("$", "").replace(/,/g, '');
        var intext = parseFloat(nmbValue, 10).toFixed(2).toString();
        lastCurrency = intext;
        $(this).val(intext);
        $(this).css("text-align", "right");
    });

  
});


function SetTotalCost(total) {
    $("#Total_Cost").removeAttr('readonly');

    var intext = '$' + parseFloat(total, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();

    $("#Total_Cost").val(intext);
    $("#Total_Cost").attr('readonly', true);
}