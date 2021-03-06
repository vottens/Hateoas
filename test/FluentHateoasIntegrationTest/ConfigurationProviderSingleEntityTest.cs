﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Web.Http.Dependencies;
using FluentAssertions;
using FluentHateoas.Builder.Handlers;
using FluentHateoas.Handling;
using FluentHateoas.Registration;
using FluentHateoasTest.Assets.Controllers;
using FluentHateoasTest.Assets.Model;
using FluentHateoasTest.Assets.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FluetHateoasIntegrationTest
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ConfigurationProviderSingleEntityTest
    {
        private Mock<IAuthorizationProvider> _authorizationProvider;
        private Mock<IDependencyResolver> _dependencyResolverMock;
        private Mock<IPersonProvider> _personProvider;

        protected IConfigurationProvider ConfigurationProvider;
        protected HateoasContainer Container;
        protected ILinkFactory LinkFactory;

        protected Person Entity;

        [TestInitialize]
        public void Initialize()
        {
            _authorizationProvider = new Mock<IAuthorizationProvider>();
            _dependencyResolverMock = new Mock<IDependencyResolver>();
            _personProvider = new Mock<IPersonProvider>();

            var linkBuilderFactory = new LinkBuilderFactory();
            _authorizationProvider.Setup(p => p.IsAuthorized(It.IsAny<MethodInfo>())).Returns(true);
            _dependencyResolverMock.Setup(p => p.GetService(It.IsAny<Type>())).Returns(_personProvider.Object);

            var idFromExpressionProcessor = new IdFromExpressionProcessor(_dependencyResolverMock.Object);
            var argumentsDefinitionsProcessor = new ArgumentDefinitionsProcessor();
            var templateArgumentsProcessor = new TemplateArgumentsProcessor();

            var configurationMock = new Mock<IHttpConfiguration>(MockBehavior.Strict);
            var properties = new ConcurrentDictionary<object, object>();
            configurationMock.SetupGet(c => c.Properties).Returns(() => properties);

            Container = HateoasContainerFactory.Create(configurationMock.Object);
            LinkFactory = new LinkFactory(
                linkBuilderFactory,
                _authorizationProvider.Object,
                idFromExpressionProcessor,
                argumentsDefinitionsProcessor,
                templateArgumentsProcessor);
            var linksForFuncProvider = new ConfigurationProviderGetLinksForFuncProvider(new InMemoryCache<int, MethodInfo>());
            ConfigurationProvider = new ConfigurationProvider(configurationMock.Object, LinkFactory, linksForFuncProvider, new InMemoryCache<Type, Func<ConfigurationProvider, object, IEnumerable<IHateoasLink>>>());

            Entity = new Person()
            {
                Id = Guid.Parse("ae213c3e-9ce8-489f-a5ff-5422b55bba44"),
                HouseId = Guid.Parse("a7a52c3d-a5ef-47ff-97db-176f4b2609e4"),
            };
        }

        [TestMethod]
        public void GetDefault()
        {
            Container
                .Register<Person>("list")
                .Get<PersonController>();

            var link = GetLink();

            link.Relation.Should().Be("list");
            link.LinkPath.Should().Be("/api/person");
            link.Template.Should().BeNull();
            link.Command.Should().BeNull();
        }

        [TestMethod]
        public void GetWithMethod()
        {
            Container
                .Register<Person>("get-parents", p => p.Id)
                .Get<PersonController>(p => p.GetParents);

            var link = GetLink();

            link.Relation.Should().Be("get-parents");
            link.LinkPath.Should().Be("/api/person/ae213c3e-9ce8-489f-a5ff-5422b55bba44/parents");
            link.Template.Should().BeNull();
            link.Command.Should().BeNull();
        }

        [TestMethod]
        public void GetWithMultipleId()
        {
            Container
                .Register<Person>("get-house", p => p.Id, p => p.HouseId)
                .Get<PersonController>();

            var link = GetLink();

            link.Relation.Should().Be("get-house");
            link.LinkPath.Should().Be("/api/person/ae213c3e-9ce8-489f-a5ff-5422b55bba44/house/a7a52c3d-a5ef-47ff-97db-176f4b2609e4");
            link.Template.Should().BeNull();
            link.Command.Should().BeNull();
        }

        [TestMethod]
        [Ignore]
        public void GetByIdWithDefinedAction()
        {
            // todo: is this even possible with overloads??
            //Container
            //    .Register<Person>("get-by-id-with-action", p => p.Id)
            //    .Get<PersonController>(p => p.Get);

            var link = GetLink();

            link.Relation.Should().Be("get-by-id-with-action");
            link.LinkPath.Should().Be("/api/person/ae213c3e-9ce8-489f-a5ff-5422b55bba44/house/a7a52c3d-a5ef-47ff-97db-176f4b2609e4");
            link.Template.Should().BeNull();
            link.Command.Should().BeNull();
        }

        [TestMethod]
        public void GetByIdAsTemplate()
        {
            Container
                .Register<Person>("get-by-id-template", p => p.Id)
                .Get<PersonController>()
                .AsTemplate();

            var link = GetLink();

            link.Relation.Should().Be("get-by-id-template");
            link.LinkPath.Should().BeNull();
            link.Template.Should().Be("/api/person/{id}");
            link.Command.Should().BeNull();
        }

        [TestMethod]
        public void GetByIdWithPartialTemplate()
        {
            Container
                .Register<Person>("get-houses", p => p.Id)
                .Get<PersonController>()
                .AsTemplate(p => p.HouseId);

            var link = GetLink();

            link.Relation.Should().Be("get-houses");
            link.LinkPath.Should().BeNull();
            link.Template.Should().Be("/api/person/ae213c3e-9ce8-489f-a5ff-5422b55bba44/house/{houseId}");
            link.Command.Should().BeNull();
        }

        [TestMethod]
        public void GetByAlternateId()
        {
            var result = Guid.Parse("5d083357-58d2-49e4-9b14-91a35109583c");
            _personProvider.Setup(p => p.GetNextId(It.IsAny<object>())).Returns(result);

            Container
                .Register<Person>("next")
                .Get<PersonController>()
                .IdFrom<IPersonProvider>((provider, person) => provider.GetNextId(person));

            var link = GetLink();

            link.Relation.Should().Be("next");
            link.LinkPath.Should().Be("/api/person/5d083357-58d2-49e4-9b14-91a35109583c");
            link.Template.Should().BeNull();
            link.Command.Should().BeNull();
        }

        [TestMethod]
        public void GetByCondition()
        {
            var result = Guid.Parse("5d083357-58d2-49e4-9b14-91a35109583c");
            _personProvider.Setup(p => p.GetNextId(It.IsAny<object>())).Returns(result);

            Container
                .Register<Person>("next")
                .Get<PersonController>()
                .When<IPersonProvider>((provider, person) => provider.HasNextId(person))
                .IdFrom<IPersonProvider>((provider, person) => provider.GetNextId(person));

            var link = GetLink();

            link.Relation.Should().Be("next");
            link.LinkPath.Should().Be("/api/person/5d083357-58d2-49e4-9b14-91a35109583c");
            link.Template.Should().BeNull();
            link.Command.Should().BeNull();
        }


        public IHateoasLink GetLink()
        {
            var links = ConfigurationProvider.GetLinksFor(Entity).ToList();

            links.Count().Should().Be(1);

            return links.Single();
        }
    }
}