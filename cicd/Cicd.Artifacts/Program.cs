using Amazon.CDK;

using RecitalBlooms.Website.Artifacts;

#pragma warning disable SA1516

var app = new App();
_ = new ArtifactsStack(app, "recitalblooms-website-cicd", new StackProps
{
    Synthesizer = new BootstraplessSynthesizer(new BootstraplessSynthesizerProps()),
});

app.Synth();
