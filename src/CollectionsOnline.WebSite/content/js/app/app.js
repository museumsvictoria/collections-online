var $ = require('jquery');
var jsCookie = require('js-cookie');
var media = require('./media');
var search = require('./search');
var map = require('./map');

var App = {
  init: function() {
    media.init();
    search.init();
    map.init();
    
    this.cacheElements();
    this.bindEvents();
    this.culturalMessageCheck();
  },
  cacheElements: function () {
    this.$culturalMessage = $('#cultural-message');
    this.$culturalMessageButton = $('button', this.$culturalMessage);
  },
  bindEvents: function () {
    $('.social-tools a').on('click', this.openWindow.bind(this));
    this.$culturalMessageButton.on('click', this.disableCulturalMessage.bind(this));
  },
  openWindow: function(e) {
    var target = e.currentTarget;

    var width = 550, height = 450;

    if (target && target.href) {
      var left = Math.round((screen.width / 2) - (width / 2));
      var top = 0;
      var windowOptions = 'scrollbars=yes,resizable=yes,toolbar=no,location=yes';

      if (screen.height > height) {
        top = Math.round((screen.height / 2) - (height / 2));
      }

      window.open(target.href, 'share', windowOptions + ',width=' + width + ',height=' + height + ',left=' + left + ',top=' + top);
      e.returnValue = false;
      e.preventDefault && e.preventDefault();
    }
  },
  culturalMessageCheck: function () {
    var culturalCookie = jsCookie.get('culturalMessage');

    if(culturalCookie === undefined) {
      jsCookie.set('culturalMessage', 'false', { secure: true, expires: 180, sameSite: 'strict' });
      this.$culturalMessage.removeClass('disabled');
    } else if(culturalCookie !== 'true' ) {
      this.$culturalMessage.removeClass('disabled');
    }
  },
  disableCulturalMessage: function () {
    jsCookie.set('culturalMessage', 'true', { secure: true, expires: 180, sameSite: 'strict' });
    this.$culturalMessage.addClass('disabled');
  }
};

$(document).ready(function () {
  App.init();
});

$.whenArray = function (array) {
  return $.when.apply(this, array);
};

$.storageAvailable = function(type) {
  try {
    var storage = window[type],
      x = '__storage_test__';
    storage.setItem(x, x);
    storage.removeItem(x);
    return true;
  } catch (e) {
    return false;
  }
};