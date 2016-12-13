using DataTracker.Model;

namespace DataTracker.Utilities
{
    interface InterfaceSetup
    {
        void GroupChangeInterfaceMethod(string value);
        void IndividualChangeInterfaceMethod(string value);
        void EvaluationChangeInterfaceMethod(string value);
        void ConditionChangeInterfaceMethod(string value);
        void CollectorChangeInterfaceMethod(string value);
        void KeyboardChangeInterfaceMethod(KeyboardStorage value);
        void TherapistChangeInterfaceMethod(string value);
    }
}
