namespace Innoactive.CreatorEditor.PackageManager.BasicTemplate
{
    /// <summary>
    /// Adds Unity's Post-Processing package as a dependency.
    /// </summary>
    public class PostProcessingPackageEnabler : Dependency
    {
        /// <inheritdoc/>
        public override string Package { get; } = "com.unity.postprocessing";

        /// <inheritdoc/>
        public override int Priority { get; } = 10;
    }
}
