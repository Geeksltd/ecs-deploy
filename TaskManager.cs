using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.ECS;
using Amazon.ECS.Model;

namespace ECS_Deploy
{
    class TaskManager : IDisposable
    {
        TaskManager()
        {
            ECSClient = new AmazonECSClient(Amazon.RegionEndpoint.GetBySystemName(DefaultSettings.General.Region));
        }

        string ServiceName => DefaultSettings.General.ServiceName;
        string TaskDefenitionFamily => DefaultSettings.TaskDefenition.Family;
        string DockerImage => DefaultSettings.Container.Image;
        AmazonECSClient ECSClient;

        internal static Task<TaskDefinition> RegisterTaskDefenition()
        {
            using (var taskManager = new TaskManager())
                return taskManager.DoRegisterTaskDefenition();
        }

        private async Task<TaskDefinition> DoRegisterTaskDefenition()
        {
            var request = new RegisterTaskDefinitionRequest
            {
                Family = TaskDefenitionFamily,
                RequiresCompatibilities = DefaultSettings.TaskDefenition.RequiresCompatibilities.Split(",").ToList(),
                ContainerDefinitions = new List<ContainerDefinition>
                {
                    new ContainerDefinition
                    {
                        Name = ServiceName,
                        Image = DockerImage,
                        Memory = DefaultSettings.Container.Memory,
                        Essential = true,
                        HealthCheck = new HealthCheck
                        {
                             Command = new List<string>{ "http://localhost/" + DefaultSettings.Container.HealthCheckSettings.Url },
                             Interval = DefaultSettings.Container.HealthCheckSettings.Interval,
                             Timeout = DefaultSettings.Container.HealthCheckSettings.Timeout,
                             StartPeriod = DefaultSettings.Container.HealthCheckSettings.StartPeriod,
                             Retries = DefaultSettings.Container.HealthCheckSettings.Retries
                        }
                    }
                }
            };

            var response = await ECSClient.RegisterTaskDefinitionAsync(request);

            return response.TaskDefinition;
        }

        public void Dispose() => ECSClient?.Dispose();
    }
}
