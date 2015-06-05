using System;
using System.Reflection;

namespace JCI.CAM.SiteProvisioningWeb.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}