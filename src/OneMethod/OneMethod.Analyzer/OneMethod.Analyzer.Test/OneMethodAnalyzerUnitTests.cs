using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = OneMethod.Analyzer.Test.CSharpAnalyzerVerifier<OneMethod.Analyzer.OneMethodAnalyzer>;

namespace OneMethod.Analyzer.Test
{
    [TestClass]
    public class OneMethodAnalyzerUnitTest
    {
        private const string _diagnosticsId = "OM0001";

        //No diagnostics expected to show up
        [TestMethod]
        public async Task Empty_Ok()
        {
            const string test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task OnePublic_OneInternal_NotOk()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public class {|#0:TypeName|}
        {
            public void {|#1:Do|}(){ }
            internal void {|#2:DoInternal|}(){ }
            private void DoPrivate(){ }
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(0).WithArguments("TypeName"),
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(1).WithArguments("TypeName"),
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(2).WithArguments("TypeName"));
        }

        [TestMethod]
        public async Task TwoPublic_NotOk()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public class {|#0:TypeName|}
        {
            public void {|#1:Do|}(){ }
            public void {|#2:Do2|}(){ }
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(0).WithArguments("TypeName"),
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(1).WithArguments("TypeName"),
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(2).WithArguments("TypeName"));
        }

        [TestMethod]
        public async Task TwoInternal_NotOk()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public class {|#0:TypeName|}
        {
            internal void {|#1:Do|}(){ }
            internal void {|#2:Do2|}(){ }
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(0).WithArguments("TypeName"),
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(1).WithArguments("TypeName"),
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(2).WithArguments("TypeName"));
        }

        [TestMethod]
        public async Task InternalClass_NotOk()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        internal class {|#0:TypeName|}
        {
            public void {|#1:Do|}(){ }
            public void {|#2:Do2|}(){ }
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(0).WithArguments("TypeName"),
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(1).WithArguments("TypeName"),
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(2).WithArguments("TypeName"));
        }

        [TestMethod]
        public async Task AbstractClass_TwoPublic_NotOk()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public abstract class {|#0:TypeName|}
        {
            public void {|#1:Do|}(){ }
            public void {|#2:Do2|}(){ }
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(0).WithArguments("TypeName"),
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(1).WithArguments("TypeName"),
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(2).WithArguments("TypeName"));
        }

        [TestMethod]
        public async Task StaticClass_TwoPublic_Ok()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public static class {|#0:TypeName|}
        {
            public static void Do(){ }
            public static void Do2(){ }
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TwoStaticPublic_NotOk()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public class {|#0:TypeName|}
        {
            public static void {|#1:Do|}(){ }
            public static void {|#2:Do2|}(){ }
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(0).WithArguments("TypeName"),
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(1).WithArguments("TypeName"),
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(2).WithArguments("TypeName"));
        }

        [TestMethod]
        public async Task Program_OnePublicAndMain_NotOk()
        {
            const string test = @"
    using System;

    namespace ConsoleApp1
    {
        class {|#0:Program|}
        {
            static void Main(string[] args)
            {
                Console.WriteLine(""Hello World!"");
            }

            public void {|#1:Test|}()
            {

            }
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(0).WithArguments("Program"),
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(1).WithArguments("Program"));
        }

        [TestMethod]
        public async Task OnePublicMethod_Ok()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public class {|#0:TypeName|}
        {
            public void Do(){ }
            protected void DoInternal(){ }
            private void DoPrivate(){ }
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task OnePublicMethod_OnePublicProperty_Ok()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public class {|#0:TypeName|}
        {
            public void Do(){ }
            public string Foo { get; }
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task MsTest_Ok()
        {
            const string test = @"

    namespace ConsoleApplication1
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;
        using System.Threading.Tasks;
        using System.Diagnostics;
        using Microsoft.VisualStudio.TestTools.UnitTesting;

        [TestClass]
        public class TypeName
        {
            [TestMethod]
            public void Foo(){ }
            
            [TestMethod]
            public void Bar(){ }
        }
    }

    namespace Microsoft.VisualStudio.TestTools.UnitTesting
    {
        public class TestClassAttribute : System.Attribute { }
        public class TestMethodAttribute : System.Attribute { }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task MsTest_NotOk()
        {
            const string test = @"
    namespace ConsoleApplication1
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;
        using System.Threading.Tasks;
        using System.Diagnostics;
        using Microsoft.VisualStudio.TestTools.UnitTesting;

        [TestClass]
        public class {|#0:TypeName|}
        {
            [TestMethod]
            public void Foo(){ }
            
            [TestMethod]
            public void Bar(){ }

            public void {|#1:Do|}(){ }
        }
    }

    namespace Microsoft.VisualStudio.TestTools.UnitTesting
    {
        public class TestClassAttribute : System.Attribute { }
        public class TestMethodAttribute : System.Attribute { }
    }";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(0).WithArguments("TypeName"),
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(1).WithArguments("TypeName"));
        }

        [TestMethod]
        public async Task NUnit_Ok()
        {
            const string test = @"
    namespace ConsoleApplication1
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;
        using System.Threading.Tasks;
        using System.Diagnostics;
        using NUnit.Framework;

        [TestFixture]
        public class TypeName
        {
            [Test]
            public void Foo(){ }
            
            [Test]
            public void Bar(){ }
        }
    }

    namespace NUnit.Framework
    {
        public class TestFixtureAttribute : System.Attribute { }
        public class TestAttribute : System.Attribute { }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task NUnit_NotOk()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using NUnit.Framework;

    namespace ConsoleApplication1
    {
        [TestFixture]
        public class {|#0:TypeName|}
        {
            [Test]
            public void Foo(){ }
            
            [Test]
            public void Bar(){ }

            public void {|#1:Do|}(){ }
        }
    }

    namespace NUnit.Framework
    {
        public class TestFixtureAttribute : System.Attribute { }
        public class TestAttribute : System.Attribute { }
    }";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(0).WithArguments("TypeName"),
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(1).WithArguments("TypeName"));
        }

        [TestMethod]
        public async Task xUnit_Ok()
        {
            const string test = @"
    namespace ConsoleApplication1
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;
        using System.Threading.Tasks;
        using System.Diagnostics;
        using Xunit;

        public class TypeName
        {
            [Fact]
            public void Foo(){ }
            
            [Fact]
            public void Bar(){ }
        }
    }

    namespace Xunit
    {
        public class FactAttribute : System.Attribute { }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task xUnit_NotOk()
        {
            const string test = @"
    namespace ConsoleApplication1
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;
        using System.Threading.Tasks;
        using System.Diagnostics;
        using Xunit;

        public class {|#0:TypeName|}
        {
            [Fact]
            public void Foo(){ }
            
            [Fact]
            public void Bar(){ }

            public void {|#1:Do|}(){ }
        }
    }

    namespace Xunit
    {
        public class FactAttribute : System.Attribute { }
    }";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(0).WithArguments("TypeName"),
                VerifyCS.Diagnostic(_diagnosticsId).WithLocation(1).WithArguments("TypeName"));
        }
    }
}
