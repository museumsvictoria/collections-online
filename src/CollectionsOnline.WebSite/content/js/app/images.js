var $ = require('jquery');
var objectFit = require('object-fit');

module.exports = {
  init: function () {
    this.cacheElements();
    this.bindEvents();

    objectFit.polyfill({
      selector: '#media img',
      fittype: 'cover'
    });
  },
  cacheElements: function () {
    this.Model = window.imagesModel;
    this.$thumbs = $('#objectthumbs .thumbnail');
    this.$activeImage = $('#objectthumbs .thumbnail img.active');
    this.$mediaArea = $('#media');
    this.$heroImage = $('#media img');
    this.$heroCaption = $('#media .caption-text');
    this.$heroCreators = $('#media .creators');
    this.$heroSources = $('#media .sources');
    this.$heroCredit = $('#media .credit');
    this.$heroRights = $('#media .rights-statement');

    this.$fullscreenButton = $('button.fullscreen');
    this.$imageHolder = $('.imageholder');
    this.defaultMaxHeight = this.$heroImage.css('max-height');
    this.defaultMaxWidth = this.$imageHolder.css('max-width');

    this.$previous = $('.previous');
    this.$next = $('.next');

    if (this.Model !== undefined && this.Model.length <= 1)
      this.$next.addClass('inactive');
  },
  bindEvents: function () {
    this.$thumbs.on('click', 'img', this.select.bind(this));
    this.$fullscreenButton.on('click', this.fullscreen.bind(this));
    this.$previous.on('click', { direction: "previous" }, this.moveTo.bind(this));
    this.$next.on('click', { direction: "next" }, this.moveTo.bind(this));
  },
  select: function(e) {
    this.$activeImage.removeClass('active');
    this.$activeImage = $(e.target).addClass('active');

    this.switchImage($(e.target).parent().index());
  },
  fullscreen: function () {
    if (this.Model !== undefined && this.Model.length != 0) {
      var $this = this;
      var image = this.Model[this.$activeImage.parent().index()];

      var fullscreenImageHeight = window.innerHeight - $('#media figcaption').height();

      //// force small viewport heights to zoom (may be stupid)
      //if (this.$heroImage.height() >= fullscreenImageHeight)
      //  fullscreenImageHeight = this.$heroImage.height() * 1.5;      


      $('html, body').animate({
        scrollTop: this.$heroImage.offset().top
      }, 200);

      if (!this.$mediaArea.hasClass('expanded')) {
        this.$heroImage.attr('src', image.Large.Uri);

        this.$heroImage.animate({
          'max-height': fullscreenImageHeight
        }, 200);

        this.$imageHolder.animate({
          maxWidth: window.innerWidth
        }, 200);

        this.$mediaArea.addClass('expanded');
      } else {
        this.$heroImage.animate({
          'max-height': this.defaultMaxHeight,
        }, 200, function() {
          $this.$heroImage.attr('src', image.Medium.Uri);
        });

        this.$imageHolder.animate({
          maxWidth: this.defaultMaxWidth
        }, 200);

        this.$mediaArea.removeClass('expanded');        
      }
    }
  },
  moveTo: function(e) {
    var moveToIndex = this.$activeImage.parent().index();
    
    if(e.data.direction == "previous")
      moveToIndex--;
    else if (e.data.direction == "next")
      moveToIndex++;

    if (moveToIndex >= 0 && moveToIndex < this.Model.length) {
      this.$activeImage.removeClass('active');
      this.$activeImage = $('img', this.$thumbs.get(moveToIndex)).addClass('active');

      this.switchImage(moveToIndex);
    }
  },
  switchImage: function (index) {
    if (this.Model !== undefined && this.Model.length != 0) {
      if (index == this.Model.length - 1) {
        this.$next.addClass('inactive');
      } else {
        this.$next.removeClass('inactive');
      }

      if (index == 0) {
        this.$previous.addClass('inactive');
      } else {
        this.$previous.removeClass('inactive');
      }

      var newImage = this.Model[index];

      if (this.$mediaArea.hasClass('expanded')) {
        this.$heroImage.attr('src', newImage.Large.Uri);
      } else {
        this.$heroImage.attr('src', newImage.Medium.Uri);
      }

      (newImage.Caption) ? this.$heroCaption.html(newImage.Caption) : this.$heroCaption.empty();
      (newImage.Creators.length > 0) ? this.$heroCreators.text(newImage.Creators.join(', ')) : this.$heroCreators.empty();
      (newImage.Sources.length > 0) ? this.$heroSources.text('Source: ' + newImage.Sources.join(', ')) : this.$heroSources.empty();
      (newImage.Credit) ? this.$heroCredit.text('Credit: ' + newImage.Credit) : this.$heroCredit.empty();
      (newImage.RightsStatement) ? this.$heroRights.text('This image is: ' + newImage.RightsStatement) : this.$heroRights.empty();
    }
  }
};