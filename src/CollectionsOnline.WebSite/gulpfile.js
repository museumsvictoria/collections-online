/// <binding AfterBuild='default' ProjectOpened='watch' />
'use strict';

const gulp = require('gulp');
const sass = require('gulp-sass');
sass.compiler = require('node-sass');
const source = require('vinyl-source-stream');
const buffer = require('vinyl-buffer');
const minify = require('gulp-minify');
const cleanCss = require('gulp-clean-css');
const browserify = require('browserify');
const del = require('del');
const CacheBuster = require('gulp-cachebust');
const cachebust = new CacheBuster({checksumLength: 16});

const filePaths = {
    css: {src: './content/scss/styles.scss', dest: './dist/css', devdest: './content/static'},
    js: {src: './content/js/app/app.js', dest: './dist/js', devdest: './content/static'},
    html: {src: './Views/*Layout.cshtml', dest: './dist/cshtml'},
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
        .pipe(minify({
            ext:{
                src:'-debug.js',
                min:'.js'
            }
        }))
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

const build = gulp.series(clean, gulp.parallel(css, js), html);
exports.build = build;
exports.watch = watch;