import {useEffect, useState} from "react";
import Axios from "axios";
import {LoadableState} from "../lib";

export const usePolling = <T, R extends object>(url: string, selector: (data: T) => R, interval: number = 2500): LoadableState<R> => {
  const [state, setState] = useState(LoadableState.loading<R>());
  const [refreshTimeout, setRefreshTimeout] = useState<NodeJS.Timeout>();

  useEffect(() => {
    const cts = Axios.CancelToken.source();

    const poll = () => Axios
      .get<T>(url, {cancelToken: cts.token})
      .then(response => setState(LoadableState.loaded(selector(response.data))))
      .catch(reason => setState(LoadableState.failed(reason)));

    const refresh = () => {
      const timeout = setTimeout(() => poll().finally(() => refresh()), interval);
      setRefreshTimeout(timeout);
    }

    poll()
      .then(() => refresh());

    return () => {
      cts.cancel();

      if (refreshTimeout) {
        clearTimeout(refreshTimeout);
      }
    };
  }, []);

  return state;
}