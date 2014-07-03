using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NTransform.Tests {
   
   [TestClass]
   public class NamespaceBindingsTests {

      [TestMethod]
      public void Sets_Namespace() {
         
         var ns = new NamespaceBindings();
         ns["a"] = "x";

         Assert.AreEqual("x", ns["a"]);

         ns["b"] = null;

         Assert.AreEqual(null, ns["b"]);
      }

      [TestMethod]
      public void Sets_Null_Prefix() {

         var ns = new NamespaceBindings();
         ns[null] = "x";

         Assert.AreEqual("x", ns[null]);
         Assert.AreEqual("x", ns[""]);

         ns[""] = "y";

         Assert.AreEqual("y", ns[""]);
         Assert.AreEqual("y", ns[null]);
      }

      [TestMethod]
      public void Returns_Null_For_Non_Existing() {

         var ns = new NamespaceBindings();

         Assert.AreEqual(null, ns["a"]);
      }

      [TestMethod]
      public void Prefixes_Are_Case_Sensitive() {

         var ns = new NamespaceBindings();
         ns["a"] = "x";
         ns["A"] = "y";

         Assert.AreEqual("x", ns["a"]);
         Assert.AreEqual("y", ns["A"]);
      }
   }
}
