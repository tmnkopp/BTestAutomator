using System;
using System.Collections.Generic;
using System.Text;

namespace BTestAutomator.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutomatorMeta: Attribute
    {
        public ContextName contextName { get; set; }
        public AutomatorMeta(ContextName ContextName)
        {
            contextName = ContextName; 
        }
    }
}
