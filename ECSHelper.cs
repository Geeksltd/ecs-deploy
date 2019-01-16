using Amazon.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECS_Deploy
{
    static class ECSHelper
    {
        internal static AmazonECSClient CreateClient() => new AmazonECSClient(Amazon.RegionEndpoint.GetBySystemName(DefaultSettings.General.Region));
    }
}
