var $ = require('jquery');
//var objectFit = require('object-fit');
var video = require('./video.js');

module.exports = {
  init: function () {
    // Dont do anything if we have no media model
    this.Model = window.mediaModel;
    if (this.Model !== undefined && this.Model.length != 0) {
      this.cacheElements();
      this.bindEvents();

      video.init();

      //objectFit.polyfill({
      //  selector: '#media img',
      //  fittype: 'cover'
      //});
    }
  },
  cacheElements: function () {       
    this.$thumbs = $('#objectthumbs .thumbnail');
    this.$activeMedia = $('#objectthumbs .thumbnail img.active');
    
    this.$mediaArea = $('#media');
    this.$mediaHolder = $('.media-holder', this.$mediaArea);
    this.$heroMedia = $('.hero-media', this.$mediaArea);
    
    this.$heroCaption = $('.caption-text', this.$mediaArea);
    this.$heroCreators = $('.creators', this.$mediaArea);
    this.$heroSources = $('.sources', this.$mediaArea);
    this.$heroCredit = $('.credit', this.$mediaArea);
    this.$heroRights = $('.rights-statement', this.$mediaArea);

    this.$fullscreenButton = $('button.fullscreen');
    
    this.defaultMaxHeight = $('img', this.$heroMedia).css('max-height');
    this.defaultMaxWidth = this.$mediaHolder.css('max-width');

    this.$previous = $('.previous');
    this.$next = $('.next');

    if (this.Model !== undefined && this.Model.length <= 1)
      this.$next.addClass('inactive');
  },
  bindEvents: function () {
    this.$thumbs.on('click', 'img', this.select.bind(this));
    this.$fullscreenButton.on('click', this.toggleFullscreen.bind(this));
    this.$previous.on('click', { direction: "previous" }, this.moveTo.bind(this));
    this.$next.on('click', { direction: "next" }, this.moveTo.bind(this));
  },
  select: function(e) {
    this.$activeMedia.removeClass('active');
    this.$activeMedia = $(e.target).addClass('active');
    this.switchMedia($(e.target).parent().index());
  },
  toggleFullscreen: function () {
    if (!this.$fullscreenButton.hasClass('disabled')) {
      var media = this.Model[this.$activeMedia.parent().index()];
      var heroMediaImage = $('img', this.$heroMedia);
      var windowWidth = $(window).width();
      var windowHeight = $(window).height();
      var fullscreenImageHeight = windowHeight - $('#media figcaption').height();

      // Default size
      var newHeroMediaImageSrc = media.Medium.Uri;

      if (this.$mediaArea.hasClass('expanded')) {
        // Contract media holder
        this.$mediaHolder.css({
          maxWidth: this.defaultMaxWidth,
          height: ''
        });
        
        // Reset hero media image max height
        heroMediaImage.css({
          maxHeight: this.defaultMaxHeight,
          height: ''
        });
      } else {
        // Scroll to top
        $('html, body').scrollTop(heroMediaImage.offset().top);
        
        // Expand media holder
        this.$mediaHolder.css({
          maxWidth: windowWidth,
          height: fullscreenImageHeight
        });
        
        // Allow hero media image to take up max height available
        heroMediaImage.css({
          maxHeight: fullscreenImageHeight
        });
        
        // Dont load large media if its even smaller than medium otherwise preload then display large image
        if (!(media.Large.Height < parseInt(this.defaultMaxHeight, 10))) {
          // Enlarge medium image temporarily to provide smooth transition
          heroMediaImage.height(Math.round((windowWidth / media.Large.Width) * media.Large.Height));
          newHeroMediaImageSrc = media.Large.Uri;
        }
      }
      
      this.preloadImages(newHeroMediaImageSrc).done(function () {
        heroMediaImage.attr('src', newHeroMediaImageSrc);
        heroMediaImage.css('height', '');
      });
      
      this.$mediaArea.toggleClass('expanded');
    }
  },
  moveTo: function(e) {
    var moveToIndex = this.$activeMedia.parent().index();
    
    if(e.data.direction == "previous")
      moveToIndex--;
    else if (e.data.direction == "next")
      moveToIndex++;

    if (moveToIndex >= 0 && moveToIndex < this.Model.length) {
      this.$activeMedia.removeClass('active');
      this.$activeMedia = $('img', this.$thumbs.get(moveToIndex)).addClass('active');

      this.switchMedia(moveToIndex);
    }
  },
  switchMedia: function (index) {
    var $this = this;
    this.$next.toggleClass('inactive', index == this.Model.length - 1);
    this.$previous.toggleClass('inactive', index == 0);

    var newMedia = this.Model[index];

    if (newMedia.$type.indexOf('ImageMedia') > 0) {
      // Handle Images
      var newHeroImage = $('<img/>', { 'alt': newMedia.AlternativeText });
      newHeroImage.css({
        maxHeight: $('img', this.$heroMedia).css('max-height')
      });
      if (this.$mediaArea.hasClass('expanded') && !(newMedia.Large.Height < parseInt(this.defaultMaxHeight, 10))) {
        newHeroImage.attr('src', newMedia.Large.Uri);
      } else {
        newHeroImage.attr('src', newMedia.Medium.Uri);
      }
            
      this.$fullscreenButton.removeClass('disabled');

      this.preloadImages(newHeroImage.attr('src')).done(function () {
        $this.$heroMedia.html(newHeroImage);
        $this.switchCaption(newMedia);
      });
    } else if (newMedia.$type.indexOf('VideoMedia') > 0) {
      // Handle Videos      
      if (this.$mediaArea.hasClass('expanded')) {
        this.toggleFullscreen();        
      }

      var newHeroVideo = $('<img/>', { 'alt': newMedia.Caption, 'src': newMedia.Medium.Uri, 'class': 'video' });
      
      this.$fullscreenButton.addClass('disabled');
      
      this.preloadImages(newHeroVideo.attr('src')).done(function () {
        $this.$heroMedia.html(newHeroVideo);
        $this.switchCaption(newMedia);
        video.init();
      });
    }           
  },
  switchCaption: function(media) {
    (media.Caption) ? this.$heroCaption.html(media.Caption) : this.$heroCaption.empty();
    (media.Creators.length > 0) ? this.$heroCreators.text(media.Creators.join(', ')) : this.$heroCreators.empty();
    (media.Sources.length > 0) ? this.$heroSources.text('Source: ' + media.Sources.join(', ')) : this.$heroSources.empty();
    (media.Credit) ? this.$heroCredit.text('Credit: ' + media.Credit) : this.$heroCredit.empty();
    var rightsLabel = (media.$type.indexOf('ImageMedia') > 0) ? 'This image is: ' : 'This video is: ';
    (media.RightsStatement) ? this.$heroRights.text(rightsLabel + media.RightsStatement) : this.$heroRights.empty();
  },
  preloadImages: function(srcs) {
    var dfd = $.Deferred(),
      promises = [],
      img,
      l,
      p;

    if (!$.isArray(srcs)) {
      srcs = [srcs];
    }

    l = srcs.length;

    for (var i = 0; i < l; i++) {
      p = $.Deferred();
      img = $("<img />");

      img.load(p.resolve);
      img.error(p.resolve);

      promises.push(p);

      img.get(0).src = srcs[i];
    }

    $.whenArray(promises).done(dfd.resolve);
    return dfd.promise();
  },
};