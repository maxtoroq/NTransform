using System;
using NTransform.Xml;

namespace XmlReaderSamples {

   class CopyLinksTransform : XmlReaderTransform {

      public CopyLinksTransform() {

         xmlns["html"] = xmlns[null] = "http://www.w3.org/1999/xhtml";

         template[match.text()] = null;

         template[r => r.Depth == 1 && match.element()(r)] = () =>
            element("div")
               .write(() => apply_templates())
            .end();
         
         template["html:link"] = () => 
            copy_of(current());

         // When <body> is found stop processing
         template["html:body"] = null;
      }
   }
}
