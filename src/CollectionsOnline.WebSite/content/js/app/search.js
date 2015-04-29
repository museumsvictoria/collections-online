var $ = require('jquery');
global.jQuery = require("jquery");
var queryString = require('query-string');

module.exports = {
  init: function () {
    this.cacheElements();
    this.bindEvents();
  },
  cacheElements: function () {
    this.$pageInput = $('.pagination input');
    this.totalPages = parseInt($('.pages .total').first().text().replace(/\D/g, ''));
    this.$searchFilter = $('#search-filter');
    this.$searchFilterButton = $('#search-filter .button-filter');
  },
  bindEvents: function () {
    this.$pageInput.on('change', this.gotoPage.bind(this));
    this.$searchFilterButton.on('click', this.toggleSearchFilter.bind(this));
  },
  gotoPage: function (e) {
    var page = $(e.target).val().replace(/\D/g, '');
    if (page) {

      if (page <= 0)
        page = 1;
      if (page > this.totalPages)
        page = this.totalPages;

      var query = queryString.parse(location.search);
      query.page = page;
      location.search = queryString.stringify(query);
    }
  },
  toggleSearchFilter: function() {
    this.$searchFilter.toggleClass('disabled');
  }
};