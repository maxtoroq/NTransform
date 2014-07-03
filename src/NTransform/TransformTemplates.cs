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
using System.Collections.Generic;
using System.ComponentModel;

namespace NTransform {
   
   public sealed class TransformTemplates<TInput, TOutput> {

      internal readonly List<TemplateRule<TInput, TOutput>> UserTemplates = new List<TemplateRule<TInput, TOutput>>();

      readonly Func<TOutput> emptyTemplate;
      readonly PatternParser<TInput> patternParser;

      public Func<TOutput> this[Func<TInput, bool> match, string mode = null] {
         set {
            this.UserTemplates.Add(new TemplateRule<TInput, TOutput>(match, value ?? emptyTemplate, mode));
         }
      }

      public Func<TOutput> this[string match, string mode = null] {
         set {
            this.UserTemplates.Add(new TemplateRule<TInput, TOutput>(patternParser.ParsePattern(match), value ?? emptyTemplate, mode));
         }
      }

      internal TransformTemplates(Func<TOutput> emptyTemplate, PatternParser<TInput> patternParser) {

         this.emptyTemplate = emptyTemplate;
         this.patternParser = patternParser;
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
