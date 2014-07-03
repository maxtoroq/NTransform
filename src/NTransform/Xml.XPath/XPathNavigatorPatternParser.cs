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
using System.Xml.XPath;

namespace NTransform.Xml.XPath {

   sealed class XPathNavigatorPatternParser : PatternParser<XPathNavigator> {

      public XPathNavigatorPatternParser(NamespaceBindings ns) 
          : base(ns) { }

      public override Func<XPathNavigator, bool> attribute() {
         return n => n.NodeType == XPathNodeType.Attribute;
      }

      protected override Func<XPathNavigator, bool> attribute(XmlQualifiedName name) {

         return n => attribute()(n)
            && n.LocalName == name.Name
            && n.NamespaceURI == name.Namespace;
      }

      public override Func<XPathNavigator, bool> comment() {
         return n => n.NodeType == XPathNodeType.Comment;
      }

      public override Func<XPathNavigator, bool> document_node() {
         return n => n.NodeType == XPathNodeType.Root;
      }

      public override Func<XPathNavigator, bool> element() {
         return n => n.NodeType == XPathNodeType.Element;
      }

      protected override Func<XPathNavigator, bool> element(XmlQualifiedName name) {

         return n => element()(n)
            && n.LocalName == name.Name
            && n.NamespaceURI == name.Namespace;
      }

      public override Func<XPathNavigator, bool> processing_instruction() {
         return n => n.NodeType == XPathNodeType.ProcessingInstruction;
      }

      protected override Func<XPathNavigator, bool> processing_instruction(XmlQualifiedName name) {

         return n => processing_instruction()(n)
            && n.LocalName == name.Name;
      }

      public override Func<XPathNavigator, bool> text() {

         return r => {
            switch (r.NodeType) {
               case XPathNodeType.SignificantWhitespace:
               case XPathNodeType.Text:
               case XPathNodeType.Whitespace:
                  return true;

               default:
                  return false;
            }
         };
      }
   }
}
