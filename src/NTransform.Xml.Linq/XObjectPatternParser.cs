// Copyright 2014 Max Toro Q.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Xml;
using System.Xml.Linq;

namespace NTransform.Xml.Linq {

   sealed class XObjectPatternParser : PatternParser<XObject> {

      public XObjectPatternParser(NamespaceBindings ns) 
          : base(ns) { }

      public override Func<XObject, bool> attribute() {
         return n => n.NodeType == XmlNodeType.Attribute;
      }

      protected override Func<XObject, bool> attribute(XmlQualifiedName name) {

         XAttribute attr;

         return n => attribute()(n)
            && (attr = (XAttribute)n).Name.LocalName == name.Name
            && attr.Name.NamespaceName == name.Namespace;
      }

      public override Func<XObject, bool> comment() {
         return n => n.NodeType == XmlNodeType.Comment;
      }

      public override Func<XObject, bool> document_node() {
         return n => n.NodeType == XmlNodeType.Document;
      }

      public override Func<XObject, bool> element() {
         return n => n.NodeType == XmlNodeType.Element;
      }

      protected override Func<XObject, bool> element(XmlQualifiedName name) {

         XElement el;

         return n => element()(n)
            && (el = (XElement)n).Name.LocalName == name.Name
            && el.Name.NamespaceName == name.Namespace;
      }

      public override Func<XObject, bool> processing_instruction() {
         return n => n.NodeType == XmlNodeType.ProcessingInstruction;
      }

      protected override Func<XObject, bool> processing_instruction(XmlQualifiedName name) {

         return n => processing_instruction()(n)
            && ((XProcessingInstruction)n).Target == name.Name;
      }

      public override Func<XObject, bool> text() {

         return n => {
            switch (n.NodeType) {
               case XmlNodeType.CDATA:
               case XmlNodeType.SignificantWhitespace:
               case XmlNodeType.Text:
               case XmlNodeType.Whitespace:
                  return true;

               default:
                  return false;
            }
         };
      }
   }
}
