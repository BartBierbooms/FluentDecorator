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
        private RecursiveClassPrincipal recursiveClassPrincipal { get; set; }
        private Func<Decorators> initDecorators = () => new Decorators();

        [SetUp]
        public void Setup()
        {
            simpleClass = new SimpleClass() { Age = 62, Name = "Me" };
            classWithMember = new ClassWithMember();
            classWithCollection = new ClassWithCollection();
            classWithDynamicProperty = new ClassWithDynamicProperty() { Id = Guid.NewGuid(), ADynamicPrioperty = new SimpleClass() };
            recursiveClassPrincipal = new RecursiveClassPrincipal()
            {
                DependantOn = new RecursiveClassDependant() { PrincipalFrom = new RecursiveClassPrincipal(), Number = 2 },
                Count = 1
            };

        }

        [Test]
        public void InitSimple_WithModelDecorators_ReturnDecoratorsForClassAndProperties()
        {
            var decorationDefinition = ModelDecorators.Init<SimpleClass, Decorators>();
            var modelDecorated = decorationDefinition(simpleClass);

            Assert.IsTrue(modelDecorated.Decorators.Enabled == false);
            Assert.IsTrue(modelDecorated.Decorators.Visible == false);
            Assert.IsTrue(modelDecorated.PropertyDecorators.Count == 5);
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
            Assert.IsTrue(modelDecorated.PropertyDecorators.Count == 7);
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
            Assert.IsTrue(modelDecorated.PropertyDecorators.Count == 8);
        }
        [Test]
        public void InitClassWithRecusriveMembers_WithModelDecorators_ReturnDecoratorsForClassAndProperties()
        {
            var decorationDefinition = ModelDecorators.Init<RecursiveClassPrincipal, Decorators>();
            var modelDecorated = decorationDefinition(recursiveClassPrincipal);

            Assert.IsTrue(modelDecorated.Decorators.Enabled == false);
            Assert.IsTrue(modelDecorated.Decorators.Visible == false);
            Assert.IsTrue(modelDecorated.PropertyDecorators.Count == 3);

        }

        [Test]
        public void InitSimpleClass_WithModelDecoratorsFiltered_ReturnDecoratorsForClassAndPropertiesFiltered()
        {
            var excludeFilter = new FilterSetting(false);
            var decorationDefinition = ModelDecorators.Init<SimpleClass, Decorators>(initDecorators, excludeFilter);
            var modelDecorated = decorationDefinition(simpleClass);

            Assert.IsTrue(modelDecorated.Decorators.Enabled == false);
            Assert.IsTrue(modelDecorated.Decorators.Visible == false);
            Assert.IsTrue(modelDecorated.PropertyDecorators.Count == 4);
            Assert.IsTrue(!modelDecorated.PropertyDecorators.ContainsKey("Name"));
        }

        [Test]
        public void InitSimpleClass_WithModelDecoratorsFilteredOut_ReturnDecoratorsForClassAndPropertiesFiltered()
        {
            var excludeFilter = new FilterSetting(true);
            var decorationDefinition = ModelDecorators.Init<SimpleClass, Decorators>(initDecorators, excludeFilter);
            var modelDecorated = decorationDefinition(simpleClass);

            Assert.IsTrue(modelDecorated.Decorators.Enabled == false);
            Assert.IsTrue(modelDecorated.Decorators.Visible == false);
            Assert.IsTrue(modelDecorated.PropertyDecorators.Count == 1);
            Assert.IsTrue(modelDecorated.PropertyDecorators.ContainsKey("Age"));
        }

        [Test]
        public void InitClassWithMembers_WithModelDecoratorsFilteredOut_ReturnDecoratorsForClassAndPropertiesFiltered()
        {
            var includeFilter = new FilterSetting(true);
            var decorationDefinition = ModelDecorators.Init<ClassWithMember, Decorators>(initDecorators, includeFilter);
            var modelDecorated = decorationDefinition(classWithMember);

            Assert.IsTrue(modelDecorated.Decorators.Enabled == false);
            Assert.IsTrue(modelDecorated.Decorators.Visible == false);
            Assert.IsTrue(modelDecorated.PropertyDecorators.Count == 0);
        }

        [Test]
        public void InitClassWithMembers_WithModelDecoratorsFiltered_ReturnDecoratorsForClassAndPropertiesFiltered()
        {
            var excludeFilter = new FilterSetting(false);
            var decorationDefinition = ModelDecorators.Init<ClassWithMember, Decorators>(initDecorators, excludeFilter);
            var modelDecorated = decorationDefinition(classWithMember);

            Assert.IsTrue(modelDecorated.Decorators.Enabled == false);
            Assert.IsTrue(modelDecorated.Decorators.Visible == false);
            Assert.IsTrue(modelDecorated.PropertyDecorators.Count == 1);
            Assert.IsTrue(modelDecorated.PropertyDecorators.ContainsKey("CreatedOn"));
        }

        [Test]
        public void InitClassDynamic_WithModelDecoratorsFiltered_ReturnDecoratorsForClassAndPropertiesFiltered()
        {
            var includeFilter = new FilterSetting(true);
            var decorationDefinition = ModelDecorators.Init<ClassWithDynamicProperty, Decorators>(initDecorators, includeFilter);
            var modelDecorated = decorationDefinition(classWithDynamicProperty);

            Assert.IsTrue(modelDecorated.Decorators.Enabled == false);
            Assert.IsTrue(modelDecorated.Decorators.Visible == false);
            Assert.IsTrue(modelDecorated.PropertyDecorators.Count == 0);
        }

        [Test]
        public void InitClassDynamic_WithModelDecoratorsFilteredOut_ReturnDecoratorsForClassAndPropertiesFiltered()
        {
            var excludeFilter = new FilterSetting(false);
            var decorationDefinition = ModelDecorators.Init<ClassWithDynamicProperty, Decorators>(initDecorators, excludeFilter);
            var modelDecorated = decorationDefinition(classWithDynamicProperty);

            Assert.IsTrue(modelDecorated.Decorators.Enabled == false);
            Assert.IsTrue(modelDecorated.Decorators.Visible == false);
            Assert.IsTrue(modelDecorated.PropertyDecorators.Count == 2);
            Assert.IsTrue(modelDecorated.PropertyDecorators.ContainsKey("ADynamicPrioperty"));
        }

    }
}