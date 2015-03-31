var $ = require('jquery');

module.exports = {
  bindEvents: function () {
    $('#title .thumbnail').on('click', 'img', this.select.bind(this));
  },
  select: function(e) {
    if (window.imagesModel) {
      $('.thumbnail img.active').removeClass('active');
      $(e.target).addClass('active');
      
      var newImage = window.imagesModel[$(e.target).parent().index()];
      
      $('#media img').attr('src', newImage.Large.Uri);
      
      if (newImage.Caption)
        $('#media .caption-text').text(newImage.Caption);
      if (newImage.Creators.length > 0)
        $('#media .creators').text(newImage.Creators.join(', '));
      if (newImage.Sources.length > 0)
        $('#media .sources').text('Source: ' + newImage.Sources.join(', '));
      if (newImage.Credit)
        $('#media .credit').text('Credit: ' + newImage.Credit);
      if (newImage.Credit)
        $('#media .rights-statement').text('This image is: ' + newImage.Credit);
    }
  }
};