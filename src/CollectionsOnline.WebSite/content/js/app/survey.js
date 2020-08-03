var $ = require('jquery');

module.exports = {
    init: function () {
        this.bindEvents();
        this.mouseMoved = false;
    },
    bindEvents: function () {
        $(window).on("mousemove", function () {
            this.mouseMoved = true;
        });
    },
    initialiseSurvey: function() {
        if (this.mouseMoved) {
            $(document).mouseleave(function () {
                this.showSurvey();
            });
        }
        else {
            setTimeout(this.showSurvey, 5000);
        }
    },
    showSurvey: function() {
        if (!$("div.Survey").length && !localStorage.getItem("dismissSurvey")) {
            var objSurvey = $("<div>").addClass("Survey").html("<div class=\"Inner\"><h3>Share your thoughts by August 31 to WIN</h3><a href=\"#\" class=\"Close\"><svg width=\"28\" height=\"28\" viewBox=\"0 0 28 28\"><path fill=\"none\" fill-rule=\"nonzero\" d=\"M28 2.82L25.18 0 14 11.18 2.82 0 0 2.82 11.18 14 0 25.18 2.82 28 14 16.82 25.18 28 28 25.18 16.82 14z\"></path></svg></a><p>Tell us about your online experiences with the museum. Our survey will take less than 10 minutes and all entries will go into the draw to win a $100 gift voucher at our online store!</p><div class=\"LinkContainer\"><ul class=\"NoBullet\"><li><a href=\"https://www.surveygizmo.com/s3/5651341/MVwebsites\" class=\"ArrowLink\"><svg class=\"ArrowForward\" width=\"16\" height=\"16\" viewBox=\"0 0 16 16\"><path fill=\"none\" fill-rule=\"nonzero\" d=\"M15.695 8L8.95.229 7.547 1.752l4.542 5.105H.334v2.21h11.755l-4.542 5.18 1.403 1.524L15.695 8z\"></path></svg>Start survey<svg class=\"ExternalLink\" width=\"16px\" height=\"16px\" viewBox=\"0 0 16 16\"><g stroke=\"none\" stroke-width=\"1\" fill=\"none\" fill-rule=\"evenodd\"><polygon points=\"0 0 15.8690058 0 15.8690058 15.8690058 0 15.8690058\"></polygon><path d=\"M13.4444444,13.4444444 L2.55555556,13.4444444 L2.55555556,2.55555556 L8,2.55555556 L8,1 L2.55555556,1 C1.69222222,1 1,1.7 1,2.55555556 L1,13.4444444 C1,14.3 1.69222222,15 2.55555556,15 L13.4444444,15 C14.3,15 15,14.3 15,13.4444444 L15,8 L13.4444444,8 L13.4444444,13.4444444 Z M9.55555556,1 L9.55555556,2.55555556 L12.3477778,2.55555556 L4.70222222,10.2011111 L5.79888889,11.2977778 L13.4444444,3.65222222 L13.4444444,6.44444444 L15,6.44444444 L15,1 L9.55555556,1 Z\" fill=\"#FFFFFF\"></path></g></svg></a></li></ul></div></div>");
            $("body").append(objSurvey);
            objSurvey.css({
                "bottom": $("div.Survey").outerHeight() * -1,
                "display": "block"
            });
            objSurvey.animate(
                {
                    "bottom": 0
                }, 500, function () {
                }
            );
            $("a.Close").click(function (e) {
                e.preventDefault();
                localStorage.setItem("dismissSurvey", true);
                $("div.Survey").animate(
                    {
                        "bottom": $("div.Survey").outerHeight() * -1
                    }, 500, function () {
                        $(this).css("display", "none");
                    }
                );
            });
        }
    }
};