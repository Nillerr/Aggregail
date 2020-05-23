import {useEffect, useRef} from "react";
import Axios, {CancelTokenSource, Method} from "axios";

export const useFormPostAction = <T extends { [key: string]: any }>(
  url: string,
  onSuccess: () => void,
  onError?: (reason: any) => void
) => {
  const ctsRef = useRef<CancelTokenSource>(Axios.CancelToken.source());

  const submit = (data: T) => {
    const form = new FormData();

    Object.keys(data)
      .forEach(key => form.set(key, data[key]));

    Axios.post(url, form, {cancelToken: ctsRef.current.token})
      .then(response => onSuccess())
      .catch(reason => Axios.isCancel(reason) ? null : onError ? onError(reason) : console.error(reason));
  };

  useEffect(() => () => ctsRef.current.cancel(), []);
  return submit;
}

export const useAction = <T extends any>(
  method: Method,
  url: string,
  onSuccess: () => void,
  onError?: (reason: any) => void
) => {
  const ctsRef = useRef<CancelTokenSource>(Axios.CancelToken.source());

  const submit = (data?: T) => {
    console.log('submit', { method, url, data, iof: (data instanceof Event) });
    Axios
      .request({
        url: url,
        data: (data instanceof Event) ? null : data,
        method: method,
        cancelToken: ctsRef.current.token
      })
      .then(response => onSuccess())
      .catch(reason => Axios.isCancel(reason) ? null : onError ? onError(reason) : console.error(reason));
  };

  useEffect(() => () => ctsRef.current.cancel(), []);
  return submit;
}