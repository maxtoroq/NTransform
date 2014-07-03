using System;
using NTransform.Xml;

namespace XmlReaderSamples {

   class GetCanonicalLinkTransform : XmlReaderTransform {

      public GetCanonicalLinkTransform() {

         xmlns["html"] = "http://www.w3.org/1999/xhtml";

         template[match.text()] = null;

         template[r => match.element("html:link")(r) && r.attr("rel") == "canonical"] = () => 
            text(current().attr("href"));
         
         // When <body> is found stop processing
         template["html:body"] = null;
      }
   }
}
