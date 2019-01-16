using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.ECS;
using Amazon.ECS.Model;

namespace ECS_Deploy
{
    class TaskManager : IDisposable
    {
        TaskManager()
        {
            ECSClient2 = new AmazonECSClient();
        }

        string ServiceName => DefaultSettings.
        string TaskDefenitionFamily;
        string DockerImage;
        AmazonECSClient ECSClient = new AmazonECSClient();
        AmazonECSClient ECSClient2;

        internal static Task<string> CreateOrUpdate() => new TaskManager().DoCreateOrUpdate();

        async Task<string> DoCreateOrUpdate()
        {
            var taskDefenition = await GetExistingTaskDefenition();

            if (taskDefenition == null)
                taskDefenition = await CreateTaskDefenition();
            else
                taskDefenition = await UpdateTaskDefenition();
            return taskDefenition.Revision;
        }

        private async Task<TaskDefinition> CreateOrUpdate()
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

        private async Task<TaskDefinition> GetExistingTaskDefenition()
        {
            var request = new DescribeTaskDefinitionRequest { TaskDefinition = TaskDefenitionFamily };

            var response = await ECSClient.DescribeTaskDefinitionAsync(request);

            return response.TaskDefinition;
        }

        public void Dispose()
        {
            ECSClient?.Dispose();
        }
    }
}
