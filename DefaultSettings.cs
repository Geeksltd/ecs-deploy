using System;
using System.Collections.Generic;
using Olive;

namespace ECS_Deploy
{
    static class DefaultSettings
    {
        const string DEFAULT_CONTAINER_MEMORY = "512";
        const string REQUIRES_COMPATIBILITIES = "EC2"
        const string DEFAULT_HEALTH_CHECK_URL = "healthcheck";
        const string DEFAULT_HEALTH_CHECK_INTERVAL = "5";
        const string DEFAULT_HEALTH_CHECK_TIMEOUT = "60";
        const string DEFAULT_HEALTH_CHECK_START_PERIOD = "300";
        const string DEFAULT_HEALTH_CHECK_RETRIES = "5";
        internal static ContainerSettings Container { get; private set; }
        internal static TaskDefenitionSettings TaskDefenition { get; private set; }
        internal static GeneralSettings General { get; private set; }
        static void LoadSettings()
        {
            Container = new ContainerSettings().LoadFromParameters();
            TaskDefenition = new TaskDefenitionSettings().LoadFromParameters();
            General = new GeneralSettings().LoadFromParameters();
        }

        [ArgumentNamePrefix("container")]
        internal class ContainerSettings
        {
            [Argument(DEFAULT_CONTAINER_MEMORY)]
            public int Memory { get; set; }

            [Argument(required: true)]
            public string Image { get; set; }

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
            [Argument(REQUIRES_COMPATIBILITIES, "Comma seperated list of compatibilities (i.e EC2)")]
            public string RequiresCompatibilities { get; set; }

            [Argument(required: true, "Comma seperated list of compatibilities (i.e EC2)")]
            public string Family { get; set; }

            [Argument(required: true, "The arn of the iAM role used by the task at runtime")]
            public string TaskRoleArn { get; set; }
        }

        [ArgumentNamePrefix]
        internal class GeneralSettings
        {
            [Argument(required: true)]
            public string ServiceName { get; set; }

            [Argument(required: true)]
            public string ClusterName { get; set; }
        }

    }
}
