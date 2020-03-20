using System.Collections.Generic;
using System.Collections.ObjectModel;
using Innoactive.Creator.Core.Behaviors;
using Innoactive.Creator.Core.Configuration;
using Innoactive.Creator.Core.Configuration.Modes;

namespace Innoactive.Hub.Training.Template.Configuration
{
    public class InnoactiveRuntimeConfiguration : DefaultRuntimeConfiguration
    {
        public override ReadOnlyCollection<IMode> AvailableModes
        {
            get
            {
                IMode noHints = new Mode("No Audio Hints", new WhitelistTypeRule<IOptional>().Add<PlayAudioBehavior>());
                
                return new List<IMode>
                {
                    DefaultMode,
                    noHints
                }.AsReadOnly();
            }
        }
    }
}