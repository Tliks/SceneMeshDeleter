using nadena.dev.ndmf;
using com.aoyon.scenemeshdeleter;

[assembly: ExportsPlugin(typeof(PluginDefinition))]

namespace com.aoyon.scenemeshdeleter
{
    public class PluginDefinition : Plugin<PluginDefinition>
    {
        public override string QualifiedName => "com.aoyon.scene-mesh-deleter";

        public override string DisplayName => "SceneMeshDeleter";

        protected override void Configure()
        {
            var sequence =
                InPhase(BuildPhase.Transforming)
                .BeforePlugin("MantisLODEditor.ndmf")
                .BeforePlugin("net.rs64.tex-trans-tool");

            sequence
            .Run(SceneMeshDeleterPass.Instance)
            .PreviewingWith(new SceneMeshDeleterPreview());
        }
    }
}