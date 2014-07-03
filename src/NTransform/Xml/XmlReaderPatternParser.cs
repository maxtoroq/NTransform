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

namespace NTransform.Xml {

   sealed class XmlReaderPatternParser : PatternParser<XmlReader> {

      public XmlReaderPatternParser(NamespaceBindings ns) 
          : base(ns) { }

      public override Func<XmlReader, bool> attribute() {
         return r => r.NodeType == XmlNodeType.Attribute;
      }

      protected override Func<XmlReader, bool> attribute(XmlQualifiedName name) {

         return r => attribute()(r)
            && r.LocalName == name.Name
            && r.NamespaceURI == name.Namespace;
      }

      public override Func<XmlReader, bool> comment() {
         return r => r.NodeType == XmlNodeType.Comment;
      }

      public override Func<XmlReader, bool> document_node() {
         return r => r.NodeType == XmlNodeType.Document;
      }

      public override Func<XmlReader, bool> element() {
         return r => r.NodeType == XmlNodeType.Element;
      }

      protected override Func<XmlReader, bool> element(XmlQualifiedName name) {

         return r => element()(r)
            && r.LocalName == name.Name
            && r.NamespaceURI == name.Namespace;
      }

      public override Func<XmlReader, bool> processing_instruction() {
         return r => r.NodeType == XmlNodeType.ProcessingInstruction;
      }

      protected override Func<XmlReader, bool> processing_instruction(XmlQualifiedName name) {

         return r => processing_instruction()(r)
            && r.LocalName == name.Name;
      }

      public override Func<XmlReader, bool> text() {

         return r => {
            switch (r.NodeType) {
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
