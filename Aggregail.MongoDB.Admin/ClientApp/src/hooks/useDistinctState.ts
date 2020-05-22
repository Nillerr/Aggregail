import React, {Dispatch, SetStateAction, useMemo, useRef, useState} from "react";
import equal from "fast-deep-equal";

const setStateIfChanged = <S>(
  ref: React.MutableRefObject<S>,
  setState: Dispatch<SetStateAction<S>>,
  newAction: SetStateAction<S>
) => {
  const currentValue = ref.current;

  const newValue = newAction instanceof Function
    ? newAction(currentValue)
    : newAction;

  if (!equal(currentValue, newValue)) {
    setState(newValue);
    ref.current = newValue;
  }
}

export const useDistinctState = <S>(initialState: S | (() => S)): [S, Dispatch<SetStateAction<S>>] => {
  const [state, setState] = useState(initialState);
  const stateRef = useRef(state);
  const setter = useMemo(() => (value: SetStateAction<S>) => setStateIfChanged(stateRef, setState, value), [setState]);
  return [state, setter];
}