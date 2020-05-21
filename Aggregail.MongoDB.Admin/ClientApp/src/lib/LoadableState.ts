export type LoadableState<T extends object> =
  | { kind: 'Loading' }
  | { kind: 'Loaded' } & T
  | { kind: 'Failed', reason: any };

export const LoadableState = {
  loading: <T extends object>(): LoadableState<T> => ({kind: 'Loading'}),
  loaded: <T extends object>(state: T): LoadableState<T> => ({kind: 'Loaded', ...state}),
  failed: <T extends object>(reason: any): LoadableState<T> => ({kind: 'Failed', reason})
}