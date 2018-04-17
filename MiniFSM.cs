using System;
using System.Collections.Generic;


public class MiniFSMHandler<TEnum> where TEnum : struct, IConvertible, IComparable, IFormattable {
	public TEnum State;
	public Action Enter;
	public Action Update;
	public Action Exit;
}

public class MiniFSM<TEnum> where TEnum : struct, IConvertible, IComparable, IFormattable {
	private TEnum _currentState;
	private TEnum _previousState;
	private MiniFSMHandler<TEnum> _currentHandler;
	private readonly Dictionary<TEnum, MiniFSMHandler<TEnum>> states = new Dictionary<TEnum, MiniFSMHandler<TEnum>>();

	public event Action<TEnum, TEnum> OnStateChanged;

	public MiniFSM(params MiniFSMHandler<TEnum>[] states) {
		foreach (MiniFSMHandler<TEnum> state in states) {
			states.Add(state.State, state);
		}
	}

	public void StartMachine(TEnum initialState) {
		if (!states.ContainsKey(initialState)) return;

		_currentState = initialState;
		_currentHandler = states[initialState];
		states[initialState].Enter();
	}

	public void Update() {
		_currentHandler.Update();
	}

	public void ChangeState(TEnum nextState) {
		// avoid changing to the same state
		if (EqualsState(_currentState, nextState)) return;

		_previousState = _currentState;
		_currentState = nextState;

		_currentHandler = states[_currentState];

		// exit the prev state, enter the next state
		states[_previousState].Exit();
		_currentHandler.Enter();

		// fire the changed event if we have a listener
		OnStateChanged?.Invoke(_previousState, _currentState);
	}

	private static bool EqualsState(TEnum first, TEnum second) {
		var asEnumType = first as TEnum?;
		return EqualityComparer<TEnum>.Default.Equals(asEnumType.Value, second);
	}
}