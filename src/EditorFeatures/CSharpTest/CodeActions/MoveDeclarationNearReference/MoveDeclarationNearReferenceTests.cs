// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.CodeRefactorings.MoveDeclarationNearReference;
using Microsoft.CodeAnalysis.CSharp.Test.Utilities;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.CodeRefactorings.MoveDeclarationNearReference
{
    public class MoveDeclarationNearReferenceTests : AbstractCSharpCodeActionTest
    {
        protected override CodeRefactoringProvider CreateCodeRefactoringProvider(Workspace workspace, TestParameters parameters)
            => new MoveDeclarationNearReferenceCodeRefactoringProvider();

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task TestMove1()
        {
            await TestInRegularAndScriptAsync(
@"class C
{
    void M()
    {
        int [||]x;
        {
            Console.WriteLine(x);
        }
    }
}",
@"class C
{
    void M()
    {
        {
            int x;
            Console.WriteLine(x);
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task TestMove2()
        {
            await TestInRegularAndScriptAsync(
@"class C
{
    void M()
    {
        int [||]x;
        Console.WriteLine();
        Console.WriteLine(x);
    }
}",
@"class C
{
    void M()
    {
        Console.WriteLine();
        int x;
        Console.WriteLine(x);
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task TestMove3()
        {
            await TestInRegularAndScriptAsync(
@"class C
{
    void M()
    {
        int [||]x;
        Console.WriteLine();
        {
            Console.WriteLine(x);
        }

        {
            Console.WriteLine(x);
        }
    }",
@"class C
{
    void M()
    {
        Console.WriteLine();
        int x;
        {
            Console.WriteLine(x);
        }

        {
            Console.WriteLine(x);
        }
    }");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task TestMove4()
        {
            await TestInRegularAndScriptAsync(
@"class C
{
    void M()
    {
        int [||]x;
        Console.WriteLine();
        {
            Console.WriteLine(x);
        }
    }",
@"class C
{
    void M()
    {
        Console.WriteLine();
        {
            int x;
            Console.WriteLine(x);
        }
    }");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task TestAssign1()
        {
            await TestInRegularAndScriptAsync(
@"class C
{
    void M()
    {
        int [||]x;
        {
            x = 5;
            Console.WriteLine(x);
        }
    }
}",
@"class C
{
    void M()
    {
        {
            int x = 5;
            Console.WriteLine(x);
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task TestAssign2()
        {
            await TestInRegularAndScriptAsync(
@"class C
{
    void M()
    {
        int [||]x = 0;
        {
            x = 5;
            Console.WriteLine(x);
        }
    }
}",
@"class C
{
    void M()
    {
        {
            int x = 5;
            Console.WriteLine(x);
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task TestAssign3()
        {
            await TestInRegularAndScriptAsync(
@"class C
{
    void M()
    {
        var [||]x = (short)0;
        {
            x = 5;
            Console.WriteLine(x);
        }
    }
}",
@"class C
{
    void M()
    {
        {
            var x = (short)0;
            x = 5;
            Console.WriteLine(x);
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task TestMissing1()
        {
            await TestMissingInRegularAndScriptAsync(
@"class C
{
    void M()
    {
        int [||]x;
        Console.WriteLine(x);
    }
}");
        }

        [WorkItem(538424, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/538424")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task TestMissingWhenReferencedInDeclaration()
        {
            await TestMissingInRegularAndScriptAsync(
@"class Program
{
    static void Main()
    {
        object[] [||]x = {
            x = null
        };
        x.ToString();
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task TestMissingWhenInDeclarationGroup()
        {
            await TestMissingInRegularAndScriptAsync(
@"class Program
{
    static void Main()
    {
        int [||]i = 5;
        int j = 10;
        Console.WriteLine(i);
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        [WorkItem(541475, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/541475")]
        public async Task Regression8190()
        {
            await TestMissingInRegularAndScriptAsync(
@"class Program
{
    void M()
    {
        {
            object x;
            [|object|] }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task TestFormatting()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        int [||]i = 5; Console.WriteLine();
        Console.Write(i);
    }
}",
@"class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine();
        int i = 5; Console.Write(i);
    }
}",
ignoreTrivia: false);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task TestMissingInHiddenBlock1()
        {
            await TestMissingInRegularAndScriptAsync(
@"class Program
{
    void Main()
    {
        int [|x|] = 0;
        Foo();
#line hidden
        Bar(x);
    }
#line default
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task TestMissingInHiddenBlock2()
        {
            await TestMissingInRegularAndScriptAsync(
@"class Program
{
    void Main()
    {
        int [|x|] = 0;
        Foo();
#line hidden
        Foo();
#line default
        Bar(x);
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task TestAvailableInNonHiddenBlock1()
        {
            await TestInRegularAndScriptAsync(
@"#line default
class Program
{
    void Main()
    {
        int [||]x = 0;
        Foo();
        Bar(x);
#line hidden
    }
#line default
}",
@"#line default
class Program
{
    void Main()
    {
        Foo();
        int x = 0;
        Bar(x);
#line hidden
    }
#line default
}",
ignoreTrivia: false);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task TestAvailableInNonHiddenBlock2()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    void Main()
    {
        int [||]x = 0;
        Foo();
#line hidden
        Foo();
#line default
        Foo();
        Bar(x);
    }
}",
@"class Program
{
    void Main()
    {
        Foo();
#line hidden
        Foo();
#line default
        Foo();
        int x = 0;
        Bar(x);
    }
}",
ignoreTrivia: false);
        }

        [WorkItem(545435, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545435")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task TestWarnOnChangingScopes1()
        {
            await TestInRegularAndScriptAsync(
@"using System.Linq;

class Program
{
    void Main()
    {
        var [||]@lock = new object();
        new[] { 1 }.AsParallel().ForAll((i) => {
            lock (@lock)
            {
            }
        });
    }
}",
@"using System.Linq;

class Program
{
    void Main()
    {
        new[] { 1 }.AsParallel().ForAll((i) => {
            {|Warning:var @lock = new object();|}
            lock (@lock)
            {
            }
        });
    }
}");
        }

        [WorkItem(545435, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545435")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task TestWarnOnChangingScopes2()
        {
            await TestInRegularAndScriptAsync(
@"using System;
using System.Linq;

class Program
{
    void Main()
    {
        var [||]i = 0;
        foreach (var v in new[] { 1 })
        {
            Console.Write(i);
            i++;
        }
    }
}",
@"using System;
using System.Linq;

class Program
{
    void Main()
    {
        foreach (var v in new[] { 1 })
        {
            {|Warning:var i = 0;|}
            Console.Write(i);
            i++;
        }
    }
}");
        }

        [WorkItem(545840, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545840")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task InsertCastIfNecessary1()
        {
            await TestInRegularAndScriptAsync(
@"using System;

static class C
{
    static int Outer(Action<int> x, object y) { return 1; }
    static int Outer(Action<string> x, string y) { return 2; }

    static void Inner(int x, int[] y) { }
    unsafe static void Inner(string x, int*[] y) { }

    static void Main()
    {
        var [||]a = Outer(x => Inner(x, null), null);
        unsafe
        {
            Console.WriteLine(a);
        }
    }
}",

@"using System;

static class C
{
    static int Outer(Action<int> x, object y) { return 1; }
    static int Outer(Action<string> x, string y) { return 2; }

    static void Inner(int x, int[] y) { }
    unsafe static void Inner(string x, int*[] y) { }

    static void Main()
    {
        unsafe
        {
            var a = Outer(x => Inner(x, null), (object)null);
            Console.WriteLine(a);
        }
    }
}",
ignoreTrivia: false);
        }

        [WorkItem(545835, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545835")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task InsertCastIfNecessary2()
        {
            await TestInRegularAndScriptAsync(
@"using System;

class X
{
    static int Foo(Func<int?, byte> x, object y) { return 1; }
    static int Foo(Func<X, byte> x, string y) { return 2; }

    const int Value = 1000;
    static void Main()
    {
        var [||]a = Foo(X => (byte)X.Value, null);
        unchecked
        {
            Console.WriteLine(a);
        }
    }
}",

@"using System;

class X
{
    static int Foo(Func<int?, byte> x, object y) { return 1; }
    static int Foo(Func<X, byte> x, string y) { return 2; }

    const int Value = 1000;
    static void Main()
    {
        unchecked
        {
            var a = Foo(X => (byte)X.Value, (object)null);
            Console.WriteLine(a);
        }
    }
}",
ignoreTrivia: false);
        }

        [WorkItem(546267, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/546267")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task MissingIfNotInDeclarationSpan()
        {
            await TestMissingInRegularAndScriptAsync(
@"using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        // Comment [||]about foo!
        // Comment about foo!
        // Comment about foo!
        // Comment about foo!
        // Comment about foo!
        // Comment about foo!
        // Comment about foo!
        int foo;
        Console.WriteLine();
        Console.WriteLine(foo);
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task Tuple()
        {
            await TestInRegularAndScriptAsync(
@"class C
{
    void M()
    {
        (int, string) [||]x;
        {
            Console.WriteLine(x);
        }
    }
}",
@"class C
{
    void M()
    {
        {
            (int, string) x;
            Console.WriteLine(x);
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsMoveDeclarationNearReference)]
        public async Task TupleWithNames()
        {
            await TestInRegularAndScriptAsync(
@"class C
{
    void M()
    {
        (int a, string b) [||]x;
        {
            Console.WriteLine(x);
        }
    }
}",
@"class C
{
    void M()
    {
        {
            (int a, string b) x;
            Console.WriteLine(x);
        }
    }
}");
        }
    }
}