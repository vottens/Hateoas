﻿// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedParameter.Global

namespace FluentHateoas.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Web.Http.Controllers;

    using Interfaces;

    public class ExpressionBuilder<TModel> : 
        IExpressionBuilder<TModel>,
        IGetExpressionBuilder<TModel>,
        IPostExpressionBuilder<TModel>,
        IPutExpressionBuilder<TModel>,
        IDeleteExpressionBuilder<TModel>,
        IWhenExpressionBuilder<TModel>,
        IWithExpressionBuilder<TModel>,
        IWithCommandExpressionBuilder<TModel>
    {
        private readonly HateoasExpression<TModel> _expression;
        private readonly HateoasRegistration<TModel> _registration;

        public ExpressionBuilder(HateoasRegistration<TModel> registration)
        {
            _registration = registration;
            _registration.Expression = _expression = HateoasExpression<TModel>.Create(registration);
        }

        public IGetExpressionBuilder<TModel> Get<TController>(Expression<Func<TController, Func<IEnumerable<TModel>>>> methodSelector) where TController : IHttpController
        {
            _expression.SetMethod<TController>(HttpMethod.Get, methodSelector);
            _registration.Update();
            return this;
        }

        public IGetExpressionBuilder<TModel> Get<TController>(Expression<Func<TController, IEnumerable<TModel>>> methodSelector) where TController : IHttpController
        {
            _expression.SetMethod<TController>(HttpMethod.Get, methodSelector);
            _registration.Update();
            return this;
        }

        public IGetExpressionBuilder<TModel> Get<TController>(Expression<Func<TController, Func<Guid, TModel>>> methodSelector) where TController : IHttpController
        {
            _expression.SetMethod<TController>(HttpMethod.Get, methodSelector);
            _registration.Update();
            return this;
        }

        public IGetExpressionBuilder<TModel> Get<TController>(LambdaExpression methodSelector = null) where TController : IHttpController
        {
            _expression.SetMethod<TController>(HttpMethod.Get, methodSelector);
            _registration.Update();
            return this;
        }

        public IPostExpressionBuilder<TModel> Post<TController>(LambdaExpression methodSelector = null) where TController : IHttpController
        {
            _expression.SetMethod<TController>(HttpMethod.Post, null);
            _registration.Update();
            return this;
        }

        public IPutExpressionBuilder<TModel> Put<TController>(LambdaExpression methodSelector = null) where TController : IHttpController
        {
            _expression.SetMethod<TController>(HttpMethod.Put, methodSelector);
            _registration.Update();
            return this;
        }

        public IDeleteExpressionBuilder<TModel> Delete<TController>(LambdaExpression methodSelector = null) where TController : IHttpController
        {
            _expression.SetMethod<TController>(HttpMethod.Delete, methodSelector);
            _registration.Update();
            return this;
        }

        public ITemplateExpressionBuilder<TModel> AsCollection()
        {
            _expression.Collection = true;
            _registration.Update();
            return this;
        }

        IExpressionBuilderBase<TModel> ITemplateExpressionBuilder<TModel>.AsTemplate()
        {
            _expression.Template = true;
            _registration.Update();
            return this;
        }

        IExpressionBuilderBase<TModel> ITemplateExpressionBuilder<TModel>.AsTemplate(params Expression<Func<TModel, object>>[] args)
        {
            _expression.Template = true;
            _expression.TemplateParameters = args;
            _registration.Update();
            return this;
        }

        // TODO BL Add TProvider constraint(s)
        public IWhenExpressionBuilder<TModel> When<TProvider>(Expression<Func<TProvider, TModel, bool>> when)
        {
            _expression.WhenExpression = when;
            _registration.Update();
            return this;
        }

        // TODO BL Add TProvider constraint(s)
        public IWithExpressionBuilder<TModel> With<TProvider>(Expression<Func<TProvider, TModel, object>> with)
        {
            _expression.WithExpression = with;
            _registration.Update();
            return this;
        }

        // TODO BL Add TCommand constraint(s)
        public IWithCommandExpressionBuilder<TModel> WithCommand<TCommand>()
        {
            _expression.Command = typeof(TCommand);
            _registration.Update();
            return this;
        }

        // TODO BL Add TCommandFactory constraint(s)
        public IWithCommandExpressionBuilder<TModel> WithCommand<TCommandFactory>(Expression<Func<TCommandFactory, object>> commandFactory)
        {
            _expression.CommandFactory = commandFactory;
            _registration.Update();
            return this;
        }

        public IHateoasExpression<TModel> GetExpression()
        {
            return _expression;
        }
    }
}