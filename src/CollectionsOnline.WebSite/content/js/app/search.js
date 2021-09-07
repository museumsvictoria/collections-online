﻿var $ = require('jquery');
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
    
    // Search menu button
    this.$searchMenuButton = $('.button-search');
    
    // Add search term button
    this.$searchAddButton = $('.search-add');
    
    // Facets
    this.$facets = $('.facetgroup h4');

    this.$downloadPageResultsLink = $('.pagination .download .link');
  },
  bindEvents: function () {
    // Search pagination
    this.$pageInput.on('change', this.gotoPage.bind(this));
    
    // Search filter
    this.$searchFilterButton.on('click keydown', this.toggleSearchFilter.bind(this));
    
    // Search menu button
    this.$searchMenuButton.on('click keydown', this.toggleSearchButton.bind(this));
    
    // Add search term button
    this.$searchAddButton.on('click keydown', this.addSearchTerm.bind(this));
    
    // Facets
    this.$facets.on('click keydown', this.toggleFacets.bind(this));

    this.$downloadPageResultsLink.on('click', this.downloadPageResults.bind(this));
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
  toggleSearchFilter: function (e) {
    if(e.keyCode == 13 || e.type == 'click')
      this.$searchFilter.toggleClass('disabled');
  },
  toggleSearchButton: function (e) {
    if (e.keyCode == 13 || e.type == 'click') {
      if (!$('#search-bar').hasClass('search')) {
        $('#search-bar').toggle();
        $('#search-button-icon').toggleClass('icon-search-header');
        $('#search-button-icon').toggleClass('icon-search-close');
      }
    }
  },
  toggleFacets: function (e) {
    if (e.keyCode == 13 || e.type == 'click') {
      var facetGroup = $(e.target).parent().parent();

      facetGroup.toggleClass('collapsed');
    }
  },
  addSearchTerm: function (e) {
    if (e.keyCode == 13 || e.type == 'click') {
      var searchQuery = $('#search-bar input').val();
      if (searchQuery) {
        var querystring = queryString.parse(location.search);

        if (!querystring.hasOwnProperty('query'))
          querystring.query = searchQuery;
        else if (Array.isArray(querystring.query))
          querystring.query.push(searchQuery);
        else {
          querystring.query = [querystring.query, searchQuery];
        }

        // remove empty strings
        if (Array.isArray(querystring.query))
          querystring.query = querystring.query.filter(Boolean);

        location.search = queryString.stringify(querystring);
      }
    }
  },
  downloadPageResults: function (e) {
    if (typeof gtag === 'function') {
      gtag('event', 'Search results', {
        'event_category': 'Downloads',
        'event_label': $(e.currentTarget).attr('href')
      });
    }
  }
};