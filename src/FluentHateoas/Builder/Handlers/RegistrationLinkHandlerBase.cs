﻿using System;
using FluentHateoas.Handling;
using FluentHateoas.Interfaces;

namespace FluentHateoas.Builder.Handlers
{
    public abstract class RegistrationLinkHandlerBase : IRegistrationLinkHandler
    {
        private IRegistrationLinkHandler _successor;

        public virtual LinkBuilder Process<TModel>(IHateoasRegistration<TModel> definition, LinkBuilder resourceBuilder, TModel data)
        {
            return _successor != null && _successor.CanProcess(definition, resourceBuilder)
                ? _successor.Process(definition, resourceBuilder, data)
                : resourceBuilder;
        }

        public void SetSuccessor(IRegistrationLinkHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            _successor = handler;
        }

        public abstract bool CanProcess<TModel>(IHateoasRegistration<TModel> definition, LinkBuilder resourceBuilder);
    }
}