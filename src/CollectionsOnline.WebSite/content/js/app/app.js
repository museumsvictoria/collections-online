var $ = require('jquery');
var media = require('./media');
var search = require('./search');
var map = require('./map');

var App = {
  init: function() {
    media.init();
    search.init();
    map.init();
  }
};

$(document).ready(function () {
  App.init();
});

$.whenArray = function (array) {
  return $.when.apply(this, array);
};