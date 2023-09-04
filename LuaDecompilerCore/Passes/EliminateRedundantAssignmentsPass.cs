﻿using LuaDecompilerCore.IR;

namespace LuaDecompilerCore.Passes;

/// <summary>
/// Super simple analysis that eliminates assignments of the form:
/// REGA = REGA
/// 
/// These are often generated by the TEST instruction and elimination of these simplifies things for future passes
/// </summary>
public class EliminateRedundantAssignmentsPass : IPass
{
    public bool RunOnFunction(DecompilationContext decompilationContext, FunctionContext functionContext, Function f)
    {
        var changed = false;
        foreach (var b in f.BlockList)
        {
            for (var i = 0; i < b.Instructions.Count; i++)
            {
                // If we encounter a closure we must skip instructions equal to the number of upValues, as the
                // assignments that follow are critical for upValue binding analysis
                if (b.Instructions[i] is Assignment { Right: Closure c })
                {
                    i += c.Function.UpValueCount;
                }
                else if (b.Instructions[i] is Assignment
                         {
                             IsSingleAssignment: true, 
                             Left: IdentifierReference ir, 
                             IsLocalDeclaration: false
                         } assn)
                {
                    if (assn.Right is IdentifierReference reference &&
                        ir.Identifier == reference.Identifier)
                    {
                        b.Instructions.RemoveAt(i);
                        i--;
                        changed = true;
                    }
                }
            }
        }

        return changed;
    }
}