using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.S3;

using Constructs;

namespace RecitalBlooms.Website.Artifacts
{
    /// <summary>
    /// Stack that contains repositories for storing artifacts.
    /// </summary>
    public class ArtifactsStack : Stack
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtifactsStack" /> class.
        /// </summary>
        /// <param name="scope">The scope to create this artifacts stack in.</param>
        /// <param name="id">The ID of the Artifacts Stack.</param>
        /// <param name="props">The props for the Artifacts Stack.</param>
        public ArtifactsStack(Construct scope, string id, IStackProps? props = null)
            : base(scope, id, props)
        {
            AddBucket();
            AddDns();
        }

        private void AddBucket()
        {
            var bucket = new Bucket(this, "Bucket");
            bucket.ApplyRemovalPolicy(RemovalPolicy.DESTROY);
            bucket.AddToResourcePolicy(new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new[] { "s3:*Object" },
                Resources = new[] { bucket.BucketArn, $"{bucket.BucketArn}/*" },
                Principals = new[]
                {
                    new AccountPrincipal(Fn.Ref("AWS::AccountId")),
                    new ArnPrincipal(Fn.ImportValue("cfn-metadata:DevAgentRoleArn")),
                    new ArnPrincipal(Fn.ImportValue("cfn-metadata:ProdAgentRoleArn")),
                },
            }));

            _ = new CfnOutput(this, "BucketName", new CfnOutputProps
            {
                Value = bucket.BucketName,
                Description = "Name of the Artifacts Bucket for Brighid Commands.",
            });
        }

        private void AddDns()
        {
            var hostedZone = new CfnCustomResource(this, "HostedZone", new CfnCustomResourceProps
            {
                ServiceToken = Fn.ImportValue("cfn-hosted-zone-resource:HostedZoneLambdaArn"),
            });

            hostedZone.AddPropertyOverride("Name", "recitalblooms.com");
            hostedZone.AddPropertyOverride("DelegationSetId", Fn.ImportValue("cfn-dns:DelegationSetId"));

            _ = new CfnOutput(this, "HostedZoneId", new CfnOutputProps
            {
                Value = Fn.GetAtt(hostedZone.LogicalId, "HostedZoneId").ToString(),
                Description = "Name of the Artifacts Bucket for Brighid Commands.",
            });
        }
    }
}
