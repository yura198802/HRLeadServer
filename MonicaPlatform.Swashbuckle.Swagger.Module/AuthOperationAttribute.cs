﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MonicaPlatform.Swashbuckle.Swagger.Module
{
	public class AuthOperationAttribute : IOperationFilter
	{

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filterDescriptor = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var isAuthorized = filterDescriptor.Select(filterInfo => filterInfo.Filter).Any(filter => filter is AuthorizeFilter);
            var allowAnonymous = filterDescriptor.Select(filterInfo => filterInfo.Filter).Any(filter => filter is IAllowAnonymousFilter);

            if (isAuthorized && !allowAnonymous)
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<OpenApiParameter>();

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Description = "JWT access token",
                    Required = true
                });

                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                operation.Security =new List<OpenApiSecurityRequirement>(new List<OpenApiSecurityRequirement>());

            }
		}
    }
}
