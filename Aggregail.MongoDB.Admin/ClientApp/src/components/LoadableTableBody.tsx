import {Alert, Spinner} from "reactstrap";
import React from "react";
import {LoadableState} from "../lib";

const TableRowSpinner = (props: { colSpan: number }) => (
  <tr>
    <td colSpan={props.colSpan} className="text-center">
      <Spinner>Loading...</Spinner>
    </td>
  </tr>
);
const TableRowError = (props: { colSpan: number, reason: any }) => (
  <tr>
    <td colSpan={props.colSpan}>
      <Alert color="danger">{`${props.reason}`}</Alert>
    </td>
  </tr>
);

interface LoadedTableBodyProps<T> {
  state: T;
  render: (item: T) => React.ReactElement | React.ReactElement[];
}

const LoadedTableRows = <T extends unknown>(props: LoadedTableBodyProps<T>): React.ReactElement => {
  if (props.state instanceof Array) {
    return <React.Fragment>{props.state.map(props.render)}</React.Fragment>;
  } else {
    return <React.Fragment>{props.render(props.state)}</React.Fragment>;
  }
};

interface LoadableTableRowsProps<T extends {}> {
  colSpan: number;
  state: LoadableState<T>;
  render: (state: T) => React.ReactElement | React.ReactElement[];
}

export const LoadableTableBody = <T extends {}>(props: LoadableTableRowsProps<T>) => (
  <tbody>
  {props.state.kind === 'Loading' ? <TableRowSpinner colSpan={props.colSpan}/> : null}
  {props.state.kind === 'Failed' ? <TableRowError colSpan={props.colSpan} reason={props.state.reason}/> : null}
  {props.state.kind === 'Loaded' ? <LoadedTableRows state={props.state} render={props.render}/> : null}
  </tbody>
);