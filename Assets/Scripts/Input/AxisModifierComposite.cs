
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad] // Automatically register in editor.
#endif

[DisplayStringFormat("{modifier}+{negative}/{positive}")]
[DisplayName("Positive/Negative Binding with Modifier")]
public class AxisModifierComposite : AxisComposite
{
    // Each part binding is represented as a field of type int and annotated with
    // InputControlAttribute. Setting "layout" restricts the controls that
    // are made available for picking in the UI.
    //
    // On creation, the int value is set to an integer identifier for the binding
    // part. This identifier can read values from InputBindingCompositeContext.
    // See ReadValue() below.
    [InputControl(layout = "Button")] public int modifier;

    // This method computes the resulting input value of the composite based
    // on the input from its part bindings.
    public override float ReadValue(ref InputBindingCompositeContext context)
    {
        if (context.ReadValueAsButton(modifier))
            return base.ReadValue(ref context);
        return default;
    }

    // This method computes the current actuation of the binding as a whole.
    public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
    {
        if (context.ReadValueAsButton(modifier))
            return base.EvaluateMagnitude(ref context);
        return default;
    }

    static AxisModifierComposite()
    {
        InputSystem.RegisterBindingComposite<AxisModifierComposite>();
    }

    [RuntimeInitializeOnLoadMethod]
    static void Init() {} // Trigger static constructor.
}
