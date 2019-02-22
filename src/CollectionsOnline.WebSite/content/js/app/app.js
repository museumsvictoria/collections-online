﻿var $ = require('jquery');
var media = require('./media');
var search = require('./search');
var map = require('./map');

var App = {
  init: function() {
    media.init();
    search.init();
    map.init();

    this.bindEvents();
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