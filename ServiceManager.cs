using Amazon.ECS;
using Amazon.ECS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using threadingTask = System.Threading.Tasks;

namespace ECS_Deploy
{
    class ServiceManager : IDisposable
    {
        AmazonECSClient ECSClient;
        TaskDefinition TaskDefenition;

        public ServiceManager(TaskDefinition taskDefenition)
        {
            TaskDefenition = taskDefenition;
            ECSClient = ECSHelper.CreateClient();
        }

        internal static threadingTask.Task CreateOrUpdate(TaskDefinition taskDefenition) => new ServiceManager(taskDefenition).DoCreateOrUpdate();

        private async threadingTask.Task DoCreateOrUpdate()
        {
            if (await DoesServiceExist())
                await UpdateService();
            else
                await CreateService();
        }

        private threadingTask.Task UpdateService()
        {
            var request = new UpdateServiceRequest
            {
                Cluster = DefaultSettings.General.ClusterName,
                Service = DefaultSettings.General.ServiceName,
                TaskDefinition = TaskDefenition.TaskDefinitionArn
            };

            return ECSClient.UpdateServiceAsync(request);
        }

        private threadingTask.Task CreateService()
        {
            var request = new CreateServiceRequest
            {
                Cluster = DefaultSettings.General.ClusterName,
                LaunchType = LaunchType.FindValue(DefaultSettings.General.ServiceLaunchType),
                DesiredCount = DefaultSettings.General.ServiceNumberOfTasks,
                ServiceName = DefaultSettings.General.ServiceName,
                TaskDefinition = TaskDefenition.TaskDefinitionArn
            };

            return ECSClient.CreateServiceAsync(request);
        }

        private async threadingTask.Task<bool> DoesServiceExist()
        {
            var request = new DescribeServicesRequest
            {
                Cluster = DefaultSettings.General.ClusterName,
                Services = new List<string> { DefaultSettings.General.ServiceName }
            };

            var response = await ECSClient.DescribeServicesAsync(request);

            return response.Services.Any();
        }

        public void Dispose() => ECSClient?.Dispose();
    }
}
