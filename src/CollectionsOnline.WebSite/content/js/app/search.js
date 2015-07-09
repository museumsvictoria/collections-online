var $ = require('jquery');
global.jQuery = require("jquery");
var queryString = require('query-string');

module.exports = {
  init: function () {
    this.cacheElements();
    this.bindEvents();
  },
  cacheElements: function () {
    // Search pagination
    this.$pageInput = $('.pagination input');
    this.totalPages = parseInt($('.pages .total').first().text().replace(/\D/g, ''));
    
    // Search filter
    this.$searchFilter = $('#search-filter');
    this.$searchFilterButton = $('#search-filter .button-filter');
    
    // Search button
    this.$searchButton = $('.button-search');
    
    // Facets
    this.$facets = $('.facetgroup h4');     
  },
  bindEvents: function () {
    // Search pagination
    this.$pageInput.on('change', this.gotoPage.bind(this));
    
    // Search filter
    this.$searchFilterButton.on('click', this.toggleSearchFilter.bind(this));
    
    // Search button
    this.$searchButton.on('click', this.toggleSearchButton.bind(this));
    
    // Facets
    this.$facets.on('click', this.toggleFacets.bind(this));
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
  toggleSearchButton: function () {

    if (!$('#search-bar').hasClass('search')) {
      $('#search-bar').toggle();
      $('#search-button-icon').toggleClass('icon-search-header');
      $('#search-button-icon').toggleClass('icon-search-close');
    }
  },
  toggleFacets: function (e) {
    var facetGroup = $(e.target).parent().parent();

    facetGroup.toggleClass('collapsed');
  }
};