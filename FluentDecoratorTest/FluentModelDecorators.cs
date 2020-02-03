using FluentDecorator;
using FluentDecoratorTest.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FluentDecoratorTest
{
    public class FluentModelDecorators
    {
        private SimpleClass simpleClass { get; set; }
        private ClassWithMember classWithMember { get; set; }
        private ClassWithCollection classWithCollection { get; set; }
        private ModelDecorators.WithDecorators<ClassWithMember, IModelDecorators<ClassWithMember, Decorators>> decorationDefinition { get; set; }
        private ModelDecorators.WithDecorators<ClassWithCollection, IModelDecorators<ClassWithCollection, Decorators>> decorationCollectionDefinition { get; set; }

        [SetUp]
        public void Setup()
        {
            simpleClass = new SimpleClass() { Age = 62, Name = "Me" };
            classWithMember = new ClassWithMember() { SimpleClass = simpleClass, CreatedOn = DateTime.Now };
            classWithCollection = new ClassWithCollection() { Id = Guid.NewGuid(), SimpleClasses = new List<SimpleClass>(new[] { simpleClass })};
            decorationDefinition = ModelDecorators.Init<ClassWithMember, Decorators>();
            decorationCollectionDefinition = ModelDecorators.Init<ClassWithCollection, Decorators>();
        }

        [Test]
        public void ApplyActionOnModel_WithModelDecorated_ReturnCorrectDecorations()
        {
            decorationDefinition = decorationDefinition.OnModel(dec => dec.Visible = true);
            var modelDecorated = decorationDefinition(classWithMember);

            Assert.IsTrue(modelDecorated.Decorators.Visible == true);
        }

        [Test]
        public void ApplyActionOnProperty_WithModelDecorated_ReturnCorrectDecorations()
        {
            decorationDefinition = decorationDefinition.OnModelProp(m => m.CreatedOn, dec => dec.Visible = true);
            var modelWithDecorators = decorationDefinition(classWithMember);

            Assert.IsTrue(modelWithDecorators.GetFor(m => m.CreatedOn).Visible == true);
        }

        [Test]
        public void ApplyActionOnDescendantProperty_WithModelDecorated_ReturnCorrectDecorations()
        {
            decorationDefinition = decorationDefinition.OnModelProp(m => m.SimpleClass.Age, dec => dec.Visible = true);
            var modelWithDecorators = decorationDefinition(classWithMember);

            Assert.IsTrue(modelWithDecorators.GetFor(m => m.SimpleClass.Age).Visible == true);
        }

        [Test]
        public void ApplyActionWithPredicateOnDescendantProperty_WithModelDecorated_andFalselyPredicate_ReturnCorrectDecorations()
        {
            decorationDefinition = decorationDefinition.OnModelPropWhen(m => m.SimpleClass.Age > 100, m => m.SimpleClass.Age, dec => dec.Visible = true);
            var modelWithDecorators = decorationDefinition(classWithMember);

            Assert.IsTrue(modelWithDecorators.GetFor(m => m.SimpleClass.Age).Visible == false);
        }

        [Test]
        public void ApplyActionWithPredicateOnDescendantProperty_WithModelDecorated_andTruelyPredicate_ReturnCorrectDecorations()
        {
            decorationDefinition = decorationDefinition.OnModelPropWhen(m => m.SimpleClass.Age > 10, m => m.SimpleClass.Age, dec => dec.Visible = true);
            var modelWithDecorators = decorationDefinition(classWithMember);

            Assert.IsTrue(modelWithDecorators.GetFor(m => m.SimpleClass.Age).Visible == true);
        }


        [Test]
        public void ApplyActionWithPredicateOnModelProperty_WithModelDecorated_andFalselyPredicate_ReturnCorrectDecorations()
        {
            decorationDefinition = decorationDefinition.OnModelPropWhen(m => m.CreatedOn > DateTime.Now.AddDays(1), m => m.CreatedOn, dec => dec.Visible = true);
            var modelWithDecorators = decorationDefinition(classWithMember);

            Assert.IsTrue(modelWithDecorators.GetFor(m => m.CreatedOn).Visible == false);
        }

        [Test]
        public void ApplyActionWithPredicateOnModelProperty_WithModelDecorated_andTruelyPredicate_ReturnCorrectDecorations()
        {
            decorationDefinition =  decorationDefinition.OnModelPropWhen(m => m.CreatedOn > DateTime.Now.AddDays(-1), m => m.CreatedOn, dec => dec.Visible = true);
            var modelWithDecorators = decorationDefinition(classWithMember);

            Assert.IsTrue(modelWithDecorators.GetFor(m => m.CreatedOn).Visible == true);
        }


        [Test]
        public void ApplyActionWithIfElsePredicateOnModelProperty_WithModelDecorated_andFalselyPredicate_ReturnCorrectDecorations()
        {
            decorationDefinition = decorationDefinition.OnModelPropIfThenElse(m => m.CreatedOn > DateTime.Now.AddDays(+1), m => m.CreatedOn, dec => dec.Visible = true, dec => dec.Visible = false);
            var modelWithDecorators = decorationDefinition(classWithMember);

            Assert.IsTrue(modelWithDecorators.GetFor(m => m.CreatedOn).Visible == false);
        }

        [Test]
        public void ApplyActionWithIfElsePredicateOnModelProperty_WithModelDecorated_andTruelyPredicate_ReturnCorrectDecorations()
        {
            decorationDefinition = decorationDefinition.OnModelPropIfThenElse(m => m.CreatedOn > DateTime.Now.AddDays(-1), m => m.CreatedOn, dec => dec.Visible = true, dec => dec.Visible = false);
            var modelWithDecorators = decorationDefinition(classWithMember);

            Assert.IsTrue(modelWithDecorators.GetFor(m => m.CreatedOn).Visible == true);
        }

        [Test]
        public void ApplyActionWithIfElsePredicateOnDescendantProperty_WithModelDecorated_andTruelyPredicate_ReturnCorrectDecorations()
        {
            decorationDefinition = decorationDefinition.OnModelPropIfThenElse(m => m.SimpleClass.Age > 10, m => m.SimpleClass.Age, dec => dec.Visible = true, dec => dec.Visible = false);
            var modelWithDecorators = decorationDefinition(classWithMember);

            Assert.IsTrue(modelWithDecorators.GetFor(m => m.SimpleClass.Age).Visible == true);
        }

        [Test]
        public void ApplyActionWithIfElsePredicateOnDescendantProperty_WithModelDecorated_andFalselyPredicate_ReturnCorrectDecorations()
        {
            decorationDefinition = decorationDefinition.OnModelPropIfThenElse(m => m.SimpleClass.Age > 100, m => m.SimpleClass.Age, dec => dec.Visible = true, dec => dec.Visible = false);
            var modelWithDecorators = decorationDefinition(classWithMember);

            Assert.IsTrue(modelWithDecorators.GetFor(m => m.SimpleClass.Age).Visible == false);
        }

        [Test]
        public void ApplyActionOnIEnumerableProperty_WithModelDecorated_ReturnInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(delegate
            {
                decorationCollectionDefinition = decorationCollectionDefinition.OnModelProp(m => m.SimpleClasses, dec => dec.Visible = true);
                var modelWithDecorators = decorationCollectionDefinition(classWithCollection);
                var ret = modelWithDecorators.GetFor(m => m.SimpleClasses).Visible;
            });
        }


    }
}