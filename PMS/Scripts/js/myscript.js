(function($){
    
    // Dynamic head-Box height:
    var headHeight = $(".head-Box").innerHeight();
    
    // Dynamic padding top in body according to nav height:
    $("body").css("paddingTop", headHeight);
    
    // Dashboard side navigation function:
    $(".toggle_button").on('click', function () {
        $("body").toggleClass("sidemini");
    });
    
    // Dashboard side navigation drop down function:
    $(document).on("click", "#sidenav_Bar ul li a", function(){
        $(this).siblings("dl").stop().slideToggle();
        $(this).closest("li").siblings("li").find("dl").stop().slideUp();
        if($(this).hasClass("rotate")){
            $(this).removeClass("rotate");
        }else{
            $("#sidenav_Bar ul li a").removeClass("rotate");
            $(this).addClass("rotate");
        }
    });

    // Dashboard clockin and clockout button function:
    $(document).on("click", "#time_foot_clockin", function(){
        $("#time_foot_clockout").removeClass("disabled");
        $(this).addClass("disabled");

        $("#time_content_msg").fadeIn();
        setTimeout(function() { $("#time_content_msg").fadeOut(); }, 5000);
    })

    $(document).on("click", "#time_foot_clockout", function(){
        $("#time_foot_clockin").addClass("disabled");
        $(this).addClass("disabled");

        $("#time_content_msg").fadeIn();
        setTimeout(function() { $("#time_content_msg").fadeOut(); }, 5000);
    })

    // Scroll to top button appear:
    $(document).on('scroll', function() {
        var scrollDistance = $(this).scrollTop();
        if (scrollDistance > 100) {
            $("#scrolltop_Box").fadeIn();
        } else {
            $("#scrolltop_Box").fadeOut();
        }
    });

    // Smooth scrolling using jQuery:
    $(document).on("click", "#scrolltop_Bar ul li a", function(e) {
        var $anchor = $(this);
        $("html, body").stop().animate({
            scrollTop: ($($anchor.attr('href')).offset().top)
        }, 1000);
        e.preventDefault();
    });
    
    // Add the following code if you want the name of the file appear on select
    $(document).on("change",".custom-file-input", function(){
        var fileName = $(this).val().split("\\").pop();
        $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
    });

})(jQuery);

var activeIDName = $("#tab_block ul").find("li.active").attr('data-trigger');
$("#" + activeIDName).show();
// alert(activeIDName);
$("#tab_block ul li").click(function(){
    var getIDName = $(this).attr('data-trigger');
    $("#" + getIDName).show();
    $(".tab-content:not(#"+getIDName+")").hide();
    // $("#tab_block ul li").removeClass('active');
    $(this).siblings('li').removeClass('active');
    $(this).addClass('active');
});






