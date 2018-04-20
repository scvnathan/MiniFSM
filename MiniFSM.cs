using System;
using System.Collections.Generic;
using UnityEngine;

public class MiniFSMHandler<TEnum> where TEnum : struct, IConvertible, IComparable, IFormattable {
	public virtual TEnum State { get; set; }
	public virtual Action Enter { get; set; }
	public virtual Action Update { get; set; }
	public virtual Action Exit { get; set; }
}

public class MiniFSM<TEnum> where TEnum : struct, IConvertible, IComparable, IFormattable {
	private TEnum _currentState;
	private TEnum _previousState;
	private MiniFSMHandler<TEnum> _currentHandler;
	private readonly Dictionary<TEnum, MiniFSMHandler<TEnum>> machineStates = new Dictionary<TEnum, MiniFSMHandler<TEnum>>();

	public event Action<TEnum, TEnum> OnStateChanged;

	public MiniFSM(params MiniFSMHandler<TEnum>[] states) {
		foreach (MiniFSMHandler<TEnum> state in states) {
			Add(state);
		}
	}

	public void StartMachine(TEnum initialState) {
		if (!machineStates.ContainsKey(initialState)) return;

		_currentState = initialState;
		_currentHandler = machineStates[initialState];
		machineStates[initialState].Enter();
	}

	public void Add(MiniFSMHandler<TEnum> state) {
		if (machineStates.ContainsKey(state.State)) {
			Debug.LogError($"Already contains {state.State}");
			return;
		}
		
		machineStates.Add(state.State, state);
	}

	public void Update() {
		_currentHandler.Update();
	}

	public void ChangeState(TEnum nextState) {
		// avoid changing to the same state
		if (EqualsState(_currentState, nextState)) return;

		_previousState = _currentState;
		_currentState = nextState;

		_currentHandler = machineStates[_currentState];

		// exit the prev state, enter the next state
		machineStates[_previousState].Exit();
		_currentHandler.Enter();

		// fire the changed event if we have a listener
		OnStateChanged?.Invoke(_previousState, _currentState);
	}

	private static bool EqualsState(TEnum first, TEnum second) {
		var asEnumType = first as TEnum?;
		return EqualityComparer<TEnum>.Default.Equals(asEnumType.Value, second);
	}
}