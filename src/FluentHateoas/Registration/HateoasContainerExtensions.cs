﻿using System;
using System.Collections;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using FluentHateoas.Contracts;

namespace FluentHateoas.Registration
{
    using FluentHateoas.Helpers;
    using FluentHateoas.Interfaces;

    public static class HateoasContainerExtensions
    {
        public static IExpressionBuilder<TModel> Register<TModel>(this IHateoasContainer container, string relation = null, Expression<Func<TModel, object>> identityDefinition = null)
        {
            if (typeof(TModel).GetInterfaces().Contains(typeof(IEnumerable)))
                throw new ArgumentException("Cannot register collections; use .AsCollection() instead");

            var registration = new HateoasRegistration<TModel>(relation, identityDefinition);
            container.Registrations.Add(registration);
            return HateoasExpressionFactory.CreateBuilder(registration);
        }

        public static void Configure(this IHateoasContainer source, dynamic vars)
        {
            var container = source as HateoasContainer;

            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.Configuration.Extend((ExpandoObject)DynamicObjectHelper.ToExpandoObject(vars));
        }
    }
}