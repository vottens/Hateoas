using System.Net.Http;

namespace FluentHateoas.Handling
{
    public static class ResponseHelper
    {
        public static HttpResponseMessage Ok(HttpRequestMessage request, object model, System.Collections.Generic.IEnumerable<IHateoasLink> links, System.Collections.Generic.IEnumerable<IHateoasCommand> commands)
        {
            return CreateResponse(request, CreateHateoasResponse(model, links, commands), System.Net.HttpStatusCode.OK);
        }

        private static HateOasResponse CreateHateoasResponse(object model, System.Collections.Generic.IEnumerable<IHateoasLink> links, System.Collections.Generic.IEnumerable<IHateoasCommand> commands)
        {
            return new HateOasResponse
            {
                Data = model,
                Links = links,
                Commands = commands
            };
        }

        private static HttpResponseMessage CreateResponse(HttpRequestMessage request, HateOasResponse response, System.Net.HttpStatusCode statusCode)
        {
            return request.CreateResponse(statusCode, response);
        }
    }
}