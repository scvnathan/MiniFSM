# MiniFSM
A lil finite state machine


This is mostly intended to be used within Unity, but I guess could be used in any C# environment. Inspired by Prime31's [StateKitLite](https://github.com/prime31/StateKit) and RSG's [Fluent State Machine](https://github.com/Real-Serious-Games/Fluent-State-Machine)


## Example
```
	private enum EnemyState {
		Idle,
		Patrolling,
		Attacking
	}

	private MiniFSM<EnemyState> machine;

	private void Start() {
		machine = new MiniFSM<EnemyState>(
			new MiniFSMHandler<EnemyState> {
				State = EnemyState.Idle,
				Enter = () => { Debug.Log("enter Idle"); },
				Exit = () => { Debug.Log("exit Idle"); },
				Update = () => { Debug.Log("update Idle"); }
			},
			
			new MiniFSMHandler<EnemyState> {
				State = EnemyState.Patrolling,
				Enter = () => { Debug.Log("enter Patrolling"); },
				Exit = () => { Debug.Log("exit Patrolling"); },
				Update = () => { Debug.Log("update Patrolling"); }
			},
			
			new MiniFSMHandler<EnemyState> {
				State = EnemyState.Attacking,
				Enter = () => { Debug.Log("enter Attacking"); },
				Exit = () => { Debug.Log("exit Attacking"); },
				Update = () => { Debug.Log("update Attacking"); }
			}
		);
		
		machine.StartMachine(EnemyState.Idle);
	}

	private void Update() {
		machine.Update();
		if (Input.GetKeyDown(KeyCode.A)) {
			machine.ChangeState(EnemyState.Idle);
		}
		
		if (Input.GetKeyDown(KeyCode.B)) {
			machine.ChangeState(EnemyState.Patrolling);
		}
		
		if (Input.GetKeyDown(KeyCode.C)) {
			machine.ChangeState(EnemyState.Attacking);
		}		
	}
		
```
