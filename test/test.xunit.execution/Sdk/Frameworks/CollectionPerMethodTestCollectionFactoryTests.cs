using System;
using NSubstitute;
using Xunit;
using Xunit.Sdk;

public class CollectionPerMethodTestCollectionFactoryTests
{
    [Fact]
    public void DefaultCollectionBehaviorIsCollectionPerMethod()
    {
        var type = Mocks.TypeInfo("Type");
        var method1 = Mocks.MethodInfo("Method1", type: type);
        var method2 = Mocks.MethodInfo("Method2", type: type);
        var assembly = Mocks.AssemblyInfo();
        assembly.AssemblyPath.Returns(@"C:\Foo\bar.dll");
        var factory = new CollectionPerMethodTestCollectionFactory(assembly);

        var result1 = factory.Get(method1);
        var result2 = factory.Get(method2);

        Assert.NotSame(result1, result2);
        Assert.Equal("Test collection for Type.Method1", result1.DisplayName);
        Assert.Equal("Test collection for Type.Method2", result2.DisplayName);
        Assert.Null(result1.CollectionDefinition);
        Assert.Null(result2.CollectionDefinition);
    }

    [Fact]
    public void MethodsDecoratedWithSameCollectionNameAreInSameTestCollection()
    {
        var attr = Mocks.CollectionAttribute("My Collection");
        var method1 = Mocks.MethodInfo(attributes: new[] { attr });
        var method2 = Mocks.MethodInfo(attributes: new[] { attr });
        var assembly = Mocks.AssemblyInfo();
        assembly.AssemblyPath.Returns(@"C:\Foo\bar.dll");
        var factory = new CollectionPerMethodTestCollectionFactory(assembly);

        var result1 = factory.Get(method1);
        var result2 = factory.Get(method2);

        Assert.Same(result1, result2);
        Assert.Equal("My Collection", result1.DisplayName);
    }

    [Fact]
    public void  MethodsWithDifferentCollectionNamesHaveDifferentCollectionObjects()
    {
        var method1 = Mocks.MethodInfo(attributes: new[] { Mocks.CollectionAttribute("Collection 1") });
        var method2 = Mocks.MethodInfo(attributes: new[] { Mocks.CollectionAttribute("Collection 2") });
        var assembly = Mocks.AssemblyInfo();
        assembly.AssemblyPath.Returns(@"C:\Foo\bar.dll");
        var factory = new CollectionPerMethodTestCollectionFactory(assembly);

        var result1 = factory.Get(method1);
        var result2 = factory.Get(method2);

        Assert.NotSame(result1, result2);
        Assert.Equal("Collection 1", result1.DisplayName);
        Assert.Equal("Collection 2", result2.DisplayName);
    }

    [Fact]
    public void ExplicitlySpecifyingACollectionWithTheSameNameAsAnImplicitWorks()
    {
        var method1 = Mocks.MethodInfo("method1", type: Mocks.TypeInfo("type"));
        var method2 = Mocks.MethodInfo(attributes: new[] { Mocks.CollectionAttribute("Test collection for type.method1") });
        var assembly = Mocks.AssemblyInfo();
        assembly.AssemblyPath.Returns(@"C:\Foo\bar.dll");
        var factory = new CollectionPerMethodTestCollectionFactory(assembly);

        var result1 = factory.Get(method1);
        var result2 = factory.Get(method2);

        Assert.Same(result1, result2);
        Assert.Equal("Test collection for type.method1", result1.DisplayName);
    }
}