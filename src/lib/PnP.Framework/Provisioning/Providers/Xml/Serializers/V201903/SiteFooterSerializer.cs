﻿using PnP.Framework.Extensions;
using PnP.Framework.Provisioning.Model;
using PnP.Framework.Provisioning.Providers.Xml.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PnP.Framework.Provisioning.Providers.Xml.Serializers
{
    /// <summary>
    /// Class to serialize/deserialize the Site Footer
    /// </summary>
    [TemplateSchemaSerializer(SerializationSequence = 820, DeserializationSequence = 820,
        MinimalSupportedSchemaVersion = XMLPnPSchemaVersion.V201903,
        Scope = SerializerScope.ProvisioningTemplate)]
    internal class SiteFooterSerializer : PnPBaseSchemaSerializer<SiteFooter>
    {
        public override void Deserialize(object persistence, ProvisioningTemplate template)
        {
            var siteFooter = persistence.GetPublicInstancePropertyValue("Footer");

            if (siteFooter != null)
            {
                var expressions = new Dictionary<Expression<Func<SiteFooter, Object>>, IResolver>
                {
                    { f => f.FooterLinks, new SiteFooterLinkFromSchemaToModelTypeResolver() }
                };

                template.Footer = new SiteFooter();
                PnPObjectsMapper.MapProperties(siteFooter, template.Footer, expressions, true);
            }
        }

        public override void Serialize(ProvisioningTemplate template, object persistence)
        {
            if (template.Footer != null)
            {
                var siteFooterTypeName = $"{PnPSerializationScope.Current?.BaseSchemaNamespace}.Footer, {PnPSerializationScope.Current?.BaseSchemaAssemblyName}";
                var siteFooterType = Type.GetType(siteFooterTypeName, true);
                var target = Activator.CreateInstance(siteFooterType, true);

                var resolvers = new Dictionary<String, IResolver>
                {
                    {
                        $"{siteFooterType}.FooterLinks",
                        new SiteFooterLinkFromModelToSchemaTypeResolver()
                    }
                };

                PnPObjectsMapper.MapProperties(template.Footer, target, resolvers, recursive: true);

                persistence.GetPublicInstanceProperty("Footer").SetValue(persistence, target);
            }
        }
    }
}
