using System;
using System.Linq;
using Mono.Cecil;

public class ReferenceCleaner
{
    ModuleDefinition moduleDefinition;
    Action<string> logInfo;

    public ReferenceCleaner(ModuleDefinition moduleDefinition, Action<string> logInfo )
    {
        this.moduleDefinition = moduleDefinition;
        this.logInfo = logInfo;
    }

    public void Execute()
    {
        var referenceToRemove = moduleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "Tail");
        if (referenceToRemove == null)
        {
            logInfo("\tNo reference to 'Tail.dll' found. References not modified.");
            return;
        }

        moduleDefinition.AssemblyReferences.Remove(referenceToRemove);
        logInfo("\tRemoving reference to 'Tail.dll'.");
    }
}