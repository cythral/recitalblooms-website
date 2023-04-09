namespace RecitalBlooms.Website.Cicd.BuildDriver
{
    /// <summary>
    /// CloudFormation outputs of the Artifacts Stack.
    /// </summary>
    public class Outputs
    {
        /// <summary>
        /// Gets or sets the hosted zone ID for the recital blooms domain name.
        /// </summary>
        public string HostedZoneId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the artifact bucket.
        /// </summary>
        public string BucketName { get; set; } = string.Empty;
    }
}
