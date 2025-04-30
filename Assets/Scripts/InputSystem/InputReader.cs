using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static InputSystem_Actions;

[CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/InputReader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    //These events get called when inputs are received
    //then the implementation of the delegates happens in other scripts
    public event UnityAction<Vector2> Move = delegate { };
    public event UnityAction<Vector2> Look = delegate { };
    public event UnityAction<bool> Sprint = delegate { };
    public event UnityAction Dodge = delegate { };
    public event UnityAction Interact = delegate { };
    public event UnityAction DrawBowStarted = delegate { };
    public event UnityAction DrawBowReleased = delegate { };
    public event UnityAction CancelDraw = delegate { };
    public event UnityAction<bool> PauseDraw = delegate { };
    public event UnityAction TriggerArrowEffect = delegate { };
    public event UnityAction<int> SelectArrowhead = delegate { };
    public event UnityAction<int> SelectArrow = delegate { };
    public event UnityAction<float> ScrollArrow = delegate { };
    public event UnityAction SelectBasicArrow = delegate { };
    public event UnityAction PauseGame = delegate { };
    public event UnityAction OpenBestiary = delegate { };

    private InputSystem_Actions input;

    //Connects to the input system by setting callback to go to this script
    private void OnEnable()
    {
        if (input == null)
        {
            input = new InputSystem_Actions();
            input.Player.SetCallbacks(this);
        }

        input.Player.Enable();
    }

    //Enable and disable inputs
    public void EnablePlayerActions()
    {
        input.Player.Enable();
        input.UI.Enable();
    }
    public void DisablePlayerActions()
    {
        input.Player.Disable();
        input.UI.Disable();
    }

    private void OnDisable()
    {
        if (input != null)
        {
            input.Player.Disable();
            input.UI.Disable();
        }
    }
    private void OnDestroy()
    {
        if (input != null)
        {
            input.Player.Disable();
            input.UI.Disable();
        }
    }

    //These methods decide what to do when an input is received
    //Most simply call the event which will then be handled in whatever script is listening to that event
    public void OnMove(InputAction.CallbackContext context)
    {
        Move?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Look?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
            Sprint?.Invoke(true);
        else if (context.canceled)
            Sprint?.Invoke(false);
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed)
            Dodge?.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            Interact?.Invoke();
    }

    public void OnDrawBow(InputAction.CallbackContext context)
    {
        if (context.started)
            DrawBowStarted?.Invoke();
        else if (context.canceled)
            DrawBowReleased?.Invoke();
    }

    public void OnCancelDraw(InputAction.CallbackContext context)
    {
        if (context.performed)
            CancelDraw?.Invoke();
    }

    public void OnPauseDraw(InputAction.CallbackContext context)
    {
        if (context.started)
            PauseDraw?.Invoke(true);
        else if (context.canceled)
            PauseDraw?.Invoke(false);
    }

    public void OnTriggerArrowEffect(InputAction.CallbackContext context)
    {
        if (context.performed)
            TriggerArrowEffect?.Invoke();
    }

    public void OnSelectArrowhead_Negative(InputAction.CallbackContext context)
    {
        if (context.performed)
            SelectArrowhead?.Invoke(0);
    }

    public void OnSelectArrowhead_Piercing(InputAction.CallbackContext context)
    {
        if (context.performed)
            SelectArrowhead?.Invoke(1);
    }

    public void OnSelectArrowhead_Positive(InputAction.CallbackContext context)
    {
        if (context.performed)
            SelectArrowhead?.Invoke(2);
    }

    public void OnSelectArrow(InputAction.CallbackContext context)
    {
        if (context.performed && int.TryParse(context.control.name, out int number))
            SelectArrow?.Invoke(number);
    }

    public void OnScrollArrow(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float scrollValue = context.ReadValue<float>();
            ScrollArrow?.Invoke(scrollValue);
        }
    }


    public void OnSelectBasicArrow(InputAction.CallbackContext context)
    {
        if (context.performed)
            SelectBasicArrow?.Invoke();
    }

    public void OnPauseGame(InputAction.CallbackContext context)
    {
        if (context.performed)
            PauseGame?.Invoke();
    }

    public void OnOpenBestiary(InputAction.CallbackContext context)
    {
        if (context.performed)
            OpenBestiary?.Invoke();
    }
}