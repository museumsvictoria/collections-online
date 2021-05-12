using System;
using System.IO;
using CollectionsOnline.Tests.Resources;

namespace CollectionsOnline.Tests
{
    public class FileBasedTest : IDisposable
    {
        public FileBasedTest(bool deleteAtDispose = true)
        {
            DeleteAtDispose = deleteAtDispose;
            Directory.CreateDirectory(Files.OutputFolder);
        }

        private bool DeleteAtDispose { get; }

        public void Dispose()
        {
            if(DeleteAtDispose)
                Directory.Delete(Files.OutputFolder, true);
        }
    }
}