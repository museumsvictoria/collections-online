var $ = require('jquery');
var jsCookie = require('js-cookie');

module.exports = {
    init: function () {
        this.cacheElements();
        this.bindEvents();
        this.surveyCheck();
    },
    cacheElements: function () {
        this.$survey = $('<div id=\"survey\"><div class=\"inner\"><h3>Share your thoughts to WIN</h3><button class=\"close\" title=\"Close\"><svg width=\"28\" height=\"28\" viewBox=\"0 0 28 28\"><path fill=\"none\" fill-rule=\"nonzero\" d=\"M28 2.82L25.18 0 14 11.18 2.82 0 0 2.82 11.18 14 0 25.18 2.82 28 14 16.82 25.18 28 28 25.18 16.82 14z\"></path></svg></button><p>We\'d love to hear about your experience with our website. Our survey takes less than 10 minutes and entries go in a draw to win a $100 gift voucher at our online store!</p><div class=\"link-container\"><ul class=\"no-bullet\"><li><a href=\"https://forms.office.com/Pages/ResponsePage.aspx?id=c7eHETV0x0mw8smxexNODmTOCBBwIMZIphXUOM4CRONUN040NFFaU09JM01WMFM1WjFNVkNLS0JESSQlQCN0PWcu\" class=\"arrow-link\" target=\"_blank\"><svg class=\"arrow-forward\" width=\"16\" height=\"16\" viewBox=\"0 0 16 16\"><path fill=\"none\" fill-rule=\"nonzero\" d=\"M15.695 8L8.95.229 7.547 1.752l4.542 5.105H.334v2.21h11.755l-4.542 5.18 1.403 1.524L15.695 8z\"></path></svg>Start survey<svg class=\"external-link\" width=\"16px\" height=\"16px\" viewBox=\"0 0 16 16\"><g stroke=\"none\" stroke-width=\"1\" fill=\"none\" fill-rule=\"evenodd\"><polygon points=\"0 0 15.8690058 0 15.8690058 15.8690058 0 15.8690058\"></polygon><path d=\"M13.4444444,13.4444444 L2.55555556,13.4444444 L2.55555556,2.55555556 L8,2.55555556 L8,1 L2.55555556,1 C1.69222222,1 1,1.7 1,2.55555556 L1,13.4444444 C1,14.3 1.69222222,15 2.55555556,15 L13.4444444,15 C14.3,15 15,14.3 15,13.4444444 L15,8 L13.4444444,8 L13.4444444,13.4444444 Z M9.55555556,1 L9.55555556,2.55555556 L12.3477778,2.55555556 L4.70222222,10.2011111 L5.79888889,11.2977778 L13.4444444,3.65222222 L13.4444444,6.44444444 L15,6.44444444 L15,1 L9.55555556,1 Z\" fill=\"#FFFFFF\"></path></g></svg></a></li></ul></div></div></div>');
        this.$closeButton = $('.close', this.$survey);
        this.$surveyLink = $('.arrow-link', this.$survey);
    },
    bindEvents: function () {
        this.$closeButton.on('click', this.onClick.bind(this, 90));
        this.$surveyLink.on('click', this.onClick.bind(this, 180));
    },
    surveyCheck: function() {
        var cookie = jsCookie.get('surveyViewed');
        
        if(cookie === 'true') {
            return;
        }

        if(cookie === undefined) {
            jsCookie.set('surveyViewed', 'false', { secure: true, expires: 90, sameSite: 'strict' });
        }

        setTimeout(this.showSurvey.bind(this), 5000);
    },
    disableSurvey: function(days) {
        jsCookie.set('surveyViewed', 'true', { secure: true, expires: days, sameSite: 'strict' });
    },
    onClick: function(days) {
        var cookie = jsCookie.get('surveyViewed');

        if(cookie !== true) {
            this.disableSurvey(days);
        }
        
        this.closeSurvey();
    },
    showSurvey: function() {
        this.$survey.appendTo('body');

        this.$survey.css({
            'bottom': this.$survey.outerHeight() * -1,
            'display': 'block'
            });

        this.$survey.animate({
            'bottom': 0
        }, 500);
    },
    closeSurvey: function() {
        this.$survey.animate({
            "bottom": this.$survey.outerHeight() * -1
        }, 500, function() {
            $(this).css("display", "none");
        });
    }
};