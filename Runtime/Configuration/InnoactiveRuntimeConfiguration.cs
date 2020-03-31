using System.Collections.Generic;
using Innoactive.Creator.Core.Behaviors;
using Innoactive.Creator.Core.Configuration;
using Innoactive.Creator.Core.Configuration.Modes;

namespace Innoactive.Hub.Training.Template.Configuration
{
    public class InnoactiveRuntimeConfiguration : DefaultRuntimeConfiguration
    {
        protected InnoactiveRuntimeConfiguration()
        {
            IMode noHints = new Mode("No Audio Hints", new WhitelistTypeRule<IOptional>().Add<PlayAudioBehavior>());
            Modes = new BaseModeHandler(new List<IMode> { DefaultMode, noHints });
        }
    }
}