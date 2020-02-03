using FluentDecorator;
using FluentDecoratorTest.Data;
using NUnit.Framework;
using System;

namespace FluentDecoratorTest
{
    public class InitModelDecorators
    {
        private SimpleClass simpleClass { get; set; }
        private ClassWithMember classWithMember { get; set; }
        private ClassWithCollection classWithCollection { get; set; }
        private ClassWithDynamicProperty classWithDynamicProperty { get; set; }

        [SetUp]
        public void Setup()
        {
            simpleClass = new SimpleClass() { Age = 62, Name = "Me" };
            classWithMember = new ClassWithMember();
            classWithCollection = new ClassWithCollection();
            classWithDynamicProperty = new ClassWithDynamicProperty() { Id = Guid.NewGuid(), ADynamicPrioperty = new SimpleClass() };
        }

        [Test]
        public void InitSimple_WithModelDecorators_ReturnDecoratorsForClassAndProperties()
        {
            var decorationDefinition = ModelDecorators.Init<SimpleClass, Decorators>();
            var modelDecorated = decorationDefinition(simpleClass);

            Assert.IsTrue(modelDecorated.Decorators.Enabled == false);
            Assert.IsTrue(modelDecorated.Decorators.Visible == false);
            Assert.IsTrue(modelDecorated.PropertyDecorators.Count == 2);
            Assert.IsTrue(modelDecorated.PropertyDecorators["Name"].Enabled == false);
            Assert.IsTrue(modelDecorated.PropertyDecorators["Age"].Enabled == false);
            Assert.IsFalse(modelDecorated.GetFor(p => p.Name).Visible);
            modelDecorated.GetFor(p => p.Name).Visible = true;
            Assert.IsTrue(modelDecorated.GetFor(p => p.Name).Visible);
        }
        [Test]
        public void InitCollection_WithModelDecorators_ReturnDecoratorsForClassAndPropertiesCollectionsExcluded()
        {
            var decorationDefinition = ModelDecorators.Init<ClassWithCollection, Decorators>();
            var modelDecorated = decorationDefinition(classWithCollection);
            Assert.IsTrue(modelDecorated.Decorators.Enabled == false);
            Assert.IsTrue(modelDecorated.Decorators.Visible == false);
            Assert.IsTrue(modelDecorated.PropertyDecorators.Count == 1);
            Assert.Catch<InvalidOperationException>(() => modelDecorated.GetFor(m => m.SimpleClasses));

        }
        [Test]
        public void InitClassWithMember_WithModelDecorators_ReturnDecoratorsForClassAndPropertiesDescendantsIncluded()
        {
            var decorationDefinition = ModelDecorators.Init<ClassWithMember, Decorators>();
            var modelDecorated = decorationDefinition(classWithMember);

            Assert.IsTrue(modelDecorated.Decorators.Enabled == false);
            Assert.IsTrue(modelDecorated.Decorators.Visible == false);
            Assert.IsTrue(modelDecorated.PropertyDecorators.Count == 4);
            Assert.IsTrue(modelDecorated.PropertyDecorators["CreatedOn"].Enabled == false);
            Assert.IsTrue(modelDecorated.PropertyDecorators["SimpleClass"].Enabled == false);
            Assert.IsTrue(modelDecorated.PropertyDecorators["SimpleClass.Name"].Enabled == false);
            Assert.IsFalse(modelDecorated.GetFor(p => p.SimpleClass.Name).Visible);
            modelDecorated.GetFor(p => p.SimpleClass.Name).Visible = true;
            Assert.IsTrue(modelDecorated.GetFor(p => p.SimpleClass.Name).Visible);
        }

        [Test]
        public void InitClassWithDynamicProeprty_WithModelDecorators_ReturnDecoratorsNotForDynamicProperty()
        {
            var decorationDefinition = ModelDecorators.Init<ClassWithDynamicProperty, Decorators>();
            var modelDecorated = decorationDefinition(classWithDynamicProperty);

            Assert.IsTrue(modelDecorated.Decorators.Enabled == false);
            Assert.IsTrue(modelDecorated.Decorators.Visible == false);
            Assert.IsTrue(modelDecorated.PropertyDecorators.Count == 2);
        }

    }
}