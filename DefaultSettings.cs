using System;
using System.Collections.Generic;
using Olive;

namespace ECS_Deploy
{
    static class DefaultSettings
    {
        const string DEFAULT_CONTAINER_MEMORY = "512";
        const string DEFAULT_CONTAINER_EXPOSED_PORTS = "80";
        const string REQUIRES_COMPATIBILITIES = "EC2";
        const string DEFAULT_SERVICE_LAUNCH_TYPE = "EC2";
        const string DEFAULT_SERVICE_NUMBER_OF_TASKS_TO_RUN = "1";
        const string DEFAULT_HEALTH_CHECK_URL = "healthcheck";
        const string DEFAULT_HEALTH_CHECK_INTERVAL = "30";
        const string DEFAULT_HEALTH_CHECK_TIMEOUT = "60";
        const string DEFAULT_HEALTH_CHECK_START_PERIOD = "1";
        const string DEFAULT_HEALTH_CHECK_RETRIES = "10";
        public static ContainerSettings Container { get; private set; }
        public static TaskDefenitionSettings TaskDefenition { get; private set; }
        public static GeneralSettings General { get; private set; }
        internal static void LoadSettings()
        {
            Container = Parameters.LoadProperties(new ContainerSettings());
            TaskDefenition = Parameters.LoadProperties(new TaskDefenitionSettings());
            General = Parameters.LoadProperties(new GeneralSettings());
        }

        [ArgumentNamePrefix("container")]
        internal class ContainerSettings
        {
            [Argument(DEFAULT_CONTAINER_MEMORY)]
            public int Memory { get; set; }

            [Argument(required: true)]
            public string Image { get; set; }

            [Argument(required: true, defaultValue: DEFAULT_CONTAINER_EXPOSED_PORTS, description: "Comma separated list of ports")]
            public string ExposedPorts { get; set; }

            public HealthCheck HealthCheckSettings { get; set; } = new HealthCheck();

            [ArgumentNamePrefix("health-check")]
            internal class HealthCheck
            {
                [Argument(DEFAULT_HEALTH_CHECK_URL)]
                public string Url { get; set; }
                [Argument(DEFAULT_HEALTH_CHECK_INTERVAL)]
                public int Interval { get; set; }
                [Argument(DEFAULT_HEALTH_CHECK_TIMEOUT)]
                public int Timeout { get; set; }
                [Argument(DEFAULT_HEALTH_CHECK_START_PERIOD)]
                public int StartPeriod { get; set; }
                [Argument(DEFAULT_HEALTH_CHECK_RETRIES)]
                public int Retries { get; set; }
            }
        }

        [ArgumentNamePrefix("task-def")]
        internal class TaskDefenitionSettings
        {
            [Argument(REQUIRES_COMPATIBILITIES, description: "Comma seperated list of compatibilities (i.e EC2)")]
            public string RequiresCompatibilities { get; set; }

            [Argument(required: true, description: "Comma seperated list of compatibilities (i.e EC2)")]
            public string Family { get; set; }

            [Argument(required: true, description: "The arn of the iAM role used by the task at runtime")]
            public string TaskRoleArn { get; set; }
        }

        [ArgumentNamePrefix]
        internal class GeneralSettings
        {
            [Argument(required: true)]
            public string ServiceName { get; set; }

            [Argument(required: true, defaultValue: DEFAULT_SERVICE_LAUNCH_TYPE)]
            public string ServiceLaunchType { get; set; }

            [Argument(required: true, defaultValue: DEFAULT_SERVICE_NUMBER_OF_TASKS_TO_RUN)]
            public int ServiceNumberOfTasks { get; set; }

            [Argument(required: true)]
            public string ClusterName { get; set; }

            [Argument(required: true)]
            public string Region { get; set; }

        }

    }
}
