var $ = require('jquery');

module.exports = {
  bindEvents: function () {
    $('#title .thumbnail').on('click', 'img', this.select.bind(this));
  },
  select: function(e) {
    if (window.imagesModel) {
      $('.thumbnail img.active').removeClass('active');
      $(e.target).addClass('active');
      
      var index = $(e.target).parent().index();
      var newUri = window.imagesModel[index].Large.Uri;
      
      $('#media img').attr('src', newUri);
    }
  }
};