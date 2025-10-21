var $ = require('jquery');

module.exports = {
  init: function () {
    this.cacheElements();
    this.buildMap();
  },
  cacheElements: function () {
    this.Model = window.latlngModel;
  },
  buildMap: function () {
      if (this.Model !== undefined && this.Model.length !== 0) {
          var map = L.map('map').setView(this.Model, 9);
          map.attributionControl.setPrefix(false);
          
          // Add OpenStreetMap tile layer
          L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
              maxZoom: 19,
              attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
          }).addTo(map);
          
          // Add a marker at the specified coordinates
          L.marker(this.Model).addTo(map);
      } else {
      $('#map').hide();
    }
  },
};