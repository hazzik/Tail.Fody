using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class ModuleWeaver
{
    void ProcessType(TypeDefinition typeDefinition)
    {
        foreach (var method in typeDefinition.Methods)
        {
            ProcessMethod(method);
        }
    }

    private class DebugContext
    {
        public int VariableIndex;
    }

    private readonly Pattern<object> release = new Pattern<object>
    {
        IsNotTailedCall,
        IsRet
    };

    private readonly Pattern<DebugContext> debug = new Pattern<DebugContext>
    {
        IsNotTailedCall,
        (i, c) => IsStLoc(i, out c.VariableIndex),
        IsJumpToNext,
        (i, c) => IsLdLoc(i, c.VariableIndex),
        IsRet
    };

    private static bool IsNotTailedCall(Instruction i)
    {
        return IsCall(i) && i.Previous?.OpCode != OpCodes.Tail;
    }

    private static bool IsRet(Instruction i)
    {
        return i.OpCode == OpCodes.Ret;
    }

    private static bool IsCall(Instruction i)
    {
        return i.OpCode == OpCodes.Call || i.OpCode == OpCodes.Calli || i.OpCode == OpCodes.Callvirt;
    }

    private void ProcessMethod(MethodDefinition method)
    {
        var body = method.Body;
        if (body == null) return;

        var instructions = body.Instructions.ToArray();
        foreach (var instruction in instructions)
        {
            var processor = body.GetILProcessor();
            if (release.Match(instruction))
            {
                processor.InsertBefore(instruction, Instruction.Create(OpCodes.Tail));
            }
            if (debug.Match(instruction))
            {
                processor.InsertBefore(instruction, Instruction.Create(OpCodes.Tail));
                processor.InsertAfter(instruction, Instruction.Create(OpCodes.Ret));
            }
        }
    }

    private static bool IsLdLoc(Instruction i, int local)
    {
        if (i.OpCode == OpCodes.Ldloc || i.OpCode == OpCodes.Ldloc_S)
            return local == ((VariableDefinition) i.Operand).Index;

        if (i.OpCode == OpCodes.Ldloc_0)
            return local == 0;

        if (i.OpCode == OpCodes.Ldloc_1)
            return local == 1;

        if (i.OpCode == OpCodes.Ldloc_2)
            return local == 2;

        if (i.OpCode == OpCodes.Ldloc_3)
            return local == 3;

        return false;
    }

    private static bool IsJumpToNext(Instruction i)
    {
        return IsJump(i) && i.Operand == i.Next;
    }

    private static bool IsJump(Instruction i)
    {
        return i.OpCode == OpCodes.Br_S || i.OpCode == OpCodes.Br_S;
    }

    private static bool IsStLoc(Instruction i, out int loc)
    {
        if (i.OpCode == OpCodes.Stloc || i.OpCode == OpCodes.Stloc_S)
        {
            loc = ((VariableDefinition) i.Operand).Index;
            return true;
        }
        if (i.OpCode == OpCodes.Stloc_0)
        {
            loc = 0;
            return true;
        }
        if (i.OpCode == OpCodes.Stloc_1)
        {
            loc = 1;
            return true;
        }
        if (i.OpCode == OpCodes.Stloc_2)
        {
            loc = 2;
            return true;
        }
        if (i.OpCode == OpCodes.Stloc_3)
        {
            loc = 3;
            return true;
        }
       
        loc = -1;
        return false;
    }

    public void AddTailToRecursion()
    {
        foreach (var type in allTypes)
        {
            if (type.IsInterface)
            {
                continue;
            }
            if (type.IsEnum)
            {
                continue;
            }
            ProcessType(type);
        }
    }
}