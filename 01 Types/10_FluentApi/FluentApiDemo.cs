using System;

namespace FluentDemo
{
    interface IGeneral
    {
        byte[] Data { get; protected set; }
    }

    /// <summary>
    /// Methoden, die nach Parse aufgerufen werden dürfen.
    /// </summary>
    interface IAfterParse : IGeneral
    {
        IAfterParse CleanupData();
        void WriteToFile(string filename);
    }

    /// <summary>
    /// Methoden, die nach Download aufgerufen werden dürfen.
    /// </summary>
    interface IAfterDownload : IGeneral
    {
        IAfterParse Parse();
    }

    static class DownloaderExtensions
    {
        /// <summary>
        /// Log darf immer aufgerufen werden. Wir können die Methode aber nicht einfach in IGeneral
        /// schreiben, denn dann wird IGeneral zurückgegeben und es können die spezifischen Methoden
        /// nicht mehr aufgerufen werden.
        /// Die Lösung ist eine generische Extension Methode.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static T Log<T>(this T instance) where T : IGeneral
        {
            var data = instance.Data; // Durch where kann ich auf Data zugreifen
            // DO logging
            Console.WriteLine("Log!");
            return instance;
        }
    }

    class Downloader : IAfterDownload, IAfterParse
    {
        private Downloader()
        {

        }

        byte[] IGeneral.Data { get => new byte[0]; set => throw new NotImplementedException(); }

        public static IAfterDownload DownloadUrl(string url)
        {
            Downloader downloader = new Downloader();
            Console.WriteLine("Downloader created");
            return downloader;
        }

        public IAfterParse CleanupData()
        {
            Console.WriteLine("Cleanup");
            return this;
        }
        public IAfterParse Parse()
        {
            Console.WriteLine("ParseUrl");
            return this;
        }
        public void WriteToFile(string filename)
        {
            Console.WriteLine("WriteToFile");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Downloader
                .DownloadUrl("http://xxx.de")
                .Log()                        // Log darf immer aufgerufen werden. --> Extension Methode
                .Parse()                      // Darf nur nach DownloadUrl aufgerufen werden.
                    .CleanupData()            // Darf nur nach Parse aufgerufen werden.
                    .Log()
                .WriteToFile("xxx.json");     // Darf nur nach Parse oder DownloadUrl aufgerufen werden.
        }
    }
}
