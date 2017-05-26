using System;

namespace Stateless
{
    public partial class StateMachine<TState, TTrigger>
    {
        internal abstract class IGuardCondition
        {
            Reflection.InvocationInfo _methodDescription;

            internal IGuardCondition(Reflection.InvocationInfo description)
            {
                _methodDescription = Enforce.ArgumentNotNull(description, nameof(description));
            }

            internal abstract bool IsSatisfied(object[] args);

            // Return the description of the guard method: the caller-defined description if one
            // was provided, else the name of the method itself
            internal string Description => _methodDescription.Description;

            // Return a more complete description of the guard method
            internal Reflection.InvocationInfo MethodDescription => _methodDescription;
        }

        internal class GuardCondition : IGuardCondition
        {
            internal Func<bool> Guard { get; }

            internal GuardCondition(Func<bool> guard, Reflection.InvocationInfo description)
                : base(description)
            {
                Guard = Enforce.ArgumentNotNull(guard, nameof(guard));
            }

            internal override bool IsSatisfied(object[] args)
            {
                return Guard();
            }
        }

        internal class GuardCondition<TArg0> : IGuardCondition
        {
            Func<TArg0, bool> GuardWithParam;

            internal GuardCondition(Func<TArg0, bool> guard, Reflection.InvocationInfo description)
                : base(description)
            {
                GuardWithParam = guard;
            }

            internal override bool IsSatisfied(object[] args)
            {
                return GuardWithParam(ParameterConversion.Unpack<TArg0>(args, 0));
            }
        }
    }
}