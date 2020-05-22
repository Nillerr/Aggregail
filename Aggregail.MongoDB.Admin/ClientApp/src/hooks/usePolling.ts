import React, {useEffect, useRef} from "react";
import Axios from "axios";
import {LoadableState} from "../lib";
import {useDistinctState} from "./useDistinctState";

function startPolling<S>(
  url: string,
  setState: React.Dispatch<React.SetStateAction<LoadableState<{ data: S }>>>,
  refreshTimeoutRef: React.MutableRefObject<NodeJS.Timeout | undefined>,
  interval: number
) {
  const cts = Axios.CancelToken.source();

  const poll = () => {
    return Axios
      .get<S>(url, {cancelToken: cts.token})
      .then(response => {
        setState(LoadableState.loaded({data: response.data}));
      })
      .catch(reason => {
        if (Axios.isCancel(reason) || (reason.response && reason.response.status === 401)) {
          return;
        }
        
        setState(LoadableState.failed(reason));
      });
  };

  const refresh = () => {
    if (cts.token.reason) {
      return;
    }
    
    refreshTimeoutRef.current = setTimeout(() => poll().finally(() => refresh()), interval);
  };

  poll().finally(() => refresh());

  return () => {
    cts.cancel();

    if (refreshTimeoutRef.current) {
      clearTimeout(refreshTimeoutRef.current);
    }
  };
}

export const usePolling = <S>(url: string, interval: number = 2500): LoadableState<{ data: S }> => {
  const [state, setState] = useDistinctState(LoadableState.loading<{ data: S }>());

  const refreshTimeoutRef = useRef<NodeJS.Timeout>();

  useEffect(() => {
    return startPolling(url, setState, refreshTimeoutRef, interval);
  }, [url, interval, setState]);

  return state;
}

export const usePauseablePolling = <S>(url: string, interval: number = 2500): [LoadableState<{ data: S }>, boolean, (paused: boolean) => void] => {
  const [state, setState] = useDistinctState(LoadableState.loading<{ data: S }>());
  const [isPaused, setIsPaused] = useDistinctState(false);

  const refreshTimeoutRef = useRef<NodeJS.Timeout>();

  useEffect(() => {
    if (isPaused) {
      return;
    }

    return startPolling(url, setState, refreshTimeoutRef, interval);
  }, [url, interval, setState, isPaused]);

  return [state, isPaused, setIsPaused];
}

