﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Http;
using FluentAssertions;
using FluentHateoasTest.Assets.Model;

namespace FluentHateoasTest.Assets.Controllers
{
    public class PersonController : ApiController
    {
        public IEnumerable<Person> Get()
        {
            return new List<Person>();
        }

        public Person Get(Guid id)
        {
            return new Person { Id = id };
        }

        [Route("{id}/father")]
        public Person GetFather(Guid id)
        {
            return new Person { Id = Guid.Parse("0CFA46CC-116D-45F0-B57B-4C5586130072") };
        }

        [Route("{id}/parents")]
        public IEnumerable<Person> GetParents()
        {
            return new[]
            {
                new Person {Id = Guid.Parse("0CFA46CC-116D-45F0-B57B-4C5586130072")},
                new Person {Id = Guid.Parse("F6D0CF9E-5153-4218-8A22-8E71C5AC5A4A")}
            };
        }

        public Person Post(CreatePersonRequest request)
        {
            return new Person { Id = Guid.NewGuid() };
        }

        [HttpPost]
        [Route("child")]
        public Person AddChild(CreatePersonRequest request)
        {
            return new Person { Id = Guid.NewGuid() };
        }

        public Person Put(UpdatePersonRequest request)
        {
            return new Person { Id = request.Id };
        }

        public void Delete(Guid id) { }
    }
}