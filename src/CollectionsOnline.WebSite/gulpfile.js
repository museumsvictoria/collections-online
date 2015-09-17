/// <vs AfterBuild='build-html' SolutionOpened='watch' />
// Steps to build
// 1. install node: http://nodejs.org/download/
// 2. install gulp globally: npm install -g gulp
// 3. install ruby http://rubyinstaller.org/downloads/
// 4. install sass: gem install sass
// 5. install modules from web project: npm install
// 6. use task runner explorer or execute gulpfile: gulp
'use strict';

var gulp = require('gulp');
var sass = require('gulp-ruby-sass');
var source = require('vinyl-source-stream');
var buffer = require('vinyl-buffer');
var uglify = require('gulp-uglify');
var minifyCss = require('gulp-minify-css');
var browserify = require('browserify');
var del = require('del');
var CacheBuster = require('gulp-cachebust');
var cachebust = new CacheBuster({ checksumLength: 16 });

var filePaths = {
  css: { src: './content/scss/styles.scss', dest: './dist/css', devdest: './content/static' },
  js: { src: './content/js/app/app.js', dest: './dist/js', devdest: './content/static' },
  html: { src: './Views/*Layout.cshtml', dest: './dist/cshtml' },
};

gulp.task('build-css', function () {  
  return gulp.src(filePaths.css.src)
    .pipe(sass({ noCache: true, sourcemap: false }))
    .pipe(minifyCss())
    .pipe(gulp.dest(filePaths.css.devdest))
    .pipe(cachebust.resources())
    .pipe(gulp.dest(filePaths.css.dest));
});

// Watch Task.
gulp.task('watch', function() {
  gulp.watch('./content/scss/**/*.scss', ['build-css']);
  gulp.watch('./content/js/app/*.js', ['build-js']);
});

gulp.task('build-js', function() {
  return browserify(filePaths.js.src)
    .bundle()
    .pipe(source('bundle.js'))
    .pipe(gulp.dest(filePaths.js.devdest))
    .pipe(buffer())
    .pipe(uglify())
    .pipe(cachebust.resources())
    .pipe(gulp.dest(filePaths.js.dest));
});

gulp.task('build-html', ['build-css', 'build-js'], function () {
  return gulp.src(filePaths.html.src)
      .pipe(cachebust.references())
      .pipe(gulp.dest(filePaths.html.dest));
});

gulp.task('clean', function () {
  return del(['./dist/css', './dist/js']);
});

gulp.task('default', ['clean', 'build-html']);