import {LoadableState} from "../lib";
import {useEffect, useState} from "react";
import Axios from "axios";

export const useRequest = <T>(url: string): LoadableState<{ data: T }> => {
  const [state, setState] = useState(LoadableState.loading<{ data: T }>());

  useEffect(() => {
    const cts = Axios.CancelToken.source();

    Axios
      .get<T>(url, {cancelToken: cts.token})
      .then(response => setState(LoadableState.loaded({data: response.data})))
      .catch(reason => setState(LoadableState.failed(reason)));

    return () => cts.cancel();
  }, [url]);

  return state;
}