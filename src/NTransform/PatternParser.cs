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

namespace NTransform {
   
   public abstract class PatternParser<TInput> {

      readonly NamespaceBindings ns;

      protected PatternParser(NamespaceBindings ns) {
         this.ns = ns;
      }

      public abstract Func<TInput, bool> attribute();

      public Func<TInput, bool> attribute(string name) {

         if (name == null) throw new ArgumentNullException("name");

         if (name.Length == 1
            && name[0] == '*') {

            return attribute();
         }

         return attribute(ParseQName(name));
      }

      protected abstract Func<TInput, bool> attribute(XmlQualifiedName name);

      public abstract Func<TInput, bool> comment();

      public abstract Func<TInput, bool> document_node();

      public abstract Func<TInput, bool> element();

      public Func<TInput, bool> element(string name) {

         if (name == null) throw new ArgumentNullException("name");

         if (name.Length == 1
            && name[0] == '*') {

            return element();
         }

         return element(ParseQName(name));
      }

      protected abstract Func<TInput, bool> element(XmlQualifiedName name);

      public abstract Func<TInput, bool> processing_instruction();

      public Func<TInput, bool> processing_instruction(string name) {

         if (name == null) throw new ArgumentNullException("name");

         if (name.Length == 1
            && name[0] == '*') {

            return processing_instruction();
         }

         return processing_instruction(ParseQName(name));
      }

      protected abstract Func<TInput, bool> processing_instruction(XmlQualifiedName name);

      public abstract Func<TInput, bool> text();

      XmlQualifiedName ParseQName(string name) {

         string[] nameParts = name.Split(':');

         XmlQualifiedName qname = (nameParts.Length > 1) ?
            new XmlQualifiedName(nameParts[1], this.ns[nameParts[0]])
            : new XmlQualifiedName(name);

         return qname;
      }

      [EditorBrowsable(EditorBrowsableState.Never)]
      public virtual Func<TInput, bool> ParsePattern(string pattern) {

         if (pattern == null) throw new ArgumentNullException("pattern");
         if (pattern.Length == 0) throw new ArgumentException("pattern cannot be empty.", "pattern");

         if (pattern.Length == 1) {

            if (pattern[0] == '/') {
               return document_node();
            }

            return element(pattern); 
         }

         if (pattern[0] == '@') {
            return attribute(pattern.Substring(1)); 
         }

         return element(pattern); 
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
