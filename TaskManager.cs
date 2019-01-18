using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.ECS;
using Amazon.ECS.Model;
using Olive;

namespace ECS_Deploy
{
    class TaskManager : IDisposable
    {
        TaskManager() { ECSClient = ECSHelper.CreateClient(); }

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
                             Command = new List<string>{ GetHealthCheckCommand() },
                             Interval = DefaultSettings.Container.HealthCheckSettings.Interval,
                             Timeout = DefaultSettings.Container.HealthCheckSettings.Timeout,
                             StartPeriod = DefaultSettings.Container.HealthCheckSettings.StartPeriod,
                             Retries = DefaultSettings.Container.HealthCheckSettings.Retries
                        },
                        PortMappings = DefaultSettings.Container.ExposedPorts.Split(",").Select(i=>
                                                                                                new PortMapping {
                                                                                                    HostPort = 0,
                                                                                                    ContainerPort = i.To<int>() ,
                                                                                                    Protocol = TransportProtocol.Tcp
                                                                                                }).ToList()
                    }
                }
            };

            var response = await ECSClient.RegisterTaskDefinitionAsync(request);

            return response.TaskDefinition;
        }

        string GetHealthCheckCommand() =>
            $"CMD-SHELL, echo 0";
        //TODO : $"CMD-SHELL, powershell -command 'try{{ if ((Invoke-WebRequest -Uri \"http://localhost:{DefaultSettings.Container.ExposedPorts.Split(",").First()}/{DefaultSettings.Container.HealthCheckSettings.Url}\" -UseBasicParsing -DisableKeepAlive).StatusCode -ne 200) {{ return 1 }} else {{ return 0}}  }} catch {{ return 1  }}'";

        public void Dispose() => ECSClient?.Dispose();


    }
}
