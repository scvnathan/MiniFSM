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
	private readonly Dictionary<TEnum, MiniFSMHandler<TEnum>> statesHandlers = new Dictionary<TEnum, MiniFSMHandler<TEnum>>();

	public event Action<TEnum, TEnum> OnStateChanged;

	public MiniFSM(params MiniFSMHandler<TEnum>[] handlers) {
		foreach (MiniFSMHandler<TEnum> miniFsmHandler in handlers) {
			statesHandlers.Add(miniFsmHandler.State, miniFsmHandler);
		}
	}

	public void StartMachine(TEnum initialState) {
		if (!statesHandlers.ContainsKey(initialState)) return;

		_currentState = initialState;
		_currentHandler = statesHandlers[initialState];
		statesHandlers[initialState].Enter();
	}

	public void Update() {
		_currentHandler.Update();
	}

	public void ChangeState(TEnum nextState) {
		// avoid changing to the same state
		if (EqualsState(_currentState, nextState)) return;

		_previousState = _currentState;
		_currentState = nextState;

		_currentHandler = statesHandlers[_currentState];

		// exit the prev state, enter the next state
		statesHandlers[_previousState].Exit();
		_currentHandler.Enter();

		// fire the changed event if we have a listener
		OnStateChanged?.Invoke(_previousState, _currentState);
	}

	private static bool EqualsState(TEnum first, TEnum second) {
		var asEnumType = first as TEnum?;
		return EqualityComparer<TEnum>.Default.Equals(asEnumType.Value, second);
	}
}