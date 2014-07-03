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

   public sealed class NodeWriter {

      readonly XmlWriter Out;
      readonly NamespaceBindings nsBindings;

      internal NodeWriter(XmlWriter writer, NamespaceBindings ns) {

         this.Out = writer;
         this.nsBindings = ns;
      }

      public NodeWriter element(string name) {

         if (name == null) throw new ArgumentNullException("name");

         string[] nameParts = name.Split(':');

         if (nameParts.Length > 1) {
            this.Out.WriteStartElement(nameParts[0], nameParts[1], this.nsBindings[nameParts[0]]);

         } else {
            this.Out.WriteStartElement(name, this.nsBindings[null]);
         }

         return this;
      }

      public NodeWriter element(string name, string ns) {

         if (name == null) throw new ArgumentNullException("name");

         string[] nameParts = name.Split(':');

         if (nameParts.Length > 1) {
            this.Out.WriteStartElement(nameParts[0], nameParts[1], ns);

         } else {
            this.Out.WriteStartElement(name, ns);
         }

         return this;
      }

      public NodeWriter end() {

         this.Out.WriteEndElement();
         return this;
      }

      public NodeWriter elem(string name, string value) {

         element(name)
            .text(value)
         .end();

         return this;
      }

      public NodeWriter elem(string name, string ns, string value) {

         element(name, ns)
            .text(value)
         .end();

         return this;
      }

      public NodeWriter attr(string name, string value) {
         return attr(name, null, value);
      }

      public NodeWriter attr(string name, string ns, string value) {

         if (name == null) throw new ArgumentNullException("name");

         string[] nameParts = name.Split(':');

         if (nameParts.Length > 1) {
            this.Out.WriteAttributeString(nameParts[0], nameParts[1], this.nsBindings[nameParts[0]], value);

         } else {
            this.Out.WriteAttributeString(name, ns, value);
         }

         return this;
      }

      public NodeWriter xmlns(string prefix, string ns) {

         this.Out.WriteAttributeString("xmlns", prefix, null, ns);

         return this;
      }

      public NodeWriter CDATA(string text) {

         this.Out.WriteCData(text);

         return this;
      }

      public NodeWriter text(string text) {

         this.Out.WriteString(text);

         return this;
      }

      public NodeWriter comment(string text) {

         this.Out.WriteComment(text);

         return this;
      }

      public NodeWriter processing_instruction(string name, string text) {

         this.Out.WriteProcessingInstruction(name, text);

         return this;
      }

      public NodeWriter write(Action action) {

         action();

         return this;
      }

      #region Object Members

      [EditorBrowsable(EditorBrowsableState.Never)]
      public override bool Equals(object obj) {
         return base.Equals(obj);
      }

      [EditorBrowsable(EditorBrowsableState.Never)]
      public override int GetHashCode() {
         return base.GetHashCode();
      }

      [EditorBrowsable(EditorBrowsableState.Never)]
      public override string ToString() {
         return base.ToString();
      }

      #endregion
   }
}
