var $ = require('jquery');
var images = require('./images');

var App = {
  init: function() {
    this.bindEvents();
  },
  bindEvents: function () {
    images.bindEvents();
  }
};

$(document).ready(function () {
  App.init();
});