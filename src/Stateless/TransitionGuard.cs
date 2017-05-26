using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Stateless
{
    public partial class StateMachine<TState, TTrigger>
    {
        internal class TransitionGuard
        {
            internal IList<IGuardCondition> Conditions { get; }

            public static readonly TransitionGuard Empty = new TransitionGuard(new Tuple<Func<bool>, string>[0]);

            internal TransitionGuard(Tuple<Func<bool>, string>[] guards)
            {
                Enforce.ArgumentNotNull(guards, nameof(guards));

                Conditions = guards
                    .Select(g => (IGuardCondition)(new GuardCondition(g.Item1, Reflection.InvocationInfo.Create(g.Item1, g.Item2))))
                    .ToList();
            }

            internal static TransitionGuard Create<TArg0>(Func<TArg0, bool> guard, string guardDescription = null)
            {
                Enforce.ArgumentNotNull(guard, nameof(guard));

                return new TransitionGuard(new GuardCondition<TArg0>(guard, Reflection.InvocationInfo.Create(guard, guardDescription)));
            }

            internal TransitionGuard(Func<bool> guard, string description = null)
            {
                Enforce.ArgumentNotNull(guard, nameof(guard));

                Conditions = new List<IGuardCondition> { (IGuardCondition)new GuardCondition(guard, Reflection.InvocationInfo.Create(guard, description)) };
            }

            private TransitionGuard(IGuardCondition guard)
            {
                Enforce.ArgumentNotNull(guard, nameof(guard));

                Conditions = new List<IGuardCondition> { guard };
            }

            // <summary>
            // Guards is the list of the guard functions for all guard conditions for this transition
            // </summary>
            // internal ICollection<Func<bool>> Guards => Conditions.Select(g => g.Guard).ToList();

            // <summary>
            // GuardConditionsMet is true if all of the guard functions return true
            // or if there are no guard functions
            // </summary>
            // public bool GuardConditionsMet => Conditions.All(c => c.Guard());

            // <summary>
            // UnmetGuardConditions is a list of the descriptions of all guard conditions
            // whose guard function returns false
            // </summary>
            //public ICollection<string> UnmetGuardConditions
            //{
            //    get
            //    {
            //        return Conditions
            //            .Where(c => !c.Guard())
            //            .Select(c => c.Description)
            //            .ToList();
            //    }
            //}

            public bool GuardConditionsAreSatsified(object[] args)
            {
                bool any = false;

                Debug.WriteLine("TransitionGuard.GuardConditionsAreSatsified(args)");

                foreach (var guard in Conditions)
                {
                    if (guard.IsSatisfied(args))
                        return true;
                    any = true;
                }

                return !any;
            }

            public Dictionary<IGuardCondition, bool> CheckGuardConditions(object[] args)
            {
                Debug.WriteLine("TransitionGuard.CheckGuardConditions(args)");

                Dictionary<IGuardCondition, bool> checks = new Dictionary<IGuardCondition, bool>();

                foreach (var guard in Conditions)
                {
                    checks[guard] = guard.IsSatisfied(args);
                    Debug.WriteLine("   guard[" + guard.Description + "].IsSatisfied(args) returned " + checks[guard]);
                }

                return checks;
            }
        }
    }
}