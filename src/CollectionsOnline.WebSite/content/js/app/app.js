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

    this.bindEvents();
    this.showCulturalMessage();
  },
  bindEvents: function () {
    $('.social-tools a').on('click', this.openWindow.bind(this));
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
  showCulturalMessage: function () {
    $('<div id="cultural-message"><div class="inner"><h3>CULTURAL SENSITIVITY MESSAGE – Please read</h3><p>First Peoples of Australia should be aware that the Museums Victoria Collections website contains images, voices or names of deceased persons. For some First Peoples communities, seeing images or hearing recordings of persons who have passed, may cause sadness or distress and, in some cases, offense.</p><h4>Language</h4><p>Certain records contain language or include depictions that are insensitive, disrespectful, offensive or racist. This material reflects the creator’s attitude or that of the period in which the item was written, recorded, collected or catalogued.</p><p>They are not the current views of Museums Victoria, do not reflect current understanding and are not appropriate today.</p><h4>Feedback</h4><p>Whilst every effort is made to ensure the most accurate information is presented, some content may contain errors. The level of documentation for collection items can and does vary, dependent on when or how the item was collected.</p><p>We encourage and welcome contact from First Peoples Communities, scholars and others to provide advice to correct and enhance information.</p></div></div>').insertBefore('footer');
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