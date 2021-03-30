using BEonTime.Data.Entities;

namespace BEonTime.Services.TimeCalculator
{
    public static class AttendanceValidatorFactory
    {
        public static AttValidator GenerateAttValidator(Attendance att)
        {
            return att.Status switch
            {
                EntryMode.In => new InAtt(),
                EntryMode.BreakStart => new BreakStartAtt(),
                EntryMode.BreakEnd => new BreakEndAtt(),
                EntryMode.Out => new OutAtt(),

                _ => new NullAtt(),
            };
        }

        public abstract class AttValidator
        {
            public abstract EntryMode[] AllowedNextModes { get; }
        }

        private class InAtt : AttValidator
        {
            public override EntryMode[] AllowedNextModes => new EntryMode[] { EntryMode.BreakStart, EntryMode.Out };
        }

        private class BreakStartAtt : AttValidator
        {
            public override EntryMode[] AllowedNextModes => new EntryMode[] { EntryMode.BreakEnd };
        }

        private class BreakEndAtt : AttValidator
        {
            public override EntryMode[] AllowedNextModes => new EntryMode[] { EntryMode.BreakStart, EntryMode.Out };
        }

        private class OutAtt : AttValidator
        {
            public override EntryMode[] AllowedNextModes => new EntryMode[] { EntryMode.In };
        }

        private class NullAtt : AttValidator
        {
            public override EntryMode[] AllowedNextModes => null;
        }
    }
}
