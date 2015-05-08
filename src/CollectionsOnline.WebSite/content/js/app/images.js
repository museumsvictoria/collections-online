var $ = require('jquery');

module.exports = {
  init: function () {
    this.cacheElements();
    this.bindEvents();
  },
  cacheElements: function () {
    this.Model = window.imagesModel;
    this.$thumbs = $('#title .thumbnail');
    this.$activeImage = $('#title .thumbnail img.active');
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
      
      if (newImage.Caption)
        this.$heroCaption.html(newImage.Caption);
      if (newImage.Creators.length > 0)
        this.$heroCreators.text(newImage.Creators.join(', '));
      if (newImage.Sources.length > 0)
        this.$heroSources.text('Source: ' + newImage.Sources.join(', '));
      if (newImage.Credit)
        this.$heroCredit.text('Credit: ' + newImage.Credit);
      if (newImage.Credit)
        this.$heroRights.text('This image is: ' + newImage.Credit);
    }
  }
};