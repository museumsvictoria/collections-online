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
          // Mapbox expects long/lat instead of lat/long
          var longLat = [this.Model[0][1], this.Model[0][0]];
          mapboxgl.accessToken = 'pk.eyJ1IjoibW1hc29uIiwiYSI6IlFtOXZGSW8ifQ.mIKIjLOaVEDuLDQOM-ddzA';

          var map = new mapboxgl.Map({
              container: 'map',
              style: 'mapbox://styles/mmason/ck9tfnhc90fv21inxraee3u6d',
              center: longLat,
              zoom: 8
          });

          var marker = new mapboxgl.Marker()
              .setLngLat(longLat)
              .addTo(map);

          var nav = new mapboxgl.NavigationControl({
              showCompass: false
          });

          map.addControl(nav, 'top-left');
      } else {
      $('#map').hide();
    }
  },
};