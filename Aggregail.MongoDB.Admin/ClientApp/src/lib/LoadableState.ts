export type LoadableState<T extends object> =
  | { kind: 'Loading' }
  | { kind: 'Loaded' } & T
  | { kind: 'Failed', reason: any };

export const LoadableState = {
  loading: <T extends object>(): LoadableState<T> => ({kind: 'Loading'}),
  loaded: <T extends object>(state: T): LoadableState<T> => ({kind: 'Loaded', ...state}),
  failed: <T extends object>(reason: any): LoadableState<T> => ({kind: 'Failed', reason}),
  map: <T, R extends object>(state: LoadableState<{ data: T }>, selector: (data: T) => R): LoadableState<R> => {
    switch (state.kind) {
      case "Loading": return state;
      case "Loaded": return {kind: 'Loaded', ...selector(state.data)}
      case "Failed": return state;
    }
  } 
}