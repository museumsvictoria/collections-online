/// <binding AfterBuild='default' ProjectOpened='watch' />
/// <vs AfterBuild='build-html' SolutionOpened='watch' />
'use strict';

var gulp = require('gulp');
var sass = require('gulp-sass');
sass.compiler = require('node-sass');
var source = require('vinyl-source-stream');
var buffer = require('vinyl-buffer');
var uglify = require('gulp-uglify');
var cleanCss = require('gulp-clean-css');
var browserify = require('browserify');
var del = require('del');
var CacheBuster = require('gulp-cachebust');
var cachebust = new CacheBuster({ checksumLength: 16 });

var filePaths = {
    css: { src: './content/scss/styles.scss', dest: './dist/css', devdest: './content/static' },
    js: { src: './content/js/app/app.js', dest: './dist/js', devdest: './content/static' },
    html: { src: './Views/*Layout.cshtml', dest: './dist/cshtml' },
};

function css() {
    return gulp.src(filePaths.css.src)
        .pipe(sass())
        .pipe(cleanCss())
        .pipe(gulp.dest(filePaths.css.devdest))
        .pipe(cachebust.resources())
        .pipe(gulp.dest(filePaths.css.dest));
}

function js() {
    return browserify(filePaths.js.src)
        .bundle()
        .pipe(source('bundle.js'))
        .pipe(gulp.dest(filePaths.js.devdest))
        .pipe(buffer())
        .pipe(uglify())
        .pipe(cachebust.resources())
        .pipe(gulp.dest(filePaths.js.dest));
}

function html() {
    return gulp.src(filePaths.html.src)
        .pipe(cachebust.references())
        .pipe(gulp.dest(filePaths.html.dest));
}

function clean() {
    return del(['./dist/css', './dist/js']);
}

function watch() {
    gulp.watch('./content/scss/**/*.scss', css);
    gulp.watch('./content/js/app/*.js', js);
}

var build = gulp.series(clean, gulp.parallel(css, js), html);
exports.build = build;
exports.watch = watch;
exports.default = build;