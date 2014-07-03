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
using System.Xml.Xsl.Runtime;

namespace NTransform {
   
   public abstract class Transform<TInput, TOutput> {

      string currentMode;
      TInput contextNode;

      protected NamespaceBindings xmlns { get; private set; }
      protected TransformTemplates<TInput, TOutput> template { get; private set; }
      protected PatternParser<TInput> match { get; private set; }

      protected Transform() {

         this.xmlns = new NamespaceBindings();
         this.match = CreatePatternParser(this.xmlns);
         this.template = new TransformTemplates<TInput, TOutput>(EmptyTemplate, this.match);
      }

      protected abstract PatternParser<TInput> CreatePatternParser(NamespaceBindings xmlns);

      // Instructions

      protected TOutput apply_templates() {
         return apply_templates(mode: null);
      }

      protected TOutput apply_templates(string mode) {

         // Only document and element nodes can have children

         Func<TOutput> template = match.element()(this.contextNode) ? ElementDefault
            : match.document_node()(this.contextNode) ? DocumentNodeDefault
            : (Func<TOutput>)EmptyTemplate;

         mode = ParseMode(mode);

         return EvaluateTemplate(this.contextNode, mode, template);
      }

      public TOutput apply_templates(TInput select) {
         return apply_templates(select, mode: null);
      }

      public virtual TOutput apply_templates(TInput select, string mode) {

         if (select == null) throw new ArgumentNullException("select");

         mode = ParseMode(mode);

         Func<TOutput> selectedTemplate = null;

         for (int i = this.template.UserTemplates.Count - 1; i >= 0; i--) {

            TemplateRule<TInput, TOutput> tmpl = this.template.UserTemplates[i];

            if (!tmpl.HasMode(mode)) {
               continue;
            }

            if (tmpl.IsMatch(select)) {

               selectedTemplate = tmpl.Evaluate;
               break;
            }
         }

         if (selectedTemplate == null) {

            selectedTemplate = this.match.element()(select) ? ElementDefault
               : this.match.text()(select) ? TextDefault
               : this.match.attribute()(select) ? AttributeDefault
               : this.match.processing_instruction()(select) ? EmptyTemplate
               : this.match.comment()(select) ? EmptyTemplate
               : this.match.document_node()(select) ? DocumentNodeDefault
               : (Func<TOutput>)EmptyTemplate;
         }

         return EvaluateTemplate(select, mode, selectedTemplate);
      }
      
      internal string ParseMode(string mode) {

         if (mode == null) {
            return mode;
         }

         if (mode == "#current") {
            return this.currentMode;
         }

         return mode;
      }

      internal TOutput EvaluateTemplate(TInput select, string mode, Func<TOutput> template) {

         TInput prevNode = this.contextNode;
         string prevMode = this.currentMode;

         this.contextNode = select;
         this.currentMode = mode;

         try {
            return EvaluateTemplate(template);

         } finally {

            this.contextNode = prevNode;
            this.currentMode = prevMode;
         }
      }

      protected abstract TOutput EvaluateTemplate(Func<TOutput> template);

      protected abstract TOutput copy_of(TInput select);

      // Functions

      protected TInput current() {

         if (this.contextNode == null) {
            throw new InvalidOperationException("The context node is not available.");
         }

         return this.contextNode;
      }

      protected string @string() {
         return @string(current());
      }

      protected abstract string @string(TInput item);

      protected string normalize_space(string value) {
         return XsltFunctions.NormalizeSpace(value);
      }

      protected string substring_before(string s1, string s2) {
         return XsltFunctions.SubstringBefore(s1, s2);
      }

      protected string substring_after(string s1, string s2) {
         return XsltFunctions.SubstringAfter(s1, s2);
      }

      // Built-in Template Rules

      protected abstract TOutput AttributeDefault();

      protected abstract TOutput DocumentNodeDefault();

      protected abstract TOutput ElementDefault();

      protected abstract TOutput TextDefault();

      protected abstract TOutput EmptyTemplate();
   }
}
