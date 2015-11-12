var $ = require('jquery');
var video = require('./video.js');
var verge = require('verge');
require('audioplayer');

module.exports = {
  init: function () {
    this.Model = window.mediaModel;

    if (this.Model !== undefined && this.Model.length != 0) {
      this.cacheElements();
      this.bindEvents();
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
    // init video if first media 
    if (this.Model[0].$type.indexOf('VideoMedia') > 0) {
      video.init();
    }
    // init audio if first media
    if (this.Model[0].$type.indexOf('AudioMedia') > 0) {
      $('audio').audioPlayer();
      this.$heroMedia.addClass('audio-loaded');
    }
    this.$thumbs.on('click keydown', 'img', this.select.bind(this));    
    this.$fullscreenButton.on('click', this.toggleFullscreen.bind(this));
    this.$previous.on('click keydown', { direction: "previous" }, this.moveTo.bind(this));
    this.$next.on('click keydown', { direction: "next" }, this.moveTo.bind(this));    

    $(document).on('keydown', this.handleKey.bind(this));
  },
  handleKey: function (e) {
    switch (e.which) {
      case 37: // left arrow
        this.$previous.triggerHandler('click');
        break;
      case 39: // right arrow
        this.$next.triggerHandler('click');
        break;
      case 70: // f
        this.toggleFullscreen();
      default: return;
    }
  },
  select: function (e) {
    if (e.keyCode == 13 || e.type == 'click') {
      this.$activeMedia.removeClass('active');
      this.$activeMedia = $(e.target).addClass('active');
      this.switchMedia($(e.target).parent().index());
    }
  },
  toggleFullscreen: function () {
    if (!this.$fullscreenButton.hasClass('disabled')) {
      var currentIndex = Math.max(this.$activeMedia.parent().index(), 0);
      var media = this.Model[currentIndex];
      var heroMediaImage = $('img', this.$heroMedia);

      var windowWidth = $(window).width();
      var windowHeight = verge.viewportH();

      // Default size
      var newHeroMediaImageSrc = media.Medium.Uri;

      // Scroll to top
      $('html, body').scrollTop(heroMediaImage.offset().top);

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
        
        // change aria state
        this.$fullscreenButton.attr('aria-pressed', 'false');
      } else {                       
        // Expand media holder
        this.$mediaHolder.css({
          maxWidth: windowWidth,
          height: windowHeight
        });
        
        // Allow hero media image to take up max height available
        heroMediaImage.css({
          maxHeight: windowHeight
        });
        
        // Dont load large media if its even smaller than medium otherwise preload then display large image
        if (!(media.Large.Height < parseInt(this.defaultMaxHeight, 10))) {
          // Enlarge medium image temporarily to provide smooth transition
          var heroMediaImageHeight = Math.round((windowWidth / media.Large.Width) * media.Large.Height);

          // Check to make sure we dont enlarge too much
          if (heroMediaImageHeight > media.Large.Height)
            heroMediaImageHeight = media.Large.Height;
          
          heroMediaImage.height(heroMediaImageHeight);
          newHeroMediaImageSrc = media.Large.Uri;
        }
        
        // change aria state
        this.$fullscreenButton.attr('aria-pressed', 'true');
      }
      
      this.preloadImages(newHeroMediaImageSrc).done(function () {
        heroMediaImage.attr('src', newHeroMediaImageSrc);
        heroMediaImage.css('height', '');
      });
      
      this.$mediaArea.toggleClass('expanded');
    }
  },
  moveTo: function (e) {
    if (e.keyCode == 13 || e.type == 'click') {
      var moveToIndex = this.$activeMedia.parent().index();

      if (e.data.direction == "previous")
        moveToIndex--;
      else if (e.data.direction == "next")
        moveToIndex++;

      if (moveToIndex >= 0 && moveToIndex < this.Model.length) {
        this.$activeMedia.removeClass('active');
        this.$activeMedia = $('img', this.$thumbs.get(moveToIndex)).addClass('active');

        this.switchMedia(moveToIndex);
      }
    }
  },
  switchMedia: function (index) {
    var $this = this;
    if (index == this.Model.length - 1) {
      this.$next.addClass('inactive');
      this.$next.attr('tabindex', -1);
    } else {
      this.$next.removeClass('inactive');
      this.$next.attr('tabindex', 0);
    }
    
    if (index == 0) {
      this.$previous.addClass('inactive');
      this.$previous.attr('tabindex', -1);
    } else {
      this.$previous.removeClass('inactive');
      this.$previous.attr('tabindex', 0);
    }

    var newMedia = this.Model[index];

    this.$heroMedia.removeClass('audio-loaded');

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
        newHeroImage.removeAttr('width');
        newHeroImage.removeAttr('height');
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
        newHeroVideo.removeAttr('width');
        newHeroVideo.removeAttr('height');
        $this.switchCaption(newMedia);
        video.init();
      });
    } else if (newMedia.$type.indexOf('AudioMedia') > 0) {
      // Handle Audio
      if (this.$mediaArea.hasClass('expanded')) {
        var heroMediaImage = $('img', this.$heroMedia);
        $('html, body').scrollTop(heroMediaImage.offset().top);
        this.$mediaHolder.css({
          maxWidth: this.defaultMaxWidth,
          height: ''
        });
        heroMediaImage.css({
          maxHeight: this.defaultMaxHeight,
          height: ''
        });
        this.$fullscreenButton.attr('aria-pressed', 'false');
      }

      var newHeroAudio = $('<audio preload="metadata" controls><source src="' + newMedia.File.Uri + '" type="audio/mpeg"/><audio/>');

      this.$fullscreenButton.addClass('disabled');

      $this.$heroMedia.html(newHeroAudio);
      this.$heroMedia.addClass('audio-loaded');
      $this.switchCaption(newMedia);

      $('audio').audioPlayer();
    }
  },
  switchCaption: function(media) {
    (media.Caption) ? this.$heroCaption.html(media.Caption) : this.$heroCaption.empty();
    (media.Creators.length > 0) ? this.$heroCreators.text(media.Creators.join(', ')) : this.$heroCreators.empty();
    (media.Sources.length > 0) ? this.$heroSources.text('Source: ' + media.Sources.join(', ')) : this.$heroSources.empty();
    (media.Credit) ? this.$heroCredit.text('Credit: ' + media.Credit) : this.$heroCredit.empty();

    var rightsLabel;
    if (media.$type.indexOf('ImageMedia') > 0)
      rightsLabel = 'This image is: ';
    else if (media.$type.indexOf('VideoMedia') > 0)
      rightsLabel = 'This video is: ';
    else if(media.$type.indexOf('AudioMedia') > 0)
      rightsLabel = 'This audio is: ';    
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