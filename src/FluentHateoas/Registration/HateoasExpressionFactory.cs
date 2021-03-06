﻿namespace FluentHateoas.Registration
{
    public static class HateoasExpressionFactory
    {
        // public static HateoasExpression<TModel> Create<TModel>(HateoasRegistration<TModel> registration)
        // {
        //     return new HateoasExpression<TModel>(registration);
        // }

        public static ExpressionBuilder<TModel> CreateBuilder<TModel>(HateoasRegistration<TModel> registration)
        {
            return new ExpressionBuilder<TModel>(registration);
        }
    }
}