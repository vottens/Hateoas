﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Net.Http;
using System.Web.Http.Dependencies;
using FluentAssertions;
using FluentHateoas.Builder.Handlers;
using FluentHateoas.Builder.Model;
using FluentHateoas.Handling;
using FluentHateoas.Interfaces;
using FluentHateoasTest.Assets.Controllers;
using FluentHateoasTest.Assets.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FluentHateoasTest.Handling
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ArgumentHandlerTest : BaseHandlerTest<ArgumentHandler>
    {
        private Mock<IArgumentProcessor> _idFromExpressionProcessorMock;
        private Mock<IArgumentProcessor> _argumentsDefinitionsProcessorMock;
        private Mock<IArgumentProcessor> _templateArgumentsProcessorMock;

        [TestInitialize]
        public void Initialize()
        {
            _idFromExpressionProcessorMock = new Mock<IArgumentProcessor>(MockBehavior.Strict);
            _argumentsDefinitionsProcessorMock = new Mock<IArgumentProcessor>(MockBehavior.Strict);
            _templateArgumentsProcessorMock = new Mock<IArgumentProcessor>(MockBehavior.Strict);

            Handler = new ArgumentHandler(
                _idFromExpressionProcessorMock.Object,
                _argumentsDefinitionsProcessorMock.Object,
                _templateArgumentsProcessorMock.Object);
        }

        [TestMethod]
        public void CanProcessShouldBeTrueWhenAllPrerequisitesHaveBeenMet()
        {
            // arrange
            var registrationMock = new Mock<IHateoasRegistration<Person>>(MockBehavior.Strict);

            _argumentsDefinitionsProcessorMock.Setup(p => p.CanProcess(It.IsAny<IHateoasRegistration<Person>>(), It.IsAny<ILinkBuilder>())).Returns(true);
            _templateArgumentsProcessorMock.Setup(p => p.CanProcess(It.IsAny<IHateoasRegistration<Person>>(), It.IsAny<ILinkBuilder>())).Returns(true);
            _idFromExpressionProcessorMock.Setup(p => p.CanProcess(It.IsAny<IHateoasRegistration<Person>>(), It.IsAny<ILinkBuilder>())).Returns(true);

            // act & assert
            Handler.CanProcess(registrationMock.Object, LinkBuilder).Should().BeTrue();
        }

        //[TestMethod]
        //public void ProcessInternalShouldAddIdToLinkBuilderUsingIdFromExpression()
        //{
        //    // arrange
        //    var expressionMock = new Mock<IHateoasExpression<Person>>(MockBehavior.Strict);
        //    expressionMock.SetupGet(e => e.TemplateParameters).Returns(default(IEnumerable<LambdaExpression>));
        //    Expression<Func<object, Person, object>> idFromExpression = (provider, person) => person.Id;
        //    expressionMock.SetupGet(e => e.IdFromExpression).Returns(idFromExpression);

        //    var registrationMock = new Mock<IHateoasRegistration<Person>>(MockBehavior.Strict);
        //    registrationMock.SetupGet(r => r.ArgumentDefinitions).Returns(default(Expression<Func<Person, object>>[]));
        //    registrationMock.SetupGet(r => r.Expression).Returns(expressionMock.Object);

        //    var registrationMockUntyped = registrationMock.As<IHateoasRegistration>();
        //    registrationMockUntyped.SetupGet(r => r.Expression).Returns(expressionMock.Object);

        //    var argumentsMock = new Mock<IDictionary<string, Argument>>(MockBehavior.Strict);
        //    argumentsMock.Setup(a => a.Add(It.IsAny<string>(), It.IsAny<Argument>()));
        //    LinkBuilderMock.SetupGet(lb => lb.Arguments).Returns(argumentsMock.Object);

        //    // act
        //    Handler.ProcessInternal(registrationMock.Object, LinkBuilder, Person);

        //    // assert
        //    argumentsMock.Verify(a => a.Add("id", It.Is<Argument>(arg => Person.Id.Equals(arg.Value))), Times.Once);
        //}

        //[TestMethod]
        //public void ProcessInternalShouldAddIdToLinkBuilderUsingArgumentDefinitions()
        //{
        //    // arrange
        //    var expressionMock = new Mock<IHateoasExpression<Person>>(MockBehavior.Strict);
        //    expressionMock.SetupGet(e => e.TemplateParameters).Returns(default(IEnumerable<LambdaExpression>));
        //    expressionMock.SetupGet(e => e.IdFromExpression).Returns(default(Expression<Func<object, Person, object>>));

        //    var registrationMock = new Mock<IHateoasRegistration<Person>>(MockBehavior.Strict);
        //    registrationMock.SetupGet(r => r.ArgumentDefinitions).Returns(new Expression<Func<Person, object>>[] { p => p.Id });
        //    registrationMock.SetupGet(r => r.Expression).Returns(expressionMock.Object);

        //    var registrationMockUntyped = registrationMock.As<IHateoasRegistration>();
        //    registrationMockUntyped.SetupGet(r => r.Expression).Returns(expressionMock.Object);

        //    var argumentsMock = new Mock<IDictionary<string, Argument>>(MockBehavior.Strict);
        //    argumentsMock.Setup(a => a.Add(It.IsAny<string>(), It.IsAny<Argument>()));
        //    LinkBuilderMock.SetupGet(lb => lb.Arguments).Returns(argumentsMock.Object);

        //    // act
        //    Handler.ProcessInternal(registrationMock.Object, LinkBuilder, Person);

        //    // assert
        //    argumentsMock.Verify(a => a.Add("id", It.Is<Argument>(arg => Person.Id.Equals(arg.Value))), Times.Once);
        //}

        //[TestMethod]
        //public void ProcessInternalShouldAddTemplateArgumentsToLinkBuilder()
        //{
        //    // arrange
        //    var expressionMock = new Mock<IHateoasExpression<Person>>(MockBehavior.Strict);
        //    var templateParameters = new Expression<Func<Person, object>>[] { p => p.Id };
        //    expressionMock.SetupGet(e => e.TemplateParameters).Returns(templateParameters);
        //    expressionMock.SetupGet(e => e.IdFromExpression).Returns(default(Expression<Func<object, Person, object>>));

        //    var registrationMock = new Mock<IHateoasRegistration<Person>>(MockBehavior.Strict);
        //    registrationMock.SetupGet(r => r.ArgumentDefinitions).Returns(new Expression<Func<Person, object>>[] { p => p.Id });
        //    registrationMock.SetupGet(r => r.Expression).Returns(expressionMock.Object);

        //    var registrationMockUntyped = registrationMock.As<IHateoasRegistration>();
        //    registrationMockUntyped.SetupGet(r => r.Expression).Returns(expressionMock.Object);
        //    registrationMockUntyped.SetupGet(r => r.IsCollection).Returns(false);

        //    var linkBuilderArgsDic = new Dictionary<string, Argument>();
        //    var argumentsMock = new Mock<IDictionary<string, Argument>>(MockBehavior.Strict);
        //    argumentsMock.Setup(a => a.Add(It.IsAny<string>(), It.IsAny<Argument>())).Callback((string key, Argument value) => linkBuilderArgsDic.Add(key, value));
        //    argumentsMock.Setup(a => a.GetEnumerator()).Returns(() => linkBuilderArgsDic.GetEnumerator());
        //    LinkBuilderMock.SetupGet(lb => lb.Arguments).Returns(argumentsMock.Object);

        //    // act
        //    Handler.ProcessInternal(registrationMock.Object, LinkBuilder, Person);

        //    // assert
        //    argumentsMock.Verify(a => a.Add("id", It.Is<Argument>(arg => Person.Id.Equals(arg.Value))), Times.Once);
        //    argumentsMock.Verify(a => a.Add("personId", It.Is<Argument>(arg => "{personId}".Equals(arg.Value))), Times.Once);
        //}

        //[TestMethod]
        //public void ProcessInternalShouldAddTemplateArgumentsToLinkBuilderWithoutExplicitIdArgument()
        //{
        //    // arrange
        //    var expressionMock = new Mock<IHateoasExpression<Person>>(MockBehavior.Strict);
        //    var templateParameters = new Expression<Func<Person, object>>[] { p => p.Id };
        //    expressionMock.SetupGet(e => e.TemplateParameters).Returns(templateParameters);
        //    expressionMock.SetupGet(e => e.IdFromExpression).Returns(default(Expression<Func<object, Person, object>>));

        //    var registrationMock = new Mock<IHateoasRegistration<Person>>(MockBehavior.Strict);
        //    registrationMock.SetupGet(r => r.ArgumentDefinitions).Returns(default(Expression<Func<Person, object>>[]));
        //    registrationMock.SetupGet(r => r.Expression).Returns(expressionMock.Object);

        //    var registrationMockUntyped = registrationMock.As<IHateoasRegistration>();
        //    registrationMockUntyped.SetupGet(r => r.Expression).Returns(expressionMock.Object);

        //    var linkBuilderArgsDic = new Dictionary<string, Argument>();
        //    var argumentsMock = new Mock<IDictionary<string, Argument>>(MockBehavior.Strict);
        //    argumentsMock.Setup(a => a.Add(It.IsAny<string>(), It.IsAny<Argument>())).Callback((string key, Argument value) => linkBuilderArgsDic.Add(key, value));
        //    argumentsMock.Setup(a => a.GetEnumerator()).Returns(linkBuilderArgsDic.GetEnumerator());
        //    LinkBuilderMock.SetupGet(lb => lb.Arguments).Returns(argumentsMock.Object);

        //    // act
        //    Handler.ProcessInternal(registrationMock.Object, LinkBuilder, Person);

        //    // assert
        //    argumentsMock.Verify(a => a.Add("id", It.Is<Argument>(arg => "{id}".Equals(arg.Value))), Times.Once);
        //}

        [TestMethod]
        public void CanProcessShouldReturnFalseIfNoProcessorCanProcess()
        {
            // arrange
            var registrationMock = new Mock<IHateoasRegistration<Person>>();

            _argumentsDefinitionsProcessorMock.Setup(p => p.CanProcess(It.IsAny<IHateoasRegistration<Person>>(), It.IsAny<ILinkBuilder>())).Returns(false);
            _templateArgumentsProcessorMock.Setup(p => p.CanProcess(It.IsAny<IHateoasRegistration<Person>>(), It.IsAny<ILinkBuilder>())).Returns(false);
            _idFromExpressionProcessorMock.Setup(p => p.CanProcess(It.IsAny<IHateoasRegistration<Person>>(), It.IsAny<ILinkBuilder>())).Returns(false);

            // act & assert
            Handler.CanProcess(registrationMock.Object, null).Should().BeFalse();
        }
    }
}