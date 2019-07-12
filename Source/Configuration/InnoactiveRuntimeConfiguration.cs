using System.Collections.Generic;
using System.Collections.ObjectModel;
using Innoactive.Hub.Training.Behaviors;
using Innoactive.Hub.Training.Configuration;
using Innoactive.Hub.Training.Configuration.Modes;

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