﻿using System;
using NSubstitute;
using Xunit;
using Xunit.Sdk;

public class CollectionPerClassTestCollectionFactoryTests
{
    [Fact]
    public void DefaultCollectionBehaviorIsCollectionPerClass()
    {
        var method1 = Mocks.MethodInfo(type: Mocks.TypeInfo("FullyQualified.Type.Number1"));
        var method2 = Mocks.MethodInfo(type: Mocks.TypeInfo("FullyQualified.Type.Number2"));
        var assembly = Mocks.AssemblyInfo();
        assembly.AssemblyPath.Returns(@"C:\Foo\bar.dll");
        var factory = new CollectionPerClassTestCollectionFactory(assembly);

        var result1 = factory.Get(method1);
        var result2 = factory.Get(method2);

        Assert.NotSame(result1, result2);
        Assert.Equal("Test collection for FullyQualified.Type.Number1", result1.DisplayName);
        Assert.Equal("Test collection for FullyQualified.Type.Number2", result2.DisplayName);
        Assert.Null(result1.CollectionDefinition);
        Assert.Null(result2.CollectionDefinition);
    }

    [Fact]
    public void ClassesDecoratedWithSameCollectionNameAreInSameTestCollection()
    {
        var attr = Mocks.CollectionAttribute("My Collection");
        var method1 = Mocks.MethodInfo(type: Mocks.TypeInfo("type1", attributes: new[] { attr }));
        var method2 = Mocks.MethodInfo(type: Mocks.TypeInfo("type2", attributes: new[] { attr }));
        var assembly = Mocks.AssemblyInfo();
        assembly.AssemblyPath.Returns(@"C:\Foo\bar.dll");
        var factory = new CollectionPerClassTestCollectionFactory(assembly);

        var result1 = factory.Get(method1);
        var result2 = factory.Get(method2);

        Assert.Same(result1, result2);
        Assert.Equal("My Collection", result1.DisplayName);
    }

    [Fact]
    public void ClassesWithDifferentCollectionNamesHaveDifferentCollectionObjects()
    {
        var method1 = Mocks.MethodInfo(type: Mocks.TypeInfo("type1", attributes: new[] { Mocks.CollectionAttribute("Collection 1") }));
        var method2 = Mocks.MethodInfo(type: Mocks.TypeInfo("type2", attributes: new[] { Mocks.CollectionAttribute("Collection 2") }));
        var assembly = Mocks.AssemblyInfo();
        assembly.AssemblyPath.Returns(@"C:\Foo\bar.dll");
        var factory = new CollectionPerClassTestCollectionFactory(assembly);

        var result1 = factory.Get(method1);
        var result2 = factory.Get(method2);

        Assert.NotSame(result1, result2);
        Assert.Equal("Collection 1", result1.DisplayName);
        Assert.Equal("Collection 2", result2.DisplayName);
    }

    [Fact]
    public void ExplicitlySpecifyingACollectionWithTheSameNameAsAnImplicitWorks()
    {
        var method1 = Mocks.MethodInfo(type: Mocks.TypeInfo("type1"));
        var method2 = Mocks.MethodInfo(type: Mocks.TypeInfo("type2", attributes: new[] { Mocks.CollectionAttribute("Test collection for type1") }));
        var assembly = Mocks.AssemblyInfo();
        assembly.AssemblyPath.Returns(@"C:\Foo\bar.dll");
        var factory = new CollectionPerClassTestCollectionFactory(assembly);

        var result1 = factory.Get(method1);
        var result2 = factory.Get(method2);

        Assert.Same(result1, result2);
        Assert.Equal("Test collection for type1", result1.DisplayName);
    }

    [Fact]
    public void UsingTestCollectionDefinitionSetsTypeInfo()
    {
        var testMethod = Mocks.MethodInfo(type: Mocks.TypeInfo("type", attributes: new[] { Mocks.CollectionAttribute("This is a test collection") }));
        var collectionDefinitionType = Mocks.TypeInfo("collectionDefinition", attributes: new[] { Mocks.CollectionDefinitionAttribute("This is a test collection") });
        var assembly = Mocks.AssemblyInfo(new[] { collectionDefinitionType });
        assembly.AssemblyPath.Returns(@"C:\Foo\bar.dll");
        var factory = new CollectionPerClassTestCollectionFactory(assembly);

        var result = factory.Get(testMethod);

        Assert.Same(collectionDefinitionType, result.CollectionDefinition);
    }

    [Fact]
    public void MultiplyDeclaredCollectionsRaisesEnvironmentalWarning()
    {
        var broker = Substitute.For<IMessageAggregator>();
        var testMethod = Mocks.MethodInfo(type: Mocks.TypeInfo("type", attributes: new[] { Mocks.CollectionAttribute("This is a test collection") }));
        var collectionDefinition1 = Mocks.TypeInfo("collectionDefinition1", attributes: new[] { Mocks.CollectionDefinitionAttribute("This is a test collection") });
        var collectionDefinition2 = Mocks.TypeInfo("collectionDefinition2", attributes: new[] { Mocks.CollectionDefinitionAttribute("This is a test collection") });
        var assembly = Mocks.AssemblyInfo(new[] { collectionDefinition1, collectionDefinition2 });
        assembly.AssemblyPath.Returns(@"C:\Foo\bar.dll");
        var factory = new CollectionPerClassTestCollectionFactory(assembly, broker);

        factory.Get(testMethod);

        var captured = broker.Captured(b => b.Add<EnvironmentalWarning>(null));
        var warning = captured.Arg<EnvironmentalWarning>();
        Assert.Equal("Multiple test collections declared with name 'This is a test collection': collectionDefinition1, collectionDefinition2", warning.Message);
    }
}