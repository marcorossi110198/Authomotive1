using System.Collections.Generic;

namespace ClusterAudi {
    public class FSM<TState> where TState : IState
    {
        protected Dictionary<string, TState> _mystatesDictionary = new();

        protected TState _currentState;
        protected TState _previousState;

        public void AddState(string stateName, TState state)
        {
            _mystatesDictionary[stateName] = state;
        }

        public void RemoveState(string stateName)
        {
            if (_mystatesDictionary.ContainsKey(stateName))
            {
                _mystatesDictionary.Remove(stateName);
            }
        }

        public void GoTo(string newState)
        {
            if (_currentState != null)
            {
                _previousState = _currentState;
                _currentState.StateOnExit();
            }

            _currentState = _mystatesDictionary[newState];

            _currentState.StateOnEnter();
        }

        public void UpdateState()
        {
            if(_currentState != null)
            {
                _currentState.StateOnUpdate();
            }
        }

        public TState GetCurrentState()
        {
            return _currentState;
        }
    }
}
