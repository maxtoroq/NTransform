using System;
using System.Linq;
using NTransform.Xml;

namespace XmlReaderSamples {
   
   class PostTitlesTransform : XmlReaderTransform {

      public PostTitlesTransform() {

         xmlns["html"] = xmlns[null] = "http://www.w3.org/1999/xhtml";

         template[match.text()] = null;

         template[r => r.Depth == 1 && match.element()(r)] = () => 
            element("ul")
               .write(() => apply_templates())
            .end();

         template[r => match.element("html:h3")(r) && hasClass("entry-title")] = () => 
            element("li")
               .write(() => apply_templates(mode: "entry-title"))
            .end();

         template[match.text(), mode: "entry-title"] = () =>
            text(@string().Trim());
      }

      bool hasClass(string @class) {

         return (current().attr("class") ?? "")
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Contains(@class, StringComparer.OrdinalIgnoreCase);
      }
   }
}
