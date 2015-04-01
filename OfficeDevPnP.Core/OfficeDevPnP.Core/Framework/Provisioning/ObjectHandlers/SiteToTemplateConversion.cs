﻿using System;
using System.Web.Script.Serialization;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    internal class SiteToTemplateConversion
    {
        /// <summary>
        /// Actual implementation of extracting configuration from existing site.
        /// </summary>
        /// <param name="web"></param>
        /// <param name="baseTemplate"></param>
        /// <returns></returns>
        internal ProvisioningTemplate GetRemoteTemplate(Web web, ProvisioningTemplate baseTemplate, FileConnectorBase connector)
        {
            // Create empty object
            ProvisioningTemplate template = new ProvisioningTemplate();

            // Hookup connector
            template.Connector = connector;

            // Get Security
            template = new ObjectSiteSecurity().CreateEntities(web, template, baseTemplate);
            // Site Fields
            template = new ObjectField().CreateEntities(web, template, baseTemplate);
            // Content Types
            template = new ObjectContentType().CreateEntities(web, template, baseTemplate);
            // Get Lists 
            template = new ObjectListInstance().CreateEntities(web, template, baseTemplate);
            // Get custom actions
            template = new ObjectCustomActions().CreateEntities(web, template, baseTemplate);
            // Get features
            template = new ObjectFeatures().CreateEntities(web, template, baseTemplate);
            // Get composite look
            template = new ObjectComposedLook().CreateEntities(web, template, baseTemplate);
            // Get files
            template = new ObjectFiles().CreateEntities(web, template, baseTemplate);
            // Get Property Bag Entires
            template = new ObjectPropertyBagEntry().CreateEntities(web, template, baseTemplate);
            // In future we could just instantiate all objects which are inherited from object handler base dynamically 

            return template;
        }

                /// <summary>
        /// Actual implementation of extracting configuration from existing site.
        /// </summary>
        /// <param name="web"></param>
        /// <param name="baseTemplate"></param>
        /// <returns></returns>
        internal ProvisioningTemplate GetRemoteTemplate(Web web, ProvisioningTemplate baseTemplate)
        {
            return GetRemoteTemplate(web, baseTemplate, null);
        }

        internal ProvisioningTemplate GetRemoteTemplate(Web web)
        {
            // Load the base template which will be used for the comparison work
            ProvisioningTemplate baseTemplate = web.GetBaseTemplate();

            return GetRemoteTemplate(web, baseTemplate, null);
        }

        internal ProvisioningTemplate GetRemoteTemplate(Web web, FileConnectorBase connector)
        {
            // Load the base template which will be used for the comparison work
            ProvisioningTemplate baseTemplate = web.GetBaseTemplate();

            return GetRemoteTemplate(web, baseTemplate, connector);
        }

        /// <summary>
        /// Actual implementation of the apply templates
        /// </summary>
        /// <param name="web"></param>
        /// <param name="template"></param>
        internal void ApplyRemoteTemplate(Web web, ProvisioningTemplate template)
        {
            // Site Security
            new ObjectSiteSecurity().ProvisionObjects(web, template);

            // Features
            new ObjectFeatures().ProvisionObjects(web, template);

            // Site Fields
            new ObjectField().ProvisionObjects(web, template);

            // Content Types
            new ObjectContentType().ProvisionObjects(web, template);

            // Lists
            new ObjectListInstance().ProvisionObjects(web, template);

            // Files
            new ObjectFiles().ProvisionObjects(web, template);

            // Custom actions
            new ObjectCustomActions().ProvisionObjects(web, template);

            // Composite look 
            new ObjectComposedLook().ProvisionObjects(web, template);

            // Property Bag Entries
            new ObjectPropertyBagEntry().ProvisionObjects(web, template);

            // Extensibility Provider CallOut the last thing we do.
            new ObjectExtensibilityProviders().ProvisionObjects(web, template);

            web.SetPropertyBagValue("_PnP_ProvisioningTemplateId", template.ID != null ? template.ID : "");
            web.AddIndexedPropertyBagKey("_PnP_ProvisioningTemplateId");

            ProvisioningTemplateInfo info = new ProvisioningTemplateInfo();
            info.TemplateID = template.ID != null ? template.ID : "";
            info.Result = true;
            info.ProvisioningTime = DateTime.Now;

            var s = new JavaScriptSerializer();
            string jsonInfo = s.Serialize(info);

            web.SetPropertyBagValue("_PnP_ProvisioningTemplateInfo",jsonInfo);
        }
    }
}
