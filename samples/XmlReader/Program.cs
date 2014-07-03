using System;
using System.IO;
using System.Net;
using System.Xml;
using NTransform.Xml;
using Sgml;

namespace XmlReaderSamples {
   
   class Program {

      static void Main() {

         //Samples<GetCanonicalLinkTransform>.RunHtml(new Uri("http://maxtoroq.blogspot.in/"));
         //Samples<CopyLinksTransform>.RunHtml(new Uri("http://maxtoroq.blogspot.com/"));
         //Samples<PostTitlesTransform>.RunHtml(new Uri("http://maxtoroq.blogspot.com/"));
         Samples<RssToAtomTransform>.RunXml(new Uri("http://feeds.dzone.com/dzone/frontpage?format=xml"));

         Console.WriteLine();
         Console.WriteLine("Press key to finish...");
         Console.ReadKey();
      }

      class Samples<T> where T : XmlReaderTransform {

         public static void RunXml(Uri uri) {

            using (var stream = new WebClient().OpenRead(uri)) {
               using (var reader = XmlReader.Create(stream)) {
                  Run(reader);
               }
            }
         }

         public static void RunXmlString(string xml) {

            using (var reader = XmlReader.Create(new StringReader(xml))) {
               Run(reader);
            }
         }

         public static void RunHtml(Uri uri) {

            using (var stream = new WebClient().OpenRead(uri)) {
               using (var textReader = new StreamReader(stream)) {
                  using (var htmlReader = new SgmlReader()) {

                     htmlReader.CaseFolding = CaseFolding.ToLower;
                     htmlReader.InputStream = textReader;

                     Run(htmlReader);
                  }
               }
            }
         }

         public static void Run(XmlReader reader) {

            var outSettings = new XmlWriterSettings { 
               ConformanceLevel = ConformanceLevel.Fragment, 
               Indent = true 
            };

            using (var writer = XmlWriter.Create(Console.Out, outSettings)) {

               T transform = Activator.CreateInstance<T>();
               transform.Out = writer;
               transform.apply_templates(reader);
            }
         }
      }
   }
}
