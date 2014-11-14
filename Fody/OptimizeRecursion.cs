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

    private void ProcessMethod(MethodDefinition method)
    {
        if (method.Body == null) return;
        var instructions = method.Body.Instructions;
        foreach (var instruction in instructions.ToArray())
        {
            if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Calli || instruction.OpCode == OpCodes.Callvirt)
            {
                if (instruction.Operand == method)
                {
                    if (instruction.Previous != null && instruction.Previous.OpCode != OpCodes.Tail)
                    {
                        if (instruction.Next != null)
                        {
                            ILProcessor processor = method.Body.GetILProcessor();
                            if (instruction.Next.OpCode == OpCodes.Ret)
                            {
                                processor.InsertBefore(instruction, Instruction.Create(OpCodes.Tail));
                            }
                            else
                            {
                                int local;
                                if (StLoc(instruction.Next, out local))
                                {
                                    if (JumpToNext(instruction.Next.Next))
                                    {
                                        if (LdLoc(instruction.Next.Next.Next, local))
                                        {
                                            if (instruction.Next.Next.Next.Next.OpCode == OpCodes.Ret)
                                            {
                                                processor.InsertBefore(instruction, Instruction.Create(OpCodes.Tail));
                                                processor.InsertAfter(instruction, Instruction.Create(OpCodes.Ret));

                                                //Ideally we want to remove these 3 instructions
                                                //processor.Remove(instruction.Next);//ldloc.0
                                                //processor.Remove(instruction.Next);//br.s
                                                //processor.Remove(instruction.Next);//
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private static bool LdLoc(Instruction instruction, int local)
    {
        if (instruction.OpCode == OpCodes.Ldloc || instruction.OpCode == OpCodes.Ldloc_S)
            return local == ((VariableDefinition) instruction.Operand).Index;

        if (instruction.OpCode == OpCodes.Ldloc_0)
            return local == 0;

        if (instruction.OpCode == OpCodes.Ldloc_1)
            return local == 1;

        if (instruction.OpCode == OpCodes.Ldloc_2)
            return local == 2;

        if (instruction.OpCode == OpCodes.Ldloc_3)
            return local == 3;

        return false;
    }

    private static bool JumpToNext(Instruction instruction)
    {
        return (instruction.OpCode == OpCodes.Br_S || instruction.OpCode == OpCodes.Br_S) &&
               instruction.Operand == instruction.Next;
    }

    private static bool StLoc(Instruction instruction, out int loc)
    {
        if (instruction.OpCode == OpCodes.Stloc || instruction.OpCode == OpCodes.Stloc_S)
        {
            loc = ((VariableDefinition) instruction.Operand).Index;
            return true;
        }
        if (instruction.OpCode == OpCodes.Stloc_0)
        {
            loc = 0;
            return true;
        }
        if (instruction.OpCode == OpCodes.Stloc_1)
        {
            loc = 1;
            return true;
        }
        if (instruction.OpCode == OpCodes.Stloc_2)
        {
            loc = 2;
            return true;
        }
        if (instruction.OpCode == OpCodes.Stloc_3)
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