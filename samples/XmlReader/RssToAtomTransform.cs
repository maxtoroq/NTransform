using System;
using NTransform.Xml;

namespace XmlReaderSamples {
   
   class RssToAtomTransform : XmlReaderTransform {

      public RssToAtomTransform() {

         xmlns["atom"] = xmlns[null] = "http://www.w3.org/2005/Atom";

         template[match.text(), mode: "#all"] = null;

         template[r => r.Depth == 0 && match.element("rss")(r)] = () =>
            element("feed")
               .write(() => apply_templates())
            .end();

         template["image"] = null;

         template["atom:link"] = () =>
            copy_of(current());

         template["title", mode: "#all"] = () =>
            elem("title", @string());

         template["item"] = () =>
            element("entry")
               .write(() => apply_templates(mode: "item"))
            .end();

         template["link", mode: "item"] = () =>
            element("link")
               .attr("href", @string())
            .end();

         template["guid", mode: "item"] = () =>
            elem("id", @string());

         template["pubDate", mode: "item"] = () =>
            elem("published", DateTime.Parse(@string()).ToString("u"));
      }
   }
}
