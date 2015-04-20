var $ = require('jquery');
global.jQuery = require("jquery");
var queryString = require('query-string');
var autoCompleter = require('jquery-autocompleter');

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

    $('#location-term').autocompleter({
      source: '/search/term/locality',
      empty: false,
      limit: 15,
      cache: false,
      highlightMatches: true,
      callback: function (value) {
        location.search = queryString.stringify({ locality: value });
      }
    });
    $('#keyword-term').autocompleter({
      source: '/search/term/keyword',
      empty: false,
      limit: 15,
      cache: false,
      highlightMatches: true,
      callback: function (value) {
        location.search = queryString.stringify({ keyword: value });
      }
    });
    $('#name-term').autocompleter({
      source: '/search/term/name',
      empty: false,
      limit: 15,
      cache: false,
      highlightMatches: true,
      callback: function (value) {
        location.search = queryString.stringify({ name: value });
      }
    });
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
  },
  fixedEncodeURIComponent: function(str) {
    return encodeURIComponent(str).replace(/[!'()*]/g, function(c) {
      return '%' + c.charCodeAt(0).toString(16);
    });
  }  
};