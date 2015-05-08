var $ = require('jquery');
var images = require('./images');
var search = require('./search');
var map = require('./map');

var App = {
  init: function() {
    images.init();
    search.init();
    map.init();
  }
};

$(document).ready(function () {
  App.init();
});