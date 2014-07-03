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

namespace NTransform {
   
   sealed class TemplateRule<TInput, TOutput> {

      readonly Func<TInput, bool> match;
      readonly Func<TOutput> sequenceConstructor;
      readonly HashSet<string> modes;
      bool allModes;

      internal TemplateRule(Func<TInput, bool> match, Func<TOutput> sequenceConstructor, string mode = null) {

         this.match = match;
         this.sequenceConstructor = sequenceConstructor;
         this.modes = new HashSet<string>(StringComparer.Ordinal);

         SetModes(mode);
      }

      void SetModes(string mode) {

         if (mode == null) {
            this.modes.Add(mode);
            return;
         }

         string[] suppliedModes = mode.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

         for (int i = 0; i < suppliedModes.Length; i++) {

            string m = suppliedModes[i];

            if (m == "#all") {
               this.allModes = true;
               break;
            }

            if (m == "#default") {
               m = null;
            }

            if (!this.modes.Contains(m)) {
               this.modes.Add(m);
            }
         }
      }

      public bool HasMode(string mode) {

         return this.allModes
            || this.modes.Contains(mode);
      }

      public bool IsMatch(TInput input) {
         return this.match(input);
      }

      public TOutput Evaluate() {
         return this.sequenceConstructor();
      }
   }
}
