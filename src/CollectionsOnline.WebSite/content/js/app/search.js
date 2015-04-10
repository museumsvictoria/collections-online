var $ = require('jquery');
var queryString = require('query-string');

module.exports = {
  init: function () {
    this.cacheElements();
    this.bindEvents();
  },
  cacheElements: function () {
    this.$pageInput = $('.pagination input');

    this.totalPages = parseInt($('.pages .total').first().text());
    this.query = queryString.parse(location.search);
  },
  bindEvents: function () {
    this.$pageInput.on('change', this.gotoPage.bind(this));
  },
  gotoPage: function (e) {
    var page = $(e.target).val().replace(/\D/g, '');

    if (page && (page >= 1 && page <= this.totalPages)) {
      this.query.page = page;
      location.search = queryString.stringify(this.query);
    }
  }
};