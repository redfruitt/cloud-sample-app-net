﻿using System;
using DancingGoat.Areas.Admin;
using DancingGoat.Infrastructure;
using DancingGoat.Models;
using KenticoCloud.Delivery;
using System.Configuration;
using System.Web.Mvc;
using DancingGoat.InlineContentItemResolver;
using DancingGoat.Localization;

namespace DancingGoat.Controllers
{
    [SelfConfigActionFilter]
    public class ControllerBase : AsyncController
    {
        protected static readonly DeliveryClient baseClient = CreateDeliveryClient();
        public readonly IDeliveryClient client;

        public ControllerBase()
        {
            var currentCulture = CultureInfo.CurrentUICulture.Name;
            if (currentCulture.Equals(LanguageClient.DEFAULT_LANGUAGE, StringComparison.InvariantCultureIgnoreCase))
            {
                client = baseClient;
            }
            else
            {
                client = new LanguageClient(baseClient, currentCulture);
            }
        }
       

        public static DeliveryClient CreateDeliveryClient()
        {
            var previewToken = ConfigurationManager.AppSettings["PreviewToken"];
            Guid? projectId = AppSettingProvider.ProjectId ?? AppSettingProvider.DefaultProjectId;

            var clientInstance = 
                !string.IsNullOrEmpty(previewToken) ? 
                    new DeliveryClient(projectId.Value.ToString(), previewToken) : 
                    new DeliveryClient(projectId.Value.ToString());

            clientInstance.CodeFirstModelProvider.TypeProvider = new CustomTypeProvider();
            clientInstance.ContentLinkUrlResolver = new CustomContentLinkUrlResolver();
            clientInstance.InlineContentItemsProcessor.RegisterTypeResolver(new HostedVideoResolver());
            clientInstance.InlineContentItemsProcessor.RegisterTypeResolver(new TweetResolver());
            return clientInstance;
        }
    }
}

