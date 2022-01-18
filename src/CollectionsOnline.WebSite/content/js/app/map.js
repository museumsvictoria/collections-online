var $ = require('jquery');

module.exports = {
  init: function () {
    this.cacheElements();
    this.buildMap();
  },
  cacheElements: function () {
    this.Model = window.lnglatModel;
  },
  buildMap: function () {
      if (this.Model !== undefined && this.Model.length !== 0) {
          mapboxgl.accessToken = 'pk.eyJ1IjoibW1hc29uIiwiYSI6IlFtOXZGSW8ifQ.mIKIjLOaVEDuLDQOM-ddzA';
          
          var map = new mapboxgl.Map({
              container: 'map',
              style: 'mapbox://styles/mmason/ckyjlbw3kdpk514lokzdw28jv',
              center: this.Model,
              zoom: 8
          });

          // Create a default Marker and add it to the map.
          var marker = new mapboxgl.Marker()
              .setLngLat(this.Model)
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