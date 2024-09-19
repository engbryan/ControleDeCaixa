using EntryControl.Contracts.Modules.Messaging.Enums;

namespace EntryControl.Contracts.Services.Impl
{
    public static class SnsTopicExtensions
    {
        public static string GetArn(this SnsTopic topic)
        {
            return topic switch
            {
                SnsTopic.CommitEntry => "arn:aws:sns:us-east-1:248428763777:commit-entry-topic",
                SnsTopic.GenerateReport => "arn:aws:sns:us-east-1:248428763777:generate-report-topic",
                SnsTopic.HealthCheck => "arn:aws:sns:us-east-1:248428763777:healthcheck-topic",
                //SnsTopic.ReportReady => "arn:aws:sns:us-east-1:123456789012:EntryControl-ReportReady",
                _ => throw new ArgumentOutOfRangeException(nameof(topic), topic, null)
            };
        }
    }

}
