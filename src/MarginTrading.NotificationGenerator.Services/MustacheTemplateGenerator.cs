﻿using System;
using System.IO;
using MarginTrading.NotificationGenerator.Core.Services;
using Microsoft.AspNetCore.Hosting;

namespace MarginTrading.NotificationGenerator.Services
{
    public class MustacheTemplateGenerator : ITemplateGenerator
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string _templatesFolder;

        public MustacheTemplateGenerator(IHostingEnvironment hostingEnvironment, string templatesFolder)
        {
            _hostingEnvironment = hostingEnvironment;
            _templatesFolder = templatesFolder;
        }

        public string Generate<T>(string templateName, T model)
        {
            var templatesFolder = Path.Combine(_hostingEnvironment.ContentRootPath, _templatesFolder);

            var path = Path.Combine(templatesFolder, templateName + ".mustache");

            try
            {
                //TODO: get template from blob
                return Nustache.Core.Render.FileToString(path, model);
            }
            catch (InvalidCastException)
            {
                Console.WriteLine($"Incorrect model was passed for template: {path}");
                throw;
            }
        }
    }
}