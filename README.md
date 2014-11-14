## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

Add a postfixed method call instruction to recursive calls

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)

## Nuget package 

http://nuget.org/packages/Tail.Fody

## Your code

	public static int Fib(int i, int acc)
	{
		if (i == 0)
		{
			return acc;
		}
		return Fib(i - 1, acc + i);
	}

## What gets compiled

	.method public hidebysig static 
		int32 Fib (
			int32 i,
			int32 acc
		) cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 19 (0x13)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: brtrue.s IL_0005

		IL_0003: ldarg.1
		IL_0004: ret

		IL_0005: ldarg.0
		IL_0006: ldc.i4.1
		IL_0007: sub
		IL_0008: ldarg.1
		IL_0009: ldarg.0
		IL_000a: add
		IL_000b: tail. // Here it is
		IL_000d: call int32 Fib(int32, int32)
		IL_0012: ret
	} // end of method Fib

    
## What fields are targeted 

 * All methods within tail call position