var $ = require('jquery');
var images = require('./images');
var search = require('./search');

var App = {
  init: function() {
    images.init();
    search.init();
  }
};

$(document).ready(function () {
  App.init();
});