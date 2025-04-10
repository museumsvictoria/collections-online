﻿using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Models;
using Google.Apis.YouTube.v3;
using ImageMagick;
using Serilog;

namespace CollectionsOnline.Import.Factories
{
    public class VideoMediaFactory : IVideoMediaFactory
    {
        private readonly YouTubeService _youTubeService;

        public VideoMediaFactory(
            YouTubeService youTubeService)
        {
            _youTubeService = youTubeService;            
        }

        public bool Make(ref VideoMedia videoMedia)
        {
            var stopwatch = Stopwatch.StartNew();

            var uri = new Uri(videoMedia.Uri);
            videoMedia.VideoId = !string.IsNullOrWhiteSpace(uri.Segments.LastOrDefault()) ? uri.Segments.Last() : null;

            if (!string.IsNullOrWhiteSpace(videoMedia.VideoId))
            {
                try
                {
                    var request = _youTubeService.Videos.List("snippet");
                    request.Id = videoMedia.VideoId;

                    var listResponse = request.Execute();

                    var youtubeVideo = listResponse.Items.FirstOrDefault();

                    if (youtubeVideo != null)
                    {
                        // try and find highest resolution
                        var thumbnail = youtubeVideo.Snippet.Thumbnails.Maxres ?? youtubeVideo.Snippet.Thumbnails.High;
                        
                        using (var webClient = new WebClient())
                        using (var memoryStream = new MemoryStream(webClient.DownloadData(thumbnail.Url)))
                        using (var thumbnailImage = new MagickImage(memoryStream))
                        {
                            var smallImage = thumbnailImage.Clone();

                            // Create thumbnail
                            var destPath = PathFactory.MakeDestPath(videoMedia.Irn, ".jpg", FileDerivativeType.Thumbnail);

                            thumbnailImage.Resize(new MagickGeometry(250) { FillArea = true });
                            thumbnailImage.Crop(250, 250, Gravity.Center);
                            thumbnailImage.Quality = 70;
                            thumbnailImage.Write(destPath);

                            stopwatch.Stop();
                            Log.Logger.Debug("Loaded video preview {Irn} in {ElapsedMilliseconds} ms", videoMedia.Irn, stopwatch.ElapsedMilliseconds);
                            stopwatch.Restart();

                            videoMedia.Thumbnail = new ImageMediaFile
                            {
                                Uri = PathFactory.BuildUriPath(videoMedia.Irn, ".jpg", FileDerivativeType.Thumbnail),
                                Size = new FileInfo(destPath).Length,
                                Width = (int)thumbnailImage.Width,
                                Height = (int)thumbnailImage.Height
                            };

                            // Create small preview placeholder
                            destPath = PathFactory.MakeDestPath(videoMedia.Irn, ".jpg", FileDerivativeType.Small);

                            smallImage.Resize(new MagickGeometry(0, 500));
                            smallImage.Quality = 80;
                            smallImage.Write(destPath);

                            videoMedia.Small = new ImageMediaFile
                            {
                                Uri = PathFactory.BuildUriPath(videoMedia.Irn, ".jpg", FileDerivativeType.Small),
                                Size = new FileInfo(destPath).Length,
                                Width = (int)smallImage.Width,
                                Height = (int)smallImage.Height
                            };
                        }

                        stopwatch.Stop();
                        Log.Logger.Debug("Completed video preview {Irn} creation in {ElapsedMilliseconds} ms", videoMedia.Irn, stopwatch.ElapsedMilliseconds);

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Log.Logger.Fatal(ex, "Unexpected error occured creating video preview {Irn}", videoMedia.Irn);
                    throw;
                }
            }

            return false;
        }
    }
}