﻿using System.Collections;
using FluentHateoas.Builder.Factories;
using FluentHateoas.Handling;
using FluentHateoas.Handling.Handlers;
using FluentHateoas.Interfaces;

namespace FluentHateoas.Builder.Handlers
{
    public class CommandHandler : RegistrationLinkHandlerBase
    {
        public override LinkBuilder Process<TModel>(IHateoasRegistration<TModel> registration, LinkBuilder linkBuilder, TModel data)
        {
            linkBuilder.Command = registration.Expression.CreateCommand(registration.Relation + "-command");

            return base.Process(registration, linkBuilder, data);
        }

        public override bool CanProcess<TModel>(IHateoasRegistration<TModel> registration, LinkBuilder linkBuilder)
        {
            return registration.Expression.Command != null;
        }
    }
}