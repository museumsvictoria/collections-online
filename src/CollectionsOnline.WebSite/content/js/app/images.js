var $ = require('jquery');

module.exports = {
  init: function () {
    this.cacheElements();
    this.bindEvents();
  },
  cacheElements: function () {
    this.Model = window.imagesModel;
    this.$thumbs = $('#objectthumbs .thumbnail');
    this.$activeImage = $('#objectthumbs .thumbnail img.active');
    this.$heroImage = $('#media img');
    this.$heroCaption = $('#media .caption-text');
    this.$heroCreators = $('#media .creators');
    this.$heroSources = $('#media .sources');
    this.$heroCredit = $('#media .credit');
    this.$heroRights = $('#media .rights-statement');
  },
  bindEvents: function () {
    this.$thumbs.on('click', 'img', this.select.bind(this));
  },
  select: function(e) {
    if (this.Model !== undefined && this.Model.length != 0) {
      this.$activeImage.removeClass('active');
      this.$activeImage = $(e.target).addClass('active');
      
      var newImage = this.Model[$(e.target).parent().index()];
      
      this.$heroImage.attr('src', newImage.Large.Uri);
      
      (newImage.Caption) ? this.$heroCaption.html(newImage.Caption) : this.$heroCaption.empty();
      (newImage.Creators.length > 0) ? this.$heroCreators.text(newImage.Creators.join(', ')) : this.$heroCreators.empty();
      (newImage.Sources.length > 0) ? this.$heroSources.text('Source: ' + newImage.Sources.join(', ')) : this.$heroSources.empty();
      (newImage.Credit) ? this.$heroCredit.text('Credit: ' + newImage.Credit) : this.$heroCredit.empty();
      (newImage.RightsStatement) ? this.$heroRights.text('This image is: ' + newImage.RightsStatement) : this.$heroRights.empty();
    }
  }
};