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
      this.reuseCheck();
    }
  },
  cacheElements: function () {
    this.$thumbs = $('#objectthumbs .thumbnail');
    this.$activeMedia = $('#objectthumbs .thumbnail img.active');
    
    this.$mediaArea = $('#media');
    this.$mediaHolder = $('.media-holder', this.$mediaArea);
    this.$heroMedia = $('.hero-media', this.$mediaArea);
    
    this.$caption = $('.caption-text');
    this.$creators = $('.creators');
    this.$sources = $('.sources');
    this.$sourcesQualifier = $('.sources-qualifier');
    this.$credit = $('.credit');
    this.$creditQualifier = $('.credit-qualifier');
    this.$rightsStatement = $('.rights-statement');
    this.$rightsStatementQualifier = $('.rights-statement-qualifier');

    this.$fullscreenButton = $('button.fullscreen');
    this.$reuseButton = $('button.reuse');
    
    this.defaultMaxHeight = $('img', this.$heroMedia).css('max-height');
    this.defaultMaxWidth = this.$mediaHolder.css('max-width');

    this.$previous = $('.previous');
    this.$next = $('.next');
    
    if (this.Model !== undefined && this.Model.length <= 1)
      this.$next.addClass('inactive');

    this.$reuseArea = $('#reuse');
    this.$reuseForm = $('form.share', this.$reuseArea);
    this.$reuseLinks = $('.download .link', this.$reuseArea);
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
    this.$reuseButton.on('click', this.toggleReuse.bind(this));
    this.$previous.on('click keydown', { direction: "previous" }, this.moveTo.bind(this));
    this.$next.on('click keydown', { direction: "next" }, this.moveTo.bind(this));
    this.$reuseForm.on('submit', this.submitReuseForm.bind(this));
    this.$reuseLinks.on('click', this.downloadMedia.bind(this));

    $(document).on('keydown', this.handleKey.bind(this));
  },
  reuseCheck: function name() {
    if ($.storageAvailable('localStorage') && localStorage.getItem('sharedMediaId' + $('#mediaid', this.$reuseForm).val())) {
      $('.input-group', this.$reuseForm).addClass('disabled');
      $('.form-response', this.$reuseForm).html('<h4>Thank you for your previous submission!</h4>').removeClass('disabled');
    }
  },
  handleKey: function (e) {
    if ($('input,textarea').is(':focus'))
      return;

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
  toggleReuse: function(e) {
    if (!$('#reuse').is(':visible')) {
      $('#reuse').show();
      $('html, body').scrollTop($('#reuse').offset().top + $('#reuse').outerHeight() - verge.viewportH());
    } else {
      $('#reuse').hide();
    }
  },
  submitReuseForm: function (e) {
    e.preventDefault();

    var data = this.$reuseForm.serializeArray();
    for (var i = 0; i < data.length; i++) {
      if (data[i].name == 'usage' && data[i].value == 'not-selected')
        return;
    }

    $.ajax({
      url: '/mediareuses',
      method: 'POST',
      data: data
    }).done(function() {
      $('.input-group', this.$reuseForm).addClass('disabled');
      $('.input-group select', this.$reuseForm).val('not-selected');
      $('.input-group textarea', this.$reuseForm).val('');
      $('.form-response', this.$reuseForm).html('<h4>Thank you for your submission!</h4>').removeClass('disabled');

      if ($.storageAvailable('localStorage')) {
        localStorage.setItem('sharedMediaId' + $('#mediaid', this.$reuseForm).val(), true);
      }

    }).fail(function () {
      $('.input-group', this.$reuseForm).addClass('disabled');
      $('.form-response', this.$reuseForm).html('<h4>Something went wrong receiving your submission, please try again later.</h4>').removeClass('disabled');
    });
  },
  downloadMedia: function(e) {
    if (window.ga.loaded) {
      ga('send', {
        hitType: 'event',
        eventCategory: 'Downloads',
        eventAction: 'Image',
        eventLabel: $(e.currentTarget).attr('href')
      });
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
      this.$reuseButton.removeClass('disabled');

      this.preloadImages(newHeroImage.attr('src')).done(function () {
        $this.$heroMedia.html(newHeroImage);
        newHeroImage.removeAttr('width');
        newHeroImage.removeAttr('height');
        $this.switchCaption(newMedia);
        $this.switchReuse(newMedia);
      });
    } else if (newMedia.$type.indexOf('VideoMedia') > 0) {
      // Handle Videos      
      if (this.$mediaArea.hasClass('expanded')) {
        this.toggleFullscreen();
      }

      var newHeroVideo = $('<img/>', { 'alt': newMedia.Caption, 'src': newMedia.Medium.Uri, 'class': 'video' });
      
      this.$fullscreenButton.addClass('disabled');
      this.$reuseButton.addClass('disabled');
      if (this.$reuseArea.is(':visible'))
        this.toggleReuse();
      
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
        this.$mediaArea.toggleClass('expanded');
      }

      var newHeroAudio = $('<audio preload="metadata" controls><source src="' + newMedia.File.Uri + '" type="audio/mpeg"/><audio/>');

      this.$fullscreenButton.addClass('disabled');
      this.$reuseButton.addClass('disabled');
      if (this.$reuseArea.is(':visible'))
        this.toggleReuse();

      $this.$heroMedia.html(newHeroAudio);
      this.$heroMedia.addClass('audio-loaded');
      $this.switchCaption(newMedia);

      $('audio').audioPlayer();
    }
  },
  switchCaption: function(media) {
    (media.Caption) ? this.$caption.html(media.Caption) : this.$caption.empty();

    (media.Creators.length > 0) ? this.$creators.html(media.Creators.join(', ')) : this.$creators.empty();

    if (media.Sources.length > 0) {
      this.$sources.html(media.Sources.join(', '));
      this.$sourcesQualifier.html('Source:');
    } else {
      this.$sources.empty();
      this.$sourcesQualifier.empty();
    }

    if (media.Credit) {
      this.$credit.html(media.Credit);
      this.$creditQualifier.html('Credit:');
    } else {
      this.$credit.empty();
      this.$creditQualifier.empty();
    }

    if (media.RightsStatement) {
      this.$rightsStatement.html(media.RightsStatement);
      if (media.$type.indexOf('ImageMedia') > 0)
        this.$rightsStatementQualifier.html('This image is:');
      else if (media.$type.indexOf('VideoMedia') > 0)
        this.$rightsStatementQualifier.html('This video is:');
      else if (media.$type.indexOf('AudioMedia') > 0)
        this.$rightsStatementQualifier.html('This audio is:');
    } else {
      this.$rightsStatement.empty();
      this.$rightsStatementQualifier.empty();
    }
  },
  switchReuse: function (media) {
    var documentId = $('#documentid', this.$reuseForm).val();

    $('.non-commercial', this.$reuseArea).toggleClass('disabled', media.Licence != 'CC BY-NC');

    if (media.PermissionRequired) {
      $('.permission', this.$reuseArea).html('No <span class="icon"><span class="icon-close2" aria-hidden="true"></span><span class="icon-label-hidden">Cross</span></span>');
      $('.attribution', this.$reuseArea).addClass('disabled');
      $('.attribution h4', this.$reuseArea).removeClass('disabled');
      $('.download', this.$reuseArea).html('<a class="request" href="http://museumvictoria.com.au/discoverycentre/ask-us-a-question/image-requests/"><span class="title">Request image</span></a>');
      $('.share', this.$reuseArea).addClass('disabled');
      $('.statement', this.$reuseArea).html('Museum Victoria does not own the copyright in all the material on this website. In some cases copyright belongs to third parties and has been published here under a licence agreement: this does not authorise you to copy that material. You may be required to obtain permission from the copyright owner.<br/><br/>Some unpublished material may require permission for reuse even if it is very old. Orphan works, where the copyright owner is unknown, also require permission for reuse. Indigenous works may have additional legal and cultural issues. You may be required to seek cultural clearances from Aboriginal and Torres Strait Islander communities, families, individuals or organisations before you reproduce Aboriginal and Torres Strait Islander material.');
    } else {
      var permissionHtml = [
        'Yes',
        '<span class="icon">',
        '  <span class="icon-tick" aria-hidden="true">',
        '  <span class="icon-label-hidden">Tick</span>'
      ];
      if (media.Licence == 'CC BY-NC') {
        permissionHtml.push(
          '<br/>',
          '<span>* For non-commercial uses only. If you wish to use this image for a commercial purpose, please contact us.</span>'
        );
      }
      permissionHtml.push(
        '</span>'
      );
      $('.permission', this.$reuseArea).html(permissionHtml.join('\n'));

      $('.attribution', this.$reuseArea).removeClass('disabled');      

      if (!(media.Creators.length > 0) && !(media.Sources.length > 0) && !media.Credit && !media.RightsStatement)
        $('.attribution h4', this.$reuseArea).addClass('disabled');

      var downloadImagesHtml = ['<h4>Download images</h4>'];
      if (media.Medium) {
        downloadImagesHtml.push(
          '<a class="link" href="/' + documentId + '/media/' + media.Irn + '/small" rel="nofollow">',
          '  <span class="title">Small</span>',
          '  <span class="sub-title">(' + media.Medium.Width + ' x ' + media.Medium.Height + ', ' + media.Medium.SizeShortened + ')</span>',
          '</a><br/>');
      }
      if (media.Large && (media.Medium.Height < media.Large.Height && media.Medium.Width < media.Large.Width)) {
        downloadImagesHtml.push(
          '<a class="link" href="/' + documentId + '/media/' + media.Irn + '/medium" rel="nofollow">',
          '  <span class="title">Medium</span>',
          '  <span class="sub-title">(' + media.Large.Width + ' x ' + media.Large.Height + ', ' + media.Large.SizeShortened + ')</span>',
          '</a><br/>');
      }
      if (media.Original && (media.Large.Height < media.Original.Height && media.Large.Width < media.Original.Width)) {
        downloadImagesHtml.push(
          '<a class="link" href="/' + documentId + '/media/' + media.Irn + '/large" rel="nofollow">',
          '  <span class="title">Large</span>',
          '  <span class="sub-title">(' + media.Original.Width + ' x ' + media.Original.Height + ', ' + media.Original.SizeShortened + ')</span>',
          '</a><br/>');
      }
      $('.download', this.$reuseArea).html(downloadImagesHtml.join('\n'));

      $('.share', this.$reuseArea).removeClass('disabled');
      $('.input-group', this.$reuseForm).removeClass('disabled');
      $('.form-response', this.$reuseForm).addClass('disabled');

      $('#mediaid', this.$reuseForm).val(media.Irn);

      $('.statement', this.$reuseArea).html('Museum Victoria supports and encourages public access to our collection by offering image downloads for reuse.<br/><br/>Images marked as Public Domain have, to the best of Museum Victoria’s knowledge, no copyright or intellectual property rights that would restrict their free download and reuse. Images marked with a Creative Commons (CC) license may be downloaded and reused in accordance with the conditions of the relevant <a href="http://creativecommons.org.au/learn/licences/">CC license</a>. Please acknowledge Museum Victoria and cite the URL for the image so that others can also find it.');
    }
    
    this.reuseCheck();
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
  }
};