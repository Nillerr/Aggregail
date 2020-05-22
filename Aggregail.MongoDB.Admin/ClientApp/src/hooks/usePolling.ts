import {useEffect, useRef, useState} from "react";
import Axios from "axios";
import {LoadableState} from "../lib";

export const usePolling = <T>(url: string, interval: number = 2500): LoadableState<{ data: T }> => {
  const [state, setState] = useState(LoadableState.loading<{ data: T }>());
  const refreshTimeoutRef = useRef<NodeJS.Timeout>();

  useEffect(() => {
    const cts = Axios.CancelToken.source();

    const poll = () => {
      return Axios
        .get<T>(url, {cancelToken: cts.token})
        .then(response => setState(LoadableState.loaded({data: response.data})))
        .catch(reason => setState(LoadableState.failed(reason)));
    };

    const refresh = () => {
      refreshTimeoutRef.current = setTimeout(() => poll().finally(() => refresh()), interval);
    };

    poll().finally(() => refresh());

    return () => {
      cts.cancel();

      if (refreshTimeoutRef.current) {
        clearTimeout(refreshTimeoutRef.current);
      }
    };
  }, [url, interval]);

  return state;
}

