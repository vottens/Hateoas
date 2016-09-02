﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using FluentHateoas.Contracts;
using FluentHateoas.Interfaces;

namespace FluentHateoas.Registration
{
    public static class HttpConfigurationExtensions
    {
        /// <summary>
        /// Updates (or adds) the HATEOAS container
        /// </summary>
        /// <param name="configuration">HTTP configuration</param>
        /// <param name="hateoasConfiguration">HATEOAS configuration</param>
        public static void UpdateConfiguration(this HttpConfiguration configuration, IHateoasConfiguration hateoasConfiguration)
        {
            configuration.Properties.AddOrUpdate(
                typeof(IHateoasConfiguration), 
                hateoasConfiguration, 
                (oldValue, newValue) => hateoasConfiguration);
        }

        /// <summary>
        /// Adds an HATEOAS registration
        /// </summary>
        /// <param name="configuration">HTTP configuration</param>
        /// <param name="registration">HATEOAS registration</param>
        public static void AddRegistration(this HttpConfiguration configuration, IHateoasRegistration registration)
        {
            var definitions = configuration.GetRegistrationsFor(registration.Model);
            definitions.Add(registration);
        }

        /// <summary>
        /// Updates (or adds) an HATEOAS registration
        /// </summary>
        /// <param name="configuration">HTTP configuration</param>
        /// <param name="registration">HATEOAS registration</param>
        public static void UpdateRegistration(this HttpConfiguration configuration, IHateoasRegistration registration)
        {
            var definition = configuration.GetRegistrationFor(registration.Model, registration.Relation);
            if (definition == null)
            {
                configuration.AddRegistration(registration);
            }
            else
            {
                definition.Expression = registration.Expression;
            }
        }

        /// <summary>
        /// Returns the HATEOASregistration for a specific model and relation. Returns null when the combination of model and relation could not be found.
        /// </summary>
        /// <param name="configuration">HTTP configuration</param>
        /// <param name="model">The model to get an HATEOAS registration for</param>
        /// <param name="relation">Relation to the model</param>
        /// <returns>HATEOAS registration for the model and relation specified</returns>
        public static IHateoasRegistration GetRegistrationFor(this HttpConfiguration configuration, Type model, string relation)
        {
            var definitions = configuration.GetRegistrationsFor(model);
            var definition = definitions.SingleOrDefault(def => def.Model == model && def.Relation == relation);

            //if (definition == null)
            //{
            //    // TODO Maybe implement AddOrUpdate behavior?
            //    throw new HateoasException($"No registration found for model {model} and relation {relation}");
            //}

            return definition;
        }

        /// <summary>
        /// Returns the HATEOAS registrations for a specific model. Returns a (registered) empty list when the model could not be found.
        /// </summary>
        /// <param name="configuration">HTTP configuration</param>
        /// <param name="model">The model to get HATEOAS registrations for</param>
        /// <returns>HATEOAS registrations for the model specified</returns>
        public static List<IHateoasRegistration> GetRegistrationsFor<TModel>(this HttpConfiguration configuration)
        {
            return configuration.GetRegistrationsFor(typeof(TModel));
        }

        /// <summary>
        /// Returns the HATEOAS registrations for a specific model. Returns a (registered) empty list when the model could not be found.
        /// </summary>
        /// <param name="configuration">HTTP configuration</param>
        /// <param name="model">The model to get HATEOAS registrations for</param>
        /// <returns>HATEOAS registrations for the model specified</returns>
        public static List<IHateoasRegistration> GetRegistrationsFor(this HttpConfiguration configuration, Type model)
        {
            var linkedResourceDefinitions = configuration.GetRegistrations();
            List<IHateoasRegistration> definitions;

            if (linkedResourceDefinitions.TryGetValue(model, out definitions))
                return definitions;

            definitions = new List<IHateoasRegistration>();
            linkedResourceDefinitions.Add(model, definitions);

            return definitions;
        }

        /// <summary>
        /// Returns a dictionary holding all HATEOAS registrations.
        /// </summary>
        /// <param name="configuration">source paramater</param>
        /// <returns>Dictionary holding all HATEOAS registrations</returns>
        public static Dictionary<Type, List<IHateoasRegistration>> GetRegistrations(this HttpConfiguration configuration)
        {
            return (Dictionary<Type, List<IHateoasRegistration>>)configuration.Properties.GetOrAdd(
                typeof(Dictionary<Type, List<IHateoasRegistration>>),
                k => new Dictionary<Type, List<IHateoasRegistration>>()
            );
        }
    }
}