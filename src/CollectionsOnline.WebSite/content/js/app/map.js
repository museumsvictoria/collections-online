var $ = require('jquery');

module.exports = {
  init: function () {
    this.cacheElements();
    this.buildMap();
  },
  cacheElements: function () {
    this.Model = window.latlongsModel;
  },
  buildMap: function () {
    if (this.Model !== undefined && this.Model.length != 0) {
      L.mapbox.accessToken = 'pk.eyJ1IjoibW1hc29uIiwiYSI6IlFtOXZGSW8ifQ.mIKIjLOaVEDuLDQOM-ddzA';
      var map = L.mapbox.map('map', 'mmason.c525da74').setView(this.Model[0], 8);
      var marker = L.marker(this.Model[0]).addTo(map);
    } else {
      $('#map').hide();
    }
  },
};