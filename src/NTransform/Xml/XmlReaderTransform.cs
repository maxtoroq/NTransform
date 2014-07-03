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
using System.ComponentModel;
using System.Xml;

namespace NTransform.Xml {
   
   public class XmlReaderTransform : Transform<XmlReader, NodeWriter>  {

      bool readNextSibling;
      NodeWriter nodeWriter;

      public XmlWriter Out { get; set; }

      [EditorBrowsable(EditorBrowsableState.Never)]
      protected override PatternParser<XmlReader> CreatePatternParser(NamespaceBindings xmlns) {
         return new XmlReaderPatternParser(xmlns);
      }

      void EnsureInitialized() {

         if (this.Out == null) {
            throw new InvalidOperationException("Must set Out property first.");
         }

         if (this.nodeWriter == null) {
            this.nodeWriter = new NodeWriter(this.Out, this.xmlns);
         }
      }

      public override NodeWriter apply_templates(XmlReader select, string mode) {

         EnsureInitialized();

         if (select.ReadState == ReadState.Initial) {
            
            select.Read();

            if (select.NodeType != XmlNodeType.Document) {
               return EvaluateTemplate(select, ParseMode(mode), DocumentNodeDefault);
            }
         }

         return base.apply_templates(select, mode);
      }

      void IterateChildren(XmlReader reader, Func<XmlReader, bool> match, Action template) {

         reader.MoveToElement();

         int parentDepth = reader.Depth;

         if (reader.IsEmptyElement) {
            return;
         }

         Func<XmlReader, bool> isSibling = r => r.Depth == parentDepth + 1
            && r.NodeType != XmlNodeType.EndElement;

         while (reader.Read()
            && reader.Depth > parentDepth) {

            if (isSibling(reader)
               && match(reader)) {

               template();

               while (this.readNextSibling
                  && isSibling(reader)
                  && match(reader)) {

                  this.readNextSibling = false;
                  template();
               }
            }

            if (reader.Depth <= parentDepth) {
               return;
            }
         }
      }

      [EditorBrowsable(EditorBrowsableState.Never)]
      protected override NodeWriter EvaluateTemplate(Func<NodeWriter> template) {

         XmlReader reader = current();

         XmlNodeType beforeNodeType = reader.NodeType;

         NodeWriter result = template();

         if (!this.readNextSibling) {

            switch (beforeNodeType) {
               case XmlNodeType.Element:

                  if (reader.NodeType == XmlNodeType.Element) {

                     reader.Skip();

                     if (reader.NodeType != XmlNodeType.EndElement) {
                        this.readNextSibling = true;
                     }
                  }
                  break;

               case XmlNodeType.Document:
                  reader.Skip();
                  break;
            }
         }

         return result;
      }

      protected override NodeWriter copy_of(XmlReader select) {

         this.Out.WriteNode(select, defattr: true);

         // XmlWriter.WriteNode moves the reader to the start of the next sibling
         // the iterating method must call apply_templates again

         if (select.NodeType != XmlNodeType.EndElement) {
            this.readNextSibling = true;
         }

         return this.nodeWriter;
      }

      protected override string @string(XmlReader item) {
         return item.ReadString();
      }

      [EditorBrowsable(EditorBrowsableState.Never)]
      protected override NodeWriter AttributeDefault() {
         
         this.Out.WriteString(current().Value);

         return this.nodeWriter;
      }

      [EditorBrowsable(EditorBrowsableState.Never)]
      protected override NodeWriter DocumentNodeDefault() {

         XmlReader reader = current();

         if (reader.NodeType == XmlNodeType.Document) {
            reader.Read();
         }

         if (reader.EOF) {
            return this.nodeWriter;
         }

         int docChildDepth = reader.Depth;

         Func<XmlReader, bool> isDocumentChild = r => r.Depth == docChildDepth
            && r.NodeType != XmlNodeType.EndElement;

         // Ignore non-XPath nodes like XmlDeclaration and DocumentType

         Func<XmlReader, bool> isXPathNode = r => match.text()(r)
            || match.processing_instruction()(r)
            || match.comment()(r)
            || match.element()(r);

         do {
            if (!isXPathNode(reader)) {
               continue;
            }

            if (isDocumentChild(reader)) {
               apply_templates(reader, mode: "#current");
            }

            while (this.readNextSibling
               && isDocumentChild(reader)) {

               this.readNextSibling = false;
               apply_templates(reader, mode: "#current");
            } 

         } while (reader.Read() && reader.Depth >= docChildDepth);

         return this.nodeWriter;
      }

      [EditorBrowsable(EditorBrowsableState.Never)]
      protected override NodeWriter ElementDefault() {

         XmlReader reader = current();

         if (reader.NodeType != XmlNodeType.Element) {
            throw new InvalidOperationException("Current node should be Element.");
         }

         IterateChildren(reader, r => true, () => apply_templates(reader, mode: "#current"));

         return this.nodeWriter;
      }

      [EditorBrowsable(EditorBrowsableState.Never)]
      protected override NodeWriter TextDefault() {
         
         this.Out.WriteString(current().Value);

         return this.nodeWriter;
      }

      [EditorBrowsable(EditorBrowsableState.Never)]
      protected override NodeWriter EmptyTemplate() {
         return this.nodeWriter;
      }

      // Writer helpers

      protected NodeWriter element(string name) {
         return this.nodeWriter.element(name);
      }

      protected NodeWriter element(string name, string ns) {
         return this.nodeWriter.element(name, ns);
      }

      protected NodeWriter elem(string name, string value) {
         return this.nodeWriter.elem(name, value);
      }

      protected NodeWriter elem(string name, string ns, string value) {
         return this.nodeWriter.elem(name, ns, value);
      }

      protected NodeWriter attr(string name, string value) {
         return this.nodeWriter.attr(name, value);
      }

      protected NodeWriter attr(string name, string ns, string value) {
         return this.nodeWriter.attr(name, ns, value);
      }

      protected NodeWriter CDATA(string text) {
         return this.nodeWriter.CDATA(text);
      }

      protected NodeWriter text(string text) {
         return this.nodeWriter.text(text);
      }

      protected NodeWriter comment(string text) {
         return this.nodeWriter.comment(text);
      }

      protected NodeWriter processing_instruction(string name, string text) {
         return this.nodeWriter.processing_instruction(name, text);
      }
   }
}
