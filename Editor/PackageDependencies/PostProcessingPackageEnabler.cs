namespace Innoactive.CreatorEditor.PackageManager.BasicTemplate
{
    /// <summary>
    /// 
    /// </summary>
    public class PostProcessingPackageEnabler : Dependency
    {
        /// <inheritdoc/>
        public override string Package { get; } = "com.unity.postprocessing";

        /// <inheritdoc/>
        public override int Priority { get; } = 10;
    }
}
